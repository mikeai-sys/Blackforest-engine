extends SceneTree

var _scene_path: String
var _settle: int
var _max_depth: int
var _frame: int = 0


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	_scene_path = args[0] if args.size() > 0 else ""
	_settle = int(args[1]) if args.size() > 1 else 5
	_max_depth = int(args[2]) if args.size() > 2 else 12
	if _scene_path.is_empty() or _scene_path == "MAIN":
		_scene_path = ProjectSettings.get_setting("application/run/main_scene", "")
	if not _scene_path.is_empty():
		change_scene_to_file(_scene_path)


func _process(_delta: float) -> bool:
	_frame += 1
	if _frame < _settle:
		return false
	if _scene_path.is_empty():
		_fail("no scene specified and project has no main scene")
		return false
	if current_scene == null:
		_fail("scene failed to load: %s" % _scene_path)
		return false
	var payload := {
		"scene": _scene_path,
		"frames_settled": _frame,
		"root": _dump(current_scene, 0),
	}
	print("BF_INSPECT_BEGIN")
	print(JSON.stringify(payload, "  "))
	print("BF_INSPECT_END")
	quit()
	return false


func _fail(msg: String) -> void:
	print("BF_INSPECT_ERROR ", msg)
	quit()


func _dump(n: Node, depth: int) -> Dictionary:
	var d := {
		"name": String(n.name),
		"class": n.get_class(),
	}
	var s := n.get_script() as Script
	if s != null:
		d["script"] = s.resource_path
	if n.get_groups().size() > 0:
		d["groups"] = n.get_groups()
	if n is Node3D:
		var t := n as Node3D
		d["position"] = _v3(t.position)
		d["rotation_degrees"] = _v3(t.rotation_degrees)
		d["scale"] = _v3(t.scale)
		d["visible"] = t.visible
	elif n is Node2D:
		var t2 := n as Node2D
		d["position"] = _v2(t2.position)
		d["rotation_degrees"] = t2.rotation_degrees
		d["scale"] = _v2(t2.scale)
		d["visible"] = t2.visible
	elif n is Control:
		var c := n as Control
		d["position"] = _v2(c.position)
		d["size"] = _v2(c.size)
		d["visible"] = c.visible
	var props := _script_props(n)
	if props.size() > 0:
		d["props"] = props
	if depth < _max_depth:
		var kids := []
		for ch in n.get_children():
			kids.append(_dump(ch, depth + 1))
		if kids.size() > 0:
			d["children"] = kids
	elif n.get_child_count() > 0:
		d["children_truncated"] = n.get_child_count()
	return d


func _script_props(n: Node) -> Dictionary:
	var out := {}
	var s := n.get_script() as Script
	if s == null:
		return out
	for p in s.get_script_property_list():
		var pname: String = p["name"]
		if pname.begins_with("_") or int(p["usage"]) & PROPERTY_USAGE_EDITOR == 0:
			continue
		out[pname] = _plain(n.get(pname))
	return out


func _plain(v) -> Variant:
	if v is Vector3:
		return _v3(v)
	if v is Vector2:
		return _v2(v)
	if v is Color:
		return "#%02x%02x%02x%02x" % [int(v.r8), int(v.g8), int(v.b8), int(v.a8)]
	if v is NodePath:
		return String(v)
	if v is Node:
		return "<%s>" % v.name
	if v is Resource:
		return v.resource_path if v.resource_path != "" else "<%s>" % v.get_class()
	if v == null:
		return null
	if v is float or v is int or v is bool or v is String:
		return v
	if v is Array or v is Dictionary:
		return JSON.parse_string(JSON.stringify(v))
	return str(v)


func _v3(v: Vector3) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001), "z": snappedf(v.z, 0.001)}


func _v2(v: Vector2) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001)}
