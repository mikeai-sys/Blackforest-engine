/**************************************************************************/
/*  video_stream_ffmpeg.cpp                                                */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */
/*                                                                        */
/* Permission is hereby granted, free of charge, to any person obtaining  */
/* a copy of this software and associated documentation files (the        */
/* "Software"), to deal in the Software without restriction, including    */
/* without limitation the rights to use, copy, modify, merge, publish,    */
/* distribute, sublicense, and/or sell copies of the Software, and to     */
/* permit persons to whom the Software is furnished to do so, subject to  */
/* the following conditions:                                              */
/*                                                                        */
/* The above copyright notice and this permission notice shall be         */
/* included in all copies or substantial portions of the Software.        */
/*                                                                        */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. */
/* NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY      */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE      */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 */
/**************************************************************************/

#include "video_stream_ffmpeg.h"

#include "core/config/project_settings.h"
#include "core/io/image.h"
#include "core/io/resource_loader.h"
#include "core/object/class_db.h"
#include "scene/resources/image_texture.h"

extern "C" {
#include <libavcodec/avcodec.h>
#include <libavformat/avformat.h>
#include <libavutil/avutil.h>
#include <libavutil/channel_layout.h>
#include <libavutil/imgutils.h>
#include <libavutil/opt.h>
#include <libswresample/swresample.h>
#include <libswscale/swscale.h>
}

struct FFMPEGPlaybackState {
	AVFormatContext *format_ctx = nullptr;

	int video_stream_idx = -1;
	const AVCodec *video_codec = nullptr;
	AVCodecContext *video_codec_ctx = nullptr;
	SwsContext *sws_ctx = nullptr;
	uint8_t *rgba_buffer = nullptr;
	int rgba_stride = 0;
	int video_w = 0;
	int video_h = 0;
	AVRational video_tb = { 1, 1 };

	int audio_stream_idx = -1;
	const AVCodec *audio_codec = nullptr;
	AVCodecContext *audio_codec_ctx = nullptr;
	SwrContext *swr_ctx = nullptr;
	AVRational audio_tb = { 1, 1 };
	int audio_channels = 0;
	int audio_sample_rate = 0;

	AVFrame *frame = nullptr;
	AVPacket *packet = nullptr;

	Vector<float> audio_ring;

	bool eof = false;
	bool has_video = false;
	bool has_audio = false;
};

void VideoStreamPlaybackFFMPEG::_clear_state() {
	if (!state) {
		return;
	}
	if (state->sws_ctx) {
		sws_freeContext(state->sws_ctx);
		state->sws_ctx = nullptr;
	}
	if (state->swr_ctx) {
		swr_free(&state->swr_ctx);
		state->swr_ctx = nullptr;
	}
	if (state->rgba_buffer) {
		av_freep(&state->rgba_buffer);
	}
	if (state->video_codec_ctx) {
		avcodec_free_context(&state->video_codec_ctx);
	}
	if (state->audio_codec_ctx) {
		avcodec_free_context(&state->audio_codec_ctx);
	}
	if (state->frame) {
		av_frame_free(&state->frame);
	}
	if (state->packet) {
		av_packet_free(&state->packet);
	}
	if (state->format_ctx) {
		avformat_close_input(&state->format_ctx);
	}
	memdelete(state);
	state = nullptr;
}

