# WP-ART-01 Worker plan — PRODUCT_CHECKPOINT

Baseline `main`: `648272bb50a0c5c718a9f0f4668faaaa0482aef9`. Canonical branch: `codex/wp-art-01`, PR #234. Predecessor ownership was reconstructed **before implementation** in `PREDECESSOR_CONTRACT_CHECK.md`, and refreshed by the receiving Worker. PR #233 is frozen non-canonical diagnostic evidence; no file under `Unity/PrototypeDemo` is edited or imported as a scene/seed.

## Worker transfer

- Worker history: Codex → Claude. Transfer SHA `290c487d089bc596117cef43be7f02a1d61f7002` (Codex's last pushed commit), recorded in the PR body before any new commit.
- Directed by the owner on 2026-09-26: "Te encargas tú, codex fuera".
- No `WORKER_PRE_REVIEW: CLEAN` existed. `fail_cycle` is unchanged (0).
- Codex's uncommitted local attempt to move the benchmark into a separate `Unity/Art01URP` project (URP 17.3.0) is **not adopted**, because it anticipated the renderer change before H1-GATE. It exists only in Codex's local checkout `C:\Juego2-ART01`.

## Claim and evaluation (unchanged)

The final candidate must supply a bounded, provenance-safe Juego2 visual vocabulary and assembly grammar. It must be demonstrated in an actual Unity third-person building/street/threshold benchmark on the retained chain. Every mandatory benchmark element must be `KEEPER_READY`.

Positive evidence:
- the target sheet;
- demand disposition;
- measured source/derivative kit metadata;
- the assembled scene and three meaningful checkpoints;
- normal/neutral human-scale captures;
- a fresh-author assembly revision.

Negative gates:
- decorated cuboids, sticker openings, a floating roof;
- unresolved contact, duplicated collision;
- an unapproved source, raw unclothed scale mannequins;
- silent `PROXY_VISUAL`/`COVERAGE_BLOCKED` success.

Not claimed: CITY topology, H1 semantics, an H2 compiler, NPC behavior, or a whole-town keeper.

## Owner execution decision — production renderer

The owner decided on 2026-09-26 that **URP is the intended Juego2 production renderer**. The initial reference is the non-canonical PR #233 prototype on URP 17.3.0, which is not modified or migrated. This does **not** change the ART-01 claim or its PASS criteria.

While `WP-H1-11` / `WP-H1-GATE` are closing `Unity/ArkusUnity`, ART-01 must not change the ArkusUnity global render pipeline, or introduce URP in any way that could interfere with the H1 proof. The work is therefore split:

- **STRUCTURAL (now, renderer-independent).** Kit IDs/provenance; dimensions/pivots/orientation; host/connection metadata; building and street assembly plans; benchmark geometry; openings, roof, plinth, threshold and ground contact; neutral-material inspection.
- **Not frozen yet.** Final materials, lighting, atmosphere, dressed visual captures and the final visual judgement of the candidate. The built-in Standard materials, the overcast built-in light rig and the `provisional_builtin/` captures are **provisional** iteration aids only.

## Checkpoints

1. **Target / baseline — complete (Codex, `e69cbfc`).**
   - Source reality check and demand matrix; ART-00/H1-04/CITY accepted inputs; selected-source pin and license adoption.
   - Effective raw/normalized Unity import audit; three frozen human-scale targets.
   - Unity `6000.3.24f1`, `ART01_SOURCE_AUDIT_GREEN rows=32`.
2. **STRUCTURAL — complete (Claude, see `STRUCTURAL_CHECKPOINT.md`).**
   - `Art01StructuralAudit` GREEN: 2009 renderer-independent checks, 0 failures, including the accepted CITY-04 route-width oracle added after pre-review #5325775001 (causally demonstrated by a W12-narrowing negative run).
   - Deterministic structural digest `a7f8534f…`; manifest `@2` with kit-ID closure.
   - Neutral captures in `structural_checkpoint/`.
   - The same audit measured 569 failures on the `290c487` geometry (`UNITY_STRUCTURAL_AUDIT_BASELINE_290c487.json`). They are fixed here rather than tolerated. This supersedes `MASSING_CHECKPOINT.md`, which is retained as history.
3. **CANDIDATE — blocked until `WP-H1-GATE` has passed and its result is on `main`.** In this order:
   1. Update/rebase `codex/wp-art-01` onto that `main`. Refresh the predecessor check; confirm H1 tests still pass with `Assets/Arkus/ART` present.
   2. Adopt URP for the production benchmark:
      - fix the URP package version compatible with the accepted Unity editor;
      - create and version the pipeline/renderer assets;
      - convert materials reproducibly (a scripted remap replacing `Art01Materials`' Standard path, fail-closed on unknown source materials);
      - resolve every Standard→URP / magenta case explicitly;
      - verify vegetation cutout, transparency (glass/water) and real source materials.
   3. Establish a lighting/atmosphere baseline appropriate to the Juego2 target: overcast wet valley, fog, no sunny-Mediterranean read.
   4. Re-run `Art01StructuralAudit`. Geometry must remain GREEN with the same digest, or record why it changed. Repeat any structural capture whose reading changed materially.
   5. Produce the dressed third-person captures under URP.
   6. Run the fresh-author smoke test under the final pipeline, with a fresh author who consumes the manifest and assembly plans.
   7. Update the gap/handoff ledger, run the strict Worker pre-review, and freeze only when every mandatory element is `KEEPER_READY` and product visual inspection is credible.

No `REVIEW_READY` or frozen candidate may be claimed before the final visual evidence has been obtained with URP.

Iteration between checkpoints is local and cheap under `PRODUCT_EXECUTION_POLICY.md`. Any CITY route/site/access/elevation semantic change stops and returns to its owner.
