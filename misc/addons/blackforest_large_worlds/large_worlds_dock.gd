@tool
extends VBoxContainer

const HLODBaker := preload("res://addons/blackforest_large_worlds/hlod_baker.gd")
const ImpostorBaker := preload("res://addons/blackforest_large_worlds/impostor_baker.gd")

var _distance: SpinBox
var _log: Label


func _ready() -> void:
	set_custom_minimum_size(Vector2(240, 0))
	add_theme_constant_override("separation", 8)

	var title := Label.new()
	title.text = "Large Worlds"
	add_child(title)

	_distance = SpinBox.new()
	_distance.max_value = 100000.0
	_distance.value = 120.0
	_distance.suffix = " m"
	add_child(_labeled("HLOD / impostor switch distance", _distance))

	_add_button("Bake HLOD district (selection)", func() -> void: _run_hlod())
	_add_button("Bake billboard impostor (selection)", func() -> void: _run_impostor())
	_add_button("Clear HLOD on selection", func() -> void: _run_clear())

	_log = Label.new()
	_log.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	_log.custom_minimum_size = Vector2(0, 64)
	add_child(_log)


func _labeled(text: String, control: Control) -> Control:
	var v := VBoxContainer.new()
	v.add_child(Label.new())
	(v.get_child(0) as Label).text = text
	v.add_child(control)
	return v


func _add_button(text: String, action: Callable) -> void:
	var b := Button.new()
	b.text = text
	b.pressed.connect(action)
	add_child(b)


func _selection() -> Node3D:
	var sel := EditorInterface.get_selection().get_selected_nodes()
	for n in sel:
		if n is Node3D:
			return n
	return null


func _run_hlod() -> void:
	var sel := _selection()
	if sel == null:
		_say("Select a Node3D district root first.")
		return
	var hlod := HLODBaker.bake_district(sel, _distance.value)
	_say("HLOD baked for %s." % sel.name if hlod else "HLOD failed: no meshes under selection.")


func _run_impostor() -> void:
	var sel := _selection()
	if sel == null:
		_say("Select a MeshInstance3D first.")
		return
	await ImpostorBaker.bake(sel, 1024, _distance.value)
	_say("Impostor baked for %s." % sel.name)


func _run_clear() -> void:
	var sel := _selection()
	if sel == null:
		_say("Select the district root first.")
		return
	HLODBaker.clear_district(sel)
	_say("HLOD cleared on %s." % sel.name)


func _say(text: String) -> void:
	_log.text = text
