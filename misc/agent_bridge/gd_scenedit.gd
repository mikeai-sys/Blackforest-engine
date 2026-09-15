extends SceneTree

var _scene_path: String
var _ops: Array = []
var _errors: Array[String] = []
var _frame: int = 0


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	_scene_path = args[0] if args.size() > 0 else ""
	if _scene_path.is_empty() or _scene_path == "MAIN":
		_scene_path = ProjectSettings.get_setting("application/run/main_scene", "")
	if _scene_path.is_empty():
		return _fail("no scene specified and project has no main scene")
	change_scene_to_file(_scene_path)
	if args.size() > 1:
		var f := FileAccess.open(args[1], FileAccess.READ)
		if f == null:
			return _fail("cannot open ops file")
		var data = JSON.parse_string(f.get_as_text())
		if data is Array:
			_ops = data
		else:
			return _fail("ops file must contain a JSON array")


func _fail(msg: String) -> void:
	print("BF_SCENEDIT_ERROR ", msg)
	quit()


func _process(_delta: float) -> bool:
	_frame += 1
	if _frame < 3:
		return false
	if current_scene == null:
		_fail("scene failed to load: %s" % _scene_path)
		return false
	for op in _ops:
		if op is Dictionary:
			_apply(op)
		else:
			_errors.append("op is not an object")
	var ok := _errors.is_empty()
	if ok:
		var ps := PackedScene.new()
		var perr := ps.pack(current_scene)
		if perr != OK:
			_errors.append("pack failed (%d)" % perr)
		else:
			var serr := ResourceSaver.save(ps, _scene_path)
			if serr != OK:
				_errors.append("save failed (%d)" % serr)
	var payload := {
		"scene": _scene_path,
		"applied": _ops.size(),
		"ok": _errors.is_empty(),
		"errors": _errors,
		"tree": _dump(current_scene, 0),
	}
	print("BF_SCENEDIT_BEGIN")
	print(JSON.stringify(payload, "  "))
	print("BF_SCENEDIT_END")
	quit()
	return false


func _apply(op: Dictionary) -> void:
	var kind: String = op.get("op", "")
	match kind:
		"add":
			_op_add(op)
		"update":
			_op_update(op)
		"remove":
			_op_remove(op)
		"connect":
			_op_connect(op)
		_:
			_errors.append("unknown op: %s" % kind)


func _node(ref) -> Node:
	if ref == null:
		return current_scene
	var n := current_scene.get_node_or_null(NodePath(String(ref)))
	if n == null:
		_errors.append("node not found: %s" % String(ref))
	return n


func _op_add(op: Dictionary) -> void:
	var type: String = op.get("type", "")
	if type.is_empty() or not ClassDB.class_exists(type):
		_errors.append("add: unknown type '%s'" % type)
		return
	var n: Node = ClassDB.instantiate(type)
	if n == null or not (n is Node):
		_errors.append("add: type '%s' is not a Node" % type)
		if n != null:
			n.free()
		return
	n.name = op.get("name", String(type))
	var script: String = op.get("script", "")
	if not script.is_empty():
		var s := load(script)
		if s is Script:
			n.set_script(s)
		else:
			_errors.append("add: script not found: %s" % script)
	for prop in op.get("props", {}):
		n.set(prop, _conv(op["props"][prop]))
	var parent := _node(op.get("parent"))
	if parent == null:
		n.free()
		return
	parent.add_child(n)
	n.owner = current_scene


func _op_update(op: Dictionary) -> void:
	var n := _node(op.get("node"))
	if n == null:
		return
	if op.has("script"):
		var s: Variant = op["script"]
		if s == null or s == "":
			n.set_script(null)
		elif load(String(s)) is Script:
			n.set_script(load(String(s)))
		else:
			_errors.append("update: script not found: %s" % String(s))
	for prop in op.get("props", {}):
		if not _has_property(n, prop):
			_errors.append("update: '%s' has no property '%s'" % [n.name, prop])
			continue
		n.set(prop, _conv(op["props"][prop]))


func _has_property(n: Node, prop: String) -> bool:
	for p in n.get_property_list():
		if p["name"] == prop:
			return true
	return false


func _op_remove(op: Dictionary) -> void:
	var n := _node(op.get("node"))
	if n == null:
		return
	n.get_parent().remove_child(n)
	n.free()


