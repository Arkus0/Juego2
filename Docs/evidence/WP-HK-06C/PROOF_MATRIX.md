# WP-HK-06C foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-06C/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted H0 authored-world model, HK06C can consume an accepted HK06A mutation journal from its exact accepted authored base, replay every entry through the accepted HK04/HK05 canonical mutation authority, and reconstruct the same final canonical authored state/hash. Replay has an explicit machine-readable compatibility policy, rejects malformed/incomplete/reordered/incompatible evidence, withholds publication on late failure or result divergence, regenerates truthful local HK06A mutation history, and proves final equivalence through HK06B semantic diff. Replay is a distinct `CanonicalReplay` orchestration, not a second mutation engine and not an HK06B snapshot rebase.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| accepted journal replay reconstructs identical final canonical state/hash | accepted HK06A journal@1 from exact authored base | replay executes every source normalized request in contiguous sequence and compares each effective persisted anchor plus final journal current anchor | two-entry replay from imported clean base reaches identical final revision/hash | integrity-valid false later result hash reaches staged execution then returns `world.replay.result_divergence` with target unchanged | PASS | no arbitrary-long-history performance claim |
| final semantic authored state is identical | HK06B authored-resource semantics | canonical hash is checked and independently accepted HK06B semantic diff compares the two final snapshots | source-final vs replay-final diff is empty / `sameAuthorableState=true` | any effective authored-resource divergence produces a non-empty HK06B diff or different final hash | PASS | HK06B semantics consumed, not re-proved |
| replay consumes HK06A evidence rather than defining a second journal | accepted `arkus.authoring.journal@1` + `journal-entry@1` | parser accepts the predecessor public shape; staged canonical mutations regenerate the normal HK06A journal; audit compares source/replayed entry IDs | source and replayed local `entryId` values match exactly one-for-one | altered request without matching identity is rejected; audit divergence rejects before outer publish | PASS | journal authenticity/signatures outside scope |
| replay executes through canonical HK04/HK05 authority | effective mutation path inside staged session | production calls `CanonicalWorldMutationAuthority.Bind(staged).Apply`; causal test supplies integrity-valid but semantically invalid later request that only HK04/HK05 validation can reject | ordinary accepted replay steps persist and produce HK06A entries | forged sequence-2 remove-root request has independently recomputed fingerprint/entryId, passes replay integrity, then fails as `world.replay.step_failed`; target remains unchanged | PASS | accepted predecessor mutation correctness consumed |
| partial/failing replay cannot publish state/history | one H0 portable-session aggregate | all steps execute in a fresh transactional session; outer `_inner` swap occurs only after complete step/result/audit/final-anchor checks | successful sequence publishes state + receipts + journal once after full audit | late canonical-validation failure and late result divergence both occur after earlier staged work yet preserve target revision/hash and journal count 0 | PASS | process-crash semantics trusted/out of scope |
| replay order/gaps are explicit and deterministic | supplied journal sequence | parser requires exact `1..N` sequence, chained base/result anchors, `entryCount`, and final current anchor | accepted two-entry sequence replays in order | swapped entries and missing final entry return `world.replay.sequence_invalid` before publication | PASS | none inside accepted v1 sequence model |
| altered entry data cannot silently claim the original transition | accepted entry identity framing | HK06C independently invokes accepted HK06A request/entry integrity binding before execution | accepted journal entries parse and later regenerate same entry IDs | changed normalized operation without reconciled identity returns `world.replay.entry_invalid` | PASS | fully regenerated alternate valid evidence is different evidence, not authenticated original history |
| wrong base/precondition cannot replay onto another state | current target anchor + journal base | public CAS checks expected revision/hash; replay also requires full target anchor == journal base | clean imported snapshot base succeeds | false expected hash returns `world.replay.concurrent_update`; mismatching base returns `world.replay.base_mismatch` | PASS | cross-process merge not claimed |
| existing local history is not silently discarded/spliced | replay target local lineage | target journal must be empty before staging, so replay cannot replace unrelated history while current anchor happens to match | snapshot import establishes a clean journal-0 replay base | matching evolved state with one retained local entry returns `world.replay.target_history_not_empty` and retains that history | PASS | history merge is future scope |
| compatibility behavior is explicit and headless | HK06A journal@1/entry@1 and HK06B snapshot@1 | dedicated read-only capability reports supplied and supported version identifiers + disposition; replay parser owns stable unsupported-version diagnostics | supported tuple returns `supported`; discovery publishes request/success/error schemas | journal@2 and snapshot version 2 report `unsupported`; direct journal@2 replay returns `world.replay.unsupported_version` | PASS | no migration capability exists yet |
| public contract truthfully classifies replay authority | HK01 public canonical meta-model | distinct `CanonicalReplay` side-effect + transaction requirement and dedicated `ICanonicalReplayHandler`; replay marker must not alias ordinary mutation/rebase handlers | production `ReplaySurfaceConformance` is empty; discovery exposes replay metadata | removing dedicated replay route makes conformance RED; inherited HK01 exact route universe caught the two new routes until reconciled | PASS | future canonical writer classes need explicit classification |
| compatibility route is truly read-only | public read-only capability | inherited effective non-writer oracle executes every non-writer route and checks current revision/hash | compatibility request succeeds without changing state | omission from the non-writer vector initially turned regression RED; final oracle includes it | PASS | none |
| replay success audit is machine-readable | public replay result schema | result binds source base/current, target previous/current, replay count and per-entry source/replayed IDs/results | successful replay result validates against `arkus.authoring.replay-result@1`; entry IDs/results align | any mismatching regenerated local journal entry causes `world.replay.audit_divergence` before publication | PASS | durable audit storage outside scope |
| snapshot rebase history is not confused with replayed mutation history | accepted HK06B lineage semantics | replay base is established by HK06B import, whose journal remains empty; replay then writes only subsequent ordinary HK06A entries | clean import has journal 0; successful 2-entry replay has the normal 2 local mutation entries | non-empty target lineage is rejected rather than blended | PASS | cross-lineage history merge not claimed |
| representative Juego2 content shape fits replay claim | bounded accepted fictional Potes/Liébana authored slice | plaza/building/NPC/reference/extension slice exercises object + extension authored mutations and clean snapshot base without adding gameplay concepts | Potes probe replays shop + social-extension changes to identical hash/revision and empty diff | inherited hash/diff/result controls would turn RED on mismatch | PASS | schedules/transforms/runtime state intentionally unclaimed |
| predecessor guarantees remain consumed | HK04/HK05/HK06A/HK06B seams | no alternate mutation/state/journal model is introduced; ordinary mutation tests and snapshot semantics remain unchanged except composition of replay surface | full regression 136/136 GREEN at implementation/test SHA | inherited HK01/HK04/HK05/HK06A/HK06B tests fail on contract/authority/provenance/portability drift | PASS | concrete contradictory evidence would reopen; none observed |
| forbidden scope remains absent | baseline-to-candidate diff | changed files are replay contract/engine/bindings, canonical metadata support, focused integration tests/scripts/evidence, plus one-line partial-class composition change | Worker diff audit shows no gameplay simulation, Unity, network, cloud, GUI or Git-history replay code | scope audit would block pre-review if such surfaces appeared | PASS | none |

