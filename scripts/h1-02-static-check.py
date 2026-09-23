#!/usr/bin/env python3
"""Deterministic repository-side checks for WP-H1-02.

This checker deliberately proves only text/graph properties available without a
Unity Editor. Effective Editor/package/assembly properties are produced and
tested by the local Unity round and are required in --mode final.
"""

from __future__ import annotations

import argparse
import json
import shutil
import tempfile
import xml.etree.ElementTree as ET
from pathlib import Path

EXPECTED_EDITOR = "6000.3.24f1"
EXPECTED_REVISION = "4e7b9b5b6244"
EXPECTED_DIRECT_PACKAGES = {"com.unity.test-framework": "1.6.0"}
PROJECT_REL = Path("Unity/ArkusUnity")
UNITY_WRAPPER_REL = Path("scripts/h1-02-unity.ps1")

REQUIRED_IGNORES = {
    "Unity/ArkusUnity/[Ll]ibrary/",
    "Unity/ArkusUnity/[Tt]emp/",
    "Unity/ArkusUnity/[Oo]bj/",
    "Unity/ArkusUnity/[Ll]ogs/",
    "Unity/ArkusUnity/[Uu]ser[Ss]ettings/",
}

FORBIDDEN_H0_REFERENCE_TOKENS = ("UnityEngine", "UnityEditor", "Unity/ArkusUnity")


class CheckFailure(RuntimeError):
    pass


def fail(message: str) -> None:
    raise CheckFailure(message)


def read_json(path: Path) -> dict:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError:
        fail(f"missing-file:{path}")
    except json.JSONDecodeError as exc:
        fail(f"invalid-json:{path}:{exc}")


def check_project_version(root: Path) -> None:
    path = root / PROJECT_REL / "ProjectSettings/ProjectVersion.txt"
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError:
        fail(f"project-version-missing:{path}")

    required = {
        f"m_EditorVersion: {EXPECTED_EDITOR}",
        f"m_EditorVersionWithRevision: {EXPECTED_EDITOR} ({EXPECTED_REVISION})",
    }
    missing = sorted(required.difference(set(text.splitlines())))
    if missing:
        fail("editor-pin-mismatch:" + ",".join(missing))


def check_manifest(root: Path, *, final: bool) -> None:
    project = root / PROJECT_REL
    manifest = read_json(project / "Packages/manifest.json")
    dependencies = manifest.get("dependencies")
    if dependencies != EXPECTED_DIRECT_PACKAGES:
        fail(
            "direct-package-set-mismatch:"
            + json.dumps(dependencies, sort_keys=True, separators=(",", ":"))
        )

    for name, version in dependencies.items():
        if not version or any(token in version.lower() for token in ("latest", "*", "file:", "git")):
            fail(f"package-not-exact:{name}:{version}")

    lock_path = project / "Packages/packages-lock.json"
    if not final:
        return
    lock = read_json(lock_path)
    locked = lock.get("dependencies", {})
    package = locked.get("com.unity.test-framework")
    if not isinstance(package, dict):
        fail("test-framework-missing-from-lock")
    if package.get("version") != "1.6.0":
        fail(f"test-framework-lock-version:{package.get('version')}")
    if package.get("depth") != 0:
        fail(f"test-framework-not-direct-in-lock:{package.get('depth')}")


def check_gitignore(root: Path) -> None:
    try:
        lines = {
            line.strip()
            for line in (root / ".gitignore").read_text(encoding="utf-8").splitlines()
            if line.strip() and not line.lstrip().startswith("#")
        }
    except FileNotFoundError:
        fail("gitignore-missing")
    missing = sorted(REQUIRED_IGNORES.difference(lines))
    if missing:
        fail("unity-cache-ignore-missing:" + ",".join(missing))


def check_h0_project_graph(root: Path) -> None:
    candidates = sorted((root / "src").glob("**/*.csproj")) + sorted(
        (root / "tests").glob("**/*.csproj")
    )
    if not candidates:
        fail("h0-project-universe-empty")

    violations: list[str] = []
    for path in candidates:
        try:
            tree = ET.parse(path)
        except ET.ParseError as exc:
            fail(f"invalid-csproj:{path}:{exc}")
        for element in tree.getroot().iter():
            include = element.attrib.get("Include", "")
            if any(token.lower() in include.lower() for token in FORBIDDEN_H0_REFERENCE_TOKENS):
                violations.append(f"{path.relative_to(root)}:{element.tag}:{include}")
    if violations:
        fail("h0-unity-dependency:" + "|".join(violations))


def check_asmdef_direction(root: Path) -> None:
    editor = read_json(
        root
        / PROJECT_REL
        / "Assets/Arkus/H1/Editor/Arkus.H1.Baseline.Editor.asmdef"
    )
    if editor.get("references") != []:
        fail("baseline-editor-asmdef-must-have-no-arkus-product-reference")

    tests = read_json(
        root
        / PROJECT_REL
        / "Assets/Arkus/H1/Tests/Editor/Arkus.H1.Baseline.Tests.Editor.asmdef"
    )
    if tests.get("references") != ["Arkus.H1.Baseline.Editor"]:
        fail("baseline-test-asmdef-reference-mismatch")
    if tests.get("optionalUnityReferences") != ["TestAssemblies"]:
        fail("baseline-test-asmdef-test-reference-mismatch")


def check_meta_files(root: Path) -> None:
    assets = root / PROJECT_REL / "Assets"
    governed = sorted(
        path
        for path in assets.glob("**/*")
        if path.is_file() and path.suffix.lower() in {".cs", ".asmdef"}
    )
    if not governed:
        fail("unity-governed-asset-universe-empty")
    missing = [
        str(path.relative_to(root))
        for path in governed
        if not Path(str(path) + ".meta").is_file()
    ]
    if missing:
        fail("visible-meta-missing:" + ",".join(missing))