Error VideoStreamPlaybackFFMPEG::open_file(const String &p_path) {
	_clear_state();
	state = memnew(FFMPEGPlaybackState);
	state->frame = av_frame_alloc();
	state->packet = av_packet_alloc();
	if (!state->frame || !state->packet) {
		_clear_state();
		ERR_FAIL_V(ERR_CANT_OPEN);
	}

	String path = p_path;
	if (path.begins_with("res://") && ProjectSettings::get_singleton() != nullptr) {
		// Resolve the resource path to a real filesystem path for FFmpeg.
		path = ProjectSettings::get_singleton()->globalize_path(path);
	}

	int ret = avformat_open_input(&state->format_ctx, path.utf8().get_data(), nullptr, nullptr);
	if (ret < 0) {
		ERR_PRINT("FFmpeg: could not open '" + p_path + "'.");
		_clear_state();
		ERR_FAIL_V(ERR_CANT_OPEN);
	}
	ret = avformat_find_stream_info(state->format_ctx, nullptr);
	if (ret < 0) {
		ERR_PRINT("FFmpeg: could not find stream info for '" + p_path + "'.");
		_clear_state();
		ERR_FAIL_V(ERR_CANT_OPEN);
	}

	// Locate the first video stream.
	state->video_stream_idx = av_find_best_stream(state->format_ctx, AVMEDIA_TYPE_VIDEO, -1, -1, &state->video_codec, 0);
	if (state->video_stream_idx < 0) {
		ERR_PRINT("FFmpeg: '" + p_path + "' has no video stream.");
		_clear_state();
		ERR_FAIL_V(ERR_CANT_OPEN);
	}

	AVStream *vstream = state->format_ctx->streams[state->video_stream_idx];
	state->video_codec_ctx = avcodec_alloc_context3(state->video_codec);
	avcodec_parameters_to_context(state->video_codec_ctx, vstream->codecpar);
	if (avcodec_open2(state->video_codec_ctx, state->video_codec, nullptr) < 0) {
		ERR_PRINT("FFmpeg: could not open the video codec.");
		_clear_state();
		ERR_FAIL_V(ERR_CANT_OPEN);
	}
	state->video_w = state->video_codec_ctx->width;
	state->video_h = state->video_codec_ctx->height;
	state->video_tb = vstream->time_base;
	state->has_video = true;

	// Allocate the RGBA conversion target once.
	state->rgba_stride = state->video_w * 4;
	state->rgba_buffer = (uint8_t *)av_malloc(state->video_w * state->video_h * 4);
	state->sws_ctx = sws_getContext(
			state->video_w, state->video_h, state->video_codec_ctx->pix_fmt,
			state->video_w, state->video_h, AV_PIX_FMT_RGBA,
			SWS_BILINEAR, nullptr, nullptr, nullptr);

	Ref<Image> empty = Image::create_empty(state->video_w, state->video_h, false, Image::FORMAT_RGBA8);
	texture->set_image(empty);

	// Locate the (audio_track)-th audio stream (audio_track is 0-based).
	int audio_index = -1;
	int audio_count = 0;
	for (int i = 0; i < (int)state->format_ctx->nb_streams; i++) {
		if (state->format_ctx->streams[i]->codecpar->codec_type == AVMEDIA_TYPE_AUDIO) {
			audio_count++;
			if (audio_count == audio_track + 1) {
				audio_index = i;
				break;
			}
		}
	}
	if (audio_index >= 0) {
		state->audio_stream_idx = audio_index;
		AVStream *astream = state->format_ctx->streams[audio_index];
		state->audio_codec = avcodec_find_decoder(astream->codecpar->codec_id);
		if (state->audio_codec) {
			state->audio_codec_ctx = avcodec_alloc_context3(state->audio_codec);
			avcodec_parameters_to_context(state->audio_codec_ctx, astream->codecpar);
			if (avcodec_open2(state->audio_codec_ctx, state->audio_codec, nullptr) >= 0) {
				state->audio_sample_rate = state->audio_codec_ctx->sample_rate;
				state->audio_tb = astream->time_base;
				// Use the decoder's native channel count for both output and the
				// public get_channels()/mix path. Keeps swr in/out channel count the
				// same, avoiding any mismatched-interleaving heap corruption.
				int out_channels = state->audio_codec_ctx->ch_layout.nb_channels;
				AVChannelLayout out_layout;
				av_channel_layout_default(&out_layout, out_channels);
				state->audio_channels = out_channels;
				state->swr_ctx = swr_alloc();
				av_opt_set_chlayout(state->swr_ctx, "in_chlayout", &state->audio_codec_ctx->ch_layout, 0);
				av_opt_set_int(state->swr_ctx, "in_sample_rate", state->audio_codec_ctx->sample_rate, 0);
				av_opt_set_sample_fmt(state->swr_ctx, "in_sample_fmt", state->audio_codec_ctx->sample_fmt, 0);
				av_opt_set_chlayout(state->swr_ctx, "out_chlayout", &out_layout, 0);
				av_opt_set_int(state->swr_ctx, "out_sample_rate", state->audio_codec_ctx->sample_rate, 0);
				av_opt_set_sample_fmt(state->swr_ctx, "out_sample_fmt", AV_SAMPLE_FMT_FLT, 0);
				if (swr_init(state->swr_ctx) >= 0) {
					state->has_audio = true;
				} else {
					swr_free(&state->swr_ctx);
				}
			}
		}
	}

	if (state->format_ctx->duration != AV_NOPTS_VALUE) {
		length = double(state->format_ctx->duration) / AV_TIME_BASE;
	} else if (vstream->duration != AV_NOPTS_VALUE) {
		length = double(vstream->duration) * av_q2d(state->video_tb);
	} else {
		length = 0.0;
	}

	current_time = 0.0;
	has_next = false;
	next_pts = 0.0;

	return OK;
}

