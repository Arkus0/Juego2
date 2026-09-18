# Workpacks

Each workpack is one independently reviewable contract. Work only inside its allowed scope and stop when its Definition of Done is met.

For all `HK-*` workpacks through `WP-HK-GATE`:

- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is binding;
- exact-SHA evidence is required;
- self-attacks must be causal and reverted before freeze;
- a fresh independent Reviewer must PASS before the dependent WP begins.

The harness sequence is intentionally serial until the gate. This prevents parallel implementation from baking unreviewed assumptions into later layers.
