# H2 future planning signal — external consumer boundary

Status: **NON-BINDING PLANNING INPUT / NOT AN ACCEPTED CONTRACT**

Date recorded: 2026-09-21

## Purpose

Preserve one future planning question so it is not lost before H2 is formally designed.

This file does **not** define H2 scope, workpacks, gate names, release mechanics, package layout, compatibility policy or a successor repository. Those remain owned by the future reviewed H2 planning work.

## Signal for the future H2 planner

When H2 is planned, explicitly evaluate whether its exit conditions should prove that Arkus is publishable/versioned and consumable from a fresh external repository without requiring that consumer to inherit Juego2 implementation history as working context.

The future H2 plan should decide, rather than assume, what evidence is required for that boundary. Candidate questions include:

- what constitutes a releasable/versioned Arkus artifact;
- which public contracts/schemas/compatibility promises cross the repository boundary;
- whether a fresh external consumer repository can discover and use Arkus through public surfaces only;
- what compatibility/breaking-change policy is required before that claim is made;
- how a consumer can surface requirements or defects back into Arkus without patching around the harness contract;
- whether an external-consumer trial belongs in the final H2 gate or another causally correct H2 workpack.

The intended architectural direction is that a future game repository should consume Arkus as a product boundary, not carry the full H0/H1/H2 proving-ground history merely to use it. The future H2 planner must validate or reject that direction using the accepted state that exists at planning time.

## Proposed DW evidence interlock

The separate DW planning PR proposes a bounded second-consumer validation track using CITY and PA data through generic Arkus surfaces. If that DW plan is independently accepted and `WP-DW-GATE` later PASSes, final H2 public/external-boundary acceptance must explicitly consume or disposition the DW-GATE evidence rather than freezing the product boundary as if the second-consumer trial did not exist.

This does **not** make CTX obsolete or turn DW into an H2 implementation prerequisite. CTX remains the process-context baseline; H1 remains the Unity-readiness owner; DW, if accepted, supplies additional evidence about non-runtime consumers and structured retrieval. H2 planning may begin earlier, but any final boundary claim must reconcile the accepted evidence available at that time.

## Explicit non-claims

This note does **not** assert that:

- the successor repository will be named `Juego3`;
- H2 must end at any particular WP or gate name;
- specific packages, registries, installers or release channels are already chosen;
- Juego2 history may be discarded before the required H1/H2/CITY/PA proof is complete;
- the proposed DW track is accepted merely because this planning signal mentions it;
- DW proves fresh-repository packaging/versioning/distribution;
- current CTX, H1, CITY or PA scope changes because this note exists.

It is a durable planning signal only. Any binding adoption requires its own reviewed H2 plan/workpack.
