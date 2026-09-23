#!/usr/bin/env python3
"""Deterministic remote proof guards for WP-H1-02.

Unity runtime/editor facts stay owned by the already accepted local round. This
checker proves repository-side invariants without launching Unity: the retained
Unity project policy cannot contain tracked generated/editor-local state, and
the accepted H0 MSBuild project universe remains engine-neutral after real
MSBuild evaluation (including central/imported props and targets).
"""

from __future__ import annotations

import argparse
import html
import json
import shutil
import subprocess
import tempfile
from pathlib import Path
from typing import Callable, Iterable

EXPECTED_EDITOR = "6000.3.24f1"
EXPECTED_REVISION = "4e7b9b5b6244"
EXPECTED_DIRECT_PACKAGES = {"com.unity.test-framework": "1.6.0"}
EVIDENCE_COMMIT_SHA = "91e1e42bc02e95952a00af6db549b78a9e94975e"
PROJECT_REL = Path("Unity/ArkusUnity")
UNITY_WRAPPER_REL = Path("scripts/h1-02-unity.ps1")

# Minimum retained-Unity policy. This is a policy-presence guard only; the
# actual tracked-file oracle asks Git to apply the policy to git ls-files.
REQUIRED_UNITY_IGNORES = {
    "Unity/ArkusUnity/[Ll]ibrary/",
    "Unity/ArkusUnity/[Tt]emp/",
    "Unity/ArkusUnity/[Oo]bj/",
    "Unity/ArkusUnity/[Bb]uild/",
    "Unity/ArkusUnity/[Bb]uilds/",
    "Unity/ArkusUnity/[Ll]ogs/",
    "Unity/ArkusUnity/[Uu]ser[Ss]ettings/",
    "Unity/ArkusUnity/MemoryCaptures/",
    "Unity/ArkusUnity/Recordings/",
    "Unity/ArkusUnity/*.csproj",
    "Unity/ArkusUnity/*.sln",
    "Unity/ArkusUnity/*.user",
}

# Defect injections span every retained-Unity generated/editor-local class plus
# equivalent generated classes already declared by the repository policy.
TRACKED_POLICY_FIXTURES = (
    "Unity/ArkusUnity/Library/state.bin",
    "Unity/ArkusUnity/Temp/state.bin",
    "Unity/ArkusUnity/Obj/state.bin",
    "Unity/ArkusUnity/Build/player.bin",
    "Unity/ArkusUnity/Builds/player.bin",
    "Unity/ArkusUnity/Logs/editor.log",
    "Unity/ArkusUnity/UserSettings/EditorUserSettings.asset",
    "Unity/ArkusUnity/MemoryCaptures/capture.snap",
    "Unity/ArkusUnity/Recordings/recording.bin",
    "Unity/ArkusUnity/Generated.csproj",
    "Unity/ArkusUnity/Generated.sln",
    "Unity/ArkusUnity/Generated.user",
    "src/Probe/bin/generated.dll",
    "src/Probe/obj/generated.dll",
    "TestResults/result.trx",
    "artifacts/output.txt",
    ".vs/state.bin",
    ".idea/state.xml",
    "__pycache__/probe.pyc",
)

MSBUILD_PROBE_XML = """<Project>
  <Target Name="H102EmitEffectiveReferenceGraph" DependsOnTargets="ResolveReferences">
    <ItemGroup>
      <_H102Record Include="@(PackageReference)" H102Kind="PackageReference" />
      <_H102Record Include="@(ProjectReference)" H102Kind="ProjectReference" />
      <_H102Record Include="@(Reference)" H102Kind="Reference" />
      <_H102Record Include="@(ReferencePath)" H102Kind="ReferencePath" />
      <_H102Record Include="@(ReferenceCopyLocalPaths)" H102Kind="ReferenceCopyLocalPaths" />
      <_H102Record Include="@(ResolvedProjectReferencePaths)" H102Kind="ResolvedProjectReferencePaths" />
    </ItemGroup>
    <WriteLinesToFile File="$(H102GraphFile)"
      Lines="@(_H102Record-&gt;'%(H102Kind)|%(Identity)|%(FullPath)|%(NuGetPackageId)|%(OriginalItemSpec)|%(ResolvedFrom)')"
      Overwrite="true" />
  </Target>
</Project>
"""


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


