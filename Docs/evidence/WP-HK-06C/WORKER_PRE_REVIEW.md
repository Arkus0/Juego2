# WP-HK-06C Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 5
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-06C/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-06C`
- Baseline: `c6534fa42f6bdd18a5bff4c3fe23f3868af993e8`
- PR: `#40`
- Branch: `wp/hk-06c-deterministic-replay`
- Direct dependency: accepted `WP-HK-06B`; transitive replay-evidence dependency: accepted `WP-HK-06A`.
- Implementation/test SHA observed GREEN: `0d7e22be092d8291d6f7e239e73ed7e91217bd3a`.
- Canonical observation: Actions `35488744134`, artifact `10598333769`.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Architecture challenged

The complete baseline-to-implementation change was re-read against `WP-HK-06C`, the Foundational Proof Standard, accepted HK04/HK05/HK06A/HK06B semantics, and the public HK01 contract/discovery machinery.

The candidate was challenged specifically for:

- whether replay accidentally becomes a second mutation engine rather than orchestration over the accepted canonical mutation authority;
- whether replay can publish the first N staged mutations before a later step fails or diverges;
- whether a syntactically/integrity-valid journal can bypass HK05 semantic validation;
- whether source journal order, gaps, base/current anchors and request/entry identity are validated independently before publication;
- whether successful replay regenerates truthful HK06A local mutation provenance rather than copying or fabricating history;
- whether a clean HK06B snapshot rebase remains journal-empty until replayed ordinary mutations are actually persisted;
- whether replay can silently splice into an existing local mutation lineage;
- whether compatibility/version behavior is explicit and machine-readable instead of being delegated accidentally to generic dispatcher schema rejection;
- whether the new writer classification is truthful and distinct from both `CanonicalMutation` and `CanonicalRebase`;
- whether inherited HK01 route-universe/projection and HK04 effective non-writer defenses still cover the enlarged public surface;
- whether final equivalence is demonstrated both by canonical hash/revision and accepted HK06B semantic diff;
- whether the required Potes/Liébana content-shape probe exposes any mismatch between abstract replay fixtures and intended authored content;
- whether scope leaks into gameplay simulation, Unity, transport, cloud persistence, history merge or other forbidden work.

## Findings fixed during Worker pre-review

**Finding 1 — initial runtime binding shape did not respect the accepted public-handler/authority seam.**

The first replay binding used an obsolete handler form and directly retained the replay-capable portable session, which also owns ordinary mutation authority. Inherited HK04 authority inspection turned RED. The repair aligned replay handlers with `ICanonicalCapabilityHandler`/`CapabilityInvocationContext`, added a dedicated `WorldReplayBindingAuthority` facade that exposes only replay/compatibility authority to Runtime, and kept actual staged mutation execution behind `CanonicalWorldMutationAuthority` inside Authoring. No HK04 mutation guard was weakened.

**Finding 2 — HK01's independent projection oracle did not know the new replay semantic tokens.**

The new `CanonicalReplay` side-effect/transaction classifications initially could not be projected by the independent oracle. The oracle was extended explicitly with `canonicalreplay`; production projection behavior was not given a route-specific exception.

**Finding 3 — unsupported journal versions were rejected by generic request-schema validation before HK06C compatibility policy could own the diagnostic.**

A direct `journal@2` replay originally returned generic `contract.invalid_request`. The replay request schema now describes a versioned compatibility envelope rather than pretending every supplied artifact is already HK06A v1. The effective HK06C parser remains the authority that accepts only reviewed `journal@1`/`journal-entry@1` and returns stable `world.replay.unsupported_version`. This does not redefine the HK06A artifact schema.

**Finding 4 — inherited exact public-route and effective non-writer universes correctly turned RED after the surface grew.**

Adding replay + compatibility changed the independently enumerated public route counts from 16 to 18 production routes and from 19 to 21 with complete scoped fixtures. HK01 exact counts were reconciled to that independently observed universe. Separately, HK04's effective non-writer test initially demanded a vector for `authoring.journal.replay`; the repair did not pretend replay is read-only. `CanonicalReplay` is now excluded semantically alongside the other explicit canonical writer classes, while `authoring.replay.compatibility` is included and effectively proven read-only. Replay itself has its own dedicated `ReplaySurfaceConformance` writer guard.

