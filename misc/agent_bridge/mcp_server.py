#!/usr/bin/env python3
"""BlackForest Engine MCP bridge.

Model Context Protocol stdio server exposing BlackForest Engine capabilities
to AI agents: project scaffolding, file authoring, headless validation,
scene execution and screenshot capture.
"""

import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
import time

PROTOCOL_VERSION = "2024-11-05"
SERVER_NAME = "blackforest-agent-bridge"
SERVER_VERSION = "1.0.0"

ROOT = os.environ.get("BF_WORKSPACE", os.getcwd()).rstrip(os.sep)


def _find_engine():
    here = os.path.dirname(os.path.abspath(__file__))
    candidates = []
    env_bin = os.environ.get("BLACKFOREST_BIN")
    if env_bin:
        candidates.append(env_bin)
    # repo build (misc/agent_bridge -> godot/bin/blackforest)
    candidates.append(os.path.normpath(os.path.join(here, "..", "..", "bin", "blackforest")))
    # well-known dev locations
    candidates.append("/home/user/labs/godot/bin/blackforest")
    candidates.append("/home/user/labs/godot/bin/blackforest.linuxbsd.editor.x86_64")
    candidates.append("/home/user/labs/godot/bin/blackforest.linuxbsd.editor.x86_64.mono")
    candidates.append("/home/user/labs/sunshine/cmake-build-debug/blackforest")
    on_path = shutil.which("blackforest")
    if on_path:
        candidates.append(on_path)
    seen = set()
    for candidate in candidates:
        c = os.path.realpath(candidate)
        if c in seen:
            continue
        seen.add(c)
        try:
            out = subprocess.run([c, "--version"], capture_output=True, text=True, timeout=15)
            if out.returncode == 0 and re.search(r"\d+\.\d+", out.stdout):
                return c
        except Exception:
            continue
    return None


ENGINE_BIN = _find_engine()


def err(msg):
    _emit({"jsonrpc": "2.0", "id": None, "error": {"code": -32603, "message": msg}})


def _emit(obj):
    sys.stdout.write(json.dumps(obj) + "\n")
    sys.stdout.flush()


def resolve(path):
    p = path if os.path.isabs(path) else os.path.join(ROOT, path)
    p = os.path.realpath(p)
    if not (p == ROOT or p.startswith(ROOT + os.sep)):
        raise ValueError(f"path escapes workspace: {path}")
    return p


def engine(args, cwd, timeout=120):
    global ENGINE_BIN
    if not ENGINE_BIN:
        ENGINE_BIN = _find_engine()
    if not ENGINE_BIN:
        return 127, "engine binary not found; set BLACKFOREST_BIN or put blackforest on PATH"
    # Prefer engine-bundled shared libs so a system ffmpeg update cannot break
    # the editor binary. Libs ship alongside this bridge (engine_libs/) and are
    # committed to the repo; fall back to bin/libs next to the binary if present.
    env = dict(os.environ)
    libs = os.path.join(os.path.dirname(os.path.realpath(__file__)), "engine_libs")
    if not os.path.isdir(libs):
        libs = os.path.join(os.path.dirname(os.path.realpath(ENGINE_BIN)), "libs")
    if os.path.isdir(libs):
        env["LD_LIBRARY_PATH"] = libs + (":" + env["LD_LIBRARY_PATH"] if env.get("LD_LIBRARY_PATH") else "")
    try:
        proc = subprocess.run(
            [ENGINE_BIN] + args,
            cwd=cwd,
            capture_output=True,
            text=True,
            timeout=timeout,
            env=env,
        )
        return proc.returncode, (proc.stdout or "") + (proc.stderr or "")
    except subprocess.TimeoutExpired:
        return 124, f"timed out after {timeout}s"


