@tool
class_name HLODBaker
extends RefCounted

static func bake_district(root: Node3D, switch_distance: float, save_dir: String = "res://scenes/hlod") -> MeshInstance3D:
	var st := SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_TRIANGLES)
	var count := _collect(root, root, st)
	if count == 0:
		push_warning("HLOD: no MeshInstance3D found under %s" % root.name)
		return null
	var mesh := ArrayMesh.new()
	st.commit(mesh)
	st.clear()

	var hlod := MeshInstance3D.new()
	hlod.name = "HLOD_%s" % root.name
	hlod.mesh = mesh
	hlod.visibility_range_end = switch_distance
	hlod.visibility_range_end_margin = switch_distance * 0.1
	root.add_child(hlod)
	hlod.owner = root.owner if root.owner else root

	_mark_sources(root, switch_distance)

	DirAccess.make_dir_recursive_absolute(save_dir.replace("res://", ProjectSettings.globalize_path("res://")))
	var res_path := "%s/%s_hlod.res" % [save_dir, root.name]
	var err := ResourceSaver.save(mesh, res_path)
	if err != OK:
		push_warning("HLOD: could not save merged mesh (%s)" % err)
	return hlod


static func _collect(current: Node, root: Node3D, st: SurfaceTool) -> int:
	var count := 0
	for child in current.get_children():
		if child is MeshInstance3D and not (child as MeshInstance3D).name.begins_with("HLOD_"):
			var mi := child as MeshInstance3D
			if mi.mesh:
				var xform := root.global_transform.affine_inverse() * mi.global_transform
				for s in mi.mesh.get_surface_count():
					st.append_from(mi.mesh, s, xform)
					count += 1
		if child is Node3D:
			count += _collect(child, root, st)
	return count


static func _mark_sources(current: Node, switch_distance: float) -> void:
	for child in current.get_children():
		if child is MeshInstance3D:
			var mi := child as MeshInstance3D
			if not mi.name.begins_with("HLOD_"):
				mi.visibility_range_begin = switch_distance
				mi.visibility_range_begin_margin = switch_distance * 0.1
		if child is Node3D:
			_mark_sources(child, switch_distance)


static func clear_district(root: Node3D) -> void:
	_clear(root)
	for child in root.get_children():
		if child is MeshInstance3D and (child as MeshInstance3D).name.begins_with("HLOD_"):
			child.queue_free()


static func _clear(current: Node) -> void:
	for child in current.get_children():
		if child is MeshInstance3D:
			var mi := child as MeshInstance3D
			if not mi.name.begins_with("HLOD_"):
				mi.visibility_range_begin = 0.0
				mi.visibility_range_begin_margin = 0.0
		if child is Node3D:
			_clear(child)
