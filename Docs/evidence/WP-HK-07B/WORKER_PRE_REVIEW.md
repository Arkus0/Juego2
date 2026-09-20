# WP-HK-07B Worker pre-review — repair cycle 1

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 1
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-07B/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-07B`.
- Baseline: `6173471245da081b21cf66fd164cbb3a30dcba1d`.
- Branch: `wp/hk-07b-mcp-projection`.
- Transfer / previously reviewed SHA: `1447e56642414ce2e75219fdfcb191ada41d6378`.
- Independent Reviewer FAIL: review `#5259817513`.
- Repair implementation/test SHA observed GREEN: `b8e7f1a628e65effa790f810519a721454a6c980`.
- Actions run: `35495362976`; artifact: `10600483330`.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Predecessor split rechecked

`Docs/evidence/WP-HK-07B/WORKER_PLAN.md` still matches the accepted HK07A evidence and current baseline. The Reviewer blocker does not reopen HK07A: canonical `CapabilityKey` legitimately permits the long identifier, composition accepts it, and the limitation existed only in MCP framing. HK07B already owns collision-safe transport naming/version mapping and explicitly classified MCP naming/framing limits as adapter work.

No Protocol, Runtime, Projection, Game state/validation/authoring semantic source was changed. The repair is limited to `Arkus.Harness.Mcp`, HK07B tests and HK07B evidence.

## Reviewer finding repaired at the causal boundary

The previous codec built the reversible `<canonical-name>@<version>` encoding and threw if the resulting MCP name exceeded 128 characters. That allowed composition to accept a valid scoped capability while MCP discovery failed, directly falsifying HK07B's projection-completeness claim.

The repaired allocator works over the complete deterministically sorted composed inventory:

- if the reversible MCP encoding fits, it is retained unchanged;
- if it exceeds 128 characters, the adapter emits `arkus-long-<canonical-ordinal>-<sha256>`;
- the canonical ordinal is unique within the sorted composed inventory, so distinct accepted capabilities remain separated even if fingerprints collide;
- the full canonical key and definition remain untouched on the descriptor and MCP metadata;
- invocation resolves the transport handle back to that descriptor and constructs the neutral request from the exact canonical name/version.

This is adapter-local framing, not a second semantic identity registry.

## Causal reproduction and collision-separation control

`OverLimitScopedCapabilitiesRemainDiscoverableUniqueAndInvokable` creates one real scoped provider containing **two** different canonical-valid capability names of exactly 123 characters. Both are accepted by normal `ContractComposer` validation with real `PublicCapabilityRoute` metadata. The test then requires:

1. both exact canonical identities are present in MCP projection;
2. both MCP tool names are non-empty and <=128 characters;
3. both use the bounded long-name family;
4. the two names are different;
5. invoking each transport handle reaches its corresponding canonical route exactly once.

The test therefore covers the reported boundary and the wider alias/collision class rather than special-casing one literal name.

## Finding fixed during this repair Worker's own validation

**Finding 1 — the first long-name test fixture contaminated HK01's independent route-universe regression.**

The initial repair used static attributed handlers in the main test assembly. Focused HK07B tests were 9/9 GREEN, but full regression correctly failed because HK01 independently enumerates that test assembly and saw the two new public-route attributes as extra routes.

The repair did not weaken HK01's oracle, exclude files, change route discovery, or amend predecessor expectations. Instead, the long-name test creates its attributed handlers at runtime in a separate dynamic assembly. `CapabilityRoute.FromHandler` and `ContractComposer` still validate genuine route metadata for the scoped provider, while HK01's explicit accepted test assembly remains unchanged. Full regression then returned GREEN.

## Complete diff / false-green challenge

The complete baseline→repair candidate was challenged for:

- a long canonical identity still causing `DescribeCapabilities()` to throw;
- truncation/hash-only aliasing between two distinct long identities;
- a long surrogate exceeding MCP's 128-character limit;
- collision between the direct/reversible name family and long-surrogate family;
- preserving display metadata while invocation accidentally dispatches a shortened/surrogate identity canonically;
- ordering nondeterminism causing two calls over one unchanged inventory to allocate different names;
- solving the transport limit by imposing a new canonical length restriction or changing HK07A neutral fields;
- using a fixed MCP registry or base-only list to allocate long names;
- weakening HK01 route-universe regression to make the new test pass;
- retaining the old proof contradiction that called tool-name length a residual while claiming zero known undetected defects;
- regression of the already accepted short encoding (`world.summary@1.0` → `world.summary_401.0`), cancellation mapping, SDK isolation, inventory/schema conformance or cross-transport H0 flows.

No such blocker remains in the repaired candidate/evidence.

## Green observation

Exact implementation/test SHA `b8e7f1a628e65effa790f810519a721454a6c980` passed:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07B*`: 9/9 GREEN;
- full regression: 160/160 GREEN;
- clean before/after exact-SHA observation: YES;
- Actions run `35495362976`: SUCCESS;
- artifact `10600483330`.

The evidence reconciliation commit after this observation changes documentation/evidence only and must pass `scripts/hk07b-verify-exact-sha.sh` unchanged before freeze.

## Scope/proof-budget conclusion

The Reviewer exposed one real HK07B-owned completeness defect; it is repaired at the adapter allocation boundary with one causal class-level test. No predecessor semantic guarantee was re-proved or changed, no HK08 behavior was pulled forward, and the former long-name residual has been closed rather than hidden. Remaining residuals are bounded in `RESIDUAL_RISK.md`.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary blocker remains. The resulting documentation/verification reconciliation SHA is eligible for exact-SHA validation and independent review after freeze.
