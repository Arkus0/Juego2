"""Rebuild the disposable, non-keeper WP-H2F-01A GC2 Core probe workspace outside the repository.

The workspace is the accepted H2F-01 spike recipe (pinned ART-01 PREFOUNDATION_INPUT + owner-vault Quaternius
inputs + selected stack) plus the owner-licensed Game Creator 2 Core package imported Assets-only, plus the
Juego2-owned probe code in `Assets/H2F01A`. Licensed GC2 bytes live only in the workspace and are never copied
into the repository.

  python bootstrap_01a.py base  --root <dir>                  # H2F-01 recipe init + the H2F-01 steps the probes use
  python bootstrap_01a.py core  --root <dir> [--gc2 <pkg>]    # state snapshot, Assets-only GC2 import, physics2d, 01A code
  python bootstrap_01a.py sync  --root <dir>                  # recopy only the 01A probe code
  python bootstrap_01a.py unity <Class.Method> <log> --root <dir>   # one batch Editor method
  python bootstrap_01a.py play  <Class.Method> <log> --root <dir>   # windowed editor; the play driver exits it
  python bootstrap_01a.py probes --root <dir>                 # C01 (batch, then windowed) + C02..C07 in order
  python bootstrap_01a.py collect --root <dir>                # copy curated outputs into the repository evidence
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[3]
H2F01 = REPO / "Docs/evidence/WP-H2F-01/spike_project"
UNITY = Path(r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe")
DEFAULT_ROOT = Path(r"C:\Juego2-H2F01A-Core")
PROJECT = "Unity/Spike"
# Exact owner-supplied package recorded by H2F-01 (results/gc2_import_report.json); anything else is refused.
GC2_SHA = "1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b"
GC2_BYTES = 28236878
GC2_DEFAULT = Path(os.environ.get("APPDATA", "")) / "Unity/Asset Store-5.x/Catsoft Works/Editor ExtensionsGame Toolkits/Game Creator 2.unitypackage"
H2F01_STEPS = [
    ("Juego2.H2F01.Editor.H2F01RenderSpike.Reproduce", "a01_reproduce"),
    ("Juego2.H2F01.Editor.H2F01RenderSpike.SetupUrp", "a02_urp"),
    ("Juego2.H2F01.Editor.H2F01RenderSpike.ConvertRouteA", "a03_convert"),
    ("Juego2.H2F01.Editor.H2F01AvatarMap.Apply", "a04_avatar"),
    ("Juego2.H2F01.Editor.H2F01PlayBuilder.EnableInputSystem", "a05_input"),
]
MARKERS = ("H2F01A_", "H2F01_", "ART01_", "error CS", "Exception")
PROBES = [
    ("unity", "Juego2.H2F01A.Editor.H2F01ACoreInventory.Run", "c01_inventory_batch"),
    ("play", "Juego2.H2F01A.Editor.H2F01ACoreInventory.RunWindowed", "c01_inventory_windowed"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C02", "p02_c02"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C02Gc2Camera", "p02_c02g"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C03", "p03_c03"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C04", "p04_c04"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C05", "p05_c05"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C06", "p06_c06"),
    ("play", "Juego2.H2F01A.Editor.H2F01ABuilder.C07", "p07_c07"),
]
SCENARIOS = ["c02", "c02g", "c03", "c04", "c05", "c06", "c07"]
EVIDENCE = HERE.parent


def file_sha(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 20), b""):
            digest.update(chunk)
    return digest.hexdigest()


def project_state(project: Path) -> dict:
    """Hashes of every project-global file a package import could change (ProjectSettings/*, Packages/manifest)."""
    files = sorted(p for p in (project / "ProjectSettings").glob("*") if p.is_file())
    files.append(project / "Packages/manifest.json")
    return {p.relative_to(project).as_posix(): file_sha(p) for p in files if p.exists()}


def copy_probe_code(project: Path) -> int:
    source = HERE / "Assets/H2F01A"
    target = project / "Assets/H2F01A"
    count = 0
    for path in source.rglob("*"):
        if path.is_file():
            out = target / path.relative_to(source)
            out.parent.mkdir(parents=True, exist_ok=True)
            if not out.exists() or out.read_bytes() != path.read_bytes():
                shutil.copyfile(path, out)
            count += 1
    for stale in target.rglob("*"):
        if stale.is_file() and stale.suffix != ".meta" and not (source / stale.relative_to(target)).exists() \
                and not stale.relative_to(target).parts[0] in ("Play", "Generated"):
            stale.unlink()
    return count


def run_unity(root: Path, method: str, log_name: str, windowed: bool) -> int:
    logs = root / "logs"
    logs.mkdir(parents=True, exist_ok=True)
    log = logs / f"{log_name}.log"
    command = [str(UNITY)] + ([] if windowed else ["-batchmode", "-quit"]) + [
        "-projectPath", str(root / PROJECT), "-executeMethod", method, "-logFile", str(log)]
    try:
        code = subprocess.call(command, timeout=1500)
    except subprocess.TimeoutExpired:
        code = 124
    text = log.read_text(encoding="utf-8", errors="replace") if log.exists() else ""
    lines = [l for l in text.splitlines() if any(m in l for m in MARKERS) and not l.startswith("UnityEngine.")]
    print("\n".join(lines[-60:]))
    quit_line = "Batchmode quit successfully invoked"
    if code != 0 and quit_line in text and "Crash!!!" in text.split(quit_line, 1)[1] and "error CS" not in text:
        print(f"UNITY_EXIT={code} POST_QUIT_SHUTDOWN_CRASH_TOLERATED log={log}")
        return 0
    print(f"UNITY_EXIT={code} log={log}")
    return code


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["base", "core", "sync", "unity", "play", "probes", "collect"])
    parser.add_argument("method", nargs="?")
    parser.add_argument("log", nargs="?")
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--gc2", type=Path, default=GC2_DEFAULT)
    args = parser.parse_args()
    root = args.root.resolve()
    if REPO in root.parents or root == REPO:
        raise SystemExit("Probe workspace must stay outside the repository")
    project = root / PROJECT
    out = root / "out"

    if args.mode == "base":
        subprocess.check_call([sys.executable, str(H2F01 / "bootstrap.py"), "init", "--root", str(root)])
        for method, log in H2F01_STEPS:
            if run_unity(root, method, log, windowed=False) != 0:
                raise SystemExit(f"H2F-01 base step failed: {method}")
        print(f"H2F01A_BASE_READY root={root}")
        return

    if args.mode == "core":
        package = args.gc2.resolve(strict=True)
        actual = file_sha(package)
        if actual != GC2_SHA or package.stat().st_size != GC2_BYTES:
            raise SystemExit(f"GC2 package identity mismatch: sha256={actual} bytes={package.stat().st_size}")
        (out / "c01").mkdir(parents=True, exist_ok=True)
        state = out / "c01/project_state_before_gc2.json"
        if not state.exists():  # first import only: the snapshot is the pre-GC2 baseline
            state.write_text(json.dumps(project_state(project), indent=2, sort_keys=True) + "\n", encoding="utf-8")
        subprocess.check_call([sys.executable, str(H2F01 / "import_unitypackage.py"), str(package), str(project),
                               "--report", str(out / "c01/gc2_import_report.json")])
        manifest_path = project / "Packages/manifest.json"
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
        manifest["dependencies"].setdefault("com.unity.modules.physics2d", "1.0.0")
        manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
        print(f"H2F01A_CORE_IMPORTED sha256={actual} probe_files={copy_probe_code(project)}")
        return

    if args.mode == "sync":
        print(f"H2F01A_PROBE_CODE_SYNCED files={copy_probe_code(project)}")
        return

    if args.mode == "probes":
        for mode, method, log in PROBES:
            code = run_unity(root, method, log, windowed=mode == "play")
            if code != 0:
                raise SystemExit(f"probe step failed: {method} exit={code}")
        missing = [s for s in SCENARIOS if not (out / s / f"{s}_result.json").exists()]
        if missing:
            raise SystemExit(f"probe results missing: {missing}")
        print(f"H2F01A_PROBES_DONE root={root}")
        return

    if args.mode == "collect":
        results = EVIDENCE / "results"
        captures = EVIDENCE / "captures"
        results.mkdir(exist_ok=True)
        captures.mkdir(exist_ok=True)
        for name in ["core_surface_batch.json", "core_surface_windowed.json", "core_extension_types.tsv",
                     "gc2_import_report.json", "project_state_before_gc2.json", "project_state_after_gc2_windowed.json"]:
            shutil.copyfile(out / "c01" / name, results / f"c01_{name}")
        for s in SCENARIOS:
            for name in [f"{s}_result.json", f"{s}_log.txt", "authoring_paths.txt", "shader_audit.json"]:
                if (out / s / name).exists():
                    shutil.copyfile(out / s / name, results / (name if name.startswith(s) else f"{s}_{name}"))
        lock = project / "Packages/packages-lock.json"
        if lock.exists():
            deps = json.loads(lock.read_text(encoding="utf-8"))["dependencies"]
            slim = {k: {"version": v.get("version"), "source": v.get("source")} for k, v in sorted(deps.items())}
            (results / "probe_packages_lock.json").write_text(json.dumps(slim, indent=2) + "\n", encoding="utf-8")
        print(f"H2F01A_COLLECTED results={len(list(results.iterdir()))}")
        return

    if not args.method or not args.log:
        raise SystemExit("unity/play mode needs <Class.Method> <log-name>")
    sys.exit(run_unity(root, args.method, args.log, windowed=args.mode == "play"))


if __name__ == "__main__":
    main()
