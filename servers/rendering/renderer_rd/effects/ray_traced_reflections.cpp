/**************************************************************************/
/*  ray_traced_reflections.cpp                                            */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */

#include "ray_traced_reflections.h"

#include "servers/rendering/rendering_device.h"
#include "servers/rendering/rendering_device_commons.h"

namespace RendererRD {

RayTracedReflections::RayTracedReflections() {}

RayTracedReflections::~RayTracedReflections() {
	deinit();
}

void RayTracedReflections::init() {
	RenderingDevice *rd = RenderingDevice::get_singleton();
	if (rd) {
		rt_supported = rd->has_feature(RDD::SUPPORTS_RAY_QUERY);
	}
}

void RayTracedReflections::deinit() {
	enabled = false;
}

void RayTracedReflections::process(RID p_render_buffers, RID p_environment) {
	if (!enabled || !rt_supported) {
		return;
	}
}

} // namespace RendererRD