def run_process(
    args: list[str],
    *,
    cwd: Path,
    input_bytes: bytes | None = None,
    allowed_returncodes: Iterable[int] = (0,),
) -> subprocess.CompletedProcess[bytes]:
    try:
        result = subprocess.run(
            args,
            cwd=cwd,
            input=input_bytes,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            check=False,
        )
    except FileNotFoundError:
        fail(f"required-tool-missing:{args[0]}")
    if result.returncode not in set(allowed_returncodes):
        stderr = result.stderr.decode("utf-8", errors="replace").strip()
        stdout = result.stdout.decode("utf-8", errors="replace").strip()
        fail(f"command-failed:{' '.join(args)}:{result.returncode}:{stderr or stdout}")
    return result


def git_bytes(root: Path, *args: str, allowed_returncodes: Iterable[int] = (0,)) -> bytes:
    return run_process(
        ["git", "-C", str(root), *args],
        cwd=root,
        allowed_returncodes=allowed_returncodes,
    ).stdout


def git_lines(root: Path, *args: str) -> list[str]:
    return [
        line
        for line in git_bytes(root, *args).decode("utf-8", errors="strict").splitlines()
        if line
    ]


def tracked_files(root: Path) -> list[str]:
    raw = git_bytes(root, "ls-files", "-z")
    return [item.decode("utf-8") for item in raw.split(b"\0") if item]


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
    if not final:
        return
    lock = read_json(project / "Packages/packages-lock.json")
    package = lock.get("dependencies", {}).get("com.unity.test-framework")
    if not isinstance(package, dict):
        fail("test-framework-missing-from-lock")
    if package.get("version") != "1.6.0":
        fail(f"test-framework-lock-version:{package.get('version')}")
    if package.get("depth") != 0:
        fail(f"test-framework-not-direct-in-lock:{package.get('depth')}")


def gitignore_policy_lines(root: Path) -> set[str]:
    try:
        return {
            line.strip()
            for line in (root / ".gitignore").read_text(encoding="utf-8").splitlines()
            if line.strip() and not line.lstrip().startswith("#")
        }
    except FileNotFoundError:
        fail("gitignore-missing")


def check_required_ignore_policy(root: Path) -> None:
    missing = sorted(REQUIRED_UNITY_IGNORES.difference(gitignore_policy_lines(root)))
    if missing:
        fail("unity-generated-policy-missing:" + ",".join(missing))


def check_tracked_generated_outputs(root: Path) -> None:
    """Fail if Git's own ignore policy matches any actually tracked file."""
    check_required_ignore_policy(root)
    tracked = tracked_files(root)
    if not tracked:
        fail("tracked-file-universe-empty")
    payload = b"\0".join(path.encode("utf-8") for path in tracked) + b"\0"
    result = run_process(
        ["git", "-C", str(root), "check-ignore", "--no-index", "-z", "--stdin"],
        cwd=root,
        input_bytes=payload,
        allowed_returncodes=(0, 1),
    )
    ignored = [item.decode("utf-8") for item in result.stdout.split(b"\0") if item]
    if ignored:
        fail("tracked-generated-or-editor-local:" + ",".join(sorted(ignored)))


def accepted_h0_projects_at(root: Path, revision: str) -> list[str]:
    paths = git_lines(root, "ls-tree", "-r", "--name-only", revision, "--", "src", "tests")
    return sorted(
        path
        for path in paths
        if path.lower().endswith(".csproj")
        and (path.startswith("src/") or path.startswith("tests/"))
    )


def current_h0_projects(root: Path) -> list[str]:
    return sorted(
        path
        for path in tracked_files(root)
        if path.lower().endswith(".csproj")
        and (path.startswith("src/") or path.startswith("tests/"))
    )


def check_evidence_ancestry(root: Path) -> None:
    result = run_process(
        ["git", "-C", str(root), "merge-base", "--is-ancestor", EVIDENCE_COMMIT_SHA, "HEAD"],
        cwd=root,
        allowed_returncodes=(0, 1),
    )
    if result.returncode != 0:
        fail(f"candidate-not-descended-from-evidence:{EVIDENCE_COMMIT_SHA}")


def check_h0_project_universe(root: Path, *, baseline_revision: str = EVIDENCE_COMMIT_SHA) -> list[str]:
    accepted = accepted_h0_projects_at(root, baseline_revision)
    current = current_h0_projects(root)
    if not accepted:
        fail(f"accepted-h0-project-universe-empty:{baseline_revision}")
    if current != accepted:
        missing = sorted(set(accepted) - set(current))
        added = sorted(set(current) - set(accepted))
        fail(
            "h0-project-universe-drift:"
            + "missing=" + ",".join(missing)
            + ";added=" + ",".join(added)
        )
    return accepted


