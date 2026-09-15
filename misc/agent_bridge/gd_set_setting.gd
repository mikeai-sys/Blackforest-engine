extends SceneTree

func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	var spec = JSON.parse_string(FileAccess.get_file_as_string(args[0]))
	var settings: Dictionary = spec.get("settings", {})
	if settings.size() == 0:
		print("BF_SET_ERROR no settings provided")
		quit()
		return
	var applied := []
	for key in settings.keys():
		var val = settings[key]
		# coerce common JSON types into engine types
		if typeof(val) == TYPE_ARRAY:
			var psa := PackedStringArray()
			for e in val:
				psa.append(str(e))
			ProjectSettings.set_setting(key, psa)
		else:
			ProjectSettings.set_setting(key, val)
		applied.append(key)
	var err := ProjectSettings.save()
	if err != OK:
		print("BF_SET_ERROR save failed (err %d)" % err)
		quit()
		return
	print("BF_SET_OK " + JSON.stringify(applied))
	quit()
