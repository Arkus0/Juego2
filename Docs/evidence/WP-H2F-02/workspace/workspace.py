"""Disposable WP-H2F-02 evidence workspace: the exact candidate project + representative real content.

  python workspace.py create --sha <candidate> [--root C:\\Juego2-H2F02-W]   # git archive of Unity/ArkusUnity at that SHA
  python workspace.py content [--root ...] [--vault C:\\Juego2-Assets]       # ART-01 PREFOUNDATION_INPUT + vault sources
  python workspace.py gc2 [--root ...]                                       # hash-gated Core via scripts/h2f02-provision.py
  python workspace.py status [--root ...]
  python workspace.py run --sha <candidate> [--root ...] [--vault ...]      # the complete evidence sequence

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


UNITY = r"C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe"


def unity(root: Path, method: str, name: str, *extra: str) -> int:
    return subprocess.call([sys.executable, str(REPO / "scripts/h2f02-unity.py"), str(root / PROJECT), method,
                            str(root / "results/logs"), name, "--timeout", "3000", "--", *extra])


def tests(root: Path, assemblies: str, name: str) -> dict:
    import xml.etree.ElementTree as ET
    xml = root / "results" / f"{name}.xml"
    subprocess.call([UNITY, "-batchmode", "-projectPath", str(root / PROJECT), "-runTests", "-testPlatform", "EditMode",
                     "-assemblyNames", assemblies, "-testResults", str(xml), "-logFile", str(root / "results/logs" / f"{name}.log")])
    if not xml.exists():
        return {"result": "NO_RESULTS"}
    run_node = ET.parse(xml).getroot()
    failed = [c.get("fullname") for c in run_node.iter("test-case") if c.get("result") != "Passed"]
    return {k: run_node.get(k) for k in ("result", "total", "passed", "failed")} | {"failedCases": failed}


def tree_compare(root: Path, sha: str) -> dict:
    """Committed candidate bytes must be left untouched by import, provisioning and evidence (line endings normalized)."""
    import hashlib
    rows = subprocess.check_output(["git", "-C", str(REPO), "ls-tree", "-r", sha, PROJECT]).decode().splitlines()
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


def run(root: Path, sha: str, vault: Path) -> int:
    summary = {"candidateSha": sha,
               "unityTree": subprocess.check_output(["git", "-C", str(REPO), "rev-parse", f"{sha}:{PROJECT}"]).decode().strip()}
    create(root, sha)
    (root / "results/logs").mkdir(parents=True, exist_ok=True)

    def res(name: str) -> str:
        return str(root / "results" / name)

    # A: clean restoration from committed bytes, Core absent (the hosted-CI / fresh-clone state)
    summary["a_clean_absent_verify_exit"] = unity(root, "Juego2.Foundation.Editor.J2FoundationBatch.Verify", "a1_verify",
                                                  "-j2-output", res("a1_clean_absent_report.json"))
    summary["a_clean_absent_tests"] = tests(root, "Juego2.Foundation.Tests.Editor", "a2_clean_absent_tests")
    summary["a_tree"] = tree_compare(root, sha)
    # B: lawful Core provisioning
    summary["b_gc2_provision_exit"] = subprocess.call([sys.executable, str(REPO / "scripts/h2f02-provision.py"), "gc2",
                                                       "--project", str(root / PROJECT)])
    shutil.copyfile(root / PROJECT / "Assets/Plugins/GameCreator/.juego2-provisioning.json", root / "results/b0_gc2_provisioning_receipt.json")
    summary["b_gc2_verify_exit"] = unity(root, "Juego2.Foundation.Editor.J2FoundationBatch.Verify", "b1_verify",
                                         "-j2-output", res("b1_gc2_report.json"), "-j2-require-gc2")
    summary["b_gc2_tests"] = tests(root, "Juego2.Foundation.Tests.Editor;Juego2.Gc2Adapter.Tests.Editor", "b2_gc2_tests")
    summary["b_tree"] = tree_compare(root, sha)
    # C: representative real content, GC2 content lint over the saved evidence scenes, player build
    content(root, vault)
    summary["c_representative_exit"] = unity(root, "Juego2.H2F02.Evidence.J2H2F02Evidence.Representative", "c1_representative")
    summary["c_lint_exit"] = unity(root, "Juego2.Gc2Adapter.Editor.J2Gc2Batch.LintProject", "c2_lint",
                                   "-j2-output", res("c2_gc2_lint.json"), "-j2-roots", "Assets/H2F02Evidence")
    summary["c_player_build_exit"] = unity(root, "Juego2.H2F02.Evidence.J2H2F02Evidence.PlayerBuild", "c3_player_build")
    summary["c_tree"] = tree_compare(root, sha)
    (root / "results/summary.json").write_text(json.dumps(summary, indent=2) + "\n")
    print(json.dumps(summary, indent=2))
    return 0


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["create", "content", "gc2", "status", "run"])
    parser.add_argument("--root", type=Path, default=DEFAULT_ROOT)
    parser.add_argument("--vault", type=Path, default=DEFAULT_VAULT)
    parser.add_argument("--sha")
    args = parser.parse_args()
    root = args.root.resolve()
    if args.mode == "run":
        if not args.sha or len(args.sha) != 40:
            raise SystemExit("run needs --sha <40-char candidate>")
        return run(root, args.sha, args.vault.resolve(strict=True))
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
