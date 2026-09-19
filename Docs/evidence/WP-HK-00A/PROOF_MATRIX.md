# WP-HK-00A — Foundational Proof Matrix

FOUNDATIONAL_PROOF_VERDICT: READY  
UNRESOLVED_PROOF_OBLIGATIONS: 0  
KNOWN_UNDETECTED_DEFECT_CLASSES: 0  
TRUST_BOUNDARY: The in-claim universe is the product/adoption architecture contract and the H0 roadmap/WP dependency surfaces named by WP-HK-00A. Git object/checkout semantics, GitHub Actions runner, normal shell/Python behaviour and the accepted HK00 build foundation are trusted infrastructure. Third-party upstream projects are research inputs only; this WP does not claim their future state or approve an exact dependency version.  
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Claim

WP-HK-00A freezes who owns Arkus semantics before HK01 implementation: canonical contracts/state/transactions/validation/provenance remain Arkus-owned; transports and engines are projections/adapters; external components remain replaceable mechanisms behind Arkus-owned conformance boundaries; the H0 route preserves those ownership rules through HK01 and HK07.

This is an architecture/process claim, not a runtime-behaviour claim. The proof therefore checks the finite set of binding documents named by the WP and exercises causal document/DAG negative controls. It deliberately does not grow a general source-code proof system for a document-only workpack.

## Oracle

Canonical consistency command:

```bash
bash scripts/hk00a-architecture-check.sh
```

Causal negative-control command:

```bash
bash scripts/self-attacks/run-hk00a-architecture-attacks.sh
```

Exact-SHA Worker observation/verification is routed through:

```bash
ARKUS_WP=WP-HK-00A bash scripts/arkus-observe-exact-sha.sh <candidate-sha>
ARKUS_WP=WP-HK-00A bash scripts/arkus-verify-exact-sha.sh <candidate-sha>
```

GitHub Actions supplies `PR_BODY` and resolves the same workpack without the local `ARKUS_WP` override.

## Matrix

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / attack | Result | Residual risk |
|---|---|---|---|---|---|---|
| Canonical Arkus semantics sit above transports/engines | `PRODUCT_ARCHITECTURE.md`, HK01, HK07 | WP names these exact ownership surfaces; checker requires the upstream/downstream statements | `hk00a-architecture-check.sh` | Make MCP the canonical authority | GREEN / RED as intended | Semantic prose can still be misread; independent Reviewer remains required |
| MCP is projection, not contract source | Product architecture + HK01 | One-source rule and HK01 upstream ownership are both required | architecture checker | Replace “not canonical semantic authority” with canonical authority | GREEN / RED | Future MCP SDK behaviour is outside this WP until exact adoption |
| Unity is first bridge, H0 remains engine-neutral | HK01 + product architecture | HK01 is the first canonical contract WP and is mechanically scanned for engine implementation concepts | architecture checker | Inject `UnityEngine.GameObject` into HK01 canonical contract | GREEN / RED | Future H1 engine abstractions are intentionally not frozen yet |
| One-source contract generation rule | Product architecture + HK01 | The checker requires one canonical contract source plus canonical-inventory-upstream wording | architecture checker | MCP authority attack also removes the required ownership condition | GREEN / RED | Exact code-generation mechanism belongs to HK01 |
| Dependency/IP/commercial rules exist before embedding | Dependency policy + adoption audit | Exact-version/license/provenance/replaceability/SBOM rules are required and the audit explicitly cannot approve adoption | architecture checker | Permit unknown license terms before embedding | GREEN / RED | Legal interpretation of unusual licenses may require human/legal review |
| Current adopt/borrow/benchmark/reject boundary is recorded | Adoption audit | Audit has explicit decision scale, current candidate table and refresh rule | audit + checker | Change engine-specific Unity Biome decision to wholesale ADOPT | GREEN / RED | Upstream projects evolve; refresh is required before actual adoption/H1 |
| External component cannot become semantic authority | Product architecture + dependency policy | Same prohibition is required independently at product and dependency boundaries | architecture checker | Wholesale-adoption and adapter-bypass attacks | GREEN / RED | A future implementation could violate the docs; downstream WPs must prove conformance |
| Foundational proofs reject self-shrinking universes | `FOUNDATIONAL_PROOF_STANDARD.md` + HK01 | Existing binding proof standard supplies independent-universe rule; HK01 repeats the specific command-discovery obligation | architecture checker | Replace HK01 independent universe with discovery-registry-only universe | GREEN / RED | HK01 must implement the actual effective/independent oracle |
| H0 dependency route/HK01/HK07 are reconciled | ROADMAP + HK00A + HK01 + HK07 + product architecture | Finite named DAG edges/outcomes are checked directly | architecture checker | Any deletion/change of required route text turns checker red | GREEN | Future roadmap edits must update all affected contracts together |
| Adapters cannot mutate canonical state outside transaction pipeline | Product architecture + dependency policy + HK07 self-attack contract | Prohibition exists at architecture and dependency boundaries and HK07 must attack hidden adapter mutation | architecture checker | Allow transport/engine adapters to mutate canonical state directly | GREEN / RED | Transaction implementation semantics belong to HK04/HK07, not this WP |

## Proof-budget rationale

The WP adds one small consistency oracle and one six-case causal attack script. They directly map to the six mandatory self-attacks and DAG reconciliation criterion. No runtime product machinery, syntax-denylist expansion or recursive proof of trusted infrastructure was added. This is proportionate to a document/architecture workpack and therefore `WITHIN_BUDGET`.
