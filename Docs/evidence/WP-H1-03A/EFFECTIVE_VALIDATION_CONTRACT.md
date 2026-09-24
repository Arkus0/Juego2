# WP-H1-03A effective Unity validation contract

The GitHub-hosted effective validation is authoritative only for the exact candidate SHA that it checks out and records in its receipt.

It must exercise the pinned Unity Editor `6000.3.24f1 (4e7b9b5b6244)` through the fixed entry point `Arkus.H1.Editor.H1EditorWorker.Run` and prove:

- two fresh project/profile inspection workers execute on the Unity Editor main thread and return the same normalized read-only result;
- the hierarchy-shaped diagnostic probe traverses the same fixed worker entry without H1-04 assets;
- a wrong launch profile cannot produce a successful accepted result;
- a bounded external timeout cannot produce a false successful result;
- a deliberately corrupted real worker result is falsifiable while the host lifecycle tests reject corrupt/missing/wrong-identity outcomes;
- the real filesystem project lease rejects a second independent same-project holder;
- only the accepted deterministic GameCI package delta may occur, after which the tracked tree is restored exactly to the candidate SHA.

The frozen-candidate verifier is registered as `scripts/h1-03a-verify-exact-sha.sh`. It re-runs the locked restore, Release build and focused remote lifecycle suite at the frozen SHA; the effective Unity claim remains separate exact-SHA evidence from this workflow and is not simulated or duplicated by the generic freeze verifier.

`.h1-03a-effective/` is validation scratch state only. It is ignored by Git so GameCI semantic-version probing cannot mistake evidence files for product drift.
