"""Disposable WP-H2F-02 evidence workspace: the exact candidate project + representative real content.

  python workspace.py create --sha <candidate> [--root C:\\Juego2-H2F02-W]   # git archive of Unity/ArkusUnity at that SHA
  python workspace.py content [--root ...] [--vault C:\\Juego2-Assets]       # ART-01 PREFOUNDATION_INPUT + vault sources
  python workspace.py gc2 [--root ...]                                       # hash-gated Core via scripts/h2f02-provision.py
  python workspace.py status [--root ...]

The workspace lives outside the repository and is never committed. `create` copies only committed candidate bytes
(no Library, no ignored state), so it doubles as the clean-restoration check. `content` reuses the pinned inputs
already accepted upstream: ART-01 structural checkpoint 174d05d2 (git) + ART-01 SOURCE_LOCK.json and H2F-01
SPIKE_INPUT_LOCK.json (owner vault, hash-checked by the H2F-01 recipe). Only the evidence editor code in ./Editor is
added on top; it drives the foundation's public surfaces and writes results under <root>/results.
"""

from __future__ import annotations

import argparse
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
DEFAULT_ROOT = Path(r"C:\Juego2-H2F02-W")
DEFAULT_VAULT = Path(r"C:\Juego2-Assets")
PROJECT = "Unity/ArkusUnity"
ART_SHA = "174d05d23c3bceb9d5df00e460b33519cf68328e"


def h2f01_recipe():
    spec = importlib.util.spec_from_file_location("h2f01_bootstrap", REPO / "Docs/evidence/WP-H2F-01/spike_project/bootstrap.py")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    module.PROJECT = PROJECT  # same pinned import functions, pointed at the adopted project layout
    return module


def untar(data: bytes, root: Path, strip: str = "", only_new: bool = False) -> int:
    count = 0
    with tarfile.open(fileobj=io.BytesIO(data)) as tar:
        for member in tar.getmembers():
            if not member.isfile():
                continue
            target = root / member.name[len(strip):]
            if only_new and target.exists():
                continue
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(tar.extractfile(member).read())
            count += 1
    return count


def create(root: Path, sha: str) -> None:
    if REPO in root.parents or root == REPO:
        raise SystemExit("Workspace must stay outside the repository")
    if root.exists():
        shutil.rmtree(root)
    data = subprocess.check_output(["git", "-C", str(REPO), "archive", "--format=tar", sha, PROJECT])
    n = untar(data, root)
    (root / "results").mkdir(parents=True, exist_ok=True)
    (root / "results/workspace.json").write_text(json.dumps({"candidateSha": sha, "files": n, "artInputSha": ART_SHA}, indent=2) + "\n")
    print(f"H2F02_WORKSPACE_CREATED root={root} sha={sha} files={n}")


def content(root: Path, vault: Path) -> None:
    recipe = h2f01_recipe()
    art = subprocess.check_output(["git", "-C", str(REPO), "archive", "--format=tar", ART_SHA,
                                   PROJECT + "/Assets/Arkus/ART", PROJECT + "/Assets/Arkus/ART.meta"])
    n_art = untar(art, root)
    for rel in ["Docs/evidence/WP-ART-01/KIT_COMPOSITION_MANIFEST.json", "Docs/evidence/WP-ART-01/SOURCE_LOCK.json"]:
        target = root / rel
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(subprocess.check_output(["git", "-C", str(REPO), "show", f"{ART_SHA}:{rel}"]))
    # evidence-only driver (references GC2 directly, so it is added after the clean/GC2 baseline checks ran)
    editor = root / PROJECT / "Assets/H2F02Evidence/Editor"
    editor.mkdir(parents=True, exist_ok=True)
    for f in list((HERE / "Editor").glob("*.cs")) + list((HERE / "Editor").glob("*.asmdef")):
        shutil.copyfile(f, editor / f.name)
    n_src = recipe.import_art_sources(root, vault)
    n_extra = recipe.import_extra(root, vault)
    (root / "results/content.json").write_text(json.dumps({"artFiles": n_art, "artSources": n_src, "h2f01Extra": n_extra,
                                                           "artInputSha": ART_SHA}, indent=2) + "\n")
    print(f"H2F02_WORKSPACE_CONTENT art_files={n_art} art_sources={n_src} extra={n_extra}")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["create", "content", "gc2", "status"])
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--vault", type=Path, default=DEFAULT_VAULT)
    parser.add_argument("--sha")
    args = parser.parse_args()
    root = args.root.resolve()
    if args.mode == "create":
        if not args.sha or len(args.sha) != 40:
            raise SystemExit("create needs --sha <40-char candidate>")
        create(root, args.sha)
    elif args.mode == "content":
        content(root, args.vault.resolve(strict=True))
    elif args.mode == "gc2":
        return subprocess.call([sys.executable, str(REPO / "scripts/h2f02-provision.py"), "gc2", "--project", str(root / PROJECT)])
    else:
        return subprocess.call([sys.executable, str(REPO / "scripts/h2f02-provision.py"), "status", "--project", str(root / PROJECT)])
    return 0


if __name__ == "__main__":
    sys.exit(main())
