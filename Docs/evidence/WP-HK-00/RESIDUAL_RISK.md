# WP-HK-00 residual-risk audit

## Central claim

HK00 establishes the smallest portable Arkus code/build boundary on which later harness work can safely grow. It does **not** claim game semantics, protocol behavior, Unity projection, transaction semantics or later-HK functionality.

## Closed causal risks

| Risk | Disposition |
|---|---|
| Self-shrinking repository/project/source universe | Closed by exact Git universe + independent fixed classification + arbitrary tracked MSBuild `<Project>` XML classification + exact solution membership. |
| Non-`.csproj`/alternate build participant disappears before classification | Closed at participant level, not by extension list: any tracked XML root `<Project>` outside fixed projects/policies is rejected; any actual extra solution member is rejected. |
| Proof tool changes its own compile universe via `.csproj`/targets | Closed: authoritative proof is direct `csc` bootstrap from all Git-tracked proof `.cs` sources under pinned SDK/ref/runtime, with pre/post source hashes. |
| Response files / solution-specific or auto-imported build behavior escapes observation | Closed: repo `.rsp` and alternate solution entrypoints are forbidden, canonical invocations use `-noAutoResponse`, and evaluated `MSBuildAllProjects` is classified. |
| Source generator/analyzer injects code after static evaluation | Closed by effective compiler command-line observation and a single external-authority model. Product/proof/test compiler extensions may come only from selected SDK/packs; locked packages are not blanket analyzer authority. |
| Compiler receives a semantic input via a channel omitted from source/ref checks | Closed by fail-closed checks for known input-bearing switches plus complete normalized `csc` argument inventory for all nine projects; any new channel is observable as evidence drift even before policy support is added. |
| Arbitrary assembly enters through `/reference` | Closed: effective references must resolve to framework packs, exact evaluated outputs of declared Arkus dependencies, or for Tests an exact id/version path present in the committed lock. |
| Test/package build logic becomes mutable external authority | Closed to the declared boundary: test dependency graph is locked with exact versions/contentHash, freshly restored in isolated cache, byte-compared to committed lock, then restored locked. External MSBuild imports are accepted only through the same authority model. |
| Build mutates tracked candidate source and PDB then agrees with modified bytes | Closed: candidate tracked bytes are captured before effective work and compared after; Git index/worktree divergence is independently checked. |
| Build substitutes produced DLL while retaining legitimate PDB | Closed: PE CodeView identity must equal portable-PDB identity and output is rechecked after tests. |
| Required dependency is decorative only | Closed: required product edges must also appear in emitted assembly references. |
| Multimodule/embedded managed-resource output bypasses compiler-source model | Closed: assembly secondary files/netmodules and managed manifest resources are rejected. |
| Proof/evidence generation mutates candidate and then proves the mutation | Closed: observations live only under ignored `artifacts`; committed evidence is compared externally; attack execution occurs in disposable Git copy. |
| NUL-delimited Git inventory loses terminal entry | Closed by byte-preserving subprocess capture and terminal-entry negative control. |
| Candidate was based on stale process protocol | Closed in current lineage: `main` process commit `dd4d2bd...` is an actual parent of the candidate. |

## Explicit trust boundaries / remaining residual risk

| Boundary | Residual risk and treatment |
|---|---|
| Git | Trusted as exact candidate-tree authority. Index/worktree divergence, tracked symlinks/gitlinks and missing tracked C# are rejected. |
| .NET SDK/runtime/reference packs | External implementation dependency pinned to SDK `8.0.425` and runtime/ref pack `8.0.31`; proof obligations remain Arkus-owned. Supply-chain identity is version/upstream trust, not a locally vendored SDK hash. |
| NuGet packages | Test-only. Exact transitive graph/content hashes are locked and restored into a fresh isolated cache. No product project consumes NuGet packages. |
| GitHub Actions runner/service | External infrastructure. Runner family and Action commits are pinned where repository-controlled. **Current blocker:** hosted jobs are failing before allocation (`runner_id=0`, `steps=[]`), so exact-SHA execution is unavailable and freeze is forbidden. |
| Candidate-owned HK00 CI workflow | Necessary execution recipe, not independent authority. A malicious/incorrect candidate could edit its own workflow; main-owned handoff checks only freeze metadata. Fresh independent Reviewer must reconstruct the exact SHA and challenge/run the proof rather than trust Worker CI prose. |
| Unknown defect classes | Worker self-review cannot establish their absence absolutely. Independent Reviewer remains mandatory specifically to search outside Worker-highlighted risks. |
| Native PE resource directory | Measured but not required to be zero: ordinary .NET assemblies may contain native PE resources. Semantic compiler resource channels are separately closed; inventing a zero-resource rule would be overfitting. |

## Current conclusion

The six-layer Worker design audit currently has no known residual risk capable of falsifying the HK00 central boundary claim **assuming the declared external toolchain components execute as pinned**. However, the foundational standard also requires executed exact-SHA evidence. Because GitHub-hosted Actions is currently not assigning a runner, the candidate remains NOT_READY and must not freeze.

`UNRESOLVED_PROOF_OBLIGATIONS: 1`  
`KNOWN_UNDETECTED_DEFECT_CLASSES: 0`

The one unresolved obligation is: **final exact-SHA positive proof + 37 causal negative controls + zero evidence drift must execute successfully on the eventual frozen lineage.**
