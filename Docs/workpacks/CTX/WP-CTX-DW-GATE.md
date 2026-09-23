# WP-CTX-DW-GATE — CTX↔DW discoverability and selective-adoption gate

Status: PLANNED / NOT_STARTED
Class: NON-PRODUCT-FOUNDATIONAL / CROSS-TRACK PROCESS-ARCHITECTURE GATE
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: accepted `WP-CTX-03` + accepted `WP-DW-GATE`
Does not block: `WP-H1-03`, `WP-H1-03A`
Adoption consequence: authorizes selective CTX↔DW routing for later H1 work and supplies a reviewed planning input to H2; it does not authorize general DW adoption.

## Objective and central claim

Prove that a fresh Worker or Reviewer can begin from CTX, use DW only when DW is materially useful, escalate to authoritative sources when required, and preserve at least the current material blocker-discovery capability while reducing irrelevant initial context.

The gate tests composition of two already accepted systems. It does not turn either representation into a new source of semantic authority.

## Architecture

The intended composition is:

```text
mission / active claim
        ↓
CTX control plane
  - role + claim
  - predecessor/authority routing
  - mandatory-read classes
  - escalation conditions
        ↓ selective
DW knowledge/data plane
  - typed facts
  - relations
  - findings/dispositions
  - provenance
  - bounded queries
        ↓ when needed
accepted authoritative sources
  - contracts / code / accepted evidence / domain sources
```

There are two different authority planes and they must not be collapsed into one total order.

For represented **semantic/content facts**:

```text
authoritative accepted source > DW derived projection > CTX semantic summary/navigation surface
```

For **routing/process**:

```text
accepted CTX effective read-set / mandatory-read / escalation authority > DW materiality advice
```

DW may help CTX choose a narrower relevant knowledge path, but it may not erase, weaken or satisfy away a CTX-mandated authoritative read or escalation condition. `DW: NOT_MATERIAL` means only that DW itself adds no causal value; it never means a required source class can be skipped.

A disagreement is never resolved by voting between CTX and DW. Any material CTX↔DW or projection↔source contradiction forces source-open, rebuild where applicable, or fail-closed classification.

## Core adoption principle

**All material information must be discoverable from CTX, but not all material information must be duplicated into DW.**

CTX owns routing and discoverability. DW owns structured derived knowledge only for shapes it actually covers. Authoritative sources remain complete truth.

The gate therefore treats correct DW abstention as a positive capability. `DW: NOT_MATERIAL` is a valid and often preferred route.

## Routing result vocabulary

For a concrete task/claim, CTX must be able to classify DW as one of:

- `USE` — a current, validated DW projection contains materially useful structured knowledge for the claim;
- `OPTIONAL` — DW may improve navigation/context precision but is not required;
- `NOT_MATERIAL` — loading DW would add noise without causal value.

This classification is advisory routing, not semantic authority. It cannot shrink the accepted CTX effective read-set. The Worker/Reviewer may always expand authoritative reads.

`USE` is additionally forbidden for a projected universe whose source identity, completeness oracle, provenance binding or rebuild/staleness state is not current and GREEN.

## H1 projection bootstrap and lifecycle

DW-GATE did not establish H1 projection semantics. In particular, H1-04 cannot be treated as if its real Quaternius/catalogue universe already existed in DW before H1-04 has accepted that universe.

The H1 sequence is therefore explicit:

1. **H1-04 runs without depending on an H1-real DW projection.** Its product claim consumes CTX plus authoritative H1/H0 contracts, the human-approved Quaternius Source adoption boundary and its own deterministic Unity/catalogue evidence. Pre-existing DW knowledge may be `OPTIONAL` only if independently material, but it cannot stand in for the catalogue/source universe H1-04 is creating and proving.
2. **After H1-04 PASS, a separate non-product projection step may materialize the accepted H1-04 universe into DW.** This step is owned by the CTX↔DW adoption/projection boundary, not by the H1-04 product Worker and not by shared H0/DW kernel semantics.
3. **Only after that projection is independently current and complete may later H1 work classify that H1 projection as `USE`.** H1-05 is the first eligible real consumer; H1-06 is the second deliberately different asset/prefab-shape observation. Neither H1 workpack is blocked if the projection is unavailable: it must fall back to CTX→authoritative sources instead of inventing or silently repairing DW machinery.

The H1 projection lifecycle owns all of the following before `USE` is legal:

