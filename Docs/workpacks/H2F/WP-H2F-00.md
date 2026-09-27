# WP-H2F-00 — Capability ecosystem survey + candidate register

Status: **PROPOSED / NOT_STARTED**  
Class: `RESEARCH_BATCH`  
Mode: `REMOTE_OK`  
Depends on: accepted H2F plan  
May overlap: late ART-01 execution as research-only work  
Blocks: `WP-H2F-01`

## Claim

The project has performed an exhaustive-by-capability survey of the tools, packages, content sources and production helpers that could materially change how Juego2 keeper content is built before CITY-07/H2 production commits to those choices.

This WP closes discovery, not adoption.

## PASS-before-work acceptance contract

- **Positive claim:** every mandatory H2F capability category has a documented native/current baseline, serious alternatives where they could change the decision, and an explicit preliminary disposition or bounded spike requirement.
- **Mandatory positive evidence:** (1) closed category checklist; (2) candidate register; (3) gap/redundancy map including already-owned Quaternius Source capabilities; (4) manual-account/acquisition map; (5) bounded `SPIKE_REQUIRED` queue with decision questions; (6) research provenance for current software/license/compatibility observations.
- **Negative gates:** FAIL if known categories are silently omitted; if the register only lists tools already familiar to the team; if every candidate is marked “try later”; if Asset Store marketing copy is treated as compatibility proof; if a provisional license observation is treated as exact-version adoption approval; or if research mutates the Unity project.
- **Non-claims:** no package is adopted, no Unity compatibility is proven, no aesthetic fit is accepted, and no keeper content is authorized by this WP.
- **Allowed residuals:** the bounded spikes explicitly handed to H2F-01; candidates too weak to merit a spike may be rejected/deferred from research evidence.
- **Evaluation method:** research argument + source/provenance review + completeness review against the H2F mandatory capability matrix.
- **Consumed predecessors:** accepted visual/product requirements from ART/CITY/H2 and the binding dependency/IP policy. This WP does not reopen their semantics.

## Required work

1. Expand every mandatory capability category from `H2F/README.md` into concrete production questions for Juego2.
2. Record the Unity-native/URP answer where one exists.
3. Inspect already-owned Quaternius Source projects and content families before proposing redundant tools.
4. Survey credible free/open-source/content-library alternatives.
5. Survey credible commercial alternatives only when they plausibly offer enough quality/authoring-cost benefit to change the decision.
6. Record options that are consciously unnecessary as `NO_ADOPT` rather than silently omitting the category.
7. Build a candidate register using the fields required by the H2F plan.
8. Identify account-gated/manual acquisitions separately from repository-installable packages.
9. Produce the smallest spike queue that can resolve the real decision uncertainty.

## Mandatory coverage interpretation

“Exhaustive” means no materially relevant **capability class** is skipped, not that the Worker enumerates every marketplace product. For a crowded market, the Worker may stop after covering the native option plus representative leading/free/commercial alternatives and documenting why additional near-duplicates cannot change the decision.

When a candidate is obsolete, unsupported, licensing-ambiguous, visually mismatched or redundant, record that fact and stop. Research completeness does not require wasting Unity time on weak candidates.

## Required output matrix

Each material candidate records at least:

`candidate | capability | source/provider | native/free/open/paid/content | observed version/commit | license/EULA observation | Unity 6.3/URP observation | runtime/editor/content role | source/repro implications | visual fit | AI automation fit | manual acquisition | authority risk | integration/replacement risk | overlap | spike? | preliminary disposition | rationale`

The final exact adopted version/license is deliberately left to H2F-02 after H2F-01 selects the stack.

## PASS only if all are true

- every H2F mandatory category is closed with candidates or an explicit `NO_ADOPT`/out-of-scope decision;
- native Unity/URP capabilities are not ignored in favor of marketplace packages;
- Quaternius Source capabilities are inspected before adopting duplicate external solutions;
- free/open and commercially meaningful alternatives are represented where they could change a decision;
- the spike queue contains a concrete question and pass/fail discriminator for each retained `SPIKE_REQUIRED` item;
- current observations are source-backed but not misrepresented as adoption approval;
- no Unity/project mutation occurred as part of survey acceptance;
- no third-party tool is proposed as canonical semantic authority.

## Negative gates

The acceptance-contract negative gates are binding. In particular, a long list of links is FAIL if it does not end in actionable dispositions and a bounded decision queue.