void VideoStreamPlaybackFFMPEG::_build_image(Ref<Image> &r_img) {
	if (!state || !state->has_video) {
		return;
	}
	uint8_t *dst[4] = { state->rgba_buffer, nullptr, nullptr, nullptr };
	int dst_stride[4] = { state->rgba_stride, 0, 0, 0 };
	sws_scale(state->sws_ctx, state->frame->data, state->frame->linesize, 0, state->video_h, dst, dst_stride);

	PackedByteArray data;
	data.resize(state->video_w * state->video_h * 4);
	memcpy(data.ptrw(), state->rgba_buffer, state->video_w * state->video_h * 4);
	r_img = Image::create_from_data(state->video_w, state->video_h, false, Image::FORMAT_RGBA8, data);
}

void VideoStreamPlaybackFFMPEG::_present_frame() {
	Ref<Image> img;
	_build_image(img);
	if (img.is_valid()) {
		texture->update(img);
	}
}

// Reads packets until a single video frame has been decoded. Returns false at end of stream.
bool VideoStreamPlaybackFFMPEG::_read_video_frame(double &r_pts) {
	if (!state || !state->has_video) {
		return false;
	}

	while (true) {
		if (av_read_frame(state->format_ctx, state->packet) < 0) {
			// End of file: flush the decoder to recover any buffered frames.
			avcodec_send_packet(state->video_codec_ctx, nullptr);
			if (avcodec_receive_frame(state->video_codec_ctx, state->frame) >= 0) {
			int64_t pts = state->frame->pts;
			r_pts = (pts == AV_NOPTS_VALUE) ? length : double(pts) * av_q2d(state->video_tb);
				return true;
			}
			state->eof = true;
			return false;
		}

		if (state->packet->stream_index == state->video_stream_idx) {
			if (avcodec_send_packet(state->video_codec_ctx, state->packet) == 0) {
				int r;
				while ((r = avcodec_receive_frame(state->video_codec_ctx, state->frame)) >= 0) {
				int64_t pts = state->frame->pts;
				r_pts = (pts == AV_NOPTS_VALUE) ? current_time : double(pts) * av_q2d(state->video_tb);
					av_packet_unref(state->packet);
					return true;
				}
				// r == AVERROR(EAGAIN) -> need more packets; fall through to keep reading.
			}
		} else if (state->packet->stream_index == state->audio_stream_idx) {
			_decode_audio_packet();
		}

		av_packet_unref(state->packet);
	}
}

void VideoStreamPlaybackFFMPEG::_decode_audio_packet() {
	if (!state || !state->has_audio || state->audio_stream_idx < 0) {
		return;
	}
	if (avcodec_send_packet(state->audio_codec_ctx, state->packet) < 0) {
		return;
	}
	while (avcodec_receive_frame(state->audio_codec_ctx, state->frame) >= 0) {
		int out_samples = swr_get_out_samples(state->swr_ctx, state->frame->nb_samples);
		if (out_samples <= 0) {
			continue;
		}
		int channels = state->audio_channels;
		uint8_t *out_ptr = nullptr;
		// Reserve the worst case; swr writes interleaved FLT (channels wide).
		Vector<float> buf;
		buf.resize(out_samples * channels);
		out_ptr = (uint8_t *)buf.ptrw();
		int converted = swr_convert(state->swr_ctx, &out_ptr, out_samples, (const uint8_t **)state->frame->extended_data, state->frame->nb_samples);
		if (converted > 0) {
			int total = converted * channels;
			int start = state->audio_ring.size();
			state->audio_ring.resize(start + total);
			for (int i = 0; i < total; i++) {
				state->audio_ring.set(start + i, buf[i]);
			}
		}
	}
}

void VideoStreamPlaybackFFMPEG::_push_audio() {
	if (!mix_callback || !state || state->audio_channels <= 0) {
		return;
	}
	int channels = state->audio_channels;
	while (state->audio_ring.size() >= (uint32_t)channels) {
		int avail = state->audio_ring.size() / channels;
		if (avail <= 0) {
			break;
		}
		PackedFloat32Array buf;
		buf.resize(avail * channels);
		for (int i = 0; i < avail * channels; i++) {
			buf.set(i, state->audio_ring[i]);
		}
		int wrote = mix_audio(avail, buf);
		if (wrote <= 0) {
			break;
		}
		int remove_count = wrote * channels;
		int remaining = state->audio_ring.size() - remove_count;
		for (int i = 0; i < remaining; i++) {
			state->audio_ring.set(i, state->audio_ring[i + remove_count]);
		}
		state->audio_ring.resize(remaining);
		if (wrote < avail) {
			break;
		}
	}
}

void VideoStreamPlaybackFFMPEG::update(double p_delta) {
	if (!state || !state->format_ctx) {
		return;
	}
	if (!playing || paused) {
		return;
	}

	current_time += p_delta;

	if (has_next && current_time >= next_pts) {
		texture->update(next_image);
		has_next = false;
		current_time = MAX(current_time, next_pts);
	}

	while (!state->eof && !has_next) {
		double pts = 0.0;
		if (_read_video_frame(pts)) {
			if (pts <= current_time) {
				_present_frame();
			} else {
				_build_image(next_image);
				next_pts = pts;
				has_next = true;
			}
		} else {
			break;
		}
	}

	_push_audio();

	if (state->eof && !has_next && current_time >= length) {
		stop();
	}
}

