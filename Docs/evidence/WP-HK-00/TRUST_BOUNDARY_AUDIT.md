# WP-HK-00 trust-boundary and guard audit

Audit date: 2026-09-18  
Binding contract: `Docs/workpacks/HK/WP-HK-00.md`  
Binding proof standard: `FOUNDATIONAL_PROOF_STANDARD.md` v1.2  
Binding handoff protocol: `WORKER_REVIEW_PROTOCOL.md` v1.3

## Central claim

HK00 proves a small, reproducible Arkus code/build boundary: the canonical modules exist, product C# source has single ownership, required dependency direction is enforced and acyclic, the portable kernel stays free of Unity/DFU/game-content coupling, toolchain/dependency closure is pinned, owned source reaches effective compilation, and clean Linux restore/build/test is reproducible.

HK00 does **not** attempt to certify the implementation integrity of the toolchain or CI infrastructure.

## Explicit trusted base

The following are trusted inputs to HK00 rather than objects HK00 attempts to prove secure against hostile implementation:

- Git object/checkout/index semantics for the candidate;
- exact pinned .NET SDK/runtime and normal documented MSBuild/C# compiler behaviour on the canonical path;
- normal documented NuGet restore/lock/content-hash behaviour;
- OS/filesystem/GitHub-hosted CI infrastructure and standard cryptographic primitives.

HK00 verifies repository configuration, versions, candidate SHA, dependency closure and effective observations at those boundaries. It does not claim that a malicious or compromised trusted-base implementation could not falsify its documented contract.

## Guard classification

| Guard / mechanism | Classification | Why |
|---|---|---|
| Fixed canonical project contract + Git project inventory | REQUIRED_FOR_HK00 | Establishes existence/classification of canonical boundary independently of mutable candidate inventory. |
| Git-wide tracked product `.cs` inventory + unique owner mapping | REQUIRED_FOR_HK00 | Directly proves single ownership and zero unclassified production C# source. |
| Evaluated `ProjectReference` graph + cycle check | REQUIRED_FOR_HK00 | Directly proves required dependency direction and acyclicity. |
| TFM/C# version/warnings/toolchain checks | REQUIRED_FOR_HK00 | Direct acceptance criteria. |
| Product package/raw-reference and Unity/DFU checks | REQUIRED_FOR_HK00 | Directly protects portable-kernel dependency boundary. |
| Effective compiler source observation + PDB/source checksum checks | REQUIRED_FOR_HK00 | Directly proves owned source is what actually compiles; static XML is not sole oracle. |
| Required project/test/proof file checks | REQUIRED_FOR_HK00 | Supports fail-closed behavior if required material disappears. |
| Locked test dependency closure with exact resolved versions/content hashes | REQUIRED_FOR_HK00 | Makes restore/build/test reproducible and prevents undeclared dependency drift. |
| Canonical Linux restore/build/test on exact SHA | REQUIRED_FOR_HK00 | Direct DoD/acceptance requirement. |
| Exact solution membership check | REQUIRED_FOR_HK00 | Ensures canonical build entrypoint contains fixed project set. |
| Read-only candidate / observations under ignored `artifacts/` | DEFENCE_IN_DEPTH | Prevents proof from mutating candidate while observing it. |
| Direct-csc bootstrap of proof oracle | DEFENCE_IN_DEPTH | Reduces self-confirmation by proof project without claiming compiler integrity. |
| Closed repository MSBuild XML shape | DEFENCE_IN_DEPTH | Prevents realistic repo-owned alternate build channels from bypassing evaluated checks. |
| Response-file / alternate-solution guards | DEFENCE_IN_DEPTH | Keeps canonical path singular/reviewable; not a claim about every possible MSBuild path. |
| Evaluated MSBuild import closure | DEFENCE_IN_DEPTH | Detects repo/external build logic on canonical path while trusting normal MSBuild behaviour. |
| Compiler analyzer/input/reference authority checks | DEFENCE_IN_DEPTH | Makes material effective inputs observable/bounded; does not certify compiler internals. |
| Full normalized `compiler-args.json` inventory | DEFENCE_IN_DEPTH | Makes new effective compiler switches/paths visible to evidence drift/review. |
| Tracked-byte stability during build | DEFENCE_IN_DEPTH | Detects repo build logic mutating candidate files before compilation. |
| PE↔PDB identity, netmodule/resource/output checks | DEFENCE_IN_DEPTH | Ties observed outputs to effective compile path and blocks realistic output substitution. |
| Correctness/security of Git implementation | OUTSIDE_TRUST_BOUNDARY | Git is declared candidate-tree authority. |
| Correctness/security of pinned .NET SDK/MSBuild/C# compiler | OUTSIDE_TRUST_BOUNDARY | HK00 pins/observes toolchain; it does not prove toolchain against malicious behavior. |
| Correctness/security of NuGet implementation/upstream service | OUTSIDE_TRUST_BOUNDARY | Arkus locks resolved graph; normal NuGet semantics are trusted. |
| Correctness/security of runner OS/hypervisor/GitHub Actions service | OUTSIDE_TRUST_BOUNDARY | Infrastructure trust; exact-SHA evidence/review do not certify infrastructure. |

## Self-attack classification

### REQUIRED_FOR_HK00

- `extra-project-anywhere`
- `missing-fixed-project`
- `unowned-source-anywhere`
- `fixed-policy-relaxed-in-build`
- `dependency-cycle-backedge`
- `cross-project-source-compile`
- `engine-raw-reference`
- `source-dropped-at-build-time`
- `generated-product-source-injected`
- `external-source-injected-at-build-time`
- `tracked-source-mutated-during-build`
- `real-warning-is-error`
- `toolchain-pin-relaxed`
- `proof-project-deleted`
- `legacy-manifest-cannot-shrink-universe`
- `solution-omits-fixed-project`
- `dependency-lock-drift`

### DEFENCE_IN_DEPTH

- `required-edge-made-decorative`
- `production-package-injected`
- `custom-analyzer-source-generator`
- `late-target-analyzer-source-generator`
- `explicit-custom-build-import`
- `auto-directory-build-target`
- `solution-noncsharp-participant`
- `unclassified-msbuild-project`
- `proof-project-inline-target`
- `repository-response-file`
- `alternate-solution-entrypoint`
- `unclassified-compiler-input-item`
- `late-compiler-input-channel`
- `proof-effective-generated-source`
- `bootstrap-recursive-proof-source`
- `postcompile-output-substitution`
- `test-analyzer-outside-trusted-roots`
- `external-build-import`
- `rogue-compiler-reference`
- `terminal-nul-inventory-entry`

### OUTSIDE_TRUST_BOUNDARY

No existing RED→GREEN attack is discarded into this bucket: current attacks mutate repository-controlled configuration/candidate inputs and remain useful regression coverage. The non-claims are attacks against trusted implementations themselves (malicious Git executable, compromised SDK/compiler, hostile NuGet implementation, runner/hypervisor). HK00 will not add self-attacks for those classes.

## Proof budget and closure rule

The proof grew while repairing a genuine self-shrinking-universe defect. That expansion has now converged on the material acceptance claim. Existing defence-in-depth remains because it is implemented and useful, but it does not expand HK00's product claim. No new hardening family is justified solely by another theoretical trusted-base subversion.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`