TOOL_DEFS = [
    {
        "name": "bf_create_project",
        "description": "Create a new BlackForest game project directory with project.godot.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "path": {"type": "string", "description": "Project dir relative to workspace"},
                "name": {"type": "string", "description": "Game title"},
                "main_scene": {"type": "string", "description": "Optional res:// main scene path"},
                "install_large_worlds": {"type": "boolean", "description": "Bundle the Large Worlds addon"},
            },
            "required": ["path", "name"],
        },
    },
    {
        "name": "bf_list_files",
        "description": "List files in the project recursively.",
        "inputSchema": {
            "type": "object",
            "properties": {"path": {"type": "string"}},
            "required": ["path"],
        },
    },
    {
        "name": "bf_read_file",
        "description": "Read a text file from the project (.tscn/.gd/.godot etc).",
        "inputSchema": {
            "type": "object",
            "properties": {"path": {"type": "string"}},
            "required": ["path"],
        },
    },
    {
        "name": "bf_write_file",
        "description": "Write/create a text file in the project (scenes .tscn, scripts .gd, resources .tres).",
        "inputSchema": {
            "type": "object",
            "properties": {"path": {"type": "string"}, "content": {"type": "string"}},
            "required": ["path", "content"],
        },
    },
    {
        "name": "bf_validate_project",
        "description": "Import + validate the project headlessly; returns parse/load errors so agents can fix them.",
        "inputSchema": {
            "type": "object",
            "properties": {"path": {"type": "string"}},
            "required": ["path"],
        },
    },
    {
        "name": "bf_run_scene",
        "description": "Run a scene headless for a bounded number of frames and capture output/errors.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "scene": {"type": "string", "description": "res:// scene path; empty = main scene"},
                "frames": {"type": "integer", "description": "Frames to run before quit (default 60)"},
            },
            "required": ["project_path"],
        },
    },
    {
        "name": "bf_screenshot_scene",
        "description": "Run a scene with GPU rendering, capture a viewport screenshot after N frames, save PNG in project, return its path.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "scene": {"type": "string"},
                "frames": {"type": "integer", "default": 30},
                "width": {"type": "integer", "default": 1280},
                "height": {"type": "integer", "default": 720},
            },
            "required": ["project_path"],
        },
    },
    {
        "name": "bf_inspect_scene",
        "description": "Load a scene headlessly, let it settle for N frames, then dump the live node tree as JSON (classes, scripts, transforms, visibility, script properties). Use to verify hierarchy/runtime state instead of guessing from .tscn text.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "scene": {"type": "string", "description": "res:// scene path; empty = main scene"},
                "settle_frames": {"type": "integer", "description": "Frames to run before dumping (default 5)"},
                "max_depth": {"type": "integer", "description": "Max tree depth (default 12; deeper nodes counted as children_truncated)"},
            },
            "required": ["project_path"],
        },
    },
    {
        "name": "bf_generate_texture",
        "description": "Generate a procedural texture PNG in the project via the engine: variants noise|normal_map|checker|gradient|radial|stripes|bricks with colors/params. Returns the res:// path of the saved PNG.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "output": {"type": "string", "description": "res:// path for the PNG, e.g. res://assets/brick_albedo.png"},
                "variant": {"type": "string", "enum": ["noise", "normal_map", "checker", "gradient", "radial", "stripes", "bricks"]},
                "size": {"type": "integer", "default": 256},
                "color_a": {"type": "array", "items": {"type": "number"}, "description": "[r,g,b,(a)] 0..1"},
                "color_b": {"type": "array", "items": {"type": "number"}},
                "color_mortar": {"type": "array", "items": {"type": "number"}, "description": "bricks only"},
                "seed": {"type": "integer", "default": 1337},
                "frequency": {"type": "number", "description": "noise frequency (default 0.03)"},
                "octaves": {"type": "integer", "description": "noise octaves (default 4)"},
                "noise_type": {"type": "string", "enum": ["perlin", "simplex", "cellular", "value"]},
                "strength": {"type": "number", "description": "normal_map bump strength (default 2)"},
                "cell": {"type": "integer", "description": "checker cell px"},
                "rows": {"type": "integer", "description": "bricks rows (default 8)"},
                "mortar": {"type": "integer", "description": "bricks mortar px"},
                "period": {"type": "integer", "description": "stripes period px"},
                "width": {"type": "integer", "description": "stripes line width px"},
                "vertical": {"type": "boolean", "description": "stripes orientation"},
                "direction": {"type": "string", "enum": ["horizontal", "vertical"], "description": "gradient direction"},
            },
            "required": ["project_path", "output", "variant"],
        },
    },
    {
        "name": "bf_create_material",
        "description": "Create a StandardMaterial3D .tres in the project, optionally referencing generated textures; supports albedo/metallic/roughness/emission/uv_scale/normal map.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "output": {"type": "string", "description": "res:// path for the .tres"},
                "albedo": {"type": "array", "items": {"type": "number"}, "description": "[r,g,b,(a)] 0..1"},
                "metallic": {"type": "number"},
                "roughness": {"type": "number"},
                "albedo_texture": {"type": "string", "description": "res:// PNG path"},
                "normal_texture": {"type": "string", "description": "res:// normal-map PNG path"},
                "normal_scale": {"type": "number"},
                "emission": {"type": "boolean"},
                "emission_color": {"type": "array", "items": {"type": "number"}},
                "emission_energy": {"type": "number"},
                "uv_scale": {"type": "array", "items": {"type": "number"}, "description": "[u,v] tiling"},
            },
            "required": ["project_path", "output"],
        },
    },
    {
        "name": "bf_scene_edit",
        "description": "Structured live edit of a scene: add/update/remove nodes, set typed properties, attach scripts, connect signals; then saves the scene and returns the resulting tree JSON. Preferred over hand-editing .tscn text.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "scene": {"type": "string", "description": "res:// scene path; empty = main scene"},
                "ops": {
                    "type": "array",
                    "description": "Ops applied in order. add:{op,type,name,parent(res-path or omit=scene root),props{},script}. update:{op,node,props{},script}. remove:{op,node}. connect:{op,node,signal,target,method}. Typed values: {\"v3\":[x,y,z]} {\"v2\":[x,y]} {\"color\":[r,g,b,a]} {\"res\":\"res://path\"} {\"npath\":\"..\"}; mesh/shape constructors {\"boxmesh\":[x,y,z]} {\"spheremesh\":[r,h]} {\"cylinder\":[topR,botR,h]} {\"capsule\":[r,h]} {\"plane\":[x,z]} {\"boxshape\":[x,y,z]} {\"sphereshape\":[r]} {\"capsuleshape\":[r,h]}; plain numbers/strings/bools pass through.",
                    "items": {"type": "object"},
                },
            },
            "required": ["project_path", "scene", "ops"],
        },
    },
    {
        "name": "bf_simulate_input",
        "description": "Playtest: run a scene and inject input events (keys, actions, mouse) on a frame timeline via the real input pipeline, then dump the final live node tree (positions/props) as JSON. Optionally save a screenshot. Use to verify gameplay behavior (movement, triggers, interactions) end-to-end.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "scene": {"type": "string", "description": "res:// scene path; empty = main scene"},
                "frames": {"type": "integer", "description": "Total frames to run (default 120, ~2s at 60fps)"},
                "screenshot": {"type": "string", "description": "Optional res:// PNG path; runs with rendering when set"},
                "events": {
                    "type": "array",
                    "description": "Sorted-by-frame input events. {frame,type:\"key\",key:\"W\",pressed:true} | {frame,type:\"action\",action:\"jump\",pressed:true} | {frame,type:\"mouse_motion\",position:[x,y],relative:[dx,dy]} | {frame,type:\"mouse_button\",button:1,pressed:true,position:[x,y]}",
                    "items": {"type": "object"},
                },
            },
            "required": ["project_path"],
        },
    },
    {
        "name": "bf_eval",
        "description": "Run arbitrary GDScript in the engine headless and return its output. The code is a script with an optional func main() that returns a value (printed as JSON), or top-level print() calls (captured on stdout). Use for engine API experiments, math, ClassDB introspection, custom logic, or anything not covered by a dedicated tool. Powerful but trust the engine to execute exactly what you write.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "code": {"type": "string", "description": "GDScript source. Define func main() -> Variant to return a value, or use print() for output."},
            },
            "required": ["project_path", "code"],
        },
    },
    {
        "name": "bf_class_doc",
        "description": "Introspect the engine API at runtime: given a class name, returns its parent, methods (name/args/return/flags), properties, signals and integer constants via ClassDB. Use this to write correct GDScript without guessing signatures.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "class": {"type": "string", "description": "Engine class, e.g. CharacterBody3D, Area3D, StandardMaterial3D, Node3D"},
            },
            "required": ["project_path", "class"],
        },
    },
    {
        "name": "bf_import_asset",
        "description": "Import assets into a Godot project and generate .import caches. Two modes: (1) provide 'source' (absolute path on disk) + 'output' (res:// dest) to COPY an external file in and import it; (2) omit 'source' (optionally set 'output' to a res:// subpath) to RE-IMPORT resources already inside the project in place, regenerating .import caches for models/textures that already live in res://. Returns the resulting res:// path or a re-import confirmation. Use mode (2) when assets are already in the project but load with 'no resource loaders / unrecognized extension' because the .import cache is missing.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "source": {"type": "string", "description": "Absolute path to an EXTERNAL source file. Omit to re-import assets already inside the project."},
                "output": {"type": "string", "description": "res:// destination, e.g. res://assets/models/hero.glb. Optional when source is omitted."},
            },
            "required": ["project_path"],
        },
    },
    {
        "name": "bf_set_setting",
        "description": "Set one or more project.godot settings via ProjectSettings (e.g. application/run/main_scene, rendering/renderer/rendering_method, application/config/features). Saves project.godot. Values may be strings, numbers, bools, or string arrays (PackedStringArray).",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "settings": {"type": "object", "description": "Map of setting key -> value"},
            },
            "required": ["project_path", "settings"],
        },
    },
    {
        "name": "bf_export",
        "description": "Build a shippable game via the editor headless export. Requires an export preset in project.godot (add one with bf_set_setting or the editor). Best-effort: reports clearly if export templates are missing.",
        "inputSchema": {
            "type": "object",
            "properties": {
                "project_path": {"type": "string"},
                "preset": {"type": "string", "description": "Export preset name, e.g. Linux/X11"},
                "output": {"type": "string", "description": "Output path, e.g. build/linux/mygame.x86_64"},
            },
            "required": ["project_path", "preset", "output"],
        },
    },
]


