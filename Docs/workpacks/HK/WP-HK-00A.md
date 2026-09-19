# WP-HK-00A — Product architecture + adoption boundary

Status: COMPLETE  
Class: FOUNDATIONAL ARCHITECTURE  
Depends on: `WP-HK-00` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `95aa0080a5a4ddd55bd02aa29fcad7aa34252c21`
Implementation PR: `#15`

Completion:
- Reviewed candidate SHA: `e66ed729c75d94fb7efdfc304cf51ec52fa25e53`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5255049240`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35430543113`)
- Merge SHA: `0a4253443326ccab8e910c3265b015fc757cc4a2`
- Completed: `2026-09-19`

## Objective

Freeze the product-level ownership and adoption boundaries before protocol implementation so Arkus can reuse mature external work without becoming a narrower Unity/MCP refit.

## Allowed scope

Architecture/policy documentation, H0 roadmap/DAG reconciliation, affected future WP contracts, exact-SHA architecture consistency validation, causal document/DAG self-attacks, and evidence required to prove this WP.

No runtime feature implementation is required or allowed merely to make this architecture WP look more substantial.

## Acceptance

- `Docs/engineering/PRODUCT_ARCHITECTURE.md` is reviewed and explicitly establishes canonical Arkus semantics above transports and engines.
- MCP is defined as a first-class standards adapter/projection, not the canonical contract source.
- Unity is defined as the first engine bridge, not the platform boundary; H0 contracts remain engine-neutral.
- Legitimate engine-scoped public capabilities have one Arkus-owned composition/registration path: the canonical contract model remains engine-neutral, bridge implementations may contribute scoped definitions/bindings, and no bridge/transport may expose a parallel public registry or schema authority.
- Scoped public capabilities remain subject to canonical discovery/schema/policy/transaction/provenance rules; canonical-state mutation cannot bypass the accepted authoring transaction pipeline.
- A single-source contract-generation rule exists for discovery/schemas/SDK/Creator metadata; hand-maintained parallel truths are forbidden as the normal design.
- `Docs/engineering/DEPENDENCY_IP_POLICY.md` defines commercial license/provenance/SBOM/replaceability rules before external code is embedded.
- `Docs/engineering/EXTERNAL_HARNESS_ADOPTION_AUDIT.md` records the current adopt/borrow/benchmark/reject decisions and the capability patterns Arkus must meet or exceed.
- No external dependency may become the sole semantic authority for canonical state, transactions, validation, provenance/replay or persisted canonical format.
- Foundational proof rules explicitly reject self-shrinking/circular completeness universes and require independent/effective oracles.
- The H0 dependency route, HK01 contract and HK07 transport boundary are reconciled with this architecture.

## Required self-attacks / adversarial review questions

Because this WP is architecture/process rather than runtime implementation, its negative controls are document/DAG consistency attacks. At minimum demonstrate that review would reject each hypothetical change:

1. making MCP request/tool definitions the only canonical schema source;
2. introducing `UnityEngine` concepts into H0 canonical contracts;
3. adopting an engine harness wholesale even when required Arkus semantics are missing;
4. embedding an external dependency with unknown/incompatible commercial terms;
5. defining command completeness only from the discovery registry being proved;
6. allowing transport/engine adapters to mutate canonical state outside the accepted transaction pipeline;
7. forcing legitimate engine-scoped public capabilities into the H0/base contract instead of the Arkus-owned scoped composition path;
8. allowing an engine bridge to publish its own public capability/discovery/schema registry outside canonical composition.

## Forbidden scope

Gameplay implementation, Unity project/assets, Creator GUI, cloud SaaS, implementing HK01+ runtime code, selecting final future non-Unity engines or concrete Unity capability APIs.

## DoD

Architecture documents, roadmap/DAG and affected future WP contracts agree on one product boundary; adversarial consistency review finds no transport/engine/dependency path that can silently redefine or narrow canonical Arkus semantics, and a legitimate future engine-scoped capability has exactly one valid Arkus-owned path to become public without contaminating the H0 kernel; independent Reviewer PASS.