def unity_semantic_identity(value: str) -> bool:
    value = value.strip().replace("\\", "/")
    if not value:
        return False
    candidates = {value.lower()}
    base = value.rsplit("/", 1)[-1]
    candidates.add(base.lower())
    candidates.add(base.split(",", 1)[0].lower())
    if "." in base:
        candidates.add(base.rsplit(".", 1)[0].lower())
    for candidate in candidates:
        if candidate in {"unity", "unityengine", "unityeditor", "com.unity"}:
            return True
        if candidate.startswith(("unity.", "unityengine.", "unityeditor.", "com.unity.")):
            return True
    return False


def unity_path_boundary(value: str) -> bool:
    normalized = "/" + value.strip().replace("\\", "/").strip("/").lower() + "/"
    return "/unity/arkusunity/" in normalized


def parse_graph_for_unity(project: str, graph_path: Path) -> list[str]:
    try:
        lines = graph_path.read_text(encoding="utf-8").splitlines()
    except FileNotFoundError:
        fail(f"msbuild-effective-graph-missing:{project}")
    violations: list[str] = []
    for line in lines:
        parts = line.split("|")
        if len(parts) != 6:
            fail(f"msbuild-effective-graph-malformed:{project}:{line}")
        kind, identity, full_path, package_id, original, resolved_from = parts
        values = (identity, full_path, package_id, original, resolved_from)
        if any(unity_semantic_identity(value) for value in values) or any(
            unity_path_boundary(value) for value in (full_path, original)
        ):
            violations.append(f"{project}:{kind}:{identity}:{package_id}:{full_path}")
    return violations


def check_effective_h0_graph(root: Path, projects: list[str]) -> None:
    if shutil.which("dotnet") is None:
        fail("required-tool-missing:dotnet")
    with tempfile.TemporaryDirectory(prefix="h1-02-msbuild-probe-") as temp:
        probe = Path(temp) / "h1-02-effective-graph.targets"
        probe.write_text(MSBUILD_PROBE_XML, encoding="utf-8")
        violations: list[str] = []
        for index, project in enumerate(projects):
            project_path = root / project
            if not project_path.is_file():
                fail(f"accepted-h0-project-missing:{project}")
            graph = Path(temp) / f"graph-{index}.txt"
            run_process(
                [
                    "dotnet",
                    "msbuild",
                    str(project_path),
                    "-nologo",
                    "-v:q",
                    "-t:H102EmitEffectiveReferenceGraph",
                    "-p:BuildProjectReferences=false",
                    f"-p:CustomAfterMicrosoftCommonTargets={probe}",
                    f"-p:H102GraphFile={graph}",
                ],
                cwd=root,
            )
            violations.extend(parse_graph_for_unity(project, graph))
        if violations:
            fail("h0-effective-unity-dependency:" + "|".join(violations))


def check_h0_project_graph(root: Path) -> None:
    projects = check_h0_project_universe(root)
    check_effective_h0_graph(root, projects)


def check_asmdef_direction(root: Path) -> None:
    editor = read_json(root / PROJECT_REL / "Assets/Arkus/H1/Editor/Arkus.H1.Baseline.Editor.asmdef")
    if editor.get("references") != []:
        fail("baseline-editor-asmdef-must-have-no-arkus-product-reference")
    tests = read_json(root / PROJECT_REL / "Assets/Arkus/H1/Tests/Editor/Arkus.H1.Baseline.Tests.Editor.asmdef")
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
    missing = [str(path.relative_to(root)) for path in governed if not Path(str(path) + ".meta").is_file()]
    if missing:
        fail("visible-meta-missing:" + ",".join(missing))


