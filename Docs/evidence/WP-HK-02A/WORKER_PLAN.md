# WP-HK-02A Worker plan

Baseline SHA: `5a07c55aeb79406a84bff579b34c09459707b713`
Worker: `Codex / GPT-5`
Governing process: proof standard v1.3 and Worker protocol v1.6, adopted prospectively by merged process PR `#24`

State: `ACTIVE`

The mixed Draft PR `#23` was closed as superseded before freeze because it attempted to introduce its own adoption standard. It had no frozen candidate and no Reviewer verdict. This is a new Worker cycle from the process-adoption `main`; no transfer or acceptance evidence is inherited from that Draft.

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-04 — Planning + transactional mutation`.

- Reviewed candidate SHA: `849ed68e41d674ab0d50883ccd9af394e9d2e456`.
- Independent Reviewer: `PASS` on PR `#19`, review `#5256156672`.
- Exact-SHA freeze validation: GREEN, Actions run `35450965678`.
- Implementation merge SHA: `b1810a5f7c5378ff06272718a11e08720d714a65`.
- DocSync main SHA: `c188899bc3c0f989993ea1d53d4b52cba9d33cb6`.
- Current baseline: `5a07c55aeb79406a84bff579b34c09459707b713`; intervening ART-00 and process-adoption merges do not alter the accepted harness implementation.

### Inherited guarantees consumed

1. HK01 owns canonical composition/discovery/schema truth and the independently enumerable effective public-route universe.
2. HK02 owns typed stable object identity, explicit world revision/schema, deterministic canonical serialization/hash and the accepted structural/reference model.
3. HK03 owns complete bounded deterministic reads and exact public reconstruction of the accepted state semantics.
4. HK04 owns shared plan/dry-run/apply candidate semantics, complete pre-commit validation, atomic commit, whole-world revision/hash CAS, idempotency, semantic change coverage and the closed internal commit-authority boundary.

HK02A evolves the data carried through those accepted boundaries. It does not rebuild route discovery, read-side-effect proof, CAS/idempotency or hidden-write authority proof.

### Guarantees newly owned by HK02A

- optional object subject identity for extension data;
- typed extension dependency edges visible to canonical referential validation;
- composite global/object-scoped extension identity and uniqueness;
- canonical format/version/hash coverage for both new semantic fields;
- complete propagation through HK03 extension reads/reconstruction and HK04 extension operations/change coverage.

### Reopen conditions

Reopen an inherited guarantee only if concrete evidence shows the widened semantics fall outside its effective path: a canonical extension field omitted from serialization/read reconstruction, a public mutation route unable to preserve it, a commit outside HK04 authority, or another accepted claim becoming factually false. Updating fixtures, schemas, keys and semantic oracles for the new fields is expected propagation, not predecessor invalidation.

## Verified correction to the exploratory note

The note correctly identified world-global extension identity and opaque reference invisibility. It overstated the concurrency consequence: HK04 uses whole-world revision/hash CAS, so simultaneous disjoint object edits already conflict regardless of extension key. HK02A claims finer resource/change/rebase granularity, not concurrent auto-merge.

Adding only `SubjectId` would also leave payload-internal object references invisible. The workpack therefore adds typed declared dependencies. Payload completeness cannot be inferred from opaque bytes; producers are contractually required to declare object identities their payload semantics use. Rich typed payload schemas remain future reviewed work.

## Implementation boundary

- `Arkus.Game.World`: model, validation, format version, codec/hash and semantic ordering.
- `Arkus.Game.Authoring`: HK03 extension selectors/results/reconstruction surface and HK04 extension operation grammar, identity, fingerprint, plans and coverage.
- canonical definitions/schemas and tests update mechanically; no new public route is introduced.
- no transform/gameplay schema, Unity type, ECS framework, payload interpreter, new transaction authority or concurrency relaxation.

## Proof approach

The independently enumerated HK02 semantic-property universe grows for the new public properties. The codec hash mutation matrix, HK03 public reconstruction hash and HK04 independent semantic change oracle remain the evaluated boundaries; they are extended for the new semantics rather than replaced. Focused defect injections remove subject/dependency contribution at each boundary and must turn the corresponding oracle red.

The required representative content-shape probe is recorded separately and will be rerun against the complete candidate before freeze.

`PROOF_BUDGET_VERDICT` remains pending implementation and strict pre-review.
