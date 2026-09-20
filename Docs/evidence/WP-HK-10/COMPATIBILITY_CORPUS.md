# WP-HK-10 Protocol v1 compatibility corpus

Machine-readable corpus: `tests/Arkus.Harness.Tests/Compatibility/protocol-v1.json`.

The corpus is deliberately independent from runtime constants: it records accepted Protocol v1 identities and numeric boundaries as literals, while `Hk10EnduranceCompatibilityTests.ProtocolV1CompatibilityCorpusMatchesAcceptedRuntimeAndResourceEnvelope` compares those literals to the current implementation. A runtime constant drifting to a new meaning therefore turns the test RED instead of moving the oracle with the implementation.

## Frozen compatibility facts

- protocol major: `1`;
- reference transport: `arkus.reference.jsonl@1`;
- JSONL maximum frame: `1,048,576` bytes;
- oversized-frame machine code: `transport.frame_too_large`;
- canonical argument ceiling: `917,504` bytes;
- portable depth: `32`;
- coherent mutation transaction: `96` operations;
- decoded mutation payload: `524,288` bytes;
- query page: `100` items;
- canonical world/snapshot state: `655,360` bytes;
- world resources: `10,000`;
- local mutation transactions: `10,000`;
- snapshot-import receipts: `1,024`;
- cooperative execution/publication budget: `5,000 ms`;
- durability: `process-local-checkpoint`;
- power-loss durability claimed: `false`.

The corpus also requires exact v1.0 discoverability with non-null request/success/error schemas for `system.describe`, `system.resource-envelope.describe`, `world.summary`, `world.object.get`, `world.object.query`, `authoring.change.apply`, `authoring.journal.read`, `authoring.snapshot.export` and `authoring.snapshot.import`.

## Scope

This is a compatibility corpus, not a cross-major migration mechanism. Future journal/snapshot/MCP SDK/protocol versions remain explicit reviewed lifecycle work. HK10 proves that the accepted current v1 facts cannot silently drift while retaining the same public identity.

## Causal control

`hk10-negative-conformance.sh` changes `MaximumSessionTransactions` from 10,000 to 10,001 and requires this corpus test to RED. The HK08A control similarly changes the accepted 96-operation shape to 95 and is caught by the interaction/corpus universe. Transport framing has an additional inherited literal oracle in HK07A.

COMPATIBILITY_CORPUS: COMPLETE
PROTOCOL_MAJOR: 1
