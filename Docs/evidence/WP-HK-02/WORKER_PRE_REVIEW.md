# WP-HK-02 Worker adversarial pre-review

Baseline SHA: `82ad746961145c2c779f5086cbc6bfde2b348c3c`  
Implementation observation SHA: `58ca3eab4090c211126425be1e3950f503f48000`  
Observation run: GitHub Actions `35436688357`

## Scope reread

Re-read before evidence reconciliation/freeze:

- `Docs/workpacks/HK/WP-HK-02.md`
- `Docs/engineering/PRODUCT_ARCHITECTURE.md`
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
- accepted HK01 contract/evidence and the HK02 predecessor contract check
- complete baseline `82ad746...` → implementation `58ca3ea...` file set

The implementation remains strictly inside HK02: canonical world identity/state, deterministic serialization/hash, integrity and a tiny generic micro-world. It adds no gameplay systems, Unity/DFU model, asset pipeline, transport host or public capability.

## Adversarial questions and conclusions

### Could caller/container ordering leak into canonical identity?

No accepted path was found. Objects, references and extensions are copied at construction and sorted by explicit ordinal semantic keys during serialization. Culture is not used for identity or numeric framing. A deliberately order-sensitive fingerprint differs under reordered input while canonical bytes/hash remain equal.

### Could semantic state exist without entering canonical bytes/hash?

The initial mutation matrix covered each current field class, but pre-review identified a future false-green class: a new public semantic constructor/property could be added without automatically entering that matrix. A reflection-based independent semantic-surface guard was added over the public constructors/properties of the four state-bearing types. New exposed state now makes the causal suite RED until the proof is updated; current semantic input mutations all change hash.

### Could that new control itself sit outside the causal gate?

Yes, initially. Its first class name (`Hk02SelfAttackSemanticSurfaceTests`) did not match the canonical `FullyQualifiedName~Hk02SelfAttackTests` filter, so the control passed only in full regression. Pre-review caught this and renamed it under `Hk02SelfAttackTests*`. Exact-SHA Actions `35436688357` now proves 11/11 controls inside the dedicated causal gate.

### Can malformed/unknown data be silently normalized away?

No known in-claim path remains. Deserialization rejects unsupported outer schema, unknown structural records, invalid UTF-8/Base64, dangling graph edges and any semantically readable payload whose bytes are not the unique canonical reserialization. Only the explicit namespaced/versioned opaque extension envelope is forward-preserved.

### Is the graph integrity policy explicit and closed?

Yes for HK02: object IDs are unique; containment targets and reference targets resolve inside the same world state; containment is acyclic; duplicate reference edges are rejected as non-semantic duplication; extension owner/version pairs are unique.

### Did HK02 reopen HK01 or build speculative downstream systems?

No. No capability, composer, discovery, transport or engine route is changed except registering HK02's exact-SHA validation scripts in the already generic CI router. Accepted HK01 guarantees are consumed. Transaction/concurrency/provenance/gameplay/engine behavior remains downstream.

## Material findings repaired during Worker phase

1. Fixed a trivial missing namespace-closing brace caught by the first clean Release observation before tests ran.
2. Added an independent reflected semantic-surface universe because the original mutation matrix alone could miss a future newly exposed semantic input/property.
3. Moved that semantic-surface control into the canonical HK02 causal-test filter after detecting it was initially only covered by full regression.

No known in-claim blocker remains.

## Validation state

Canonical observation on implementation head `58ca3eab4090c211126425be1e3950f503f48000`:

- locked restore: GREEN
- Release build: GREEN, 0 warnings / 0 errors
- canonical positive tests: 6/6 GREEN
- causal negative controls: 11/11 GREEN
- total regression: 56/56 GREEN
- candidate clean before/after: YES
- Actions run: `35436688357`

The evidence-bearing candidate created by this reconciliation must still pass `scripts/hk02-verify-exact-sha.sh` before freeze. That verifier reruns the canonical observation and requires the proof/pre-review markers committed here.

## Residual-risk / proof-budget check

The custom codec is intentionally small and Arkus-owned because HK02 needs a documented deterministic semantic authority, not a generic serializer framework. Unknown structure is not guessed; only the explicit opaque extension envelope is preserved. No arbitrary platform vocabulary, migration engine or gameplay schema was introduced.

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-02/WORKER_PRE_REVIEW.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
