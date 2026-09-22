# Operational hardening — Batch B: mechanical automation

Status: PROCESS_ONLY candidate
Scope: routing, DocSync mechanics, PA selector convention, Reviewer-dispatch dedupe, same-SHA receipt-reuse safety.

## Goals

Batch B removes repetitive process work without moving semantic authority into automation.

1. **Exact-SHA routing registry.** `Docs/engineering/workpack-verifiers.json` is checker-owned routing only. `scripts/workpack-verifier.py self-test` parses both legacy shell dispatchers and requires exact WP→script equality. The old dispatchers remain in place during this batch; any divergence is RED before a later cleanup may delete them.
2. **DocSync mechanical core.** `scripts/docsync.py prepare` persists exact accepted identity, WP status/Acceptance, generated DocSync evidence core and the derived accepted-state index. It refuses ambiguous/nonstandard contracts. Free-form accepted-claim/README prose remains semantic reconciliation. `docsync.py check` binds WP Acceptance, evidence identity and index source-main identity before `DOCSYNC_COMPLETE`.
3. **Future PA selector convention.** PA-01..05 keep their frozen selectors. `WP-PA-06+` use one checker-owned `## Canonical disposition table` with key/status columns 0/1, so capsules cannot self-select completeness while future PA acceptance no longer needs one Python registry edit per WP.
4. **One Reviewer launch per closed candidate.** Telegram ignores intermediate `REVIEW_READY`; the existing idempotent closure workflow first establishes `REVIEW_READY_CLOSED`, and only that closed marker becomes the Reviewer handoff notification. This removes the observed duplicate-review launch without weakening exact-SHA/context checks.
5. **Safe same-SHA receipt reuse.** The ordinary validation-context digest remains routing identity, not a complete mutable-input identity. `scripts/reuse-context.py` binds reuse to exact PR + exact SHA + Draft state + the full live PR body + mutable external Worker-pre-review comment bytes. Before reuse, the canonical Worker handoff oracle is rerun against the exact candidate tree. The expensive exact-SHA execution receipt may be reused only after those live inputs still validate.

## Deliberately unchanged

- The per-WP verifier scripts remain the proof executors in this batch.
- Independent Reviewer PASS/FAIL remains semantic and exact-SHA bound.
- Receipt reuse still reuses only already-proven exact-SHA computation. It does not reuse current readiness: mutable handoff inputs are revalidated and a changed consumed input changes the reuse identity.
- DocSync automation does not infer acceptance from `ACCEPTED_STATE_INDEX.json`; accepted identity is explicit input from the already accepted transition and the index remains derived navigation only.
- DW-02's active implementation contract is not changed. If DW-02 adds a legacy dispatcher route before Batch B merges, the registry equivalence check will require the B branch to rebase and add the same route before merge.

## Migration rule

Add before delete. Batch B establishes the generic registry and shadow equivalence. Batch C may remove duplicated legacy routing only after B is accepted and equivalence remains GREEN on the then-current `main`.
