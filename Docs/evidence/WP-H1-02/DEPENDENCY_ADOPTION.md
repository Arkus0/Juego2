# WP-H1-02 — Dependency and toolchain adoption record

Status: COMPLETE; EFFECTIVE LOCAL CONFIRMATION RECORDED IN ROUND 4

## Unity Editor

- Identity: Unity Editor, Unity 6.3 LTS family
- Exact patch: `6000.3.24f1`
- Changeset: `4e7b9b5b6244`
- Release date: 2026-09-10
- Official release source: `https://unity.com/releases/editor/whats-new/6000.3.24f1`
- Linkage: external authoring/toolchain executable only; Editor binaries are never committed or vendored.
- License/terms: Unity engine/editor terms current for the installed entitlement. The H1 planning contract already selects Unity as the downstream engine; H1-02 does not transfer Unity terms into Arkus canonical semantics.
- Arkus guarantee implemented: effective Unity project import/compile/test substrate.
- Outside Unity authority: canonical game/world state, capability semantics, mutation semantics, validation identity, provenance/replay and canonical persistence remain H0-owned.
- Replacement boundary: Unity is the first Engine Bridge target; canonical Arkus contracts remain engine-neutral. A different engine requires a different bridge, not a change to H0 semantics.
- Update owner: future H1/toolchain maintenance; changing the patch invalidates this baseline and requires explicit revalidation.
- Classification: local build/authoring dependency; not an Arkus kernel/runtime package.

Selection rationale: `6000.3.24f1` is the newest official Unity 6.3 LTS release page verified during the 2026-09-23 Worker cycle. The exact changeset is pinned in `ProjectVersion.txt`. Round 4 proved the installed Editor reports product version `6000.3.24f1_4e7b9b5b6244`; the executable fingerprint and effective inventory are recorded in `LOCAL_EXECUTION_RESULT.md`.

## Unity Test Framework

- Package: `com.unity.test-framework`
- Exact direct version: `1.6.0`
- Official docs: `https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/manual/index.html`
- Purpose: deterministic EditMode execution in Unity batchmode.
- Linkage: Unity Package Manager dependency in the retained Unity project; no source is copied or vendored into Arkus.
- Exact observed license: the resolved package-root `LICENSE.md` identifies the Unity Companion License for Unity-dependent projects. `PACKAGE_LEGAL_OBSERVATION.md` records its exact SHA-256 and the resolved `package.json` fingerprint; no package source is copied into Arkus.
- Notices: retain/propagate notices as required by the resolved package; do not copy package code into Arkus.
- Arkus guarantee implemented: test execution mechanism only.
- Outside dependency authority: the package does not define Arkus acceptance semantics or H0/H1 product contracts.
- Replacement boundary: H1 proof consumes normalized pass/fail/test output; no Arkus semantic contract depends on NUnit/Test Framework API identity.
- Update owner: H1 toolchain maintenance.
- Classification: build/test-only for H1-02.

Version rationale: Unity publishes Test Framework 1.6.0 documentation for Unity 6.2+ and it is the direct test package selected for this pinned 6.3 baseline. Effective package resolution, including transitives, is not inferred from the manifest: the local Editor must generate `packages-lock.json` and an effective Package Manager inventory.

## Render pipeline baseline

- Baseline: Unity **Built-in Render Pipeline**.
- Direct render-pipeline package: none.
- Rationale: H1-02 owns project/toolchain reproducibility, not rendering semantics. Built-in keeps the foundational package graph minimal, avoids adopting an SRP before a real bridge/content requirement exists, and remains sufficient for later bridge mechanics and source-first asset inspection.
- Effective proof: Unity test/inventory requires `GraphicsSettings.currentRenderPipeline == null`.
- Change rule: adopting URP/HDRP later is an explicit downstream project/toolchain decision with its own package/provenance impact; it is not silently folded into H1-02.

## Effective lock result

`Packages/manifest.json` contains only exact direct decisions. The pinned Editor generated and committed `Packages/packages-lock.json`, so the candidate records the **effective resolved transitive graph** rather than a Worker-authored guessed lock. Round 4 observed four resolved packages and five assemblies; the generated lock and effective inventory remained equal across the clean second import for every normalized field owned by this WP.
