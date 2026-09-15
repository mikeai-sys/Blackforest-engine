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


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	var spec = JSON.parse_string(FileAccess.get_file_as_string(args[0]))
	var cls: String = spec["class"]
	if not ClassDB.class_exists(cls):
		print("BF_CLASSDOC_ERROR unknown class: " + cls)
		quit()
		return
	var out := {
		"class": cls,
		"parent": ClassDB.get_parent_class(cls),
		"methods": ClassDB.class_get_method_list(cls, true),
		"properties": ClassDB.class_get_property_list(cls, true),
		"signals": ClassDB.class_get_signal_list(cls),
		"constants": ClassDB.class_get_integer_constant_list(cls, true),
	}
	var j := JSON.stringify(_jsonable(out))
	if j == "":
		j = str(out)
	print("BF_CLASSDOC_BEGIN")
	print(j)
	print("BF_CLASSDOC_END")
	quit()