def check_unity_wrapper(root: Path) -> None:
    path = root / UNITY_WRAPPER_REL
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError:
        fail(f"unity-wrapper-missing:{path}")
    executable = "\n".join(line for line in text.splitlines() if not line.lstrip().startswith("#"))
    required = (
        "Start-Process",
        "-PassThru",
        "$process.WaitForExit()",
        "$process.Refresh()",
        "$process.ExitCode",
        "if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf))",
        "UNITY_OUTPUT_MISSING",
        "UNITY_OUTPUT_EMPTY",
        "ConvertTo-UnityProcessArgument",
    )
    for token in required:
        if token not in executable:
            fail(f"unity-wrapper-wait-contract-missing:{token}")
    if "$LASTEXITCODE" in executable:
        fail("unity-wrapper-must-not-use-last-exit-code-for-unity-gui-process")
    if "-Wait" in executable:
        fail("unity-wrapper-must-not-use-start-process-tree-wait")
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
        fail("effective-test-framework-version:" + str(found.get("com.unity.test-framework")))
    names = {item.get("name") for item in assemblies if isinstance(item, dict)}
    for expected in ("Arkus.H1.Baseline.Editor", "Arkus.H1.Baseline.Tests.Editor"):
        if expected not in names:
            fail(f"effective-assembly-missing:{expected}")


def run_checks(root: Path, *, final: bool) -> None:
    check_evidence_ancestry(root)
    check_project_version(root)
    check_manifest(root, final=final)
    check_tracked_generated_outputs(root)
    check_h0_project_graph(root)
    check_asmdef_direction(root)
    check_meta_files(root)
    check_unity_wrapper(root)
    if final:
        check_effective_inventory(root)


def expect_targeted_red(
    root: Path,
    name: str,
    mutate: Callable[[Path], None],
    check: Callable[[Path], None],
) -> None:
    with tempfile.TemporaryDirectory(prefix="h1-02-negative-") as temp:
        fixture = Path(temp) / "repo"
        shutil.copytree(
            root,
            fixture,
            ignore=shutil.ignore_patterns(".git", "bin", "obj", "Library", "Temp", "artifacts"),
        )
        mutate(fixture)
        try:
            check(fixture)
        except CheckFailure:
            print(f"NEGATIVE_CONTROL_RED:{name}")
            return
        fail(f"negative-control-false-green:{name}")


def git_init(root: Path) -> None:
    run_process(["git", "init", "-q"], cwd=root)
    run_process(["git", "config", "user.email", "h1-02@example.invalid"], cwd=root)
    run_process(["git", "config", "user.name", "H1-02 self-test"], cwd=root)


def git_commit_all(root: Path, message: str) -> str:
    run_process(["git", "add", "-A"], cwd=root)
    run_process(["git", "commit", "-q", "-m", message], cwd=root)
    return git_lines(root, "rev-parse", "HEAD")[0]


def self_test_tracked_policy(root: Path) -> None:
    policy = (root / ".gitignore").read_text(encoding="utf-8")
    for relative in TRACKED_POLICY_FIXTURES:
        with tempfile.TemporaryDirectory(prefix="h1-02-tracked-policy-") as temp:
            fixture = Path(temp) / "repo"
            fixture.mkdir()
            (fixture / ".gitignore").write_text(policy, encoding="utf-8")
            git_init(fixture)
            git_commit_all(fixture, "policy")
            target = fixture / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_text("forbidden\n", encoding="utf-8")
            run_process(["git", "add", "-f", relative], cwd=fixture)
            try:
                check_tracked_generated_outputs(fixture)
            except CheckFailure:
                print(f"NEGATIVE_CONTROL_RED:tracked-policy:{relative}")
                continue
            fail(f"negative-control-false-green:tracked-policy:{relative}")


def self_test_h0_universe_cannot_shrink() -> None:
    with tempfile.TemporaryDirectory(prefix="h1-02-h0-universe-") as temp:
        fixture = Path(temp) / "repo"
        (fixture / "src/A").mkdir(parents=True)
        (fixture / "tests/B").mkdir(parents=True)
        (fixture / "src/A/A.csproj").write_text("<Project />\n", encoding="utf-8")
        (fixture / "tests/B/B.csproj").write_text("<Project />\n", encoding="utf-8")
        git_init(fixture)
        baseline = git_commit_all(fixture, "accepted universe")
        (fixture / "tests/B/B.csproj").unlink()
        run_process(["git", "add", "-u"], cwd=fixture)
        try:
            check_h0_project_universe(fixture, baseline_revision=baseline)
        except CheckFailure:
            print("NEGATIVE_CONTROL_RED:h0-universe-shrink")
            return
        fail("negative-control-false-green:h0-universe-shrink")


