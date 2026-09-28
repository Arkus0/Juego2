"""Disposable WP-H2F-03 evidence workspace: the exact candidate repository + the integrated NON_KEEPER fixture.

  python workspace.py create  --sha <candidate> [--root C:\\Juego2-H2F03-W]   # git archive of the whole candidate
  python workspace.py build   [--root ...]                                    # .NET Release build of the public H1 hosts
  python workspace.py vault   --sha <candidate> [--root ...] [--vault ...]    # pinned H1 source slice (Tools/AssetVault)
  python workspace.py gc2     [--root ...]                                    # hash-gated GC2 Core (scripts/h2f02-provision.py)
  python workspace.py content [--root ...] [--vault ...]                      # ART-01 PREFOUNDATION_INPUT + H2F-01 inputs
  python workspace.py deploy  [--root ...]                                    # evidence-only Unity code -> Assets/H2F03Evidence
  python workspace.py unity <Class.Method> <log-name> [--windowed] [-- extra editor args]
  python workspace.py tree    --sha <candidate> [--root ...]                  # committed candidate bytes left untouched?
  python workspace.py prepare --sha <candidate>                               # create + build + vault + gc2 + content + deploy

The workspace lives outside the repository and is never committed. `create` copies only committed candidate bytes (no
Library, no ignored state). The H1 public host needs the repository root (Juego2.sln, H1 catalogue files), so the
whole candidate is archived, not only the Unity project. Content reuses the pinned inputs accepted upstream: ART-01
structural checkpoint 174d05d2 (git) + ART-01 SOURCE_LOCK.json and H2F-01 SPIKE_INPUT_LOCK.json (owner vault,
hash-checked by the H2F-01 recipe). The evidence code in ./Unity/H2F03Evidence is added on top; it drives the
foundation's public surfaces and writes results under <root>/results.
"""

from __future__ import annotations