def t_create_project(a):
    proj = resolve(a["path"])
    name = a["name"]
    os.makedirs(os.path.join(proj, "scenes"), exist_ok=True)
    os.makedirs(os.path.join(proj, "scripts"), exist_ok=True)
    cfg = (
        "config_version=5\n\n"
        "[application]\n\n"
        f'config/name="{name}"\n'
        "config/features=PackedStringArray(\"4\", \"Forward Plus\")\n"
    )
    ms = a.get("main_scene")
    if ms:
        cfg += f"run/main_scene=\"{ms}\"\n"
    addons = ""
    if a.get("install_large_worlds"):
        src = os.path.join(os.path.dirname(__file__), "..", "addons", "blackforest_large_worlds")
        dst = os.path.join(proj, "addons", "blackforest_large_worlds")
        os.makedirs(dst, exist_ok=True)
        for fn in os.listdir(src):
            if fn.endswith((".gd", ".cfg")):
                shutil.copy(os.path.join(src, fn), dst)
        addons = "\n[editor_plugins]\n\nenabled=PackedStringArray(\"res://addons/blackforest_large_worlds/plugin.cfg\")\n"
    with open(os.path.join(proj, "project.godot"), "w") as f:
        f.write(cfg + addons)
    return f"created project '{name}' at {a['path']}"


