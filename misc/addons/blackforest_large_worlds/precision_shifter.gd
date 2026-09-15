class_name PrecisionShifter
extends Node

signal world_shifted(offset: Vector3)

@export var shift_threshold: float = 4096.0
@export var excluded: Array[NodePath] = []

var total_offset := Vector3.ZERO


func _process(_delta: float) -> void:
	var root3d := get_tree().current_scene as Node3D
	if root3d == null:
		return
	var anchor := _find_anchor(root3d)
	if anchor == null:
		return
	var p: Vector3 = anchor.global_position
	if absf(p.x) < shift_threshold and absf(p.z) < shift_threshold:
		return
	var offset := Vector3(-p.x, 0.0, -p.z)
	_apply(root3d, offset, anchor)
	total_offset += offset
	world_shifted.emit(offset)


func _find_anchor(root3d: Node3D) -> Node3D:
	if not excluded.is_empty():
		var n := get_node_or_null(excluded[0])
		if n is Node3D:
			return n
	var cam := root3d.get_viewport().get_camera_3d()
	return cam


func _apply(node: Node, offset: Vector3, skip: Node3D) -> void:
	for child in node.get_children():
		if child == skip or (skip != null and skip.is_ancestor_of(child)):
			continue
		if child is PrecisionShifter or child is WorldStreamManager:
			continue
		if child is Node3D:
			(child as Node3D).global_position += offset
		_apply(child, offset, skip)
