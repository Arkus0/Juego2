# WP-HK-00A — Worker Pre-Review

WORKER_PRE_REVIEW: CLEAN  
WORKER_PRE_REVIEW_FINDINGS_FIXED: 0  
WORKER_PRE_REVIEW_EVIDENCE: `Docs/evidence/WP-HK-00A/WORKER_PRE_REVIEW.md`

## Repair-cycle context

Baseline: `95aa0080a5a4ddd55bd02aa29fcad7aa34252c21`  
Previously failed frozen candidate: `8b121545a07782dbab2475bf4fca24b735e35ae6`  
Repair candidate before this final evidence record: `762fa49bb297699d27fac0cf21577ad407371488`  
Implementation PR: `#15`  
Fail cycle: `1`

The first independent Reviewer found a material architecture ambiguity: the accepted documents prohibited engine implementation types in H0 and prohibited adapter-owned semantic registries, but did not define the valid Arkus-owned path by which a legitimate later engine-scoped public capability becomes canonical/discoverable. This repair Worker independently reconstructed the same contradiction from the product/HK01/HK07/H1 boundaries before changing the candidate.

The repair defines one third path: H0 owns an engine-neutral canonical contract/composition model and base set; a later reviewed engine package may contribute scoped canonical definitions plus implementation bindings through an Arkus-owned composer; only the resulting composed canonical inventory is public/discoverable/projectable. The bridge may not expose a parallel registry, and engine implementation classes/types do not enter the canonical kernel/meta-model.

## Scope reviewed

Re-read and challenged:

- `AGENTS.md`;
- `Docs/workpacks/HK/WP-HK-00A.md` acceptance, all eight required self-attacks, forbidden scope and DoD;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`;
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`;
- complete baseline `95aa0080...` → repair candidate `762fa49b...` diff, not only the last repair commit;
- first independent FAIL on frozen SHA `8b121545...` and its minimal correction boundary;
- `PRODUCT_ARCHITECTURE.md`, `DEPENDENCY_IP_POLICY.md`, `EXTERNAL_HARNESS_ADOPTION_AUDIT.md`;
- H0/H1 route in `Docs/ROADMAP.md`, plus `WP-HK-01.md` and `WP-HK-07.md`;
- architecture checker, exact-SHA routing/entrypoints and all eight causal attack mutations;
- foundational proof matrix and residual-risk/proof-budget classification;
- Draft Actions run `35430463898` at exact SHA `762fa49bb297699d27fac0cf21577ad407371488`.

## Falsification attempts

1. **Engine-scoped capability has nowhere valid to live.** Challenged the repaired architecture by requiring all three properties simultaneously: H0/base remains engine-neutral; a legitimate engine-scoped public capability can exist; and discovery/schema authority remains singular. The new scoped-composition contract supplies the missing path without choosing concrete Unity APIs.
2. **Bridge becomes semantic authority through “contribution”.** Checked that provider source material cannot itself be projected publicly: a contribution is public only after the Arkus-owned composer accepts it into the single composed canonical inventory. The `engine-bridge-parallel-registry` mutation turns the oracle RED.
3. **Engine scope is solved by contaminating H0.** Checked that the H0/base capability set and contract meta-model remain engine-neutral while later scoped definitions may use portable namespaced domain data/references rather than runtime/editor implementation types. The original `unity-leak-into-h0-contract` and new `engine-scope-forced-into-h0-base` mutations both turn RED.
4. **Transport creates the second registry later.** Checked HK07 is now generic over the composed canonical inventory rather than a fixed/base-only list. Its future self-attack contract covers both omission of a scoped canonical capability and publication of an adapter-only capability never accepted by canonical composition.
5. **Scoped execution escapes policy/transaction/provenance.** Checked that every scoped public invocation still enters the canonical dispatch/policy envelope; canonical-state mutation still uses the authoring transaction pipeline; engine-only side effects must declare side-effect/rollback-or-irreversibility semantics and canonical provenance/evidence rather than becoming an untracked editor side door.
6. **One-source rule quietly becomes “many provider truths”.** Checked that scoped provider definitions are inputs to one Arkus-owned composition system, not independently discoverable registries. Generated SDK/Creator/transport surfaces consume the composed inventory, so a provider manifest cannot become a peer public authority.
7. **Composition path is only architectural prose and disappears from downstream contracts.** Checked the same ownership rule is bound into HK01 implementation/acceptance/self-attacks, HK07 projection/conformance, ROADMAP H1 instantiation rules and HK00A DoD. The architecture checker requires all of those surfaces together.
8. **Completeness can self-shrink around scoped extensions.** HK01 still requires an independently/effectively enumerable dispatcher/discovery/schema universe, now explicitly across the composed inventory. A provider/adapter route cannot make both itself and its proof obligation disappear by omitting discovery metadata.
9. **Existing six defect classes regress during repair.** Re-ran the full eight-case suite, retaining MCP authority, H0 type leakage, wholesale harness adoption, unknown-license acceptance, circular discovery completeness and adapter transaction bypass controls. Draft exact-SHA observation is GREEN.
10. **Proof-support machinery is growing faster than product clarity.** The repair adds one architecture rule, reconciles the two affected future contracts/roadmap and adds exactly two causal controls corresponding to the Reviewer's defect alternatives. No concrete Unity API, runtime feature, generic policy parser or extra proof framework was introduced. `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` remains justified.

## Findings

No new in-claim blocker was found during this Worker pre-review (`WORKER_PRE_REVIEW_FINDINGS_FIXED: 0`). The Reviewer-originated blocking class that triggered repair cycle 1 is fixed and regression-protected by attacks 7 and 8 in addition to the existing engine-neutrality control.

The remaining risks in `RESIDUAL_RISK.md` are downstream implementation choices: exact composer API/serialization belongs to HK01; concrete Unity abstractions/capability set belong to blocked H1; exact transaction/runtime realization belongs to later foundational WPs. None requires reopening the ownership boundary in HK00A.

## Validation observed before final evidence record

Draft Actions run `35430463898` checked out exact SHA `762fa49bb297699d27fac0cf21577ad407371488` and completed `Run canonical observation` successfully. `scripts/hk00a-observe-exact-sha.sh` runs both:

```bash
bash scripts/hk00a-architecture-check.sh
bash scripts/self-attacks/run-hk00a-architecture-attacks.sh
```

so that green run covers architecture consistency plus all eight causal negative controls on the exact repair candidate before this evidence-only commit.

## Freeze readiness

This file is the final Worker pre-review evidence mutation. No further implementation/evidence byte may change before freeze. After this commit receives green Draft candidate observation, bind its exact 40-character HEAD as `Candidate HEAD SHA` and `Frozen candidate SHA`, set `Worker state: FROZEN_FOR_REVIEW` / `Branch frozen: YES`, mark the PR Ready, and require Automation V2 frozen exact-SHA verification before starting a fresh independent Reviewer.
