extends SceneTree

var _shot_path: String
var _target_frame: int


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	_shot_path = args[0] if args.size() > 0 else "user://agent_screenshot.png"
	_target_frame = int(args[1]) if args.size() > 1 else 30
	var scene := args[2] if args.size() > 2 else ""
	if scene.is_empty() or scene == "MAIN":
		scene = ProjectSettings.get_setting("application/run/main_scene", "")
	if not scene.is_empty():
		change_scene_to_file(scene)


func _process(_delta: float) -> bool:
	if Engine.get_frames_drawn() >= _target_frame:
		var img := root.get_texture().get_image()
		img.save_png(_shot_path)
		print("BF_CAPTURE_SAVED ", _shot_path)
		quit()
	return false
