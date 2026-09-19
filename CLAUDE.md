# Claude Code — Juego2

Read `AGENTS.md` first.

Juego2 is harness-first. H0 (`HK-*`) builds and proves the AI-native authoring kernel before Unity/gameplay.

There is no automation bootstrap. The human invokes Worker, Reviewer and finalization/documentation roles manually.

For any implementation task:

1. reconstruct current `main`, open PRs and active ownership;
2. read the exact WP + dependencies;
3. follow `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`;
4. if foundational, bind `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`;
5. preserve exact-SHA validation evidence under `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md`;
6. stop at the next role boundary.

Do not import architecture/code from `Arkus0/Juego` unless an explicit Juego2 contract authorizes it. Process lessons may be reused; technical baggage is not inherited.

A session that acted as Worker or directed implementation of a candidate must not act as its independent Reviewer.

Cloud execution must never fabricate later local/editor evidence. GitHub Actions and repository-triggered role transitions are not part of Juego2's operating model.
