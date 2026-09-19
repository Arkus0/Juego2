# WP-ART-00 — Minimal visual bible + Quaternius→Cantabria adaptation rules

Status: PLANNED  
Class: PRODUCT / CONTENT (NON-FOUNDATIONAL)  
Depends on: none  
Binding policy: `Docs/engineering/DEPENDENCY_IP_POLICY.md` (asset/pack selection only)

## Objective

Turn the already-agreed art direction into a written selection contract a contributor can apply without asking its author.

`Docs/ROADMAP.md` specifies that H2 will "rebuild the visual target from the visual bible", but no visual bible exists. The direction currently lives only in conversation, and sessions are disposable. This WP produces the minimal durable artifact: a style definition, a draft palette, scale/camera guidance, character rules, an authorized Quaternius pack subset with per-pack vetoes, and adaptation rules that turn generic low-poly kit assets into one coherent Cantabrian coastal-village look.

This is direction, not production. It imports no assets, opens no engine and starts no H2 content.

## Relationship to H0

This WP is outside the H0 kernel track. It:

- does not gate, block, unblock or reorder any `HK-*` workpack;
- is not bound by `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`;
- requires no exact-SHA evidence, self-attack matrix or `PREDECESSOR_CONTRACT_CHECK`;
- may run fully in parallel with H0 work by a different contributor.

If this WP is ever cited as a reason to delay, weaken or reinterpret an `HK-*` acceptance criterion, that citation is invalid.

## Deliverables

1. `Docs/art/VISUAL_BIBLE.md` — the bible itself, containing: style one-liner; explicit Yes / No lists; draft hex palette; indicative scale and camera; character/archetype rules; authorized Quaternius pack table with what is forbidden from each pack; the H2 hero target; and a reusable asset triage procedure.
2. `Docs/art/Refs/` — reference folder structure with a README stating what belongs in each subfolder and what does not. Structure and rules only; downloading images is not required by this WP.
3. Adaptation rules table inside the bible covering at minimum roof, stone, vegetation, clothing and sky, each mapping a Quaternius default to a Cantabria rule, the mechanism (recolor / swap / hide / reject) and a concrete reject condition.
4. Triage dry-run record — the asset triage procedure executed against five named candidate assets, with verdicts, plus a second contributor's independent verdicts on the same five.

## Acceptance

- The bible states one style one-liner that is falsifiable: a reader can name at least three concrete looks it excludes.
- Yes and No lists are explicit and mutually exclusive; every direction decision already taken (low-poly stylized, no photoreal, no Dreamcast/Shenmue as a graphical style, Atlantic-coastal Cantabrian identity, archetype-plus-outfit characters) appears in one of them.
- The palette is given as hex values grouped by material role, with the color space and the pre-lighting/albedo intent stated, and with accent colors bounded by a stated maximum surface share.
- Scale and camera are numeric and marked indicative for H2 rather than a gameplay contract.
- Character rules define identity by archetype, outfit and silhouette, explicitly reject realistic regional physiognomy, and name 6–8 archetypes for the hero target.
- Every authorized Quaternius pack row carries a verification status, what it is authorized for, and at least one concrete thing forbidden from that specific pack. Packs proposed from research rather than first-party verification are marked as hypotheses to be confirmed at quaternius.com.
- Licensing is recorded under `Docs/engineering/DEPENDENCY_IP_POLICY.md`: license claims remain unverified for adoption until read at the exact downloaded version, and the record fields required at adoption time are named.
- The adaptation table covers roof, stone, vegetation, clothing and sky at minimum, and each row's reject condition is decidable by inspection rather than by taste.
- The H2 hero target is bounded and countable: one plaza, one to two streets, one simple interior, 4–8 archetype NPCs, plus an explicit unique-mesh and material-set budget.
- **Independent triage test**: a second contributor, given only the bible, accepts or rejects five named candidate assets without asking the author. At least two verdicts must be rejections, and the five verdicts must match the author's. A disagreement is resolved by tightening the bible, not by verbal clarification.
- No binary asset, pack archive or reference image is committed by this WP.

## Forbidden scope

Unity or any engine implementation; importing Quaternius packs or any binary asset into the repository; modelling assets from scratch; reopening H0 decisions, sequence, gates or exit criteria; proposing photorealism or a "top-tier realistic PS3" target; inventing gameplay systems, NPC schedules or interaction mechanics; treating this WP as authorization to begin H2 content production.

## DoD

`Docs/art/VISUAL_BIBLE.md` and `Docs/art/Refs/README.md` exist and are reviewable by a contributor with no prior context; the independent triage test passed and its record is preserved; an `Open questions` section is present and explicitly empty if there are none; the bible states that it does not block H0; the diff contains no binary files.

## Implementation notes for Worker

One day of documentation and references. No engine, no scene, no build.

1. **(~1 h) Read only what applies.** `Docs/ROADMAP.md` H2 section and `Docs/engineering/DEPENDENCY_IP_POLICY.md`. Do not reconstruct HK contracts: this WP inherits no foundational guarantee and owns none.
2. **(~2 h) Verify the packs.** Open each row of the pack table at quaternius.com. Confirm the pack exists, what it actually contains, and the license line shown at download. Update each row's verification status. A pack that cannot be verified is marked rejected-unverified; it is not left ambiguous.
3. **(~2 h) Close palette and adaptation rules.** A flat color-block sheet (Krita, Figma or a Blender viewport render) is sufficient and optional. No scene and no engine are required.
4. **(~1 h) Populate `Docs/art/Refs/`.** Respect the naming and provenance rules in its README. Anti-references matter as much as positive ones.
5. **(~1 h) Run the triage procedure** against five named candidate assets and record the verdicts. If fewer than two are rejections, the bible is too permissive — tighten the No list and rerun.
6. **(~1 h) Independent dry-run.** Hand the bible alone to a second contributor and compare verdicts. Every mismatch is repaired in the document, not in conversation.

Stop at the end of step 6. Selecting, downloading or importing actual assets belongs to H2, after the Unity parity gate.