- **independent source universe:** enumerate from the accepted H1-04 source-adoption and catalogue/identity authority/evidence, not from the DW output being checked;
- **projection owner/adapter/schema:** a non-product H1-to-DW adapter maps that accepted universe onto generic DW facts/relations/provenance; H1/domain meaning remains consumer-owned and does not become shared DW/H0 semantics;
- **completeness oracle:** compare the independently enumerated authoritative universe with projected identities, required represented fields/relations, cardinality/targets and provenance according to the declared H1 projection schema; the projection may not define its own completeness universe;
- **provenance:** bind projected records to exact accepted source/evidence identities, hashes/fingerprints and projection/schema version sufficient to source-open the represented fact;
- **staleness:** any material source identity/fingerprint, accepted H1-04 authority or projection-schema change makes the projection non-current until rebuilt and revalidated;
- **rebuild:** deterministic rebuild from the same accepted universe must reproduce the same normalized projection, and stale/corrupted projection evidence must turn RED rather than remain self-consistent;
- **routing admission:** `USE` requires the above lifecycle evidence GREEN for the exact projection consumed. Otherwise CTX must classify that H1 projection `OPTIONAL`/`NOT_MATERIAL` or route directly to authority.

This lifecycle is context infrastructure. It does not alter H1 product semantics, make DW canonical, or create an H1-04 acceptance dependency on CTX↔DW.

## H1 initial coverage map

This is a planning/default map, not a hard-coded semantic oracle:

| H1 area | CTX | DW expected value | Adoption intent |
|---|---|---|---|
| `H1-03` host/workspace authority | HIGH | NONE/LOW | do not block; normally `NOT_MATERIAL` |
| `H1-03A` Editor execution/lifecycle | HIGH | NONE/LOW | do not block; normally `NOT_MATERIAL` |
| `H1-04` catalogue/identity + first real source | HIGH | no assumed H1-real projection | authority/projection bootstrap; product route remains source-first |
| post-`H1-04` projection | HIGH | lifecycle/proof step | build + validate H1 projection outside product semantics |
| `H1-05` managed scenes | HIGH | MEDIUM/HIGH if current | first eligible material consumer; fallback to authority if not ready |
| `H1-06` assets/prefabs | HIGH | HIGH if current | second materially different adoption observation |
| `H1-07+` | HIGH | claim-dependent | route by evidence, never by blanket requirement |
| `H1-GATE` | HIGH for normal H1 process | separate from required public-client bootstrap | do not contaminate the mandatory fresh-agent trial |

The map may be refined by evidence without rewriting H1 product semantics.

## Inherited guarantees

From CTX:

- compact/index/capsule surfaces are navigation/projection, not authority;
- role boot sets are minimum starting packs, not maximum context ceilings;
- stale, missing, contradictory or materially ambiguous compact context forces authoritative reads;
- fresh Reviewer independence is preserved;
- effective mandatory reads cannot be silently caller-shrunk.

From DW:

- accepted sources remain semantic authority;
- DW is derived/rebuildable and provenance-bearing;
- generic fact/relation/provenance behavior is not allowed to depend on CITY/PA semantics;
- consumer/domain semantic necessity remains consumer-owned;
- DW-04 quality/context evidence is bounded to its frozen experiment;
- DW-GATE does not prove H1 projection semantics, general adoption, arbitrary-domain universality, external packaging or Unity/runtime correctness.

## New guarantees owned by this gate

1. CTX can route a role to material DW knowledge without making DW mandatory everywhere.
2. CTX can route directly to authoritative sources when DW is absent or not material.
3. Material source classes cannot become causally undiscoverable merely because a compact representation omitted the clue that would have prompted the read.
4. Stale or contradictory DW/CTX state cannot silently authorize a verdict.
5. Critical DW-derived facts remain source-open through effective provenance.
6. Correct abstention avoids context inflation on claims where DW adds no material information.
7. The combined path preserves Reviewer freedom to challenge the compact route and inspect deeper authority.
8. CTX process/read-set authority cannot be weakened by DW semantic/materiality advice.
9. An H1 projection cannot be marked `USE` until its independent universe, projection ownership, completeness, provenance, staleness and rebuild lifecycle are accepted/current.
10. Selective H1 adoption can begin without making H1-03/H1-03A or H1-04 product acceptance depend on the gate.
11. The required H1-GATE fresh public-client trial remains an independent proof of public discovery/schemas and cannot be made easier by private Juego2 CTX/DW knowledge.

