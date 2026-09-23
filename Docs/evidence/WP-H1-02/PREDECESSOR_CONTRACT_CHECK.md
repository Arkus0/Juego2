# WP-H1-02 — PREDECESSOR_CONTRACT_CHECK

Status: COMPLETE BEFORE IMPLEMENTATION
Worker: ChatGPT GPT-5.6 Sol; refreshed by receiving Worker Codex local GPT-5
Baseline main: `82369fdb69e33e3492b41c4aaa7f1ae9aaed18af`
Direct dependency: `WP-HK-GATE`

## Receiving-Worker transfer refresh

Transfer SHA: `3811a19257f20a2be0bef2572600d0e77c9733b0`
Live main at refresh: `18d4aa33be526c65a53796aa305a56fe8df15d12`

The two commits added to `main` after the original baseline are the accepted
DW-04 trial and its DocSync. A path-scoped comparison confirms that they do not
change `WP-HK-GATE`, its capsule, the foundational proof standard, the H1
architecture/execution ADR or the dependency/IP policy consumed here. The
accepted HK-GATE identity and inherited/current ownership split below are
therefore unchanged; the receiving Worker consumes the existing predecessor
check rather than re-proving H0.

## Accepted predecessor identity

The CTX-02 accepted-contract capsule
`Docs/engineering/context-capsules/WP-HK-GATE.json` was attempted as the
initial navigation representation, but the production checker rejected its
current `identity_source` blob fingerprint (`36d0ae...` recorded versus
`81ce6f...` effective). The receiving Worker therefore treated the capsule as
unusable and reconstructed the dependency from the authoritative gate
contract, verdict, proof matrix and residual-risk record. The contract and
verdict resolve the proof-matrix/residual pre-final snapshot to the accepted
final state:

- reviewed candidate: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`;
- independent PASS review: `#5261636151`;
- merge: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca`;
- status: `COMPLETE` / PASS.

Authoritative escalations performed for this consumer:

- `Docs/workpacks/HK/WP-HK-GATE.md`,
  `Docs/evidence/WP-HK-GATE/VERDICT.md`,
  `Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md` and
  `Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md` because capsule validation failed;
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` because H1-02 is FOUNDATIONAL;
- `Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md` and `Docs/architecture/ADR-H1-004-PUBLIC-EDITOR-EXECUTION-SEAM.md` because the current claim fixes the downstream project/toolchain and batch-launch substrate;
- `Docs/engineering/DEPENDENCY_IP_POLICY.md` because H1-02 owns exact Unity/package adoption records.

## Inherited guarantees consumed

H1-02 consumes, rather than re-proves:

1. H0 passed the accepted AI-authoring readiness gate and is valid as the engine-neutral authoring foundation.
2. Canonical H0 semantics remain engine-neutral; Unity is downstream of the bridge boundary.
3. HK-GATE PASS authorizes the Unity bridge track but does not authorize gameplay.
4. Existing H0 canonical build/semantic guarantees remain accepted unless effective Unity integration contradicts them.

## Guarantees newly owned by H1-02

H1-02 owns only the Unity substrate/toolchain boundary:

- exact Unity 6.3 LTS patch and effective editor fingerprint;
- pinned/locked minimal package graph and dependency/IP records;
- deterministic project settings including Force Text and visible `.meta` policy;
- reproducible clean import, compile and EditMode batch test execution;
- effective package/assembly inventory and clean-second-import equivalence;
- assembly-direction evidence preventing Unity dependencies from flowing into canonical H0 projects;
- one fixed non-interactive project-path / Arkus batch-entry recipe for later H1-03A binding;
- retained render-pipeline baseline.

## Explicit non-reproof / boundary

This WP does not re-prove H0 canonical semantics, transaction/replay guarantees, bridge public capabilities, asset identity, materialization, scenes, gameplay or third-party art. Quaternius Source remains H1-04-owned.

## Reopen condition

Reopen HK-GATE/H0 only if effective H1-02 evidence shows that the Unity integration requires changing an accepted H0 engine-neutral product/build boundary rather than adding a conforming downstream Unity project/adapter, or otherwise concretely falsifies an inherited HK-GATE guarantee.

No such contradictory evidence is known before implementation.
