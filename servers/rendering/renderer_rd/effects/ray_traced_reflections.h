/**************************************************************************/
/*  ray_traced_reflections.h                                              */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */

#pragma once

#include "servers/rendering/rendering_device.h"

namespace RendererRD {

class RayTracedReflections {
	bool enabled = false;
	bool rt_supported = false;

public:
	RayTracedReflections();
	~RayTracedReflections();

	void init();
	void deinit();

	bool is_supported() const { return rt_supported; }
	bool is_enabled() const { return enabled; }
	void set_enabled(bool p_enabled) { enabled = p_enabled && rt_supported; }

	void process(RID p_render_buffers, RID p_environment);
};

} // namespace RendererRD