## Negative-claim rule

A material negative claim such as “no dependency exists”, “no accepted source requires X”, “no path grants Y”, or equivalent absence/completeness claim may not rely solely on a compact CTX/DW view unless that view has a separately accepted completeness oracle for exactly that universe.

Otherwise the route must open or mechanically query the authoritative universe that can falsify the negative claim.

## Acceptance criteria

PASS requires all of the following:

- **discoverability preservation:** no bounded gate scenario loses a material blocker that the authoritative baseline route exposes;
- **dual-authority preservation:** semantic source contradictions override compact state, while accepted CTX mandatory-read/escalation rules cannot be neutralized by DW routing advice;
- **staleness handling:** stale DW provenance/source identity cannot remain silently GREEN;
- **H1 projection lifecycle safety:** a synthetic/planned H1 projection cannot become `USE` until an independently enumerated source universe, completeness comparison, exact provenance and deterministic stale/rebuild behavior are demonstrated;
- **abstention:** at least one technically dense H1-like scenario where DW has no causal value routes `NOT_MATERIAL` without loading the DW corpus;
- **selective utility:** at least two materially different content/design scenarios use DW to reach relevant facts/relations/provenance with less irrelevant initial context than the authoritative-baseline boot route;
- **no domain leakage:** CTX↔DW routing does not teach shared H0/CTX/DW-kernel logic CITY/PA/H1 semantic rules;
- **negative-claim safety:** absence/completeness claims cannot be closed from an incomplete compact universe;
- **Reviewer independence:** Worker-selected routing cannot prevent a fresh Reviewer from opening any material source;
- **H1-GATE trial purity:** this gate does not redefine or pre-supply private CTX/DW knowledge to the required H1-GATE public-client trial;
- **no claim inflation:** the result remains bounded and does not convert accepted DW-04/DW-GATE evidence into general model-performance or universal-domain claims.

Context reduction is a secondary success measure, not the primary oracle. There is no new arbitrary percentage threshold. Correctness and blocker discoverability dominate savings.

## Bounded falsification suite

The gate should use a small, predeclared suite rather than another large paired campaign. At minimum include:

1. **Hidden-materiality scenario** — compact CTX omits the clue that makes an authoritative source relevant; claim/dependency rules must still force the read and discover the blocker.
2. **CTX↔DW contradiction** — both compact surfaces are individually plausible but disagree; route must open authority rather than choose one.
3. **Stale-DW scenario** — DW fact/provenance refers to superseded source identity; route must reject/rebuild/escalate.
4. **DW abstention scenario** — host/lifecycle-style technical claim has no material DW contribution; route must remain CTX→H1/H0 authority without corpus loading.
5. **Material-DW scenario** — catalogue/asset/design relation query uses a valid projected universe successfully, then source-opens a decision-critical fact through provenance.
6. **Negative-claim scenario** — compact view appears to prove absence but an omitted authoritative record falsifies it; route must not close early.
7. **Domain-leakage differential** — alpha-renamed/behaviorally equivalent adopted consumer semantics must not change shared routing/kernel behavior merely because CITY/PA/H1 vocabulary changed.
8. **False-positive control** — opaque/diagnostic text resembling adopted vocabulary must not manufacture materiality or forced DW use.
9. **H1 projection-bootstrap scenario** — attempt to mark H1 `USE` before authoritative universe/completeness/provenance/rebuild is current must fail; after a valid independent projection it may become eligible without changing the H1 product oracle.
10. **Routing-authority conflict** — DW says `NOT_MATERIAL` or appears complete while CTX requires a source class/escalation; the CTX read-set wins and the required authority is opened.

The suite should prefer behavioral/differential evidence over hand-maintained keyword inventories.

## Agent execution evidence

Reuse accepted deterministic/exact-SHA CTX and DW evidence for already-proven mechanical facts. Do not rerun DW-04's 36-call campaign merely to restate its accepted result.

A fresh-agent probe is permitted only for the genuinely new CTX↔DW discoverability/composition claim. Keep it small and predeclared (for example, a few representative Worker/Reviewer-like scenarios spanning USE, NOT_MATERIAL and forced escalation). Model prose is never the oracle; deterministic expected blocker/source-open outcomes are.

This CTX↔DW probe is **not** the required fresh public-client AI-agent trial owned by `WP-H1-GATE`. It may not be counted as H1-GATE evidence or used to weaken that trial's public bootstrap.

