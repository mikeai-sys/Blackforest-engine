@tool
class_name ImpostorBaker
extends RefCounted

const VIEWS := 8


static func bake(source: MeshInstance3D, atlas_size: int = 1024, switch_distance: float = 120.0) -> Node3D:
	if source == null or source.mesh == null:
		push_error("Impostors: source MeshInstance3D with a mesh is required")
		return null

	var aabb := source.get_aabb()
	var radius: float = aabb.size.length() * 0.5
	var center: Vector3 = aabb.get_center()

	var vp := SubViewport.new()
	vp.size = Vector2i(atlas_size // VIEWS, atlas_size // VIEWS)
	vp.transparent_bg = true
	vp.render_target_update_mode = SubViewport.UPDATE_ONCE
	var cam := Camera3D.new()
	cam.projection = Camera3D.PROJECTION_ORTHOGONAL
	cam.size = radius * 2.4
	vp.add_child(cam)
	_add_lights(vp)
	source.get_tree().root.add_child(vp)

	var cell := atlas_size // VIEWS
	var atlas := Image.create_empty(atlas_size, cell, false, Image.FORMAT_RGBA8)
	var gc: Vector3 = source.global_position + center

	for i in VIEWS:
		var angle := TAU * float(i) / float(VIEWS)
		var orbit := radius * 4.0
		cam.global_position = gc + Vector3(sin(angle), 0.22, cos(angle)) * orbit
		cam.look_at(gc, Vector3.UP)
		vp.render_target_update_mode = SubViewport.UPDATE_ONCE
		await RenderingServer.frame_post_draw
		var img := vp.get_texture().get_image()
		atlas.blit_rect(img, Rect2i(Vector2i.ZERO, img.get_size()), Vector2i(cell * i, 0))

	vp.queue_free()

	var tex := ImageTexture.create_from_image(atlas)
	var mat := StandardMaterial3D.new()
	mat.albedo_texture = tex
	mat.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA_SCISSOR
	mat.billboard_mode = BaseMaterial3D.BILLBOARD_FIXED_Y
	mat.cull_mode = BaseMaterial3D.CULL_DISABLED

	var quad := QuadMesh.new()
	quad.size = Vector2(aabb.size.x, aabb.size.y).max(Vector2(0.01, 0.01)) * 1.05
	quad.center_offset = Vector3(0.0, aabb.size.y * 0.5 - aabb.get_center().y, 0.0)

	var imp := MeshInstance3D.new()
	imp.name = "Impostor_%s" % source.name
	imp.mesh = quad
	imp.material_override = mat
	imp.visibility_range_begin = switch_distance
	imp.visibility_range_begin_margin = switch_distance * 0.15
	source.add_child(imp)
	if source.owner:
		imp.owner = source.owner
	return imp


static func _add_lights(vp: SubViewport) -> void:
	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-45.0, 30.0, 0.0)
	vp.add_child(sun)
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color(0, 0, 0, 0)
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.6, 0.65, 0.7)
	env.ambient_light_energy = 1.0
	var wec := WorldEnvironment.new()
	wec.environment = env
	vp.add_child(wec)
