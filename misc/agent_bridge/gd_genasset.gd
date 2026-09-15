extends SceneTree

var _rng := RandomNumberGenerator.new()


func _initialize() -> void:
	var args := OS.get_cmdline_user_args()
	if args.is_empty():
		return _fail("missing spec path")
	var f := FileAccess.open(args[0], FileAccess.READ)
	if f == null:
		return _fail("cannot open spec file")
	var data = JSON.parse_string(f.get_as_text())
	if typeof(data) != TYPE_DICTIONARY:
		return _fail("spec must be a JSON object")
	_rng.seed = int(data.get("seed", 1337))
	match String(data.get("kind", "")):
		"texture":
			_gen_texture(data)
		"material":
			_gen_material(data)
		_:
			_fail("unknown kind: %s (use texture|material)" % String(data.get("kind", "")))


func _fail(msg: String) -> void:
	print("BF_GEN_ERROR ", msg)
	quit()


func _done(msg: String) -> void:
	print("BF_GEN_OK ", msg)
	quit()


func _color(d, key: String, def: Color) -> Color:
	if d is Dictionary and d.has(key):
		var a = d[key]
		if a is Array and a.size() >= 3:
			return Color(float(a[0]), float(a[1]), float(a[2]), float(a[3]) if a.size() > 3 else 1.0)
	return def


func _lerp_color(a: Color, b: Color, t: float) -> Color:
	return Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t)


func _ensure_parent(path: String) -> void:
	var dir := path.get_base_dir()
	if dir.begins_with("res://"):
		DirAccess.make_dir_recursive_absolute(dir)


func _gen_texture(spec: Dictionary) -> void:
	var size: int = clamp(int(spec.get("size", 256)), 16, 2048)
	var variant: String = spec.get("variant", "noise")
	if not ["checker", "gradient", "radial", "stripes", "bricks", "noise", "normal_map"].has(variant):
		return _fail("unknown texture variant: %s" % variant)
	var out: String = spec.get("output", "")
	_ensure_parent(out)
	var img := Image.create_empty(size, size, false, Image.FORMAT_RGBA8)
	var ca := _color(spec, "color_a", Color(0.55, 0.42, 0.3))
	var cb := _color(spec, "color_b", Color(0.85, 0.8, 0.72))
	for y in size:
		for x in size:
			img.set_pixel(x, y, _pixel(variant, spec, x, y, size, ca, cb))
	if out.is_empty():
		return _fail("missing output")
	var err := img.save_png(out)
	if err != OK:
		return _fail("save_png failed (%d) for %s" % [err, out])
	_done(out)


func _pixel(variant: String, spec: Dictionary, x: int, y: int, size: int, ca: Color, cb: Color) -> Color:
	match variant:
		"checker":
			var cell: int = maxi(1, int(spec.get("cell", size / 8)))
			return ca if ((x / cell) + (y / cell)) % 2 == 0 else cb
		"gradient":
			var dir: String = spec.get("direction", "vertical")
			var t := (float(y) / size) if dir != "horizontal" else (float(x) / size)
			return _lerp_color(ca, cb, t)
		"radial":
			var c := float(size) * 0.5
			var t: float = clamp(Vector2(x - c, y - c).length() / c, 0.0, 1.0)
			return _lerp_color(cb, ca, t)
		"stripes":
			var period: int = maxi(2, int(spec.get("period", size / 8)))
			var w: int = maxi(1, int(spec.get("width", period / 2)))
			var v: bool = spec.get("vertical", true)
			var p := x if v else y
			return ca if p % period < w else cb
		"bricks":
			var rows: int = maxi(1, int(spec.get("rows", 8)))
			var mortar_px: int = maxi(1, int(spec.get("mortar", size / rows / 12)))
			var bh := size / rows
			var bw := bh * 2
			var row := y / bh
			var offset := (row % 2) * (bw / 2)
			var bx := (x + offset) % bw
			var by := y % bh
			if bx < mortar_px or by < mortar_px:
				return _color(spec, "color_mortar", Color(0.75, 0.73, 0.7))
			var j := _brick_tint(row, (x + offset) / bw, spec)
			return _lerp_color(ca, cb, j)
		"noise", "normal_map":
			var v := _sample_noise(spec, x, y)
			if variant == "normal_map":
				var vx := _sample_noise(spec, mini(x + 1, size - 1), y) - _sample_noise(spec, maxi(x - 1, 0), y)
				var vy := _sample_noise(spec, x, mini(y + 1, size - 1)) - _sample_noise(spec, x, maxi(y - 1, 0))
				var strength: float = float(spec.get("strength", 2.0))
				var n := Vector3(-vx * strength, vy * strength, 1.0).normalized()
				return Color(n.x * 0.5 + 0.5, n.y * 0.5 + 0.5, n.z * 0.5 + 0.5, 1.0)
			return _lerp_color(ca, cb, v)
		_:
			return ca