import argparse
import hashlib
import importlib.util
import io
import json
import shutil
import subprocess
import sys
import tarfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[3]
DEFAULT_ROOT = Path(r"C:\Juego2-H2F03-W")
DEFAULT_VAULT = Path(r"C:\Juego2-Assets")
PROJECT = "Unity/ArkusUnity"
ART_SHA = "174d05d23c3bceb9d5df00e460b33519cf68328e"
VAULT_SHA = "ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6"
UNITY = Path(r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe")
MARKER = ".h2f03-workspace"
EVIDENCE_DIR = "Assets/H2F03Evidence"
MARKERS = ("H2F03_", "J2_", "ARKUS_H1_", "error CS", "Exception", "Compilation failed")


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def git(*args: str) -> bytes:
    return subprocess.check_output(["git", "-C", str(REPO), *args])


def untar(data: bytes, root: Path, only: tuple[str, ...] = ()) -> int:
    count = 0
    with tarfile.open(fileobj=io.BytesIO(data)) as tar:
        for member in tar.getmembers():
            if not member.isfile() or (only and not member.name.startswith(only)):
                continue
            target = root / member.name
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(tar.extractfile(member).read())
            count += 1
    return count


def results(root: Path) -> Path:
    path = root / "results"
    path.mkdir(parents=True, exist_ok=True)
    return path


def write_json(path: Path, value) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def create(root: Path, sha: str) -> None:
    if root == REPO or REPO in root.parents:
        raise SystemExit("Workspace must stay outside the repository")
    if root.exists():
        if not (root / MARKER).exists():
            raise SystemExit(f"refusing to replace {root}: it is not an H2F-03 workspace")
        shutil.rmtree(root)
    root.mkdir(parents=True)
    count = untar(git("archive", "--format=tar", sha), root)
    (root / MARKER).write_text(sha + "\n", encoding="utf-8")
    unity_tree = git("rev-parse", f"{sha}:{PROJECT}").decode().strip()
    write_json(results(root) / "workspace.json", {"candidateSha": sha, "unityTree": unity_tree, "files": count,
                                                  "artInputSha": ART_SHA, "vaultSha": VAULT_SHA})
    print(f"H2F03_WORKSPACE_CREATED root={root} sha={sha} files={count} unityTree={unity_tree}")


def build(root: Path) -> int:
    for command in (["dotnet", "restore", "Juego2.sln", "--locked-mode"],
                    ["dotnet", "build", "Juego2.sln", "--no-restore", "-c", "Release", "-nologo", "-v", "q"]):
        code = subprocess.call(command, cwd=root)
        if code != 0:
            return code
    for project in ("Arkus.Harness.Cli", "Arkus.Harness.Mcp"):
        if not (root / f"src/{project}/bin/Release/net8.0/{project}.dll").is_file():
            raise SystemExit(f"public host {project} was not built")
    print("H2F03_WORKSPACE_BUILT")
    return 0


def vault(root: Path, sha: str, vault_root: Path) -> int:
    return subprocess.call([sys.executable, str(root / "Tools/AssetVault/verify_h1_asset_vault.py"),
                            "--public-root", str(root), "--vault-root", str(vault_root), "--candidate-sha", sha,
                            "--vault-sha", VAULT_SHA, "--mount", "--receipt", str(results(root) / "h1_vault_receipt.json")])


def gc2(root: Path, mode: str = "gc2") -> int:
    return subprocess.call([sys.executable, str(root / "scripts/h2f02-provision.py"), mode, "--project", str(root / PROJECT)])


def h2f01_recipe():
    spec = importlib.util.spec_from_file_location("h2f01_bootstrap", REPO / "Docs/evidence/WP-H2F-01/spike_project/bootstrap.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    module.PROJECT = PROJECT  # same pinned import functions, pointed at the adopted project layout
    return module


def content(root: Path, vault_root: Path) -> None:
    recipe = h2f01_recipe()
    art = git("archive", "--format=tar", ART_SHA, PROJECT + "/Assets/Arkus/ART", PROJECT + "/Assets/Arkus/ART.meta")
    n_art = untar(art, root)
    for rel in ["Docs/evidence/WP-ART-01/KIT_COMPOSITION_MANIFEST.json", "Docs/evidence/WP-ART-01/SOURCE_LOCK.json"]:
        target = root / rel
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(git("show", f"{ART_SHA}:{rel}"))
    n_src = recipe.import_art_sources(root, vault_root)
    n_extra = import_h2f01_animation(recipe, root, vault_root)
    collisions = guid_collisions(root)
    write_json(results(root) / "content.json", {"artFiles": n_art, "artSources": n_src, "h2f01AnimationExtra": n_extra, "artInputSha": ART_SHA,
                                                "guidCollisions": collisions})
    if collisions:
        raise SystemExit(f"H2F03_WORKSPACE_GUID_COLLISION {collisions[:5]}")
    print(f"H2F03_WORKSPACE_CONTENT art_files={n_art} art_sources={n_src} extra={n_extra} guid_collisions=0")


def import_h2f01_animation(recipe, root: Path, vault_root: Path) -> int:
    """Only the UAL2 / root-motion libraries (+ UAL2 licence) of H2F-01's SPIKE_INPUT_LOCK. Its Medieval URP Source
    materials keep the vendor .meta GUIDs, which the mounted H1 SourceSlice already owns: co-importing them makes Unity
    reassign a GUID and H1 fails closed with catalogue.stale-mapping (found by this WP's first lifecycle run)."""
    lock = json.loads(recipe.INPUT_LOCK.read_text(encoding="utf-8"))
    if lock["files"] != recipe.extra_rows(vault_root):
        raise SystemExit("SPIKE_INPUT_LOCK mismatch: vault bytes or selection changed")
    rows = [r for r in lock["files"] if r["dest"].startswith(("Animation/", "Licenses/"))]
    for row in rows:
        data = (vault_root / row["file"]).read_bytes()
        if sha256_bytes(data) != row["sha256"]:
            raise SystemExit(f"H2F-01 input byte mismatch: {row['dest']}")
        target = root / PROJECT / "Assets/H2F01Inputs" / row["dest"]
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(data)
    return len(rows)


def guid_collisions(root: Path) -> list[str]:
    """Every .meta GUID under Assets must be unique; a duplicate silently re-identifies an asset (fail closed)."""
    seen: dict[str, str] = {}
    duplicates = []
    for meta in (root / PROJECT / "Assets").rglob("*.meta"):
        for line in meta.read_text(encoding="utf-8", errors="replace").splitlines()[:3]:
            if line.startswith("guid:"):
                guid = line.split(":", 1)[1].strip()
                rel = meta.relative_to(root / PROJECT).as_posix()
                if guid in seen:
                    duplicates.append(f"{guid} {seen[guid]} {rel}")
                seen[guid] = rel
    return duplicates


def evidence_code_digest() -> dict:
    files = sorted(p for p in (HERE / "Unity/H2F03Evidence").rglob("*") if p.is_file())
    return {p.relative_to(HERE).as_posix(): sha256_bytes(p.read_bytes()) for p in files}


def deterministic_meta(path: Path, rel: str) -> None:
    """Stable GUIDs, so a redeploy keeps the sidecar scene's script references (sha256 of the evidence-relative path)."""
    guid = sha256_bytes(f"juego2-h2f03-evidence:{rel}".encode())[:32]
    lines = ["fileFormatVersion: 2", f"guid: {guid}"] + (["folderAsset: yes"] if path.is_dir() else [])
    Path(str(path) + ".meta").write_text("".join(line + chr(10) for line in lines), encoding="utf-8")


def deploy(root: Path) -> None:
    target = root / PROJECT / EVIDENCE_DIR
    if target.exists():
        shutil.rmtree(target)
    shutil.copytree(HERE / "Unity/H2F03Evidence", target)
    deterministic_meta(target, "H2F03Evidence")
    for path in sorted(target.rglob("*")):
        if not path.name.endswith(".meta"):
            deterministic_meta(path, "H2F03Evidence/" + path.relative_to(target).as_posix())
    write_json(results(root) / "evidence_code.json", evidence_code_digest())
    print(f"H2F03_EVIDENCE_CODE_DEPLOYED files={len(evidence_code_digest())}")


def unity(root: Path, method: str, name: str, windowed: bool = False, extra: list[str] | None = None, timeout: int = 3600) -> int:
    logs = results(root) / "logs"
    logs.mkdir(parents=True, exist_ok=True)
    log = (logs / f"{name}.log").resolve()
    command = [str(UNITY)] + ([] if windowed else ["-batchmode", "-quit"]) + [
        "-projectPath", str((root / PROJECT).resolve()), "-logFile", str(log)]
    if method != "-":
        command += ["-executeMethod", method]
    command += ["-h2f03-results", str(results(root).resolve())] + [a for a in (extra or []) if a != "--"]
    try:
        code = subprocess.call(command, timeout=timeout)
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


def tree(root: Path, sha: str) -> dict:
    """Committed candidate bytes must be left untouched by import, provisioning, lifecycle and evidence runs."""
    rows = git("ls-tree", "-r", sha, PROJECT).decode().splitlines()
    changed = []
    for line in rows:
        meta, path = line.split("\t", 1)
        target = root / path
        if not target.exists():
            changed.append("MISSING " + path)
            continue
        data = target.read_bytes()
        if b"\0" not in data:
            data = data.replace(b"\r\n", b"\n")
        if hashlib.sha1(b"blob %d\0" % len(data) + data).hexdigest() != meta.split()[2]:
            changed.append("CHANGED " + path)
    return {"files": len(rows), "changed": changed}


def prepare(root: Path, sha: str, vault_root: Path) -> int:
    create(root, sha)
    steps = [("build", lambda: build(root)), ("vault", lambda: vault(root, sha, vault_root)), ("gc2", lambda: gc2(root))]
    for name, step in steps:
        code = step()
        if code != 0:
            print(f"H2F03_PREPARE_FAILED step={name} exit={code}")
            return code
    content(root, vault_root)
    deploy(root)
    return 0


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["create", "build", "vault", "gc2", "gc2-remove", "gc2-status", "content", "deploy",
                                         "unity", "tree", "prepare"])
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--vault", type=Path, default=DEFAULT_VAULT)
    parser.add_argument("--sha")
    parser.add_argument("--windowed", action="store_true")
    args, rest = parser.parse_known_args()
    root = args.root.resolve()

    def need_sha() -> str:
        if not args.sha or len(args.sha) != 40:
            raise SystemExit(f"{args.mode} needs --sha <40-char candidate>")
        return args.sha

    if args.mode == "create":
        create(root, need_sha())
    elif args.mode == "build":
        return build(root)
    elif args.mode == "vault":
        return vault(root, need_sha(), args.vault.resolve(strict=True))
    elif args.mode == "gc2":
        return gc2(root)
    elif args.mode == "gc2-remove":
        return gc2(root, "remove")
    elif args.mode == "gc2-status":
        return gc2(root, "status")
    elif args.mode == "content":
        content(root, args.vault.resolve(strict=True))
    elif args.mode == "deploy":
        deploy(root)
    elif args.mode == "unity":
        if len(rest) < 2:
            raise SystemExit("unity needs <Class.Method> <log-name>")
        return unity(root, rest[0], rest[1], args.windowed, rest[2:])
    elif args.mode == "tree":
        result = tree(root, need_sha())
        print(json.dumps(result, indent=2))
        return 0 if not result["changed"] else 1
    elif args.mode == "prepare":
        return prepare(root, need_sha(), args.vault.resolve(strict=True))
    return 0


if __name__ == "__main__":
    sys.exit(main())
