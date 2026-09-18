# WP-HK-00 residual-risk audit

## Central claim

HK00 establishes a complete-enough, fixed portable code/build boundary for the Arkus harness kernel. It does **not** claim game semantics, protocol behavior, Unity projection, authoring transactions or later-HK functionality.

## Risks considered and disposition

| Risk | Disposition |
|---|---|
| Self-shrinking repository/project/source universe | Closed: exact Git tree is the independent universe; fixed contract is separate from observation; manifest/scan-root attempts cannot remove obligations. |
| A fixed project exists but disappears from canonical build/test entrypoint | Closed: solution membership is independently enumerated and must exactly match the fixed project set. |
| MSBuild syntax bypasses static XML checks | Closed at the causal boundary: evaluated properties/items/import closure and effective compiler command line are observed. Static XML checks are defense-in-depth. |
| Custom auto-imported `.targets`/`.props` changes build behavior | Closed for repository-owned extensions: effective `MSBuildAllProjects` rejects imports outside fixed project/policy surface while allowing untracked SDK-generated `obj` intermediates. |
| Source generator/analyzer injects code after static evaluation | Closed for product projects: effective `/analyzer` inputs may originate only from exact selected SDK or `.NET packs` beneath the same `DOTNET_ROOT`; repo/NuGet/external analyzers are rejected. Generated/untracked product C# inputs are rejected. |
| Build mutates tracked source before compile so PDB checksum agrees with altered bytes | Closed: tracked bytes are hashed before effective execution and compared afterward; index/worktree divergence is checked. |
| Required dependency is declared but not exercised | Closed: required product edges must appear in emitted assembly references; boundary markers are non-const to leave causal IL references. |
| NUL-delimited inventory parser drops terminal entry | Closed: subprocess stdout preserves delimiters and a lexicographically terminal project attack must make the project-universe oracle red. |
| Engine/DFU dependency hidden behind package/raw/compiler/assembly reference | Closed for the HK00 engine/DFU identity boundary through static/effective/output oracles and empty production package allowance. |
| Semantic game-content code added inside an otherwise owned source | Not suitable for a sound name/filename denylist. Current complete product-source inventory contains only HK00 boundary markers and remains small enough for independent full-diff review. Future semantic growth is governed by later WPs. |
| Test framework versions become stale | Test-only dependencies are centrally pinned and isolated to `Arkus.Harness.Tests`; they do not define product semantics and are replaceable at the test boundary. `DEPENDENCIES.md` records this explicitly. |
| Hosted runner image or NuGet availability changes externally | External infrastructure risk. Runner family, SDK, package versions and Action code SHAs are pinned as far as repository control permits. Exact-SHA CI is mandatory immediately before freeze. |
| Unknown defect class exists | Worker cannot prove absence of unknown classes by self-review. Fresh independent Reviewer is therefore mandatory and must reconstruct scope and challenge at least one omission class not highlighted by Worker. No **known** material defect class remains undetected at handoff. |

## Worker residual-risk conclusion

No known residual risk can falsify the central HK00 boundary/completeness claim while all required oracles and exact-SHA CI remain green.

`UNRESOLVED_PROOF_OBLIGATIONS: 0`  
`KNOWN_UNDETECTED_DEFECT_CLASSES: 0`