def t_list_files(a):
    proj = resolve(a["path"])
    out = []
    skip = {".godot", ".git"}
    for base, dirs, files in os.walk(proj):
        dirs[:] = [d for d in dirs if d not in skip]
        for fn in files:
            out.append(os.path.relpath(os.path.join(base, fn), proj))
    return "\n".join(sorted(out))[:100000]


def t_read_file(a):
    p = resolve(a["path"])
    with open(p, "r", encoding="utf-8", errors="replace") as f:
        return f.read()[:200000]


def t_write_file(a):
    p = resolve(a["path"])
    os.makedirs(os.path.dirname(p) or ROOT, exist_ok=True)
    with open(p, "w", encoding="utf-8") as f:
        f.write(a["content"])
    return f"wrote {len(a['content'])} bytes to {a['path']}"


def t_validate_project(a):
    proj = resolve(a["path"])
    code, log = engine(["--headless", "--import", "--quit-after", "2"], proj, timeout=300)
    interesting = [
        ln for ln in log.splitlines()
        if any(k in ln.lower() for k in ("error", "parse", "failed", "invalid", "cannot"))
    ]
    status = "OK" if code == 0 else f"EXIT {code}"
    body = "\n".join(interesting[-80:]) or "no errors detected"
    return f"validation: {status}\n{body}"


