# NPC depth tiers — planning and authoring rule

Status: **ACCEPTED product direction** via PR `#257`, Reviewer PASS `#5333313032`, merge `6bf2d6e74215be51e73d85f659a2a734752ea34c`. Counts are orientation for the final city, not H2, slice, performance or recruitment gates.

| Tier | Typical final planning range | Gameplay obligation | Persistent cost |
|---|---:|---|---|
| A — MAIN / SYSTEMIC | ~10–15 named leads | Multi-state character, authored history, relevant relationships/knowledge/memory, readable routine and consequences across scenes/days | Stable Arkus identity and reviewed durable facts; save/revisit validation |
| B — INTERACTIVE / REACTIVE | ~20–40 named/recurring characters | Shop, work, investigation, contextual dialogue and useful local routine; selective memories where a later encounter needs them | Arkus only for named consequential facts; otherwise local presentation |
| C — POPULATION / AMBIENT | Remainder of an initial ~80–120 visible people | Believable occupancy, motion and immediate response with reused variants | Usually GC2/Unity local state; no permanent per-person history required |

The ranges overlap intentionally: the 80–120 total includes A and B. Visible at once, authored identities, unique models and runtime-active actors are different budgets. `GC2-06` measures a small quiet/busy block before setting any streaming or crowd target for multiple districts. A six-character H2 visual benchmark is a first sample, never the shipped city ceiling.

Promote a C/B NPC only when a concrete authored encounter requires remembering, cross-system causality or persistent identity. The promoted actor receives a stable Juego2 identity and a bounded Arkus fact model; visual duplicates never share canonical person identity by accident. A principal NPC can still use GC2 Behavior/Dialogue/Melee for immediate actions. The tier specifies depth of **meaning**, not which Unity component moves them.

Example budget per block: one A witness, two B shop/work/investigation people and several C pedestrians. Record the authored reaction the player can notice on a later visit and its state owner; avoid storing every glance as history. Character design, clothing, animation and density remain subject to ART and player-scale inspection.
