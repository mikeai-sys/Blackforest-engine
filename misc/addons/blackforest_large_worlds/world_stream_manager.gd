class_name WorldStreamManager
extends Node3D

signal chunk_loaded(cell: Vector2i)
signal chunk_unloaded(cell: Vector2i)

@export var chunks_dir: String = "res://scenes/chunks"
@export var chunk_size: Vector3 = Vector3(64, 0, 64)
@export var load_radius: int = 2
@export var unload_radius: int = 4
@export var follow_path: NodePath
@export var max_instantiations_per_frame: int = 1

var _loaded: Dictionary = {}
var _pending: Dictionary = {}
var _requested: Dictionary = {}


func _get_follow_position() -> Vector3:
	if not follow_path.is_empty():
		var n := get_node_or_null(follow_path)
		if n is Node3D:
			return (n as Node3D).global_position
	return global_position


func _cell_of(p: Vector3) -> Vector2i:
	return Vector2i(floori(p.x / maxf(chunk_size.x, 0.001)), floori(p.z / maxf(chunk_size.z, 0.001)))


func _chunk_resource(cell: Vector2i) -> String:
	return "%s/chunk_%d_%d.tscn" % [chunks_dir, cell.x, cell.y]


func _process(_delta: float) -> void:
	spawned_this_frame = 0
	var center := _cell_of(_get_follow_position())

	for cell in _pending.keys():
		var status := ResourceLoader.load_threaded_get_status(_pending[cell])
		if status == ResourceLoader.THREAD_LOAD_LOADED:
			var packed: PackedScene = ResourceLoader.load_threaded_get(_pending[cell])
			_pending.erase(cell)
			if packed:
				_instantiate(cell, packed)
			else:
				push_warning("WorldStream: chunk failed to load: %s" % _pending[cell])
		elif status == ResourceLoader.THREAD_LOAD_FAILED or status == ResourceLoader.THREAD_LOAD_INVALID_RESOURCE:
			push_warning("WorldStream: unavailable chunk %s" % _pending[cell])
			_pending.erase(cell)

	var spawned := 0
	var dx := -load_radius
	while dx <= load_radius:
		var dy := -load_radius
		while dy <= load_radius:
			var cell := center + Vector2i(dx, dy)
			if not _loaded.has(cell) and not _requested.has(cell) and not _pending.has(cell):
				var path := _chunk_resource(cell)
				if ResourceLoader.exists(path):
					ResourceLoader.load_threaded_request(path)
					_requested[cell] = path
					_pending[cell] = path
					dx = load_radius + 1
					break
			dy += 1
		dx += 1

	for cell in _loaded.keys():
		var d := Vector2(cell - center)
		if d.length() > float(unload_radius):
			var holder: Node3D = _loaded[cell]
			holder.queue_free()
			_loaded.erase(cell)
			chunk_unloaded.emit(cell)


func _instantiate(cell: Vector2i, packed: PackedScene) -> void:
	if spawned_this_frame >= max_instantiations_per_frame:
		_pending[cell] = _chunk_resource(cell)
		return
	spawned_this_frame += 1
	var inst := packed.instantiate()
	if inst is Node3D:
		var holder := Node3D.new()
		holder.name = "Chunk_%d_%d" % [cell.x, cell.y]
		holder.position = Vector3(
			float(cell.x) * chunk_size.x,
			0.0,
			float(cell.y) * chunk_size.z
		)
		add_child(holder)
		holder.add_child(inst)
		_loaded[cell] = holder
		chunk_loaded.emit(cell)


var spawned_this_frame: int = 0


func unload_all() -> void:
	for cell in _loaded.keys():
		(_loaded[cell] as Node3D).queue_free()
	_loaded.clear()
	_pending.clear()
	_requested.clear()


func loaded_cells() -> Array:
	return _loaded.keys()
