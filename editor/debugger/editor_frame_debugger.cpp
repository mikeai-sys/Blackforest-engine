/**************************************************************************/
/*  editor_frame_debugger.cpp                                             */
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
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. */
/* NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY      */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE      */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 */
/**************************************************************************/

#include "editor_frame_debugger.h"

#include "core/os/os.h"
#include "servers/rendering/rendering_server.h"

void EditorFrameDebugger::_build_tree() {
	tree->clear();
	tree->set_columns(2);
	tree->set_column_titles_visible(true);
	tree->set_column_title(0, TTRC("Metric"));
	tree->set_column_title(1, TTRC("Value"));
	tree->set_column_expand(0, true);
	tree->set_column_expand(1, false);

	TreeItem *root = tree->create_item();

	for (const Entry &e : entries) {
		TreeItem *it = tree->create_item(root);
		it->set_text(0, e.label);
		it->set_text(1, "—");
		it->set_selectable(0, false);
		it->set_selectable(1, false);
		items[e.info] = it;
	}
}

void EditorFrameDebugger::_update_values() {
	RenderingServer *rs = RenderingServer::get_singleton();
	if (!rs) {
		return;
	}

	for (const Entry &e : entries) {
		TreeItem *it = items[e.info];
		if (!it) {
			continue;
		}
		it->set_text(1, String::num_int64((int64_t)rs->get_rendering_info(e.info)));
	}
}

void EditorFrameDebugger::_notification(int p_what) {
	switch (p_what) {
		case NOTIFICATION_PROCESS: {
			uint64_t now = OS::get_singleton()->get_ticks_usec();
			if (now - last_update >= UPDATE_INTERVAL_US) {
				last_update = now;
				_update_values();
			}
		} break;
		case NOTIFICATION_READY: {
			_update_values();
		} break;
		case NOTIFICATION_THEME_CHANGED: {
			_update_values();
		} break;
	}
}

EditorFrameDebugger::EditorFrameDebugger() {
	set_name(TTRC("Frame"));

	entries.push_back({ TTRC("Objects in Frame"), RSE::RENDERING_INFO_TOTAL_OBJECTS_IN_FRAME });
	entries.push_back({ TTRC("Primitives in Frame"), RSE::RENDERING_INFO_TOTAL_PRIMITIVES_IN_FRAME });
	entries.push_back({ TTRC("Draw Calls in Frame"), RSE::RENDERING_INFO_TOTAL_DRAW_CALLS_IN_FRAME });
	entries.push_back({ TTRC("Texture Memory"), RSE::RENDERING_INFO_TEXTURE_MEM_USED });
	entries.push_back({ TTRC("Buffer Memory"), RSE::RENDERING_INFO_BUFFER_MEM_USED });
	entries.push_back({ TTRC("Video Memory"), RSE::RENDERING_INFO_VIDEO_MEM_USED });
	entries.push_back({ TTRC("Pipeline Compiles (Canvas)"), RSE::RENDERING_INFO_PIPELINE_COMPILATIONS_CANVAS });
	entries.push_back({ TTRC("Pipeline Compiles (Mesh)"), RSE::RENDERING_INFO_PIPELINE_COMPILATIONS_MESH });
	entries.push_back({ TTRC("Pipeline Compiles (Surface)"), RSE::RENDERING_INFO_PIPELINE_COMPILATIONS_SURFACE });
	entries.push_back({ TTRC("Pipeline Compiles (Draw)"), RSE::RENDERING_INFO_PIPELINE_COMPILATIONS_DRAW });
	entries.push_back({ TTRC("Pipeline Compiles (Spec)"), RSE::RENDERING_INFO_PIPELINE_COMPILATIONS_SPECIALIZATION });

	tree = memnew(Tree);
	tree->set_v_size_flags(SIZE_EXPAND_FILL);
	add_child(tree);

	_build_tree();
	set_process(true);
}