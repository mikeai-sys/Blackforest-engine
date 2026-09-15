# BlackForest Large Worlds

Open-world toolkit for building big 3D games. Install by copying this folder to
`res://addons/blackforest_large_worlds/` and enabling it in Project Settings →
Plugins (or ask your agent to call `bf_create_project` with
`install_large_worlds: true`).

## Components

### WorldStreamManager (runtime)
Grid chunk streamer. Author chunks as `chunk_<x>_<z>.tscn` under
`res://scenes/chunks/`, each built around the origin. Drop a
`WorldStreamManager` node into your world, set `follow_path` to the player,
tune `load_radius`/`unload_radius`. Chunks load threaded and unload behind you;
signals `chunk_loaded`/`chunk_unloaded` are emitted for spawning logic.

### PrecisionShifter (runtime)
Re-centers the world when the player drifts beyond `shift_threshold` from the
origin, keeping floating-point precision stable at 50 km+ play areas. Add it,
optionally set `excluded[0]` to the anchor NodePath (defaults to camera).
Emits `world_shifted(offset)` — physics bodies outside the shifted tree should
listen and rebase themselves.

### HLOD baker (editor)
Select a district root → "Bake HLOD district". Merges every static mesh under
it into one draw-call mesh shown only beyond the switch distance, while source
meshes hide at the same distance (VisibilityRange cross-fade). Result: distant
districts cost one node instead of thousands.

### Impostor baker (editor)
Select a MeshInstance3D → "Bake billboard impostor". Renders an 8-view orbit
atlas via SubViewport and attaches an alpha-scissored Y-billboard quad that
takes over past the switch distance.

## Recommended pipeline

1. Block out districts as static meshes.
2. Bake impostors for hero landmarks, HLOD per district.
3. Split terrain/gameplay into chunks, wire WorldStreamManager + PrecisionShifter.
4. Profile: `blackforest --profiler` or Tracy (`scons profiler=tracy`).
