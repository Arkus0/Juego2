# WP-HK-07A representative content-shape probe

## Approved bounded slice

The probe uses the accepted fictional Potes/Liébana authored shape: one plaza, one contained market building, one NPC with an authored `works.at` reference, and one future-owned opaque social extension whose dependency points to the market. These are accepted H0 object, containment, reference and extension concepts; the probe does not introduce a gameplay schema.

## Fresh-process scenario

`Hk07AExternalClientTests.FreshExternalClientDiscoversAndExercisesAcceptedH0SurfaceEndToEnd` is an external-client test: it links to no product assembly and communicates only with compiled Release processes using `arkus.reference.jsonl@1` frames.

The client:

1. starts a fresh source host and calls `system.describe`;
2. confirms discovery of representative read, validation, mutation, provenance, diff, snapshot and replay capabilities;
3. reads the empty summary, current validation and base snapshot;
4. plans and then applies one canonical four-operation mutation creating the plaza, market, NPC/reference and extension/dependency;
5. reads the NPC and accepted mutation journal;
6. exports the final snapshot and proves it differs semantically from the base;
7. starts a second fresh host;
8. imports the source base snapshot, then replays the source journal;
9. exports the reconstructed snapshot and compares it with the source final snapshot;
10. requires identical final hash, `sameAuthorableState=true`, and zero semantic changes.

## Result

Remote implementation/test/evidence SHA `acd112665db0fec32e238ed847ae18702cf02320` passed from a clean fetched worktree:

- real source and target process launch/clean EOF;
- public discovery before use;
- current validation valid;
- plan non-persisting and apply persisting;
- one truthful journal entry;
- non-empty base→final semantic diff;
- snapshot establishment in a second process;
- one-entry replay;
- identical final canonical hash;
- empty final→replayed semantic diff.

## Classification

- Object identity, containment, references, opaque extensions and canonical validation are **consumed predecessor semantics**.
- Generic discovery and transport of their portable request/result artifacts are an **HK07A in-scope claim** and passed.
- Fresh-process snapshot/journal transfer through public JSON frames is an **HK07A external-client DoD claim**; snapshot/replay interpretation itself remains inherited from HK06B/HK06C.
- Schedules, transforms, runtime clocks, AI, physics, animation and Unity scene state are **future/out-of-boundary observations**, not missing HK07A schema.
- Large-world pagination/compact workflows, host capability policy and operational resource limits remain **HK08/HK09-owned**.

No content-shape mismatch or predecessor reopen condition was observed.
