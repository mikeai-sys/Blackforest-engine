extends SceneTree

var _spec := {}
var _events: Array = []
var _errors: Array[String] = []
var _frame: int = 0


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	var scene: String = args[0] if args.size() > 0 else ""
	var spec_path: String = args[1] if args.size() > 1 else ""
	if scene.is_empty() or scene == "MAIN":
		scene = ProjectSettings.get_setting("application/run/main_scene", "")
	if scene.is_empty():
		return _fail("no scene specified and project has no main scene")
	change_scene_to_file(scene)
	var f := FileAccess.open(spec_path, FileAccess.READ)
	if f == null:
		return _fail("cannot open spec file")
	var data = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		return _fail("spec must be a JSON object")
	_spec = data
	_events = data.get("events", [])
	_events.sort_custom(func(a, b): return int(a.get("frame", 0)) < int(b.get("frame", 0)))


func _fail(msg: String) -> void:
	print("BF_SIM_ERROR ", msg)
	quit()


func _process(_delta: float) -> bool:
	_frame += 1
	while _events.size() > 0 and int(_events[0].get("frame", 0)) <= _frame:
		_dispatch(_events.pop_front())
	if _frame < int(_spec.get("frames", 120)):
		return false
	if current_scene == null:
		_fail("scene failed to load")
		return false
	var payload := {
		"frames_ran": _frame,
		"errors": _errors,
		"tree": _dump(current_scene, 0),
	}
	var shot: String = _spec.get("screenshot", "")
	if shot != "" and DisplayServer.get_name() != "headless":
		var img := root.get_texture().get_image()
		var serr := img.save_png(shot)
		payload["screenshot"] = shot
		payload["screenshot_ok"] = serr == OK
	print("BF_SIM_BEGIN")
	print(JSON.stringify(payload, "  "))
	print("BF_SIM_END")
	quit()
	return false


func _dispatch(e: Dictionary) -> void:
	var t: String = e.get("type", "")
	var ev: InputEvent = null
	match t:
		"action":
			var a := InputEventAction.new()
			a.action = String(e.get("action", ""))
			a.pressed = bool(e.get("pressed", true))
			a.strength = float(e.get("strength", 1.0))
			ev = a
		"key":
			var code := OS.find_keycode_from_string(String(e.get("key", "")))
			if code == 0:
				_errors.append("unknown key: %s" % String(e.get("key", "")))
				return
			var k := InputEventKey.new()
			k.keycode = code
			k.physical_keycode = code
			k.pressed = bool(e.get("pressed", true))
			ev = k
		"mouse_motion":
			var m := InputEventMouseMotion.new()
			var p = e.get("position", [0, 0])
			var r = e.get("relative", [0, 0])
			m.position = Vector2(float(p[0]), float(p[1]))
			m.global_position = m.position
			m.relative = Vector2(float(r[0]), float(r[1]))
			ev = m
		"mouse_button":
			var b := InputEventMouseButton.new()
			b.button_index = int(e.get("button", 1))
			b.pressed = bool(e.get("pressed", true))
			var p2 = e.get("position", [0, 0])
			b.position = Vector2(float(p2[0]), float(p2[1]))
			b.global_position = b.position
			ev = b
		_:
			_errors.append("unknown event type: %s" % t)
			return
	Input.parse_input_event(ev)


func _dump(n: Node, depth: int) -> Dictionary:
	var d := {
		"name": String(n.name),
		"class": n.get_class(),
	}
	var s := n.get_script() as Script
	if s != null:
		d["script"] = s.resource_path
	if n is Node3D:
		var t3 := n as Node3D
		d["position"] = _v3(t3.position)
		d["visible"] = t3.visible
	var props := _script_props(n)
	if props.size() > 0:
		d["props"] = props
	if depth < 12:
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
	if v == null or v is float or v is int or v is bool or v is String:
		return v
	return str(v)


func _v3(v: Vector3) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001), "z": snappedf(v.z, 0.001)}


func _v2(v: Vector2) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001)}
