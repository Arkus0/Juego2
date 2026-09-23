# Context Discoverability v1

Status: prospective Operational Hardening Batch C process contract.
Authority: navigation/discoverability only; never semantic acceptance authority.

## Core invariant

A source being technically reachable from a capsule is not enough. A Reviewer may rely on compact navigation only when the already-consumed compact context contains an observable reason that can cause the Reviewer to open every authoritative source that may become material to the verdict.

If the only clue that a source is material exists inside that still-unopened source, discoverability is circular and compact navigation is unusable for that boundary.

The safe result is always:

`RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`

This fallback is intentionally allowed to cost tokens. It may not cost review quality.

## Track coverage policy

`Docs/engineering/context-capsule-coverage.json` is checker-owned and classifies every repository workpack track as exactly one of:

- `required`: accepted workpacks are expected to have indexed capsules; a coverage gap is process RED, while Reviewer behavior still falls back to authoritative reconstruction.
- `optional`: a valid/discoverable capsule may be used; missing, invalid or undiscoverable capsule falls back to authoritative reconstruction.
- `none`: capsule navigation is not used for that track; authoritative reconstruction is the normal path.

A repository workpack track with no explicit classification is RED. This prevents a new track from silently inheriting an accidental compact-navigation policy.

DW and H2 are prepared as `optional`. Batch C does not require retroactive DW/H2 capsules and does not reopen accepted DW work.

## Observable cues

For discoverability testing, `authoritative_sources` membership alone is reachability, not a cue. Observable source-opening cues must come from context the role consumes before opening that source, such as:

- an explicit mandatory source read;
- accepted-identity validation for the canonical identity source;
- a guarantee/exclusion source pointer that identifies the source or its canonical semantic label;
- an escalation/reopen rule that identifies the source or its canonical semantic label.

The checker deliberately excludes `authoritative_sources[].path` and `.kind` themselves when deciding whether a clue exists, otherwise reachability would self-confirm discoverability.

## Semantic-content boundary

Automation may derive identities, paths, fingerprints, coverage classes and mechanical discoverability relationships. It must not generate or infer `exported_guarantees`, `exclusions_nonclaims`, `reopen_conditions` or semantic escalation prose. Those remain reviewed human-authored contract content.

## Adoption

Batch C adds guarantees prospectively. It does not reopen CTX-02, CTX-03 or accepted DW work unless a new concrete causal reproduction independently contradicts an accepted guarantee.