def t_run_scene(a):
    proj = resolve(a["project_path"])
    args = ["--headless", "--quit-after", str(int(a.get("frames", 60)))]
    if a.get("scene"):
        args += [a["scene"]]
    code, log = engine(args, proj, timeout=180)
    errs = [ln for ln in log.splitlines() if "error" in ln.lower() or "script" in ln.lower()]
    return f"run exit={code}\n" + ("\n".join(errs[-60:]) or log[-4000:])


def t_screenshot_scene(a):
    proj = resolve(a["project_path"])
    frames = int(a.get("frames", 30))
    w, h = int(a.get("width", 1280)), int(a.get("height", 720))
    cap = os.path.join(tempfile.mkdtemp(prefix="bfshot_"), "capture")
    shutil.copy(
        os.path.join(os.path.dirname(__file__), "gd_capture.gd"),
        os.path.join(proj, "bf_capture_tmp.gd"),
    )
    shot = os.path.join(proj, "agent_screenshot.png")
    try:
        args = [
            "--resolution", f"{w}x{h}", "--quit-after", str(frames * 2 + 120),
            "--script", "res://bf_capture_tmp.gd", "--", shot, str(frames), a.get("scene") or "MAIN",
        ]
        if a.get("scene"):
            args += [a["scene"]]
        code, log = engine(args, proj, timeout=240)
    finally:
        tmp = os.path.join(proj, "bf_capture_tmp.gd")
        if os.path.exists(tmp):
            os.remove(tmp)
    if os.path.exists(shot):
        return f"screenshot saved: {os.path.relpath(shot, proj)} ({os.path.getsize(shot)} bytes)"
    return f"capture failed exit={code}\n{log[-2000:]}"


def t_inspect_scene(a):
    proj = resolve(a["project_path"])
    settle = max(1, int(a.get("settle_frames", 5)))
    depth = int(a.get("max_depth", 12))
    tmp = os.path.join(proj, "bf_inspect_tmp.gd")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_inspect.gd"), tmp)
    try:
        args = [
            "--headless",
            "--quit-after",
            str(settle + 120),
            "--script",
            "res://bf_inspect_tmp.gd",
            "--",
            a.get("scene") or "MAIN",
            str(settle),
            str(depth),
        ]
        code, log = engine(args, proj, timeout=180)
    finally:
        if os.path.exists(tmp):
            os.remove(tmp)
    m = re.search(r"BF_INSPECT_BEGIN\n(.*)\nBF_INSPECT_END", log, re.S)
    if m:
        body = m.group(1)
        try:
            json.loads(body)
        except json.JSONDecodeError as e:
            return f"inspect produced invalid JSON ({e}) exit={code}\n{body[:4000]}"
        return body[:100000]
    em = re.search(r"BF_INSPECT_ERROR (.*)", log)
    if em:
        return f"inspect error: {em.group(1).strip()}"
    return f"inspect failed exit={code}\n{log[-2000:]}"


def _run_genasset(a, spec):
    proj = resolve(a["project_path"])
    tmp_script = os.path.join(proj, "bf_genasset_tmp.gd")
    tmp_spec = os.path.join(proj, "bf_genasset_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_genasset.gd"), tmp_script)
    with open(tmp_spec, "w") as f:
        json.dump(spec, f)
    try:
        args = [
            "--headless",
            "--quit-after",
            "180",
            "--script",
            "res://bf_genasset_tmp.gd",
            "--",
            "res://bf_genasset_tmp.json",
        ]
        code, log = engine(args, proj, timeout=120)
    finally:
        for p in (tmp_script, tmp_spec):
            if os.path.exists(p):
                os.remove(p)
    ok = re.search(r"BF_GEN_OK (.*)", log)
    if ok:
        return f"generated: {ok.group(1).strip()}"
    em = re.search(r"BF_GEN_ERROR (.*)", log)
    if em:
        return f"generate error: {em.group(1).strip()}"
    return f"generate failed exit={code}\n{log[-2000:]}"