def self_test_effective_msbuild_import() -> None:
    if shutil.which("dotnet") is None:
        fail("required-tool-missing:dotnet")
    with tempfile.TemporaryDirectory(prefix="h1-02-msbuild-negative-") as temp:
        base = Path(temp)
        dependency = base / "dependency"
        dependency.mkdir()
        (dependency / "InjectedUnity.csproj").write_text(
            '<Project Sdk="Microsoft.NET.Sdk">\n'
            '  <PropertyGroup><TargetFramework>net8.0</TargetFramework>'
            '<AssemblyName>UnityEngine.CoreModule</AssemblyName></PropertyGroup>\n'
            '</Project>\n',
            encoding="utf-8",
        )
        (dependency / "Class1.cs").write_text("public sealed class Class1 {}\n", encoding="utf-8")
        run_process(
            ["dotnet", "build", str(dependency / "InjectedUnity.csproj"), "-nologo", "-v:q", "-c", "Release"],
            cwd=dependency,
        )
        injected_dll = dependency / "bin/Release/net8.0/UnityEngine.CoreModule.dll"
        if not injected_dll.is_file():
            fail("msbuild-negative-dependency-output-missing")

        fixture = base / "repo"
        project_dir = fixture / "src/H0"
        project_dir.mkdir(parents=True)
        (project_dir / "H0.csproj").write_text(
            '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>'
            '<TargetFramework>net8.0</TargetFramework>'
            '</PropertyGroup></Project>\n',
            encoding="utf-8",
        )
        escaped = html.escape(str(injected_dll), quote=True)
        (fixture / "Directory.Build.targets").write_text(
            '<Project><ItemGroup><Reference Include="CentralAlias">'
            f'<HintPath>{escaped}</HintPath><Private>false</Private>'
            '</Reference></ItemGroup></Project>\n',
            encoding="utf-8",
        )
        run_process(["dotnet", "restore", str(project_dir / "H0.csproj"), "-nologo", "-v:q"], cwd=fixture)
        try:
            check_effective_h0_graph(fixture, ["src/H0/H0.csproj"])
        except CheckFailure:
            print("NEGATIVE_CONTROL_RED:h0-unity-via-directory-build-targets")
            return
        fail("negative-control-false-green:h0-unity-via-directory-build-targets")


def run_self_tests(root: Path) -> None:
    expect_targeted_red(
        root,
        "editor-pin",
        lambda r: (r / PROJECT_REL / "ProjectSettings/ProjectVersion.txt").write_text(
            "m_EditorVersion: 6000.3.0f1\n", encoding="utf-8"
        ),
        check_project_version,
    )

    def loosen_package(r: Path) -> None:
        path = r / PROJECT_REL / "Packages/manifest.json"
        value = read_json(path)
        value["dependencies"]["com.unity.test-framework"] = "latest"
        path.write_text(json.dumps(value, indent=2) + "\n", encoding="utf-8")

    expect_targeted_red(root, "package-pin", loosen_package, lambda r: check_manifest(r, final=False))

    def remove_library_ignore(r: Path) -> None:
        path = r / ".gitignore"
        text = path.read_text(encoding="utf-8").replace("Unity/ArkusUnity/[Ll]ibrary/\n", "")
        path.write_text(text, encoding="utf-8")

    expect_targeted_red(root, "unity-policy-source", remove_library_ignore, check_required_ignore_policy)

    def remove_exact_process_wait(r: Path) -> None:
        path = r / UNITY_WRAPPER_REL
        text = path.read_text(encoding="utf-8")
        old = "$process.WaitForExit()"
        if old not in text:
            fail("unity-process-wait-negative-fixture-anchor-missing")
        path.write_text(text.replace(old, "$null = $process.HasExited", 1), encoding="utf-8")

    expect_targeted_red(root, "unity-exact-process-wait", remove_exact_process_wait, check_unity_wrapper)

    def disable_output_postcondition(r: Path) -> None:
        path = r / UNITY_WRAPPER_REL
        text = path.read_text(encoding="utf-8")
        old = "if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf))"
        if old not in text:
            fail("unity-output-postcondition-negative-fixture-anchor-missing")
        path.write_text(text.replace(old, "if ($false)", 1), encoding="utf-8")

    expect_targeted_red(root, "unity-output-postcondition", disable_output_postcondition, check_unity_wrapper)
    self_test_tracked_policy(root)
    self_test_h0_universe_cannot_shrink()
    self_test_effective_msbuild_import()


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", default=".")
    parser.add_argument("--mode", choices=("remote-prep", "final"), default="remote-prep")
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
