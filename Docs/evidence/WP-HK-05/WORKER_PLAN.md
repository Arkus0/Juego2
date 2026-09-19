# WP-HK-05 Worker plan

Baseline SHA: `dbd8121411079f22a01d5cb85345e180ff41f7e2`
Worker: `ChatGPT / Codex`
Governing process: `FOUNDATIONAL_PROOF_STANDARD.md` v1.3 and `WORKER_REVIEW_PROTOCOL.md` v1.6
State: `ACTIVE`

No prior HK05 Worker branch or implementation PR existed at baseline. This cycle owns branch
`wp/hk-05-validation-diagnostics` and only the write set required by `WP-HK-05`.

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-02A — Object-scoped extension data + typed dependencies`.

- Baseline of predecessor cycle: `5a07c55aeb79406a84bff579b34c09459707b713`.
- Reviewed candidate SHA: `f39a1994524c42213dafb63d440faaf9de7c040f`.
- Independent Reviewer: `PASS`, PR `#25`, review `#5256593405`.
- Exact-SHA candidate observation: Actions `35456397714` GREEN.
- Exact-SHA freeze validation: Actions `35456445373` GREEN.
- Implementation observation: `2cb7a3daaa565a6b0ca5b68882541191750dac60`, Actions
  `35456331653`, Release 0 warnings/errors, 11/11 focused and 98/98 regression.
- Merge SHA: `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`.
- DocSync/main SHA: `dbd8121411079f22a01d5cb85345e180ff41f7e2`.

### Inherited guarantees consumed

1. HK01 owns canonical capability composition/discovery/schema truth and the independently
   enumerable effective public-route universe.
2. HK02 owns stable world/object identity, the finite canonical `WorldState`, structural and
   reference invariants, deterministic canonical serialization/hash and explicit schema version.
3. HK03 owns bounded deterministic inspection and exact public reconstruction of accepted state
   semantics without mutation.
4. HK04 owns shared plan/dry-run/apply candidate semantics, closed canonical commit authority,
   atomic replacement, whole-world revision/hash CAS, idempotency and semantic change coverage.
5. HK02A owns global/object-scoped extension identity, declared typed dependency semantics,
   referential integrity and their complete propagation through canonical format, HK03 reads and
   HK04 mutation/change coverage.

HK05 consumes those state, route and transaction boundaries. It will not rebuild route discovery,
commit-authority closure, codec completeness, paging completeness, CAS, idempotency or atomicity.

### Guarantees newly owned by HK05

- one stable, versioned machine diagnostic/result model with code, severity, resource/path,
  invariant identity and bounded remediation context;
- a mechanically enumerable validator inventory mapped against an independently derived universe
  of every invariant and expected rejection site owned by the accepted micro-world model;
- deterministic aggregation of independent violations without expected-error exceptions;
- explicit validation of current or proposed state;
- pre-commit validation on every accepted public canonical mutation route, reusing HK04's closed
  route and commit-authority guarantees rather than reopening them;
- discoverable validation request/result schemas and no known-invalid candidate reaching canonical
  state through the public mutation surface.

### Reopen conditions

Reopen an inherited guarantee only if concrete evidence shows HK05 diagnostics or validation do not
cover the effective accepted path: a canonical invariant/rejection site omitted from the independent
universe, a public route reaching HK04 commit without the HK05 validator, a state field inaccessible
to validation, or a predecessor claim becoming factually false. A theoretical alternate route,
duplicate defence-in-depth proof or unsupported path is not a reopen condition.

## Implementation boundary

- `Arkus.Game.Validation`: canonical diagnostic/result contracts, validator inventory and validator.
- `Arkus.Game.Authoring` / `Arkus.Harness.Runtime`: explicit validate command plus binding of the
  same validator to the inherited transaction candidate path.
- canonical contract definitions/schemas, focused tests, exact-SHA scripts and HK05 evidence.
- no AI prose repair generation, Unity/engine validation, gameplay-specific invariant, journal or
  replay implementation.

## Proof approach

Derive the invariant/rejection-site universe independently from the accepted canonical model and
public mutation contract, then reconcile it against the registered validator inventory. Exercise
the effective explicit-validation and public-mutation paths against the same known-invalid proposed
states. Required defect injections must demonstrate causal RED for skipped mutation validation,
missing inventory registration, escaped exceptions, unstable ordering, ambiguous context and
validate/apply disagreement, followed by GREEN after restoring the candidate.

The representative content-shape probe will use the accepted bounded Potes/Liébana authored slice
without adding schedules, transforms, gameplay schemas or payload interpretation.