def t_generate_texture(a):
    spec = {
        "kind": "texture",
        "output": a["output"],
        "variant": a.get("variant", "noise"),
    }
    for key in ("size", "seed", "octaves", "cell", "rows", "mortar", "period", "width"):
        if key in a:
            spec[key] = int(a[key])
    for key in ("frequency", "strength"):
        if key in a:
            spec[key] = float(a[key])
    for key in ("color_a", "color_b", "color_mortar"):
        if key in a:
            spec[key] = [float(v) for v in a[key]]
    for key in ("noise_type", "direction", "vertical"):
        if key in a:
            spec[key] = a[key]
    return _run_genasset(a, spec)


def t_create_material(a):
    spec = {"kind": "material", "output": a["output"]}
    for key in ("metallic", "roughness", "normal_scale", "emission_energy"):
        if key in a:
            spec[key] = float(a[key])
    for key in ("albedo", "emission_color", "uv_scale"):
        if key in a:
            spec[key] = [float(v) for v in a[key]]
    for key in ("albedo_texture", "normal_texture"):
        if key in a:
            spec[key] = a[key]
    if a.get("emission"):
        spec["emission"] = True
    return _run_genasset(a, spec)


def t_scene_edit(a):
    proj = resolve(a["project_path"])
    ops = a.get("ops")
    if not isinstance(ops, list) or not ops:
        return "scene_edit error: 'ops' must be a non-empty array"
    tmp_script = os.path.join(proj, "bf_scenedit_tmp.gd")
    tmp_ops = os.path.join(proj, "bf_scenedit_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_scenedit.gd"), tmp_script)
    with open(tmp_ops, "w") as f:
        json.dump(ops, f)
    try:
        args = [
            "--headless",
            "--quit-after",
            "180",
            "--script",
            "res://bf_scenedit_tmp.gd",
            "--",
            a.get("scene") or "MAIN",
            "res://bf_scenedit_tmp.json",
        ]
        code, log = engine(args, proj, timeout=180)
    finally:
        for p in (tmp_script, tmp_ops):
            if os.path.exists(p):
                os.remove(p)
    m = re.search(r"BF_SCENEDIT_BEGIN\n(.*)\nBF_SCENEDIT_END", log, re.S)
    if m:
        body = m.group(1)
        try:
            json.loads(body)
        except json.JSONDecodeError as e:
            return f"scene_edit produced invalid JSON ({e}) exit={code}\n{body[:4000]}"
        return body[:100000]
    em = re.search(r"BF_SCENEDIT_ERROR (.*)", log)
    if em:
        return f"scene_edit error: {em.group(1).strip()}"
    return f"scene_edit failed exit={code}\n{log[-2000:]}"


def t_simulate_input(a):
    proj = resolve(a["project_path"])
    frames = max(1, int(a.get("frames", 120)))
    spec = {"frames": frames, "events": a.get("events", [])}
    shot = a.get("screenshot", "")
    if shot:
        spec["screenshot"] = shot
    tmp_script = os.path.join(proj, "bf_simulate_tmp.gd")
    tmp_spec = os.path.join(proj, "bf_simulate_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_simulate.gd"), tmp_script)
    with open(tmp_spec, "w") as f:
        json.dump(spec, f)
    try:
        args = []
        if not shot:
            args.append("--headless")
        args += [
            "--quit-after",
            str(frames + 180),
            "--script",
            "res://bf_simulate_tmp.gd",
            "--",
            a.get("scene") or "MAIN",
            "res://bf_simulate_tmp.json",
        ]
        code, log = engine(args, proj, timeout=300)
    finally:
        for p in (tmp_script, tmp_spec):
            if os.path.exists(p):
                os.remove(p)
    m = re.search(r"BF_SIM_BEGIN\n(.*)\nBF_SIM_END", log, re.S)
    if m:
        body = m.group(1)
        try:
            json.loads(body)
        except json.JSONDecodeError as e:
            return f"simulate produced invalid JSON ({e}) exit={code}\n{body[:4000]}"
        return body[:100000]
    em = re.search(r"BF_SIM_ERROR (.*)", log)
    if em:
        return f"simulate error: {em.group(1).strip()}"
    return f"simulate failed exit={code}\n{log[-2000:]}"


