/**************************************************************************/
/*  virtual_shadow_maps.cpp                                               */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://blackforestengine.org                    */
/**************************************************************************/
/* Copyright (c) 2014-present BlackForest Engine contributors (see AUTHORS.md). */

#include "virtual_shadow_maps.h"

namespace RendererRD {

VirtualShadowMaps::VirtualShadowMaps() {}

VirtualShadowMaps::~VirtualShadowMaps() {
	deinit();
}

void VirtualShadowMaps::init() {}

void VirtualShadowMaps::deinit() {
	enabled = false;
}

void VirtualShadowMaps::process() {
	if (!enabled) {
		return;
	}
}

} // namespace RendererRD
