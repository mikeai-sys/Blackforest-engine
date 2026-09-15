extends SceneTree

func _jsonable(v):
	var t := typeof(v)
	match t:
		TYPE_VECTOR3:
			var vv: Vector3 = v
			return {"x": snappedf(vv.x, 0.001), "y": snappedf(vv.y, 0.001), "z": snappedf(vv.z, 0.001)}
		TYPE_VECTOR2:
			var vv: Vector2 = v
			return {"x": snappedf(vv.x, 0.001), "y": snappedf(vv.y, 0.001)}
		TYPE_COLOR:
			var c: Color = v
			return {"r": snappedf(c.r, 0.001), "g": snappedf(c.g, 0.001), "b": snappedf(c.b, 0.001), "a": snappedf(c.a, 0.001)}
		TYPE_RECT2:
			var r: Rect2 = v
			return {"pos": _jsonable(r.position), "size": _jsonable(r.size)}
		TYPE_TRANSFORM3D:
			var xf: Transform3D = v
			return {"origin": _jsonable(xf.origin), "basis": _jsonable(xf.basis)}
		TYPE_QUATERNION:
			var q: Quaternion = v
			return {"x": snappedf(q.x, 0.001), "y": snappedf(q.y, 0.001), "z": snappedf(q.z, 0.001), "w": snappedf(q.w, 0.001)}
		TYPE_DICTIONARY:
			var d := {}
			for k in v.keys():
				d[k] = _jsonable(v[k])
			return d
		TYPE_ARRAY:
			var arr := []
			for e in v:
				arr.append(_jsonable(e))
			return arr
		TYPE_OBJECT:
			if v == null:
				return null
			return "<%s#%d>" % [v.get_class(), v.get_instance_id()]
		_:
			return v


func _stringify(v) -> String:
	var j := JSON.stringify(_jsonable(v))
	if j != "":
		return j
	return str(v)


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	if args.size() == 0:
		print("BF_EVAL_ERROR no spec file")
		quit()
		return
	var spec = JSON.parse_string(FileAccess.get_file_as_string(args[0]))
	if typeof(spec) != TYPE_DICTIONARY or not spec.has("code"):
		print("BF_EVAL_ERROR spec must be {\"code\": \"...\"}")
		quit()
		return
	var s := GDScript.new()
	s.source_code = spec["code"]
	var err := s.reload(true)
	if err != OK:
		print("BF_EVAL_ERROR script load failed (err %d)" % err)
		quit()
		return
	var obj = s.new()
	if obj == null:
		print("BF_EVAL_ERROR could not instantiate script")
		quit()
		return
	print("BF_EVAL_BEGIN")
	if obj.has_method("main"):
		print(_stringify(obj.main()))
	else:
		print("(no main(); top-level code executed)")
	print("BF_EVAL_END")
	quit()