def _strip_banner(log):
    return "\n".join(
        ln for ln in log.splitlines()
        if not ln.startswith("BlackForest Engine") and not ln.startswith("https://")
    )


def t_eval(a):
    proj = resolve(a["project_path"])
    code = a.get("code", "")
    tmp_script = os.path.join(proj, "bf_eval_tmp.gd")
    tmp_spec = os.path.join(proj, "bf_eval_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_eval.gd"), tmp_script)
    with open(tmp_spec, "w") as f:
        json.dump({"code": code}, f)
    try:
        rc, log = engine(
            ["--headless", "--quit-after", "120", "--script", "res://bf_eval_tmp.gd",
             "--", "res://bf_eval_tmp.json"], proj, timeout=120)
    finally:
        for p in (tmp_script, tmp_spec):
            if os.path.exists(p):
                os.remove(p)
    em = re.search(r"BF_EVAL_ERROR (.*)", log)
    if em:
        return f"eval error: {em.group(1).strip()}"
    if "BF_EVAL_BEGIN" in log:
        body = re.search(r"BF_EVAL_BEGIN\n(.*?)\nBF_EVAL_END", log, re.S)
        if body:
            return body.group(1).strip()
    return _strip_banner(log)[:100000]


def t_class_doc(a):
    proj = resolve(a["project_path"])
    spec = {"class": a["class"]}
    tmp_script = os.path.join(proj, "bf_classdoc_tmp.gd")
    tmp_spec = os.path.join(proj, "bf_classdoc_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_classdoc.gd"), tmp_script)
    with open(tmp_spec, "w") as f:
        json.dump(spec, f)
    try:
        rc, log = engine(
            ["--headless", "--quit-after", "120", "--script", "res://bf_classdoc_tmp.gd",
             "--", "res://bf_classdoc_tmp.json"], proj, timeout=120)
    finally:
        for p in (tmp_script, tmp_spec):
            if os.path.exists(p):
                os.remove(p)
    em = re.search(r"BF_CLASSDOC_ERROR (.*)", log)
    if em:
        return f"class_doc error: {em.group(1).strip()}"
    m = re.search(r"BF_CLASSDOC_BEGIN\n(.*)\nBF_CLASSDOC_END", log, re.S)
    if m:
        return m.group(1).strip()[:100000]
    return f"class_doc failed exit={rc}\n{_strip_banner(log)[-2000:]}"


def t_import_asset(a):
    proj = resolve(a["project_path"])
    src = a.get("source", "")
    out = a.get("output", "")
    if out.startswith("res://"):
        out = out[len("res://"):]
    if src:
        # copy an external file into the project, then import
        if not os.path.isabs(src) or not os.path.exists(src):
            return f"import error: source not found: {src}"
        if not out:
            return "import error: 'output' res:// path is required when 'source' is given"
        dst = os.path.join(proj, out)
        os.makedirs(os.path.dirname(dst) or proj, exist_ok=True)
        shutil.copy(src, dst)
        label = f"res://{out}"
    else:
        # no source: re-import resources already inside the project so .import caches are generated
        label = f"res://{out}" if out else "(whole project)"
    # trigger import so .godot cache + .import files are generated
    rc, log = engine(["--headless", "--import", "--quit-after", "240"], proj, timeout=600)
    if rc == 0:
        if src:
            if os.path.exists(dst):
                return f"imported: {label} ({os.path.getsize(dst)} bytes)"
            return f"import copy ok but file missing: {dst}"
        return f"re-import triggered: {label}"
    return f"import exited {rc}\n{_strip_banner(log)[-1500:]}"