**Finding 5 — the initial proof plan described a test executor seam that was unnecessary and weaker than effective defect injection.**

The proof was simplified rather than adding production test machinery. Two causal controls independently recompute accepted HK06A identity framing around deliberately forged but internally coherent later entries: one produces a semantically invalid second mutation and proves the real canonical mutation authority rejects it; the other records a false second result hash and proves replay detects effective persisted-result divergence. Both occur after earlier staged work and both assert the outer target state/journal remain unchanged. This directly covers validation bypass, partial publication and false-success classes without a test-only executor.

## Strong causal controls

`Hk06CReplayAtomicitySelfAttackTests` is the central late-failure proof:

- `IntegrityValidSemanticallyInvalidLaterStepMustBeRejectedByCanonicalValidationWithoutPartialPublish` passes replay parsing/identity checks, then sequence 2 fails effective canonical validation as `world.replay.step_failed`; the outer target keeps its exact original revision/hash and journal count 0.
- `IntegrityValidLateResultDivergenceCannotPublishStagedStateOrJournal` passes replay parsing/identity checks, executes the staged sequence, then sequence 2's actual persisted result disagrees with the independently forged expected result; replay returns `world.replay.result_divergence` and publishes none of the staged aggregate.

These are effective-behavior controls: they cannot turn GREEN merely because the replay parser rejects the fixture early.

## Content-shape result

`Hk06CContentShapeTests.PotesAuthoredSliceReplaysFromAcceptedSnapshotToIdenticalHashAndEmptySemanticDiff` reuses the accepted fictional Potes/Liébana slice with plaza, bar, shop, NPC relation and future-owned social extension. Two ordinary authored edits are journaled, the original base is imported into a clean lineage, and replay reconstructs the exact final hash/revision with an empty HK06B semantic diff. No gameplay-only concept was promoted into canonical authored state.

## Green implementation observation

Exact implementation/test SHA `0d7e22be092d8291d6f7e239e73ed7e91217bd3a` passed `Arkus Candidate Validation` run `35488744134`:

- Release build: 0 warnings / 0 errors;
- focused `Hk06C*`: 8/8 GREEN;
- full regression: 136/136 GREEN;
- canonical observation receipt: GREEN;
- candidate clean before and after: YES;
- artifact: `10598333769`.

An immediately prior strong-control SHA `973adfdd3d2c117c81d015e9e03173ca95701f06` also passed Actions `35488708584`, confirming the late validation/result-divergence controls independently before the content-shape probe was added.

## Baseline-to-candidate scope audit

The PR changes only:

- HK06C replay contract/parser/orchestration and a narrow binding facade;
- explicit canonical replay metadata/projection support;
- composition/binding/conformance for the two public HK06C capabilities;
- one `partial` declaration needed to extend the existing portable authoring aggregate;
- focused HK06C tests plus exact inherited-universe reconciliations in HK01/HK04 tests;
- canonical observation/verification routing and WP evidence.

No deterministic gameplay simulation, scheduler/clock, Unity replay, networking, GUI history browser, cloud persistence, Git-history replay source, cross-process merge or durable journal store was added.

## No remaining blocker found

- Replay is a separately classified orchestration but every replayed authored change still executes through the accepted canonical mutation authority.
- The complete staged aggregate is published only after sequence, effective-result, regenerated-journal audit and final-anchor checks pass.
- Failed/incompatible/tampered/gapped/reordered/divergent inputs cannot report successful completion or leak replayed state/history.
- Successful replay regenerates the accepted HK06A local journal and exact source entry identities rather than importing snapshot/rebase history.
- Existing local history is preserved by fail-closed refusal to splice.
- Compatibility is explicit, discoverable and stable for supported/unsupported versions.
- HK01/HK04 predecessor defenses remain effective over the enlarged surface rather than being bypassed.
- Final canonical hash/revision and HK06B semantic diff agree on equivalence.
- The Potes content-shape probe is GREEN.
- No concrete evidence reopens HK06A or HK06B.
- Remaining risks are outside the declared claim and recorded in `RESIDUAL_RISK.md`.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

This evidence reconciliation is documentation-only after the GREEN implementation observation. The resulting exact HEAD must pass `scripts/hk06c-verify-exact-sha.sh` unchanged before the PR is frozen and handed to a fresh independent Reviewer.