# BlackForest Engine

<p align="center">
  <a href="https://blackforestengine.org">
    <img src="misc/logo/logo_outlined.svg" width="400" alt="BlackForest Engine logo">
  </a>
</p>

## 2D and 3D cross-platform game engine

**[BlackForest Engine](https://blackforestengine.org) is a feature-packed, cross-platform
game engine to create 2D and 3D games from a unified interface.** It provides a
comprehensive set of [common tools](https://blackforestengine.org/features), so that
users can focus on making games without having to reinvent the wheel. Games can
be exported with one click to a number of platforms, including the major desktop
platforms (Linux, macOS, Windows), mobile platforms (Android, iOS), as well as
Web-based platforms and [consoles](https://blackforestengine.org/consoles).

BlackForest features integrated AI capabilities, including an MCP bridge for AI
agent-driven game development, and aggressive 3D rendering support delivering a
high-performance 3D workflow comparable to Unity.

## Free, open source and community-driven

BlackForest is completely free and open source under the very permissive [MIT license](https://blackforestengine.org/license).
No strings attached, no royalties, nothing. The users' games are theirs, down
to the last line of engine code. BlackForest's development is fully independent and
community-driven, empowering users to help shape their engine to match their
expectations. It is supported by the [BlackForest Foundation](https://blackforest.foundation/)
not-for-profit.

> **Note:** BlackForest Engine is a fork of [Godot Engine](https://github.com/godotengine/godot), built with integrated AI capabilities and aggressive 3D rendering support.

## New Features

### Editor & Debugging
- **Frame Debugger** — New debugger tab showing Objects/Primitives/Draw-calls per frame, texture/buffer/video memory, and all 5 pipeline-compile counters via `RenderingServer::get_rendering_info`

### AI Agent Integration
- **MCP bridge** — AI agents can scaffold projects, author scenes and scripts, validate headlessly, run, and capture screenshots
- **`bf_eval`** — Run arbitrary GDScript in the engine headless and return output (planned)
- **`bf_class_doc`** — Introspect engine class API (methods, signals, properties) via ClassDB (planned)
- **`bf_export`** — Export/build projects to shippable builds (planned)
- **`bf_import_asset`** — Import external assets (glb, png, wav, etc.) into projects (planned)
- **`bf_set_setting`** — Edit project.godot settings (planned)

### Media & Video
- **FFmpeg H.264/AAC module** — Full video decode (video → `sws_scale` → RGBA texture, audio → `swr` → mix ring); fixed build blockers and audio heap-corruption crash; verified with H.264 playback

### 3D Rendering
- **Large Worlds** — Chunk streaming, precision origin shifting for huge coordinates, HLOD district baking, and billboard impostors via bundled addon
- **Aggressive 3D support** — High-performance 3D workflow comparable to Unity; planned HW ray-traced reflections, virtual shadow maps, and Nanite-lite GPU-driven LODs

### C# / .NET
- **Mono editor** — C# glue generation via `--generate-mono-glue`; managed assemblies building (GodotSharp, GodotPlugins, GodotSharpEditor)

### Cross-Platform
- **Android ABI Gradle cache fix** — Exports now pick up ABI changes correctly

Before being open sourced in [February 2014](https://github.com/godotengine/godot/commit/0b806ee0fc9097fa7bda7ac0109191c9c5e0a1ac),
BlackForest had been developed by [Juan Linietsky](https://github.com/reduz) and
[Ariel Manzur](https://github.com/punto-) for several years as an in-house
engine, used to publish several work-for-hire titles.

![Screenshot of a 3D scene in the BlackForest Engine editor](https://raw.githubusercontent.com/godotengine/godot-design/master/screenshots/editor_tps_demo_1920x1080.jpg)

## AI agent integration

BlackForest ships an MCP bridge so AI agents can build complete games:
scaffold projects, author scenes and scripts, validate headlessly, run, and
capture screenshots. See [misc/agent_bridge/README.md](misc/agent_bridge/README.md).

## Large 3D worlds

The bundled **Large Worlds** addon provides chunk streaming, precision origin
shifting for huge coordinates, HLOD district baking and billboard impostors.
See [misc/addons/blackforest_large_worlds/README.md](misc/addons/blackforest_large_worlds/README.md).

## Getting the engine

### Binary downloads

Official binaries for the BlackForest editor and the export templates can be found
[on the BlackForest website](https://blackforestengine.org/download).

### Compiling from source

[See the official docs](https://docs.blackforestengine.org/en/latest/engine_details/development/compiling)
for compilation instructions for every supported platform.

## Community and contributing

BlackForest is not only an engine but an ever-growing community of users and engine
developers. The main community channels are listed [on the homepage](https://blackforestengine.org/community).

The best way to get in touch with the core engine developers is to join the
[BlackForest Contributors Chat](https://chat.blackforestengine.org).

To get started contributing to the project, see the [contributing guide](CONTRIBUTING.md).
This document also includes guidelines for reporting bugs.

## Documentation and demos

The official documentation is hosted on [Read the Docs](https://docs.blackforestengine.org).
It is maintained by the BlackForest community in its own [GitHub repository](https://github.com/godotengine/godot-docs).

The [class reference](https://docs.blackforestengine.org/en/latest/classes/)
is also accessible from the BlackForest editor.

We also maintain official demos in their own [GitHub repository](https://github.com/godotengine/godot-demo-projects)
as well as a list of [awesome BlackForest community resources](https://github.com/godotengine/awesome-blackforest).

There are also a number of other
[learning resources](https://docs.blackforestengine.org/en/latest/community/tutorials.html)
provided by the community, such as text and video tutorials, demos, etc.
Consult the [community channels](https://blackforestengine.org/community)
for more information.

[![Code Triagers Badge](https://www.codetriage.com/godotengine/blackforest/badges/users.svg)](https://www.codetriage.com/godotengine/blackforest)
[![Translate on Weblate](https://hosted.weblate.org/widgets/blackforest-engine/-/blackforest/svg-badge.svg)](https://hosted.weblate.org/engage/blackforest-engine/?utm_source=widget)
