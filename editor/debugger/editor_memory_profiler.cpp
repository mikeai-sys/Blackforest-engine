/**************************************************************************/
/*  editor_memory_profiler.cpp                                            */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */
/* Copyright (c) 2007-2014 Juan Linietsky, Ariel Manzur.                  */
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

#include "editor_memory_profiler.h"

#include "core/os/os.h"
#include "main/performance.h"

void EditorMemoryProfiler::_build_tree() {
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
		items[e.type] = it;
	}
}

void EditorMemoryProfiler::_update_values() {
	Performance *perf = Performance::get_singleton();
	for (const Entry &e : entries) {
		TreeItem *it = items[e.type];
		if (!it) {
			continue;
		}
		if (perf) {
			double v = perf->get_monitor(e.type);
			if (e.bytes) {
				it->set_text(1, String::humanize_size((uint64_t)MAX(0.0, v)));
			} else {
				it->set_text(1, String::num_int64((int64_t)v));
			}
		} else {
			it->set_text(1, "—");
		}
	}

	TreeItem *total = items[Performance::OBJECT_COUNT];
	if (total) {
		total->set_text(1, String::num_int64(ObjectDB::get_object_count()));
	}
}

void EditorMemoryProfiler::_notification(int p_what) {
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

EditorMemoryProfiler::EditorMemoryProfiler() {
	set_name(TTRC("Memory"));

	entries.push_back({ TTRC("Static Memory"), Performance::MEMORY_STATIC, true });
	entries.push_back({ TTRC("Static Memory Peak"), Performance::MEMORY_STATIC_MAX, true });
	entries.push_back({ TTRC("Message Buffer Peak"), Performance::MEMORY_MESSAGE_BUFFER_MAX, true });
	entries.push_back({ TTRC("Total Objects"), Performance::OBJECT_COUNT, false });
	entries.push_back({ TTRC("Resources"), Performance::OBJECT_RESOURCE_COUNT, false });
	entries.push_back({ TTRC("Nodes"), Performance::OBJECT_NODE_COUNT, false });
	entries.push_back({ TTRC("Orphan Nodes"), Performance::OBJECT_ORPHAN_NODE_COUNT, false });
	entries.push_back({ TTRC("Video Memory"), Performance::RENDER_VIDEO_MEM_USED, true });
	entries.push_back({ TTRC("Texture Memory"), Performance::RENDER_TEXTURE_MEM_USED, true });
	entries.push_back({ TTRC("Buffer Memory"), Performance::RENDER_BUFFER_MEM_USED, true });
	entries.push_back({ TTRC("Physics 2D Objects"), Performance::PHYSICS_2D_ACTIVE_OBJECTS, false });
	entries.push_back({ TTRC("Physics 3D Objects"), Performance::PHYSICS_3D_ACTIVE_OBJECTS, false });

	tree = memnew(Tree);
	tree->set_v_size_flags(SIZE_EXPAND_FILL);
	add_child(tree);

	_build_tree();
	set_process(true);
}
