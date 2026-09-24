# H2 future planning signal — external consumer boundary

Status: **NON-BINDING H2 PLANNING INPUT / DW-GATE INTERLOCK ACCEPTED**

Date recorded: 2026-09-21  
DW interlock accepted: 2026-09-22  
DW-GATE accepted: 2026-09-23

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

## Accepted DW evidence interlock

The DW Design World plan was accepted on frozen candidate `cbb0114bb9ee1248739c093109a9b71e26751444` after independent PASS review `#5274794434`, PR `#117`, merge `b831050e9df8b61b76744e0c5f544bd7ec2d79b5` and post-PASS DocSync.

`WP-DW-GATE` is now also **COMPLETE / ACCEPTED** on exact candidate `6152239062d3b25b91682f98f18a9eccf5e1ca1e`, independent PASS review `#5294645167`, implementation PR `#161`, implementation merge `79586c8176d0f2c05bec6bc53f1e9802095e8e0d`, followed by bounded DocSync in PR `#162`.

Its bounded second-consumer evidence uses CITY and PA through generic Arkus surfaces, preserves a neutral third-shape stress, and produces an H2 handoff while explicitly retaining non-claims for arbitrary-domain universality, general DW adoption, Unity/runtime correctness and fresh external-repository packaging/consumption.

Final H2 public/external-boundary acceptance must explicitly consume accepted `WP-DW-GATE` evidence or review and disposition the DW interlock rather than freezing the product boundary as if the accepted second-consumer trial did not exist.

This does **not** make CTX obsolete or turn DW into an H2 gameplay/implementation prerequisite. CTX remains the process-context baseline; H1 remains the Unity-readiness owner; DW supplies additional evidence about non-runtime consumers and structured retrieval. H2 planning may begin earlier under its existing prerequisites, but any final public/external boundary claim must reconcile the accepted DW interlock.

## CTX↔DW future planning input

A separate planned `WP-CTX-DW-GATE` tests whether CTX can act as the control/routing plane while DW remains a selective structured knowledge plane, preserving authoritative-source escalation and correct DW abstention.

If that gate is later accepted, future H2 planning should explicitly distinguish:

1. **product portability** — versioned/public Arkus is consumable from a fresh external repository;
2. **knowledge/context portability** — a fresh Worker/Reviewer in that repository can discover and use the material knowledge it needs without inheriting Juego2 implementation history as working context.

Passing either claim does not imply the other. The detailed non-binding planning input is `Docs/workpacks/H2/CTX_DW_PLANNING_INPUT.md`.

## Explicit non-claims

This note does **not** assert that:

- the successor repository will be named `Juego3`;
- H2 must end at any particular WP or gate name;
- specific packages, registries, installers or release channels are already chosen;
- Juego2 history may be discarded before the required H1/H2/CITY/PA proof is complete;
- accepted DW-GATE proves fresh-repository packaging/versioning/distribution;
- accepted DW-GATE authorizes general DW adoption;
- planned CTX↔DW composition has passed before independent review;
- current H1, CITY or PA product scope changes because this note exists.

It remains a planning signal rather than an H2 implementation contract. The binding cross-track milestone consequence remains the conditional interlock recorded in `Docs/ROADMAP.md`; any concrete H2 scope still requires its own reviewed H2 plan/workpack.
