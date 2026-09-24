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

`.h1-03a-effective/` is validation scratch state only. It is ignored by Git so GameCI semantic-version probing cannot mistake evidence files for product drift.
