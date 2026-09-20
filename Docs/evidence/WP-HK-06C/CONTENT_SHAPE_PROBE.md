# WP-HK-06C representative content-shape probe

## Approved bounded slice

The probe reuses the same fictional Potes/Liébana H2 authored slice already accepted by HK06B: one plaza, bar, shop, NPC with an authored `works.at` relation, and one future-owned social extension. This is a bounded representability probe, not a new canonical gameplay schema.

## HK06C scenario

`Hk06CContentShapeTests.PotesAuthoredSliceReplaysFromAcceptedSnapshotToIdenticalHashAndEmptySemanticDiff` executes only HK06C-owned boundaries:

1. export the accepted Potes authored base as HK06B `arkus.authoring.snapshot@1`;
2. persist two ordinary HK04/HK06A canonical authored mutations: refresh `building.shop` and update the `future.social` extension payload;
3. read the resulting accepted HK06A journal;
4. establish a clean target lineage by importing the original HK06B base snapshot;
5. replay the journal through `authoring.journal.replay@1.0`;
6. compare original and replayed final states by canonical hash/revision and HK06B semantic diff.

## Result

Implementation/test SHA `0d7e22be092d8291d6f7e239e73ed7e91217bd3a` passed candidate observation run `35488744134`:

- focused `Hk06C*`: 8/8 GREEN;
- full regression: 136/136 GREEN;
- final canonical hash/revision are identical;
- HK06B semantic diff reports `sameAuthorableState=true` with zero changes;
- candidate clean before/after: YES;
- artifact: `10598333769`.

## Assumptions and classification

- Plaza/building/NPC identity, containment/references and extension payload are accepted authored-state concepts inherited from HK02/HK02A/HK06B: **consumed predecessor semantics**.
- The shop refresh and extension update are ordinary authored mutations already representable through HK04/HK06A: **in-scope replay input**.
- Replay preserving those changes from the accepted base is an **HK06C in-scope acceptance claim** and passed.
- Transforms, schedules, AI routines, gameplay clocks, animation, Unity objects and transient runtime state are intentionally absent: **future/out-of-boundary observations**, not blockers.
- No predecessor reopen condition was observed.

The probe therefore found no product-shape mismatch inside HK06C's replay claim.