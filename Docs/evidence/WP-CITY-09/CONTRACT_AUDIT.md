# WP-CITY-09 — contract audit

PREDECESSOR_CONTRACT_CHECK: PASS  
WORKER_PRE_REVIEW: CLEAN

Candidate branch: `worker/wp-city-09-play-design`  
Purpose: prove that the rescued play-design package is reviewable without silently changing accepted CITY or ART authority.

## 1. Closed PR #198 disposition

PR #198 remains closed and is not reused as the canonical candidate.

The rescue intentionally excludes its ART/SETTING edits and keeps only the CITY play-design problem in a new branch based on current `main`.

## 2. Accepted authority checks

### CITY-01 semantics

Accepted `Docs/production/CITY_MOBILITY_TOPOLOGY.md` defines route costs as planning weights / target hypotheses. They are not measured seconds and are not runtime schedules.

Candidate rule:

```text
CITY-01 planning weight != measured player traversal time != runtime schedule time
```

`CITY_PLAY_DESIGN_PROPOSAL.md` preserves all three labels and does not rename one into another.

### CITY-03 exact-seed semantics

Accepted `Docs/production/CITY_PRODUCT_SEED.md` freezes the bounded seed and represented topology for CITY-04. In particular:

- X1 is the represented permanent Río crossing;
- X5 is the represented low-water Arroyo crossing;
- `E.X5` is a receiving component/stub across X5;
- X2/X3/X4 are not represented as alternate Arroyo crossings inside the retained seed;
- no extra dry connection may be invented to make a route work.

Candidate consequence:

> Closing X5 in the current exact seed disconnects the represented `E.X5` component. That is expected current truth.

The proposal therefore removes the old PR #198 requirement to walk a designed X5 detour in CITY-04.

### CITY-04 bounded-build semantics

Accepted `Docs/workpacks/CITY/WP-CITY-04.md` says CITY-04 builds only the bounded CITY-03 greybox and routes semantic defects back to their causal owner.

Candidate consequence:

- current-seed observations may inspect orientation, readability, traversal time and seam quality;
- proposed paseo/stairs/passage/drop/bolera geometry is **not** a current CITY-04 requirement;
- any future traversal check for that geometry is post-amendment only.

## 3. Structural proposal ownership

| Candidate item | Cannot become truth until |
|---|---|
| stronger F02 civic landmark | CITY-05, and CITY-03/06 only if their commitments materially change |
| represented paseo segment | CITY-01 + CITY-03 amendment |
| market stairs | CITY-01 + CITY-03 amendment; CITY-05 if grammar needs extension |
| covered passage | CITY-01 + CITY-03; CITY-05 for physical assembly if needed |
| one-way drop/gate | CITY-01 state/direction/access + CITY-03 geometry |
| Bolera del Puente | CITY-02 place + CITY-03 site, plus CITY-05 only if needed |
| Cuesta mirador | CITY-03 site/geometry; CITY-02 only if promoted to programmed place |
| quantified villa-density thresholds | causal CITY-03/05 owners before any CITY-04 acceptance use |

The proposal itself applies none of those amendments.

## 4. Crossing audit

The candidate:

- proposes zero current new crossings;
- preserves CITY-00 landmass truth;
- does not create a dry Wedge→Puerto continuation;
- does not create Ensanche↔Orilla-sur connectivity;
- does not count a visual line, seam payoff or soft-envelope continuation as a graph edge.

## 5. Runtime-semantics audit

The candidate does not define:

- NPC schedules;
- dialogue;
- beliefs;
- quests;
- combat;
- live events;
- AI route choice;
- time compression;
- save-state consequences.

Stage types are spatial test vocabulary only.

## 6. ART authority audit

Compared with closed PR #198, this candidate deliberately excludes:

- `Docs/art/SETTING.md` edits;
- `Docs/art/VISUAL_BIBLE.md` edits;
- new art-reference folders or anti-reference rules;
- alt-Liébana visual direction;
- reserved colours;
- Quaternius adaptation direction.

No ART file is changed in this candidate.

## 7. Reviewer falsification targets

An independent Reviewer should FAIL this candidate if any of the following is found:

1. A sentence treats a proposed route/site as currently accepted topology.
2. A current CITY-04 check requires geometry outside the exact accepted seed.
3. X5 closure is presented as having an in-seed alternate route today.
4. CITY-01 route-cost weights are presented as NPC schedule truth or physical measurements.
5. A play-design proposal implicitly modifies ART authority.
6. A proposed structural change lacks a causal owner.
7. An illustrative numerical threshold is presented as measured evidence.

## 8. Expected review result if the audit holds

A PASS on WP-CITY-09 would mean only:

> The amendment package is internally bounded, ownership-safe and suitable as reviewed input for later owner-specific amendments.

It would **not** mean that P1–P7 are accepted CITY topology, that CITY-04 has new required runs, or that any ART direction from PR #198 has been adopted.