func _brick_tint(row: int, col: int, spec: Dictionary) -> float:
	var s := hash(str(row, ":", col, ":", int(spec.get("seed", 1337))))
	return float(s % 1000) / 1000.0 * float(spec.get("jitter", 0.35))


func _make_noise(spec: Dictionary) -> FastNoiseLite:
	var n := FastNoiseLite.new()
	n.seed = int(spec.get("seed", 1337))
	n.frequency = float(spec.get("frequency", 0.03))
	n.fractal_octaves = int(spec.get("octaves", 4))
	var nt: String = spec.get("noise_type", "perlin")
	match nt:
		"simplex": n.noise_type = FastNoiseLite.TYPE_SIMPLEX
		"cellular": n.noise_type = FastNoiseLite.TYPE_CELLULAR
		"value": n.noise_type = FastNoiseLite.TYPE_VALUE
		_: n.noise_type = FastNoiseLite.TYPE_PERLIN
	return n


var _noise_cache := {}


func _sample_noise(spec: Dictionary, x: int, y: int) -> float:
	if not _noise_cache.has(spec):
		_noise_cache[spec] = _make_noise(spec)
	var raw: float = (_noise_cache[spec] as FastNoiseLite).get_noise_2d(x, y)
	return clamp(raw * 0.5 + 0.5, 0.0, 1.0)


func _gen_material(spec: Dictionary) -> void:
	var m := StandardMaterial3D.new()
	m.albedo_color = _color(spec, "albedo", Color.WHITE)
	if spec.has("metallic"):
		m.metallic = clamp(float(spec["metallic"]), 0.0, 1.0)
	if spec.has("roughness"):
		m.roughness = clamp(float(spec["roughness"]), 0.0, 1.0)
	var tex: String = spec.get("albedo_texture", "")
	if not tex.is_empty():
		var t := load(tex)
		if t is Texture2D:
			m.albedo_texture = t
		else:
			return _fail("albedo_texture not found: %s" % tex)
	var norm: String = spec.get("normal_texture", "")
	if not norm.is_empty():
		var t2 := load(norm)
		if t2 is Texture2D:
			m.normal_enabled = true
			m.normal_texture = t2
			m.normal_scale = float(spec.get("normal_scale", 1.0))
		else:
			return _fail("normal_texture not found: %s" % norm)
	if spec.get("emission", false):
		m.emission_enabled = true
		m.emission = _color(spec, "emission_color", m.albedo_color)
		m.emission_energy_multiplier = float(spec.get("emission_energy", 1.0))
	if spec.has("uv_scale"):
		var u = spec["uv_scale"]
		if u is Array and u.size() >= 2:
			m.uv1_scale = Vector3(float(u[0]), float(u[1]), 1.0)
	var out: String = spec.get("output", "")
	if out.is_empty():
		return _fail("missing output")
	_ensure_parent(out)
	var err := ResourceSaver.save(m, out)
	if err != OK:
		return _fail("ResourceSaver failed (%d)" % err)
	_done(out)