def t_set_setting(a):
    proj = resolve(a["project_path"])
    spec = {"settings": a.get("settings", {})}
    tmp_script = os.path.join(proj, "bf_set_setting_tmp.gd")
    tmp_spec = os.path.join(proj, "bf_set_setting_tmp.json")
    shutil.copy(os.path.join(os.path.dirname(__file__), "gd_set_setting.gd"), tmp_script)
    with open(tmp_spec, "w") as f:
        json.dump(spec, f)
    try:
        rc, log = engine(
            ["--headless", "--quit-after", "120", "--script", "res://bf_set_setting_tmp.gd",
             "--", "res://bf_set_setting_tmp.json"], proj, timeout=120)
    finally:
        for p in (tmp_script, tmp_spec):
            if os.path.exists(p):
                os.remove(p)
    em = re.search(r"BF_SET_(OK|ERROR) (.*)", log)
    if em:
        if em.group(1) == "OK":
            return f"settings applied: {em.group(2).strip()}"
        return f"set_setting error: {em.group(2).strip()}"
    return f"set_setting failed exit={rc}\n{_strip_banner(log)[-1500:]}"


def t_export(a):
    proj = resolve(a["project_path"])
    preset = a["preset"]
    out_path = a["output"]
    rc, log = engine(
        ["--headless", "--export-release", preset, out_path], proj, timeout=600)
    if rc == 0 and os.path.exists(out_path):
        return f"exported: {out_path} ({os.path.getsize(out_path)} bytes)"
    return f"export failed exit={rc}\n{_strip_banner(log)[-2500:]}"


TOOLS = {
    "bf_create_project": t_create_project,
    "bf_list_files": t_list_files,
    "bf_read_file": t_read_file,
    "bf_write_file": t_write_file,
    "bf_validate_project": t_validate_project,
    "bf_run_scene": t_run_scene,
    "bf_screenshot_scene": t_screenshot_scene,
    "bf_inspect_scene": t_inspect_scene,
    "bf_generate_texture": t_generate_texture,
    "bf_create_material": t_create_material,
    "bf_scene_edit": t_scene_edit,
    "bf_simulate_input": t_simulate_input,
    "bf_eval": t_eval,
    "bf_class_doc": t_class_doc,
    "bf_import_asset": t_import_asset,
    "bf_set_setting": t_set_setting,
    "bf_export": t_export,
}


def dispatch(msg):
    method = msg.get("method", "")
    mid = msg.get("id")
    if method == "initialize":
        _emit({"jsonrpc": "2.0", "id": mid, "result": {
            "protocolVersion": PROTOCOL_VERSION,
            "capabilities": {"tools": {}},
            "serverInfo": {"name": SERVER_NAME, "version": SERVER_VERSION},
        }})
    elif method.startswith("notifications/"):
        pass
    elif method == "ping":
        _emit({"jsonrpc": "2.0", "id": mid, "result": {}})
    elif method == "tools/list":
        _emit({"jsonrpc": "2.0", "id": mid, "result": {"tools": TOOL_DEFS}})
    elif method == "tools/call":
        params = msg.get("params", {})
        name = params.get("name")
        args = params.get("arguments", {})
        handler = TOOLS.get(name)
        if not handler:
            _emit({"jsonrpc": "2.0", "id": mid, "result": {
                "content": [{"type": "text", "text": f"unknown tool {name}"}], "isError": True}})
            return
        try:
            text = handler(args)
            _emit({"jsonrpc": "2.0", "id": mid, "result": {
                "content": [{"type": "text", "text": str(text)}]}})
        except Exception as e:
            _emit({"jsonrpc": "2.0", "id": mid, "result": {
                "content": [{"type": "text", "text": f"{type(e).__name__}: {e}"}], "isError": True}})
    elif mid is not None:
        _emit({"jsonrpc": "2.0", "id": mid, "error": {"code": -32601, "message": f"unhandled: {method}"}})


def main():
    started = time.time()
    for line in sys.stdin:
        line = line.strip()
        if not line:
            continue
        try:
            msg = json.loads(line)
        except json.JSONDecodeError:
            err("invalid JSON")
            continue
        try:
            dispatch(msg)
        except Exception as e:
            err(f"dispatch failure: {e}")
        if time.time() - started > 3600 * 12:
            break


if __name__ == "__main__":
    main()
