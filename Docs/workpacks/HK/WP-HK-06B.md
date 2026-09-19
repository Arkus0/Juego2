# WP-HK-06B — Semantic diff + canonical snapshot portability

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Completion metadata

- implementation PR: `#35`;
- baseline SHA: `5281160ce4eff601ab52dfb568fd4e4976560383`;
- reviewed frozen candidate: `2b05e982c96e7ece08cca999075c183e75eee2fd`;
- independent Reviewer verdict: `PASS` (review `#5258130916`);
- exact-SHA validation: GREEN, Actions `35473614054`, artifact `10594111979`;
- implementation merge SHA: `28e2d0aadf63fe322eac636955e9223dc9249328`;
- historical failed candidate: `bebc1b6165f7228f33dc593534052cd80eb6e041` (review `#5258024978`), repaired causally at the classification ↔ authority ↔ history/evidence seam.

Accepted semantics: semantic authored-state diff over the complete current `WorldState` resource model; versioned canonical snapshot export/import; fail-closed snapshot validation before replacement; explicit authored/live exclusion; and snapshot import as a distinct `CanonicalRebase` that starts a new local lineage, emits truthful rebase evidence and does not fabricate HK06A mutation history.

## Objective

Make authored-world state comparable and portable by semantic meaning rather than serializer text, without yet taking on journal replay.

## Acceptance

- Semantic diff reports added, removed and changed authorable resources between canonical authored states without relying on raw serializer text diff.
- Diff identity and ordering are deterministic and use the accepted canonical identities, including accepted extension identity/dependency semantics.
- Material authorable changes cannot disappear merely because their serialized representation is reordered or reformatted.
- Canonical snapshot export is machine-readable and represents the complete accepted authored state needed to reconstruct the same canonical hash.
- The **snapshot format/schema and its version identifier are owned by HK06B**. Snapshot import owns format/version validation for the accepted snapshot contract; it does not redefine HK06A journal/provenance versions.
- Snapshot import/export round-trips a representative nontrivial world to the same semantic state and canonical hash in a clean state holder/process boundary appropriate to H0.
- Snapshot import validates format/version and canonical state validity before replacing the current authored state; a rejected import does not partially replace canonical state.
- Snapshot operations preserve the accepted HK06A authored/live boundary: transient runtime observations are not silently captured as authored snapshot state.
- Import/export semantics explicitly state what happens to local provenance/journal history; snapshot import must not fabricate mutation history that did not occur.
- Diff and snapshot surfaces are versioned/discoverable through the canonical contract and consume accepted HK02/HK02A/HK03/HK05 state/inspection/validation guarantees.
- **Accepted HK06A journal truthfulness and authored/live authority boundary remain consumed, not re-proved.** HK06B owns only their integration seam with semantic diff/snapshot portability unless concrete contradictory evidence reopens them.

## Required negative-conformance tests

RED→GREEN for: hidden authored-state change absent from semantic diff, serializer-only reordering creating a false semantic change, altered snapshot data producing a false same-state claim, unsupported snapshot version, invalid snapshot partially replacing current state, transient runtime-only data leaking into canonical snapshot state, and snapshot import fabricating provenance/history.

## Forbidden scope

Journal replay, deterministic scenario simulation, Unity serialization, GUI diff/history browser, cloud persistence, Git integration as source of truth, filesystem/network isolation policy beyond what is strictly needed for the canonical in-memory/file-neutral snapshot contract.

## DoD

Two nontrivial authored states can be semantically compared, and a representative canonical authored world can be exported and reconstructed with the identical canonical hash under an explicit snapshot version contract, without serializer-text coupling, runtime-state leakage or fabricated history; independent PASS.
