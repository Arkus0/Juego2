# WP-H2F-01 predecessor contract check

PREDECESSOR_CONTRACT_CHECK

Date: 2026-09-27. Worker start `main`: `a47f879c46abce88ee23d5959736e32ba5bd62be`; updated onto `main` `2aa179b2eebd99d6d25152e87a105f3791b59408` (H2F-00 DocSync). Worker: Claude (owner-invoked Worker session).

## Accepted predecessors and identities

| Predecessor | Accepted identity | Status used here |
|---|---|---|
| `WP-H2F-00` capability survey | PR #251; reviewed candidate `8600c43da8cd7ef0cf7776845de3ee53593134d7`; independent PASS review `#5330291941`; merge `a47f879c46abce88ee23d5959736e32ba5bd62be` | Direct dependency, accepted. Its DocSync PR #253 merged during this cycle; the branch was updated onto `main` `2aa179b2eebd99d6d25152e87a105f3791b59408` before evidence closure. |
| H2F plan | PR #250; candidate `d128aef60c951f3b19daae1d57cddae55c2519ac`; PASS `#5330069733`; merge `9d3bb90ebfae8837155576c13b705fc1c72c2579` | Phase contract, disposition vocabulary, authority rules. |
| `WP-H1-GATE` | PR #238; candidate `8bcf171f2520a0e23ccba094bf353c8a78b5d239`; PASS `#5329808171`; merge `b730ba0c0b1a4ad454edbd420cd8e44555dfaf10`; `Docs/evidence/WP-H1-GATE/DOCSYNC.md` | Accepted Unity bridge boundary (6000.3.24f1, H1 Source slice, projection/lifecycle authority). |
| ART-01 | PR #234, **Draft, not PASS**; structural checkpoint consumed at `174d05d23c3bceb9d5df00e460b33519cf68328e` | Consumed only as a pre-foundation input source (see the sequencing note below). No ART-01 readiness or PASS is claimed or assumed. |

No context capsule covers H2F-00 or the H2F plan; the exact documents were read (`Docs/workpacks/H2F/**`, `Docs/evidence/WP-H2F-00/**`, H1-GATE DocSync).

## ART-01 sequencing: open freeze precondition

The `WP-H2F-01.md` contract on `main` names `WP-ART-01` PASS as a dependency "before any spike claims keeper-source fit". The owner has directed a sequencing amendment (branch `process/h2f-before-art01-final`, head `bc77748f3bdb55e4f1a4c8695e012821042571b7`, decision `Docs/decisions/H2F_ART01_SEQUENCE_2026-09-27.md`). It changes that dependency to "ART-01 `PREFOUNDATION_INPUT` checkpoint published; ART-01 PASS is intentionally not a predecessor", because the previous contracts formed a cycle: ART-01's final Unity benchmark needs the render/toolchain baseline that H2F selects.

This Worker executes under the amended wording. Until the amendment is on `main` and ART-01 publishes `PREFOUNDATION_INPUT`, this candidate:

- stays Draft + ACTIVE and is **not** frozen for review;
- makes no keeper-source-fit, `KEEPER_READY` or ART readiness claim;
- consumes only ART-01's renderer-independent structural checkpoint as a pinned, read-only input.

If the amendment is rejected or changed, this check and the affected spike conclusions are revised before freeze.

## Inherited guarantees consumed, not re-proved

- **H2F-00:** 27-category capability closure and candidate register; G1–G7 gap map; acquisition map; the bounded S01–S07 queue, each with a decision discriminator; S08 belongs to H2F-03; owner cost constraint (no paid default, owner approves any purchase).
- **H1 / H1-GATE:** Unity 6000.3.24f1 editor identity; pinned Quaternius Medieval Village MegaKit Source (URP archive SHA-256 `b9d757dd…8b10`) and UAL1 Source identities and license record; Arkus canonical authority and H1 projection/lifecycle boundary; Unity objects are bridge locators, not canonical identity.
- **ART (accepted ART-00 direction + ART-01 structural input):** visual bible, assembly/dimensional grammar and the no-silent-proxy rule; ART-01's selected-source lock (Medieval, UAL1, Nature, Props, Base Characters; all CC0 per bundled license bytes) and its structural benchmark route with CITY-04 width classes (X1 5.5, W12 2.8, micro-route B 2.4). These are consumed as representative inputs, not re-audited.
- **CITY:** spatial/topological authority. Spikes reuse ART-01's local specimen stations and never author or mutate keeper CITY geometry.

## Newly owned by H2F-01

- Real Unity spikes for each retained H2F-00 `SPIKE_REQUIRED` family (S01–S07) on representative Juego2/Quaternius inputs in an isolated, disposable, non-keeper spike project.
- Final `ADOPT_NOW / AVAILABLE_ASSET / DEFER / REDUNDANT / REJECT / NO_ADOPT` disposition matrix and one recommended baseline intent for H2F-02.
- Owner-visible visual evidence and recorded owner judgement for appearance/feel discriminators.
- Separation of manual-acquisition blockers from technical failures; the spike ledger and residue isolation; dependency-policy questions handed to H2F-02.

## Explicit non-claims

No package, dependency or render pipeline becomes project baseline (H2F-02 owns that). No `Unity/ArkusUnity` mutation. No exact-version license admission. No H1 lifecycle classification of selected state (H2F-02) or composed lifecycle/authoring proof (H2F-03/S08). No ART-01 readiness, no CITY-07 keeper content.

## Reopen conditions

- A spike shows an accepted H1 Source identity/hash, or the effective H1 import slice, contradicts H1-04/H1-11 evidence. → Reopen the H1 Source boundary.
- The ART-01 structural input cannot be reproduced byte- or digest-identically from its pinned sources. → Stop consuming it; escalate to ART-01.
- A candidate can only be realized by making plugin-private state canonical or by mutating CITY topology. → This is a REJECT for the candidate, not a reason to reopen CITY/H0.
- The sequencing amendment is not accepted. → Revisit the ART-01 dependency before freeze.
