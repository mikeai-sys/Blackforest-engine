/**************************************************************************/
/*  video_stream_ffmpeg.h                                                */
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

#ifndef VIDEO_STREAM_FFMPEG_H
#define VIDEO_STREAM_FFMPEG_H

#include "core/io/image.h"
#include "core/io/resource_loader.h"
#include "scene/resources/image_texture.h"
#include "scene/resources/video_stream.h"

// Opaque FFmpeg state, defined in the implementation file.
struct FFMPEGPlaybackState;

class VideoStreamPlaybackFFMPEG : public VideoStreamPlayback {
	GDCLASS(VideoStreamPlaybackFFMPEG, VideoStreamPlayback);

private:
	FFMPEGPlaybackState *state = nullptr;
	double length = 0.0;
	double current_time = 0.0;
	bool playing = false;
	bool paused = false;
	int audio_track = 0;
	Ref<ImageTexture> texture;

	bool has_next = false;
	double next_pts = 0.0;
	Ref<Image> next_image;

	void _build_image(Ref<Image> &r_img);
	void _present_frame();
	bool _read_video_frame(double &r_pts);
	void _decode_audio_packet();
	void _push_audio();
	void _clear_state();

protected:
	static void _bind_methods();

public:
	virtual void stop() override;
	virtual void play() override;
	virtual bool is_playing() const override;
	virtual void set_paused(bool p_paused) override;
	virtual bool is_paused() const override;
	virtual double get_length() const override;
	virtual double get_playback_position() const override;
	virtual void seek(double p_time) override;
	virtual void set_audio_track(int p_idx) override;
	virtual Ref<Texture2D> get_texture() const override;
	virtual void update(double p_delta) override;
	virtual int get_channels() const override;
	virtual int get_mix_rate() const override;

	Error open_file(const String &p_path);

	VideoStreamPlaybackFFMPEG();
	~VideoStreamPlaybackFFMPEG();
};

class VideoStreamFFMPEG : public VideoStream {
	GDCLASS(VideoStreamFFMPEG, VideoStream);

protected:
	static void _bind_methods();

public:
	virtual Ref<VideoStreamPlayback> instantiate_playback() override;

	VideoStreamFFMPEG() {}
	~VideoStreamFFMPEG() {}
};

class ResourceFormatLoaderFFMPEG : public ResourceFormatLoader {
	GDCLASS(ResourceFormatLoaderFFMPEG, ResourceFormatLoader);

public:
	virtual Ref<Resource> load(const String &p_path, const String &p_original_path = "", Error *r_error = nullptr, bool p_use_sub_threads = false, float *r_progress = nullptr, CacheMode p_cache_mode = CACHE_MODE_REUSE) override;
	virtual void get_recognized_extensions(List<String> *r_extensions) const override;
	virtual bool handles_type(const String &p_type) const override;
	virtual String get_resource_type(const String &p_path) const override;
};

#endif // VIDEO_STREAM_FFMPEG_H
