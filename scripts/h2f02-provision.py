"""Lawful local provisioning for the Juego2 Unity foundation (WP-H2F-02).

GC2 Core is an owner-purchased Unity Asset Store package (per-seat EULA, not redistributable). Its bytes are never
committed. This script restores it into a Juego2 Unity project from the owner's local copy:

  python scripts/h2f02-provision.py gc2 [--project Unity/ArkusUnity] [--package <Game Creator 2.unitypackage>]
  python scripts/h2f02-provision.py status [--project ...]
  python scripts/h2f02-provision.py remove [--project ...]

`gc2` refuses any package whose SHA-256/size differ from the admitted Core 2.19.61, extracts `Assets/**` only (the
vendor `Packages/manifest.json` is never applied), proves that no project-global file changed, enables the Juego2
GC2 assemblies through the git-ignored `Assets/csc.rsp` define and writes a provisioning receipt. `remove` returns
the project to the Core-absent state that a clean clone and hosted CI have.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
import shutil
import sys
import tarfile
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
DEFAULT_PROJECT = REPO / "Unity/ArkusUnity"
DEFAULT_PACKAGE = Path(os.environ.get("APPDATA", "")) / "Unity/Asset Store-5.x/Catsoft Works/Editor ExtensionsGame Toolkits/Game Creator 2.unitypackage"
GC2_SHA256 = "1e4f3ba0560eadb944b3cf3ca63f7095d95b47c55a94ee655f28a15952f3380b"
GC2_BYTES = 28236878
GC2_VERSION = "2.19.61"
GC2_ROOT = "Assets/Plugins/GameCreator"
RECEIPT = GC2_ROOT + "/.juego2-provisioning.json"
DEFINE = "JUEGO2_GC2_CORE"
RSP = "Assets/csc.rsp"
GLOBAL_STATE = ["Packages/manifest.json", "Packages/packages-lock.json"]


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1 << 20), b""):
            digest.update(chunk)
    return digest.hexdigest()


def global_snapshot(project: Path) -> dict[str, str]:
    files = [project / p for p in GLOBAL_STATE] + sorted((project / "ProjectSettings").glob("*"))
    return {f.relative_to(project).as_posix(): sha256(f) for f in files if f.is_file()}


def extract_assets_only(package: Path, project: Path) -> tuple[int, list[str]]:
    imported, excluded = 0, []
    with tarfile.open(package, "r:gz") as tar:
        members = {m.name: m for m in tar.getmembers()}
        for name, member in sorted(members.items()):
            if not name.endswith("/pathname"):
                continue
            guid = name.split("/")[0]
            path = tar.extractfile(member).read().decode("utf-8").splitlines()[0].strip()
            if not path.startswith(GC2_ROOT + "/") and path != GC2_ROOT:
                if path.startswith("Assets/"):
                    raise SystemExit(f"H2F02_GC2_UNEXPECTED_PATH {path}")
                excluded.append(path)
                continue
            target = project / path
            asset, meta = members.get(f"{guid}/asset"), members.get(f"{guid}/asset.meta")
            if asset is not None:
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_bytes(tar.extractfile(asset).read())
            else:
                target.mkdir(parents=True, exist_ok=True)
            if meta is not None:
                Path(str(target) + ".meta").write_bytes(tar.extractfile(meta).read())
            imported += 1
    return imported, excluded


def write_rsp(project: Path, enabled: bool) -> None:
    rsp = project / RSP
    if enabled:
        rsp.write_text(f"-define:{DEFINE}\n", encoding="utf-8")
    elif rsp.exists():
        rsp.unlink()


def provision(project: Path, package: Path) -> None:
    if not package.is_file():
        raise SystemExit(f"H2F02_GC2_PACKAGE_MISSING {package} (owner Asset Store download required)")
    size, digest = package.stat().st_size, sha256(package)
    if (digest, size) != (GC2_SHA256, GC2_BYTES):
        raise SystemExit(f"H2F02_GC2_PACKAGE_REFUSED sha256={digest} bytes={size}")
    before = global_snapshot(project)
    imported, excluded = extract_assets_only(package, project)
    version = (project / GC2_ROOT / "Packages/Core/Editor/Version.txt").read_text(encoding="utf-8").strip()
    if version != GC2_VERSION:
        raise SystemExit(f"H2F02_GC2_VERSION_MISMATCH {version}")
    if global_snapshot(project) != before:
        raise SystemExit("H2F02_GC2_PROJECT_GLOBAL_STATE_CHANGED")
    write_rsp(project, True)
    receipt = {"schema": "juego2.h2f02.gc2-provisioning@1", "product": "Game Creator 2 Core", "version": version,
               "packageSha256": digest, "packageBytes": size, "route": "assets-only", "imported": imported,
               "excluded": excluded, "define": DEFINE}
    (project / RECEIPT).write_text(json.dumps(receipt, indent=2) + "\n", encoding="utf-8")
    print(f"H2F02_GC2_PROVISIONED version={version} imported={imported} excluded={excluded}")


def status(project: Path) -> int:
    receipt = project / RECEIPT
    if not (project / GC2_ROOT).exists():
        print(f"H2F02_GC2_ABSENT rsp={'present' if (project / RSP).exists() else 'absent'}")
        return 0 if not (project / RSP).exists() else 3
    data = json.loads(receipt.read_text(encoding="utf-8")) if receipt.exists() else {}
    ok = data.get("packageSha256") == GC2_SHA256 and data.get("version") == GC2_VERSION and (project / RSP).exists()
    print(f"H2F02_GC2_{'PROVISIONED' if ok else 'INVALID'} version={data.get('version')} sha256={data.get('packageSha256')}")
    return 0 if ok else 3


def remove(project: Path) -> None:
    write_rsp(project, False)
    for p in [project / GC2_ROOT, project / (GC2_ROOT + ".meta")]:
        if p.is_dir():
            shutil.rmtree(p)
        elif p.exists():
            p.unlink()
    plugins = project / "Assets/Plugins"
    if plugins.is_dir() and not any(plugins.iterdir()):
        plugins.rmdir()
        meta = project / "Assets/Plugins.meta"
        if meta.exists():
            meta.unlink()
    print("H2F02_GC2_REMOVED")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["gc2", "status", "remove"])
    parser.add_argument("--project", type=Path, default=DEFAULT_PROJECT)
    parser.add_argument("--package", type=Path, default=DEFAULT_PACKAGE)
    args = parser.parse_args()
    project = args.project.resolve()
    if not (project / "ProjectSettings/ProjectVersion.txt").is_file():
        raise SystemExit(f"H2F02_NOT_A_UNITY_PROJECT {project}")
    if args.mode == "gc2":
        provision(project, args.package)
        return 0
    if args.mode == "remove":
        remove(project)
        return 0
    return status(project)


if __name__ == "__main__":
    sys.exit(main())
