@tool
extends EditorPlugin

var _dock: Control


func _enter_tree() -> void:
	_dock = preload("res://addons/blackforest_large_worlds/large_worlds_dock.gd").new()
	_dock.name = "LargeWorlds"
	add_control_to_dock(DOCK_SLOT_RIGHT_UR, _dock)


func _exit_tree() -> void:
	if _dock:
		remove_control_from_docks(_dock)
		_dock.queue_free()


func _has_main_screen() -> bool:
	return false


func _get_plugin_name() -> String:
	return "Large Worlds"
