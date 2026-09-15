/**************************************************************************/
/*  virtual_shadow_maps.h                                                 */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */

#pragma once

#include "core/object/object.h"

namespace RendererRD {

class VirtualShadowMaps {
	bool enabled = false;
	uint32_t page_size = 128;
	uint32_t max_pages = 4096;

public:
	VirtualShadowMaps();
	~VirtualShadowMaps();

	void init();
	void deinit();

	bool is_enabled() const { return enabled; }
	void set_enabled(bool p_enabled) { enabled = p_enabled; }

	uint32_t get_page_size() const { return page_size; }
	void set_page_size(uint32_t p_size) { page_size = p_size; }

	void process();
};

} // namespace RendererRD
