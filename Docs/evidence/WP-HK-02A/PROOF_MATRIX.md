# WP-HK-02A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-02A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

For the accepted finite canonical `WorldState`, an opaque extension is now globally or object scoped by the canonical identity `(owner, schemaVersion, subjectId-or-global)` and may publish typed `(kind, targetId)` dependency edges. Subject/dependency targets resolve inside the same world; both fields participate in canonical format/hash, complete HK03 extension inspection and HK04 transactional extension mutation/change coverage. The accepted HK01 public-route universe and HK04 commit-authority/transaction boundary are consumed unchanged.

Latest implementation observation before evidence reconciliation:

- candidate SHA `f3c9a8f918a4389298cbb4a9515e4cd701f990ad`;
- GitHub Actions run `35454985416`;
- Release build: 0 warnings / 0 errors;
- focused HK02A tests: 8/8 GREEN;
- full regression: 95/95 GREEN;
- canonical receipt: `Result: GREEN`.

The evidence reconciliation commit changes documentation only. Canonical observation must rerun on its exact resulting SHA before freeze.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/evaluated evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| global and object-scoped model | `WorldExtensionData` and `WorldExtensionIdentity` | constructor/property inventory is exact; old three-argument construction remains valid | global regression fixtures plus two subjects sharing owner/version | same owner/version on different subjects would fail if identity still ignored subject | PASS | no migration reader for historical V1 bytes is claimed |
| composite uniqueness | all extensions in one `WorldState` | validator uses `HashSet<WorldExtensionIdentity>`; identity equality/order includes subject | distinct subjects coexist | duplicate exact composite identity fails closed | PASS | none inside current model |
| subject/dependency referential integrity | complete candidate object/extension collections | validation builds the accepted object-ID universe once, then checks every subject and every dependency edge | valid subject and multi-edge examples | dangling subject, dangling dependency and duplicate dependency all fail; transactional removal of either target stays atomic | PASS | Arkus cannot infer undeclared references hidden inside opaque payload bytes |
| schema/version and canonical identity | V2 canonical codec | subject is carried on extension and dependency-source records; dependencies are separately sorted canonical records | exact round trip and stable hash | changing subject or dependency changes hash; unsupported schema fails closed | PASS | V1 import/migration is outside this additive in-repository candidate |
| ordering is non-semantic | object, extension and dependency caller ordering | codec sorts objects, references, composite extension identities and dependency edges | deliberately reversed objects/extensions/dependencies serialize and hash identically | the dependency-order control was corrected so reversal occurs before constructor copy | PASS | none |
| HK03 complete extension inspection | current accepted extension query/read routes | descriptors expose scope/subject/count; bounded reads expose sorted dependency pages; the inherited reconstruction oracle rebuilds all pages and compares exact canonical hash | subject filter and exact reconstruction | omission of subject or dependencies makes reconstructed hash diverge | PASS | payload and dependency page sizes are bounded, total world size is not yet budgeted |
| HK04 composite mutation addressing | existing four-operation grammar | put/remove parse optional subject; dictionary keys use canonical composite identity; final candidate validation is unchanged | two same-owner puts and one subject-specific remove | removing one subject must leave the other; dangling subject/dependency candidates never reach commit | PASS | whole-world CAS remains intentionally unchanged |
| HK04 dependency change coverage | current semantic before/after oracle | planner and independent `WorldMutationCoverage` each derive dependency field and edge deltas from effective states | plan reports resource plus dependency field/reference effects | empty declared changes against a real dependency change produce missing field/add/remove mismatches | PASS | payload remains one opaque field by contract |
| existing global call sites retain semantics | accepted HK02–HK04 fixtures | new constructor/request fields are optional and global key is explicit | full 95-test regression GREEN | accepted global extension tests would fail on address or mutation regression | PASS | additive response fields may require strict external clients to follow canonical schema evolution later |
| representative Juego2 shape fits | bounded Potes/Liébana hero slice | one approved content scenario exercises per-NPC identity, global data, dependencies, inspect and mutate boundaries | `CONTENT_SHAPE_PROBE.md` plus focused tests | deletion of subject/dependency rejects the candidate | PASS | time, behavior, assets and partition remain classified future/out-of-boundary decisions |
| forbidden scope absent | baseline-to-candidate diff | changes are confined to engine-neutral model/codec, existing inspection/mutation surfaces, tests, scripts and evidence | complete diff audit | scope search finds no Unity/ECS/gameplay schema or concurrency relaxation | PASS | none |

## Independent/effective universes

1. **Canonical property/input universe** — the accepted HK02 reflection inventory is extended for the exact public inputs/properties of `WorldExtensionData` and `WorldExtensionIdentity`; future growth forces an explicit proof update.
2. **Effective serialization universe** — exact canonical bytes/hash and round-trip equality observe the actual codec result, not a parallel field registry.
3. **Effective inspection universe** — the inherited HK03 reconstruction path queries and reads the public surface and must reproduce the exact canonical hash.
4. **Effective mutation universe** — the inherited HK04 before/after semantic oracle independently compares effective state with the returned plan; composite keys and dependency deltas extend that oracle.
5. **Object target universe** — one dictionary derived from every canonical object is used to validate every extension subject and typed dependency.

The current WP does not rebuild HK01 route discovery or HK04 commit authority. Concrete evidence of an omitted effective route or external commit path would reopen those accepted boundaries; none was found.

## Proof-budget verdict

The product change extends eight existing engine-neutral source files. Proof reuses the accepted semantic inventory, exact codec hash, HK03 reconstruction and HK04 semantic-diff oracle, plus one focused HK02A test class and thin exact-SHA entrypoints. No dependency, generalized framework or alternate registry was added. Every added control maps directly to an acceptance criterion or a defect found during Worker pre-review.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Repair cycle 1 — independent FAIL reconciliation

The original READY verdict above described the superseded frozen candidate and was falsified by the independent Reviewer on `9d0dc3739f31bcc6a7f8df12b5f6e876837efacc`. It is historical, not sufficient for freeze. This section replaces its two affected proof rows for the repaired candidate:

- **Composite HK04 mutation identity/idempotency:** global and valid `subjectId: "-"` have distinct fingerprints for put and remove. Public same-key calls must conflict without a second commit; independent keys address both resources. No valid canonical object ID has been forbidden.
- **Bounded and complete HK03 extension dependency inspection:** the public read returns 100 and then 1 of 101 distinct typed edges, the reconstruction helper follows `nextDependencyOffset`, and the exact canonical hash is recovered. A first-page-only reconstruction has a different hash.

Implementation SHA `2cb7a3daaa565a6b0ca5b68882541191750dac60`, Actions `35456331653`: Release 0 warnings/0 errors, focused 11/11, regression 98/98, canonical GREEN. The documentation-only reconciliation needs its own exact-SHA observation. All other existing proof rows and trust boundaries remain unchanged. The two new focused defect classes are material, in-claim HK02A corrections, not generalized HK04 re-proof or new framework machinery.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