def check_unity_wrapper(root: Path) -> None:
    path = root / UNITY_WRAPPER_REL
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError:
        fail(f"unity-wrapper-missing:{path}")

    executable = "\n".join(
        line for line in text.splitlines() if not line.lstrip().startswith("#")
    )
    required = (
        "Start-Process",
        "-Wait",
        "-PassThru",
        "$process.ExitCode",
        "ConvertTo-UnityProcessArgument",
    )
    for token in required:
        if token not in executable:
            fail(f"unity-wrapper-wait-contract-missing:{token}")

    if "$LASTEXITCODE" in executable:
        fail("unity-wrapper-must-not-use-last-exit-code-for-unity-gui-process")
    if "& $editor @arguments" in executable:
        fail("unity-wrapper-direct-gui-invocation-reintroduced")


def check_effective_inventory(root: Path) -> None:
    inventory = read_json(root / "Docs/evidence/WP-H1-02/effective-inventory.json")
    expected_scalars = {
        "schema": "arkus.h1-02-effective-inventory@1",
        "projectIdentity": "arkus-h1-unity",
        "unityVersion": EXPECTED_EDITOR,
        "serializationMode": "ForceText",
        "externalVersionControl": "Visible Meta Files",
        "renderPipeline": "builtin",
    }
    for key, expected in expected_scalars.items():
        if inventory.get(key) != expected:
            fail(f"effective-inventory-{key}:{inventory.get(key)!r}")

    packages = inventory.get("packages")
    assemblies = inventory.get("assemblies")
    if not isinstance(packages, list) or not packages:
        fail("effective-package-inventory-empty")
    if not isinstance(assemblies, list) or not assemblies:
        fail("effective-assembly-inventory-empty")

    found = {item.get("name"): item.get("version") for item in packages if isinstance(item, dict)}
    if found.get("com.unity.test-framework") != "1.6.0":
        fail(
            "effective-test-framework-version:"
            + str(found.get("com.unity.test-framework"))
        )

    names = {item.get("name") for item in assemblies if isinstance(item, dict)}
    for expected in ("Arkus.H1.Baseline.Editor", "Arkus.H1.Baseline.Tests.Editor"):
        if expected not in names:
            fail(f"effective-assembly-missing:{expected}")


def run_checks(root: Path, *, final: bool) -> None:
    check_project_version(root)
    check_manifest(root, final=final)
    check_gitignore(root)
    check_h0_project_graph(root)
    check_asmdef_direction(root)
    check_meta_files(root)
    check_unity_wrapper(root)
    if final:
        check_effective_inventory(root)


def expect_red(root: Path, name: str, mutate) -> None:
    with tempfile.TemporaryDirectory(prefix="h1-02-negative-") as temp:
        fixture = Path(temp) / "repo"
        shutil.copytree(
            root,
            fixture,
            ignore=shutil.ignore_patterns(".git", "bin", "obj", "Library", "Temp", "artifacts"),
        )
        mutate(fixture)
        try:
            run_checks(fixture, final=False)
        except CheckFailure:
            print(f"NEGATIVE_CONTROL_RED:{name}")
            return
        fail(f"negative-control-false-green:{name}")


def run_self_tests(root: Path) -> None:
    expect_red(
        root,
        "editor-pin",
        lambda r: (r / PROJECT_REL / "ProjectSettings/ProjectVersion.txt").write_text(
            "m_EditorVersion: 6000.3.0f1\n",
            encoding="utf-8",
        ),
    )

    def loosen_package(r: Path) -> None:
        path = r / PROJECT_REL / "Packages/manifest.json"
        value = read_json(path)
        value["dependencies"]["com.unity.test-framework"] = "latest"
        path.write_text(json.dumps(value, indent=2) + "\n", encoding="utf-8")

    expect_red(root, "package-pin", loosen_package)

    def inject_h0_unity_reference(r: Path) -> None:
        path = next((r / "src").glob("**/*.csproj"))
        text = path.read_text(encoding="utf-8")
        text = text.replace(
            "</Project>",
            '  <ItemGroup><Reference Include="UnityEngine" /></ItemGroup>\n</Project>',
        )
        path.write_text(text, encoding="utf-8")

    expect_red(root, "h0-unity-reference", inject_h0_unity_reference)

    def remove_library_ignore(r: Path) -> None:
        path = r / ".gitignore"
        text = path.read_text(encoding="utf-8").replace(
            "Unity/ArkusUnity/[Ll]ibrary/\n", ""
        )
        path.write_text(text, encoding="utf-8")

    expect_red(root, "unity-cache-ignore", remove_library_ignore)

    def remove_unity_wait(r: Path) -> None:
        path = r / UNITY_WRAPPER_REL
        text = path.read_text(encoding="utf-8").replace(
            " -Wait -PassThru", " -PassThru", 1
        )
        path.write_text(text, encoding="utf-8")

    expect_red(root, "unity-process-wait", remove_unity_wait)


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", default=".")
    parser.add_argument(
        "--mode",
        choices=("remote-prep", "final"),
        default="remote-prep",
    )
    parser.add_argument("--self-test", action="store_true")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    root = Path(args.root).resolve()
    try:
        run_checks(root, final=args.mode == "final")
        print(f"H1_02_STATIC_CHECK_GREEN mode={args.mode}")
        if args.self_test:
            run_self_tests(root)
            print("H1_02_STATIC_NEGATIVE_CONTROLS_GREEN")
        return 0
    except CheckFailure as exc:
        print(f"H1_02_STATIC_CHECK_RED {exc}")
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
