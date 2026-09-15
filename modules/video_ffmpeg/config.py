def _get_pkg_config():
    """Return the real pkg-config binary.

    The PATH may contain a stub 'pkg-config' (used to satisfy engine build
    probes) that returns fake versions and NO cflags/libs. That breaks this
    module's link step. Prefer the real binary under /usr/bin or /usr/local/bin.
    """
    import os
    import shutil

    for candidate in ["/usr/bin/pkg-config", "/usr/local/bin/pkg-config", "/bin/pkg-config"]:
        if os.path.exists(candidate):
            return candidate

    fallback = shutil.which("pkg-config")
    if fallback:
        return fallback
    return None


def can_build(env, platform):
    # Only Linux/Unix desktop for now.
    if platform not in ["linuxbsd", "macos", "windows"]:
        return False

    # Require the FFmpeg development libraries to be available via pkg-config.
    import subprocess

    if platform == "windows":
        # Windows links statically/bundled; assume available when building.
        return True

    pc = _get_pkg_config()
    if pc is None:
        return False

    ok = True
    for lib in ["libavformat", "libavcodec", "libavutil", "libswscale", "libswresample"]:
        try:
            r = subprocess.run([pc, "--exists", lib], capture_output=True, timeout=30)
            if r.returncode != 0:
                ok = False
                break
        except Exception:
            ok = False
            break
    return ok


def configure(env):
    import subprocess

    if env["platform"] == "windows":
        # Windows builders bundle FFmpeg; adjust paths as needed.
        return

    # Debug symbols for this module (helps diagnosing crashes under gdb).
    env.Append(CCFLAGS=["-g", "-O0"])

    pc = _get_pkg_config()
    if pc is None:
        return

    flags = subprocess.run(
        [pc, "--cflags", "--libs", "libavformat", "libavcodec", "libavutil", "libswscale", "libswresample"],
        capture_output=True,
        text=True,
        timeout=30,
    ).stdout.strip()
    env.Append(LINKFLAGS=[flag for flag in flags.split() if flag.startswith("-L")])
    env.Append(LIBS=[flag[2:] for flag in flags.split() if flag.startswith("-l")])
    env.Append(CPPFLAGS=[flag for flag in flags.split() if flag.startswith("-I")])


def get_doc_classes():
    return [
        "VideoStreamFFMPEG",
    ]


def get_doc_path():
    return "doc_classes"