func _op_connect(op: Dictionary) -> void:
	var src := _node(op.get("node"))
	var tgt := _node(op.get("target"))
	if src == null or tgt == null:
		return
	var sig: String = op.get("signal", "")
	var method: String = op.get("method", "")
	if sig.is_empty() or method.is_empty():
		_errors.append("connect: need signal and method")
		return
	if not src.has_signal(sig):
		_errors.append("connect: '%s' has no signal '%s'" % [src.name, sig])
		return
	var e := src.connect(sig, Callable(tgt, method))
	if e != OK:
		_errors.append("connect failed (%d)" % e)


func _conv(v):
	if v is Dictionary:
		if v.has("v3"):
			var a = v["v3"]
			return Vector3(float(a[0]), float(a[1]), float(a[2]))
		if v.has("v2"):
			var a2 = v["v2"]
			return Vector2(float(a2[0]), float(a2[1]))
		if v.has("color"):
			var c = v["color"]
			return Color(float(c[0]), float(c[1]), float(c[2]), float(c[3]) if c.size() > 3 else 1.0)
		if v.has("res"):
			var r := load(String(v["res"]))
			if r == null:
				_errors.append("resource not found: %s" % String(v["res"]))
			return r
		if v.has("npath"):
			return NodePath(String(v["npath"]))
		if v.has("boxmesh"):
			var bm := BoxMesh.new()
			var bs = v["boxmesh"]
			if bs is Array and bs.size() >= 3:
				bm.size = Vector3(float(bs[0]), float(bs[1]), float(bs[2]))
			return bm
		if v.has("spheremesh"):
			var sm := SphereMesh.new()
			var ss = v["spheremesh"]
			if ss is Array and ss.size() >= 2:
				sm.radius = float(ss[0])
				sm.height = float(ss[1])
			return sm
		if v.has("cylinder"):
			var cm := CylinderMesh.new()
			var cs = v["cylinder"]
			if cs is Array and cs.size() >= 3:
				cm.top_radius = float(cs[0])
				cm.bottom_radius = float(cs[1])
				cm.height = float(cs[2])
			return cm
		if v.has("capsule"):
			var pm := CapsuleMesh.new()
			var ps = v["capsule"]
			if ps is Array and ps.size() >= 2:
				pm.radius = float(ps[0])
				pm.height = float(ps[1])
			return pm
		if v.has("plane"):
			var plm := PlaneMesh.new()
			var pls = v["plane"]
			if pls is Array and pls.size() >= 2:
				plm.size = Vector2(float(pls[0]), float(pls[1]))
			return plm
		if v.has("boxshape"):
			var bs3 := BoxShape3D.new()
			var bss = v["boxshape"]
			if bss is Array and bss.size() >= 3:
				bs3.size = Vector3(float(bss[0]), float(bss[1]), float(bss[2]))
			return bs3
		if v.has("sphereshape"):
			var ss3 := SphereShape3D.new()
			var sss = v["sphereshape"]
			if sss is Array and sss.size() >= 1:
				ss3.radius = float(sss[0])
			return ss3
		if v.has("capsuleshape"):
			var cs3 := CapsuleShape3D.new()
			var css = v["capsuleshape"]
			if css is Array and css.size() >= 2:
				cs3.radius = float(css[0])
				cs3.height = float(css[1])
			return cs3
	return v


func _dump(n: Node, depth: int) -> Dictionary:
	var d := {
		"name": String(n.name),
		"class": n.get_class(),
	}
	var s := n.get_script() as Script
	if s != null:
		d["script"] = s.resource_path
	if n is Node3D:
		var t := n as Node3D
		d["position"] = _v3(t.position)
		d["visible"] = t.visible
	elif n is Control:
		var ctl := n as Control
		d["size"] = _v2(ctl.size)
	var mesh: Mesh = null
	if n is MeshInstance3D:
		mesh = (n as MeshInstance3D).mesh
		var mat_override: Material = (n as MeshInstance3D).get_surface_override_material(0)
		if mat_override != null:
			d["material"] = mat_override.resource_path if mat_override.resource_path != "" else mat_override.resource_name
	if mesh != null:
		d["mesh"] = mesh.resource_path if mesh.resource_path != "" else "<embedded>"
	if depth < 12:
		var kids := []
		for ch in n.get_children():
			kids.append(_dump(ch, depth + 1))
		if kids.size() > 0:
			d["children"] = kids
	elif n.get_child_count() > 0:
		d["children_truncated"] = n.get_child_count()
	return d


func _v3(v: Vector3) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001), "z": snappedf(v.z, 0.001)}


func _v2(v: Vector2) -> Dictionary:
	return {"x": snappedf(v.x, 0.001), "y": snappedf(v.y, 0.001)}