## Independent/effective universes

1. **Journal evidence universe:** the supplied public HK06A dictionary artifact is walked independently by HK06C for top-level version, exact field sets, sequence, anchors, request binding and entry identity; missing/reordered elements cannot disappear from the proof by shrinking a production registry.
2. **Effective mutation authority:** late-step defect injections are deliberately made HK06A-integrity-valid so parsing succeeds; only actual canonical staged execution can expose semantic validation failure/result divergence.
3. **Final state oracle:** canonical final hash/revision plus the accepted HK06B semantic-diff capability compare independently reconstructed snapshots.
4. **Public route universe:** inherited HK01 assembly-based route enumeration independently discovered the two new routes; exact counts are 18 production routes and 21 with complete scoped fixtures.
5. **Writer classification universe:** ordinary HK04 `CanonicalMutation`, HK06B `CanonicalRebase`, and HK06C `CanonicalReplay` are distinct semantic classes with distinct handler markers. HK04 mutation conformance is not weakened.
6. **Effective read-only universe:** inherited HK04 non-writer behavior test executes `authoring.replay.compatibility` and checks unchanged canonical revision/hash.
7. **Representative product shape:** bounded accepted Potes/Liébana authored object/reference/extension slice exercises replay without silently introducing gameplay/runtime schemas.

## Implementation observation

Implementation/test SHA `0d7e22be092d8291d6f7e239e73ed7e91217bd3a` passed `Arkus Candidate Validation` run `35488744134` on Ubuntu 24.04 with pinned .NET SDK 8.0.425:

- Release build: 0 warnings / 0 errors;
- focused `Hk06C*`: 8/8 GREEN;
- full regression: 136/136 GREEN;
- exact-SHA clean-before/clean-after observation receipt: GREEN;
- artifact `10598333769` contains the observation log/receipt.

Earlier observations were allowed to fail closed while integration was incomplete: the first replay binding exposed HK04 authority ownership and independent projection-token integration seams; later regression exposed HK01 exact route-universe counts and HK04 non-writer classification. Those were reconciled without weakening the predecessor guards. The final pre-evidence implementation observation above is GREEN.

The subsequent evidence-reconciliation commits are documentation-only. Their resulting exact branch SHA must pass `scripts/hk06c-verify-exact-sha.sh` unchanged before handoff.

## Proof-budget verdict

The product additions are one replay compatibility/parser/orchestration path, one dedicated public writer classification/binding seam, and the minimal canonical metadata support needed to describe it. The additional proof machinery maps directly to explicit HK06C negative-conformance classes, two real integration seams exposed by inherited defenses, and the required content-shape probe. No general-purpose new proof framework or predecessor re-proof was introduced.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.