void VideoStreamPlaybackFFMPEG::play() {
	if (playing || !state || !state->format_ctx) {
		return;
	}
	playing = true;
}

void VideoStreamPlaybackFFMPEG::stop() {
	playing = false;
	seek(0);
}

bool VideoStreamPlaybackFFMPEG::is_playing() const {
	return playing;
}

void VideoStreamPlaybackFFMPEG::set_paused(bool p_paused) {
	paused = p_paused;
}

bool VideoStreamPlaybackFFMPEG::is_paused() const {
	return paused;
}

double VideoStreamPlaybackFFMPEG::get_length() const {
	return length;
}

double VideoStreamPlaybackFFMPEG::get_playback_position() const {
	return current_time;
}

void VideoStreamPlaybackFFMPEG::seek(double p_time) {
	if (!state || !state->format_ctx) {
		return;
	}
	if (p_time >= length) {
		return;
	}

	has_next = false;
	next_pts = 0.0;
	state->eof = false;
	state->audio_ring.clear();

	double tb = av_q2d(state->video_tb);
	int64_t seek_ts = (int64_t)(p_time / tb);
	av_seek_frame(state->format_ctx, state->video_stream_idx, seek_ts, AVSEEK_FLAG_BACKWARD);
	avcodec_flush_buffers(state->video_codec_ctx);
	if (state->audio_codec_ctx) {
		avcodec_flush_buffers(state->audio_codec_ctx);
	}

	current_time = p_time;
}

void VideoStreamPlaybackFFMPEG::set_audio_track(int p_idx) {
	audio_track = p_idx;
}

Ref<Texture2D> VideoStreamPlaybackFFMPEG::get_texture() const {
	return texture;
}

int VideoStreamPlaybackFFMPEG::get_channels() const {
	return state ? state->audio_channels : 0;
}

int VideoStreamPlaybackFFMPEG::get_mix_rate() const {
	return state ? state->audio_sample_rate : 0;
}

void VideoStreamPlaybackFFMPEG::_bind_methods() {}

VideoStreamPlaybackFFMPEG::VideoStreamPlaybackFFMPEG() {
	texture.instantiate();
}

VideoStreamPlaybackFFMPEG::~VideoStreamPlaybackFFMPEG() {
	_clear_state();
}

void VideoStreamFFMPEG::_bind_methods() {}

Ref<VideoStreamPlayback> VideoStreamFFMPEG::instantiate_playback() {
	Ref<VideoStreamPlaybackFFMPEG> pb;
	pb.instantiate();
	pb->set_audio_track(audio_track);
	pb->open_file(file);
	return pb;
}

Ref<Resource> ResourceFormatLoaderFFMPEG::load(const String &p_path, const String &p_original_path, Error *r_error, bool p_use_sub_threads, float *r_progress, CacheMode p_cache_mode) {
	Ref<VideoStreamFFMPEG> stream;
	stream.instantiate();
	stream->set_file(p_path);

	if (r_error) {
		*r_error = OK;
	}
	return stream;
}

void ResourceFormatLoaderFFMPEG::get_recognized_extensions(List<String> *p_extensions) const {
	static const char *exts[] = { "mp4", "mov", "mkv", "webm", "avi", "m4v", "flv", "mpg", "mpeg", "wmv", "3gp", nullptr };
	for (int i = 0; exts[i] != nullptr; i++) {
		p_extensions->push_back(exts[i]);
	}
}

bool ResourceFormatLoaderFFMPEG::handles_type(const String &p_type) const {
	return ClassDB::is_parent_class(p_type, "VideoStream");
}

String ResourceFormatLoaderFFMPEG::get_resource_type(const String &p_path) const {
	if (p_path.get_extension().to_lower() == "mp4" ||
			p_path.get_extension().to_lower() == "mov" ||
			p_path.get_extension().to_lower() == "mkv" ||
			p_path.get_extension().to_lower() == "webm" ||
			p_path.get_extension().to_lower() == "avi" ||
			p_path.get_extension().to_lower() == "m4v" ||
			p_path.get_extension().to_lower() == "flv" ||
			p_path.get_extension().to_lower() == "mpg" ||
			p_path.get_extension().to_lower() == "mpeg" ||
			p_path.get_extension().to_lower() == "wmv" ||
			p_path.get_extension().to_lower() == "3gp") {
		return "VideoStreamFFMPEG";
	}
	return "";
}
