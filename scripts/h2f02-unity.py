"""Run one pinned Unity 6000.3.24f1 editor method against a Juego2 Unity project (WP-H2F-02).

  python scripts/h2f02-unity.py <project> <Class.Method> <log-dir> <log-name> [--windowed] [-- extra editor args]

Batch mode (default) quits after the method. `--windowed` opens a normal editor window for evidence that needs
the first interactive session (GC2 generates its settings assets from deferred editor callbacks) or play mode;
the method itself must call EditorApplication.Exit. Prints Juego2/H1 markers and compiler errors, and returns the
editor exit code. The only tolerated failure is the known 6000.3.24f1 crash *after* a successful batch quit.
"""

from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path

UNITY = Path(r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe")
MARKERS = ("J2_", "H2F02_", "ARKUS_H1_", "error CS", "Exception", "Test run", "Compilation failed")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("project", type=Path)
    parser.add_argument("method")
    parser.add_argument("logdir", type=Path)
    parser.add_argument("name")
    parser.add_argument("--windowed", action="store_true")
    parser.add_argument("--timeout", type=int, default=1800)
    parser.add_argument("extra", nargs=argparse.REMAINDER)
    args = parser.parse_args()
    args.logdir.mkdir(parents=True, exist_ok=True)
    log = (args.logdir / f"{args.name}.log").resolve()
    extra = [a for a in args.extra if a != "--"]
    command = [str(UNITY)] + ([] if args.windowed else ["-batchmode", "-quit"]) + [
        "-projectPath", str(args.project.resolve()), "-logFile", str(log)]
    if args.method != "-":
        command += ["-executeMethod", args.method]
    command += extra
    try:
        code = subprocess.call(command, timeout=args.timeout)
    except subprocess.TimeoutExpired:
        code = 124
    text = log.read_text(encoding="utf-8", errors="replace") if log.exists() else ""
    lines = [l for l in text.splitlines() if any(m in l for m in MARKERS) and "UnityEngine.Debug" not in l]
    print("\n".join(lines[-60:]))
    quit_line = "Batchmode quit successfully invoked"
    if code != 0 and quit_line in text and "Crash!!!" in text.split(quit_line, 1)[1] and "error CS" not in text:
        print(f"UNITY_EXIT={code} POST_QUIT_SHUTDOWN_CRASH_TOLERATED log={log}")
        return 0
    print(f"UNITY_EXIT={code} log={log}")
    return code


if __name__ == "__main__":
    sys.exit(main())
