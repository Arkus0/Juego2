# WP-HK-00 foundational proof matrix

Candidate family: `wp-hk-00-v4-sol-exclusive`
Binding contract: `Docs/workpacks/HK/WP-HK-00.md`
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.2
Trust boundary: `Docs/evidence/WP-HK-00/TRUST_BOUNDARY_AUDIT.md`

This matrix states the Worker completeness argument inside the declared finite trust boundary. Observation run #104 supplied the evidence reconciled into this candidate. Freeze still requires the canonical exact-SHA verifier to be GREEN on the final evidence-bearing HEAD; the PR/receipt is authoritative for that execution state.

| Proof obligation | Completeness argument | Positive evidence | Causal control(s) | Worker result |
|---|---|---|---|---|
| Canonical product source has single ownership | Git defines the tracked candidate universe independently of generated classification; every production `.cs` has exactly one fixed owner and is cross-checked against effective compilation/PDB observations. | `inventory/sources.json`; compiler/PDB checks | `unowned-source-anywhere`, `cross-project-source-compile`, dropped/generated/external-source controls | OBSERVED GREEN |
| Portable Unity-compatible baseline | Fixed contract plus evaluated/effective properties require `netstandard2.1`/C#9 for portable modules and pinned net8 for host/tests/proof. | `inventory/projects.json`; compiler/MSBuild facts | `fixed-policy-relaxed-in-build` | OBSERVED GREEN |
| No Unity/UnityEditor/DFU/game-content coupling | Complete product project/source/package/reference boundary is inspected and effective references are bounded. | source/project/reference inventories and checks | `engine-raw-reference`, `production-package-injected` | OBSERVED GREEN |
| Dependency graph directional and acyclic | Evaluated `ProjectReference` graph must equal the fixed graph; cycles and missing effective assembly edges are checked. | `inventory/graph.json`; assembly checks | `dependency-cycle-backedge`, `required-edge-made-decorative` | OBSERVED GREEN |
| Canonical build includes every required participant | Git/fixed project universe is independent of solution membership; the canonical solution must contain exactly the fixed projects. | project inventory; solution oracle | extra/missing/solution-omit/non-C# participant controls | OBSERVED GREEN |
| SDK/toolchain pinned | `global.json`, runtime/ref-pack checks and exact-SHA preflight bind the canonical toolchain. | `global.json`; proof/CI preflight | `toolchain-pin-relaxed` | OBSERVED GREEN |
| Test dependency closure reproducible | Direct roots are centrally pinned; the complete transitive lock with content hashes is freshly observed then canonical restore uses locked mode. | `Directory.Packages.props`; `packages.lock.json`; `scripts/proof.sh` | `dependency-lock-drift` | OBSERVED GREEN |
| Clean headless Linux restore/build/test | Canonical solution is restored, rebuilt and tested with pinned toolchain, locked dependencies and automatic response files disabled. | `scripts/proof.sh`; run #104 | pristine GREEN; `real-warning-is-error` | OBSERVED GREEN |
| Required project/test/proof material fails closed | Fixed required-file checks plus independent universe prevent material participants disappearing silently. | repository checks | `proof-project-deleted`, `missing-fixed-project` | OBSERVED GREEN |
| Inventories cannot self-shrink | Git candidate tree defines existence; fixed classification is not sourced from the inventory under test. | projects/sources inventories | `legacy-manifest-cannot-shrink-universe`, terminal NUL control | OBSERVED GREEN |
| Owned source is what compiler consumes | Effective compiler sources, normalized arguments, PDB documents/checksums and tracked-byte stability bind candidate input to build output. | `inventory/compiler-args.json`; source/PDB/output checks | dropped/generated/external/mutated-source controls | OBSERVED GREEN |
| Repository-controlled alternate channels do not silently bypass proof | Closed build XML, solution/response surface, preprocessed import closure, effective analyzers/refs and output checks cover realistic repository-owned alternate channels. | build/import/compiler/output checks | closure, external-authority, reference-authority and output controls | OBSERVED GREEN |
| Candidate/evidence bind to one exact SHA | Positive proof is read-only; observations live under ignored `artifacts/`; final verifier diffs committed inventory and 37-control summary against fresh observation before freeze. | `scripts/hk00-verify-exact-sha.sh`; PR exact-SHA receipt | evidence-drift/fail-closed verifier | FREEZE REQUIRES FINAL EXACT-SHA GREEN |

## Proof-budget conclusion

The proof machinery is now bounded to concrete HK00 acceptance and previously observed false-green classes. No known in-claim proof obligation remains unresolved and no known material defect class remains undetected.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`
