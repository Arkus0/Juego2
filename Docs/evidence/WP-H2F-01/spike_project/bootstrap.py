"""Rebuild the disposable, non-keeper H2F-01 spike workspace outside the repository.

The workspace mirrors only the directory depth that ART-01's pinned Editor scripts expect
(`<root>/Unity/<project>` + `<root>/Docs/evidence/WP-ART-01`). It never writes to the
repository, `Unity/ArkusUnity` or the owner's read-only vault.

  python bootstrap.py lock    # (re)write SPIKE_INPUT_LOCK.json from the vault (Worker only)
  python bootstrap.py init    # create/refresh the workspace from pinned inputs
  python bootstrap.py unity <Class.Method> <log-name>   # run one batch Editor method
"""

from __future__ import annotations

import argparse
import hashlib
import io
import json
import shutil
import subprocess
import sys
import tarfile
import zipfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[3]
EVIDENCE = HERE.parent
INPUT_LOCK = EVIDENCE / "SPIKE_INPUT_LOCK.json"
ART_SHA = "174d05d23c3bceb9d5df00e460b33519cf68328e"  # ART-01 structural checkpoint (PR #234)
ART_DIGEST = "a7f8534f33a46a189c5afb726125b01b6ab0eeba5907cbc3ba4aaafb8e30601e"
UNITY = Path(r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe")
DEFAULT_ROOT = Path(r"C:\Juego2-H2F01-Spike")
DEFAULT_VAULT = Path(r"C:\Juego2-Assets")
PROJECT = "Unity/Spike"

MEDIEVAL_ZIP = "Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip"
MEDIEVAL_SHA = "b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10"
MEDIEVAL_PREFIX = "Medieval Village MegaKit[Unity URP]/Quaternius/Assets/Quaternius/Medieval Village MegaKit/"

# Inputs beyond ART-01's own SOURCE_LOCK. Source-native URP material route keeps the
# original Source `.meta` files so material -> shader graph -> texture GUID links survive.
EXTRA_ZIP_DIRS = ["Materials/"]
EXTRA_ZIP_MODELS = ["Prop_WoodenFence_Single", "Prop_WoodenFence_Extension1", "Prop_ExteriorBorder_Straight1",
                    "Stairs_Exterior_Straight"]
EXTRA_FILES = [
    ("Animation/Unity/UAL1_RM.fbx", "Animation/UAL1_RM.fbx", "quaternius-ual1-source"),
    ("Animation2/Unity/UAL2.fbx", "Animation/UAL2.fbx", "quaternius-ual2-source"),
    ("Animation2/Unity/UAL2_RM.fbx", "Animation/UAL2_RM.fbx", "quaternius-ual2-source"),
    ("Animation2/License.txt", "Licenses/UAL2_License.txt", "quaternius-ual2-source"),
]


def sha(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def file_sha(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 20), b""):
            digest.update(chunk)
    return digest.hexdigest()


def git_bytes(rev_path: str) -> bytes:
    return subprocess.check_output(["git", "-C", str(REPO), "show", rev_path])


def extra_rows(vault: Path) -> list[dict]:
    archive = vault / MEDIEVAL_ZIP
    if file_sha(archive) != MEDIEVAL_SHA:
        raise SystemExit("H1-04 Medieval archive pin mismatch")
    rows = []
    with zipfile.ZipFile(archive) as bundle:
        for member in sorted(bundle.namelist()):
            for prefix in EXTRA_ZIP_DIRS:
                if member.startswith(MEDIEVAL_PREFIX + prefix) and not member.endswith("/"):
                    rel = member[len(MEDIEVAL_PREFIX):]
                    rows.append({"source": "quaternius-medieval-source", "member": member,
                                 "dest": "QuaterniusURP/" + rel, "sha256": sha(bundle.read(member))})
        for name in EXTRA_ZIP_MODELS:
            member = MEDIEVAL_PREFIX + f"Modules/Source Models/{name}.fbx"
            rows.append({"source": "quaternius-medieval-source", "member": member,
                         "dest": f"QuaterniusURP/Models/{name}.fbx", "sha256": sha(bundle.read(member))})
    for rel, dest, source in EXTRA_FILES:
        rows.append({"source": source, "file": rel, "dest": dest, "sha256": file_sha(vault / rel)})
    return rows


def deterministic_meta(path: Path, relative: str) -> None:
    """Same rule as ART-01's importer, so rebuilt GUIDs match the pinned builder's expectations."""
    meta = path.with_name(path.name + ".meta")
    guid = sha(f"juego2-art01-external:{relative}".encode())[:32]
    header = f"fileFormatVersion: 2\nguid: {guid}\n"
    if path.suffix.lower() == ".png":
        header += "TextureImporter:\n  serializedVersion: 13\n"
    if not meta.exists():
        meta.write_text(header, encoding="utf-8")


def extract_art(root: Path, refresh: bool) -> None:
    """Pinned ART input is written once; later spike steps (URP conversion) mutate the workspace copy.
    `--refresh-art` restores the pinned bytes, which also restarts the S01 sequence from Reproduce."""
    tar_bytes = subprocess.check_output(["git", "-C", str(REPO), "archive", "--format=tar", ART_SHA,
                                         "Unity/ArkusUnity/Assets/Arkus/ART",
                                         "Unity/ArkusUnity/Assets/Arkus/ART.meta"])
    with tarfile.open(fileobj=io.BytesIO(tar_bytes)) as tar:
        for member in tar.getmembers():
            if not member.isfile():
                continue
            rel = member.name[len("Unity/ArkusUnity/"):]
            target = root / PROJECT / rel
            target.parent.mkdir(parents=True, exist_ok=True)
            data = tar.extractfile(member).read()
            if not target.exists() or (refresh and target.read_bytes() != data):
                target.write_bytes(data)
    for rel in ["Docs/evidence/WP-ART-01/KIT_COMPOSITION_MANIFEST.json",
                "Docs/evidence/WP-ART-01/SOURCE_LOCK.json",
                "Unity/ArkusUnity/Assets/Arkus/CITY/City04Layout.json"]:
        target = root / rel
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(git_bytes(f"{ART_SHA}:{rel}"))


def import_art_sources(root: Path, vault: Path) -> int:
    lock = json.loads(git_bytes(f"{ART_SHA}:Docs/evidence/WP-ART-01/SOURCE_LOCK.json"))
    dest_root = root / PROJECT / "Assets/Arkus/ART/External"
    with zipfile.ZipFile(vault / MEDIEVAL_ZIP) as bundle:
        for row in lock["files"]:
            data = bundle.read(row["member"]) if "member" in row else (vault / row["file"]).read_bytes()
            if sha(data) != row["sha256"]:
                raise SystemExit(f"ART-01 source byte mismatch: {row['dest']}")
            target = dest_root / row["dest"]
            target.parent.mkdir(parents=True, exist_ok=True)
            if not target.exists() or file_sha(target) != row["sha256"]:
                target.write_bytes(data)
            deterministic_meta(target, row["dest"])
    return len(lock["files"])


def import_extra(root: Path, vault: Path) -> int:
    lock = json.loads(INPUT_LOCK.read_text(encoding="utf-8"))
    if lock["files"] != extra_rows(vault):
        raise SystemExit("SPIKE_INPUT_LOCK mismatch: vault bytes or selection changed")
    dest_root = root / PROJECT / "Assets/H2F01Inputs"
    with zipfile.ZipFile(vault / MEDIEVAL_ZIP) as bundle:
        for row in lock["files"]:
            data = bundle.read(row["member"]) if "member" in row else (vault / row["file"]).read_bytes()
            target = dest_root / row["dest"]
            target.parent.mkdir(parents=True, exist_ok=True)
            if not target.exists() or file_sha(target) != row["sha256"]:
                target.write_bytes(data)
    return len(lock["files"])


def copy_recipe(root: Path) -> None:
    project = root / PROJECT
    (project / "Packages").mkdir(parents=True, exist_ok=True)
    shutil.copyfile(HERE / "Packages/manifest.json", project / "Packages/manifest.json")
    # An existing ProjectVersion makes Unity open this as an existing project; without it Unity creates a new
    # project and injects its default template packages into the manifest (observed on first import).
    (project / "ProjectSettings").mkdir(parents=True, exist_ok=True)
    if not (project / "ProjectSettings/ProjectVersion.txt").exists():
        shutil.copyfile(HERE / "ProjectSettings/ProjectVersion.txt", project / "ProjectSettings/ProjectVersion.txt")
    source = HERE / "Assets/H2F01"
    target = project / "Assets/H2F01"
    for path in source.rglob("*"):
        if path.is_file():
            out = target / path.relative_to(source)
            out.parent.mkdir(parents=True, exist_ok=True)
            if not out.exists() or out.read_bytes() != path.read_bytes():
                shutil.copyfile(path, out)
    # Only recipe-owned code is pruned; generated spike settings/scenes stay in the workspace.
    for stale in (target / "Editor").rglob("*"):
        if stale.is_file() and stale.suffix != ".meta" and not (source / stale.relative_to(target)).exists():
            stale.unlink()


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["lock", "init", "unity", "play"])
    parser.add_argument("method", nargs="?")
    parser.add_argument("log", nargs="?")
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--vault", type=Path, default=DEFAULT_VAULT)
    parser.add_argument("--refresh-art", action="store_true")
    args = parser.parse_args()
    root, vault = args.root.resolve(), args.vault.resolve(strict=True)
    if REPO in root.parents or root == REPO:
        raise SystemExit("Spike workspace must stay outside the repository")
    if args.mode == "lock":
        rows = extra_rows(vault)
        INPUT_LOCK.write_text(json.dumps({"schema": "juego2.h2f01.spike-input-lock@1",
                                          "medievalArchiveSha256": MEDIEVAL_SHA, "artInputSha": ART_SHA,
                                          "files": rows}, indent=2) + "\n", encoding="utf-8")
        print(f"H2F01_INPUT_LOCK_WRITTEN files={len(rows)}")
        return
    if args.mode == "init":
        extract_art(root, args.refresh_art)
        art = import_art_sources(root, vault)
        extra = import_extra(root, vault)
        copy_recipe(root)
        print(f"H2F01_SPIKE_WORKSPACE_READY root={root} art_inputs={art} extra_inputs={extra} art_sha={ART_SHA}")
        return
    if not args.method or not args.log:
        raise SystemExit("unity mode needs <Class.Method> <log-name>")
    logs = root / "logs"
    logs.mkdir(parents=True, exist_ok=True)
    log = logs / f"{args.log}.log"
    # `play` leaves the editor running after the builder method; the play-mode driver exits it.
    # Batch mode without -quit requires the paid `com.unity.editor.headless` entitlement (observed on this
    # Personal-licensed workstation), so play-mode spikes run a windowed editor that the driver closes.
    command = [str(UNITY)] + ([] if args.mode == "play" else ["-batchmode", "-quit"]) + [
        "-projectPath", str(root / PROJECT), "-executeMethod", args.method, "-logFile", str(log)]
    try:
        code = subprocess.call(command, timeout=900)
    except subprocess.TimeoutExpired:
        code = 124
    text = log.read_text(encoding="utf-8", errors="replace") if log.exists() else ""
    markers = [line for line in text.splitlines() if ("H2F01_" in line or "ART01_" in line or "error CS" in line) and "UnityEngine." not in line]
    print("\n".join(markers[-40:]))
    # Known 6000.3.24f1 behaviour on this workstation (also recorded by ART-01): the editor can crash
    # during shutdown *after* the method returned and batchmode quit was invoked. Everything the
    # method saved is already on disk; only that exact post-quit shape is tolerated.
    quit_line = "Batchmode quit successfully invoked"
    if code != 0 and quit_line in text and "Crash!!!" in text.split(quit_line, 1)[1] and "error CS" not in text:
        print(f"UNITY_EXIT={code} POST_QUIT_SHUTDOWN_CRASH_TOLERATED log={log}")
        sys.exit(0)
    print(f"UNITY_EXIT={code} log={log}")
    sys.exit(code)


if __name__ == "__main__":
    main()
