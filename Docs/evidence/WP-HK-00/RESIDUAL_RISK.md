# WP-HK-00 residual-risk audit

## Central claim

HK00 establishes a complete-enough, reproducible portable code/build boundary for the Arkus harness kernel. It does **not** claim game semantics, protocol behavior, Unity projection, authoring transactions or later-HK functionality, and it does not certify the security/correctness of the trusted toolchain implementation itself.

## Explicit trusted base

HK00 trusts Git as candidate-tree authority; the exact pinned .NET SDK/runtime and normal documented MSBuild/C# compiler behaviour; normal documented NuGet restore/lock/content-hash behaviour; and OS/GitHub-hosted CI infrastructure. Arkus verifies its own configuration, pins and effective observations at those boundaries rather than recursively proving the implementations.

## Risks inside the HK00 claim

| Risk | Disposition |
|---|---|
| Self-shrinking project/source universe | Closed: exact Git candidate tree is the independent universe; fixed classification is separate from generated inventory/manifest data. |
| Fixed project omitted from canonical build | Closed: solution membership must exactly equal the fixed project set. |
| Product source unowned, cross-owned or excluded from actual compilation | Closed: Git ownership + evaluated/effective compiler inputs + PDB/source checks. |
| Generated/external source silently becomes product implementation | Closed on canonical build path: effective compiler sources must satisfy the ownership model. |
| Wrong dependency direction or cycle | Closed: evaluated graph is compared to fixed edges and cycle checked. |
| Unity/UnityEditor/DFU dependency enters portable kernel | Closed for HK00 dependency boundary through package/raw/effective/output checks and complete small source inventory. |
| Toolchain or warning policy drifts | Closed by exact SDK pin, fixed/evaluated properties, effective compiler switches and CI preflight. |
| Test dependency graph changes underneath candidate | Closed: complete NuGet lock with resolved versions/content hashes; fresh isolated resolution comparison; canonical locked restore. |
| Required project/test/proof material disappears | Closed by required-file/project/solution checks. |
| Evidence generation mutates candidate | Closed by read-only orchestration; observations go only under ignored `artifacts/`. |
| Repo-controlled realistic alternate build channel bypasses static checks | Defence in depth: response/solution/build XML/import/compiler-extension/reference/output checks cover material canonical-path variants without expanding central claim. |
| Semantic game-content code added inside otherwise owned source | Current product source set is tiny boundary-only code and remains full-diff reviewable; later WPs govern semantic growth. |

## Risks outside the trust boundary

| Risk | Disposition |
|---|---|
| Git implementation lies about candidate bytes | Trusted base; not claimed/proved by HK00. |
| Pinned SDK/MSBuild/C# compiler maliciously violates documented behaviour | Trusted base; identity/configuration/effective observations are checked, implementation integrity is not. |
| NuGet implementation/service maliciously violates lock/restore semantics | Trusted base; Arkus commits resolved graph/content hashes and uses locked mode, but does not prove NuGet internals. |
| Runner OS/hypervisor/GitHub Actions service is compromised | Infrastructure trust; exact-SHA CI and independent review are controls, not infrastructure certification. |
| Upstream package/tool vendor is compromised while preserving expected identity/lock semantics | Supply-chain residual outside HK00; dependency/commercial policy continues in later architecture/gate work. |

## Proof-budget disposition

The repair history legitimately grew proof machinery to close the self-shrinking-universe defect class. Further growth now has diminishing value relative to HK00's actual code/build-boundary claim. Existing useful defence-in-depth remains, but new hostile-toolchain attack families are outside the budget unless a concrete in-claim acceptance gap is shown.

Final exact-SHA CI, exact observed-evidence reconciliation and mandatory Worker pre-review remain pending at this ACTIVE stage.

`UNRESOLVED_PROOF_OBLIGATIONS: 1`  
`KNOWN_UNDETECTED_DEFECT_CLASSES: 0`  
`TRUST_BOUNDARY: Docs/evidence/WP-HK-00/TRUST_BOUNDARY_AUDIT.md`  
`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`

The single unresolved obligation is procedural/evidentiary: complete exact-SHA validation + evidence reconciliation + Worker pre-review before freeze.