No statistical-power or generalized model-performance claim may be made from this bounded probe.

## H1 adoption after PASS

PASS does not rewrite H1 workpack claims.

- `H1-03` and `H1-03A` remain free to progress independently; expected route is normally CTX plus authoritative H1/H0 sources, with DW `NOT_MATERIAL` unless concrete evidence says otherwise.
- `H1-04` remains source-first and is the authority/projection-bootstrap point, not a consumer of not-yet-existing H1 catalogue knowledge in DW.
- after H1-04 PASS, the non-product CTX↔DW projection owner may build/validate the H1 projection using the lifecycle above; no later H1 work is blocked if this is not ready.
- `H1-05` is the first eligible real H1 consumer where the accepted/current projection is material; `H1-06` is the second planned observation because asset/prefab relationships provide a different material shape.
- normal H1 execution should record lightweight route telemetry only when useful: DW classification, source escalations, material blocker discovery and obvious context/cognitive-load changes. It must not spawn shadow duplicate Workers merely to produce metrics.
- if a CTX↔DW routing/projection defect is discovered during H1, repair that causal non-product owner; do not distort the H1 product claim to manufacture a PASS.

### H1-GATE separation

The mandatory `WP-H1-GATE` fresh independent public-client AI-agent trial remains exactly a proof of the accepted public H1/Arkus discovery boundary. Its bootstrap stays the one owned by H1-GATE: fixed public launch-profile instructions, public MCP discovery and returned schemas, with no private implementation-source/product call.

Do **not** pre-seed that required trial with Juego2-private CTX capsules, DW projections, proving-ground history or routing knowledge that is not part of the declared public client boundary. CTX↔DW usefulness may be observed in a separate composition probe, but that observation is not an H1-GATE oracle and cannot repair a public-discoverability failure.

If a later H2/public-product plan chooses to publish/version CTX/DW as part of an external consumer boundary, that is a new reviewed portability claim. It does not retroactively change what H1-GATE proves.

## H2 planning handoff after PASS

The future H2 planner must distinguish two independent questions:

1. **product portability** — versioned/public Arkus can be consumed from a fresh external repository;
2. **knowledge/context portability** — a fresh agent in that repository can discover and use the necessary product/domain knowledge without inheriting Juego2 implementation history as working context.

CTX↔DW is candidate infrastructure for the second question, not proof of the first.

A future external-consumer trial should fail if success requires hidden Juego2 paths/history, unpublished authority, stale projections, H0 patching or copying the proving-ground context wholesale.

Final H2 public/external-boundary acceptance must consume or explicitly disposition both accepted DW-GATE evidence and this CTX↔DW gate result if this gate is accepted.

## Explicit non-claims

PASS does not assert:

- DW should be loaded for every workpack;
- every repository fact belongs in DW;
- DW replaces CTX;
- CTX or DW replaces authoritative accepted sources;
- H1 projection semantics were already proven by DW-GATE;
- H1-04 depends on a pre-existing H1 catalogue projection;
- arbitrary-domain universality;
- automatic generation of domain semantic oracles;
- external package/version/repository consumability;
- Unity/runtime correctness;
- generalized model-performance or token-cost guarantees.

## Process constraints learned from CTX/DW execution

- do not add a second lifecycle solely to prove the first lifecycle;
- reuse trustworthy exact-SHA evidence for mechanical facts rather than rerunning expensive suites for independence theater;
- protocol/admin defects are `PROTOCOL_FIX` / `REVIEW_BLOCKED`, not semantic FAIL, unless they invalidate material evidence;
- avoid checker-per-sabotage escalation: protect causal classes, not every synthetic evasion;
- prefer independent oracles and behavioral differentials over self-confirming projections or lexical whitelists;
- do not split this gate into administrative micro-workpacks unless a newly discovered independently rejectable causal claim requires it;
- DocSync is minimal and durable-meaning-driven.

## PASS consequence

PASS authorizes bounded selective CTX↔DW adoption and the explicit post-H1-04 projection lifecycle described above, without blocking or reopening H1-03/H1-03A or changing H1-04's product claim. It also becomes a reviewed planning input for future H2 knowledge/context portability design.

FAIL routes only to the causal CTX/DW composition/projection owner or an actually falsified predecessor guarantee. It does not automatically reopen H1 product work or accepted CTX/DW predecessors.
