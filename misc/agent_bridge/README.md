# BlackForest Agent Bridge

MCP (Model Context Protocol) stdio server that lets AI agents build full
BlackForest games: scaffold projects, author scenes/scripts as text, validate
headlessly, run scenes, and capture screenshots for visual iteration.

## Setup

1. Build the engine (`scons platform=linuxbsd production=yes`) and make the
   `blackforest` binary available on `PATH`, or point `BLACKFOREST_BIN` at it.
2. Register the server with your MCP client:

Claude Code:
```bash
claude mcp add blackforest \
  -e BLACKFOREST_BIN=/path/to/blackforest \
  -e BF_WORKSPACE=/home/you/games \
  -- python3 /path/to/misc/agent_bridge/mcp_server.py
```

Cursor / any JSON config:
```json
{
  "mcpServers": {
    "blackforest": {
      "command": "python3",
      "args": ["/path/to/misc/agent_bridge/mcp_server.py"],
      "env": {
        "BLACKFOREST_BIN": "/path/to/blackforest",
        "BF_WORKSPACE": "/home/you/games"
      }
    }
  }
}
```

## Tools

| Tool | Purpose |
| --- | --- |
| `bf_create_project` | Scaffold a project (+ optional Large Worlds addon install) |
| `bf_write_file` / `bf_read_file` / `bf_list_files` | Author `.tscn`/`.gd`/`.tres` text resources |
| `bf_validate_project` | Headless import + error extraction (the agent feedback loop) |
| `bf_run_scene` | Bounded headless execution with log capture |
| `bf_screenshot_scene` | GPU render + viewport PNG capture for visual checks |

## Notes

- All paths are confined to `BF_WORKSPACE`.
- Screenshots require a GPU/EGL-capable environment (headless machines need a
  virtual display); validation and runs work fully headless.
- Suggested agent loop: write files → validate → fix errors → screenshot → refine.
