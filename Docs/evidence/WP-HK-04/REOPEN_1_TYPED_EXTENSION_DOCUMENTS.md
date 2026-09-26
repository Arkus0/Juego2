# WP-HK-04 — Reopen 1: typed extension documents in the authoring transaction

Status: **CORRECTION / OWNER-DIRECTED (independent review waived by owner decision)**

## Trigger and owner decision

The WP-H1-GATE fresh independent public-client AI-agent trials 4 and 5 (PR `#238`; runs `36246411530` and `36248237941`, record `#5847067730`) showed the same failure. The only way to author an extension was to transcribe an opaque ~476-character `payloadBase64`, returned by the owner's tooling (`unity.binding.compile`), into `authoring.change.*`. Model-driven clients corrupt such strings: the binding payload contains a run of 71 `A` characters, which encodes zero bytes. That makes the authoring kernel's public path depend on a client's ability to transcribe binary data, which is the opposite of the H0 AI-native authoring goal.

Owner decisions (2026-09-26, in the WP-H1-GATE Worker session), taken after three options were compared:

- Choose the structural fix ("option 3"), because it is the best for the future and there is no rush.
- Design: typed documents in the transaction, with an engine-agnostic codec registry.
- Process: a direct reopen, with independent review waived.
- The complementary Base64 diagnostic was merged separately (WP-HK-05 reopen 2, `#243`).

## PREDECESSOR_CONTRACT_CHECK

This reopen corrects WP-HK-04 itself. The direct accepted dependency is `WP-HK-03`, which is COMPLETE.

The correction consumes these accepted guarantees unchanged:

- HK-02/02A canonical world state and object-scoped extension semantics (opaque owner payload, owner, schema version, subject, dependencies);
- the HK-04 transactional envelope, including optimistic `expectedRevision` + `expectedHash`, idempotency receipts and the request fingerprint over parsed canonical operations;
- the HK-05 repairable-diagnostic shape and its reopens 1–2;
- HK-06A journal normalization, where entries store the normalized canonical request;
- HK-06B snapshot import and HK-06C replay;
- the H0 `StructuredError` contract and `PortableData`.

Reopen condition checked: concrete effective evidence shows that the accepted public mutation path is not usable by a fresh model-driven public client for extension authoring. No HK-02/02A/03/05/06 guarantee is contradicted.

## Design

- **Port (engine-agnostic, in the neutral contract assembly `Arkus.Harness.Protocol`, so that the H0 kernel and scoped providers depend on it without depending on each other).**
  - `IExtensionDocumentCodec` has `Owner`, `SchemaVersion`, and `Encode(subjectId, document) → ExtensionDocumentEncoding`, which is either `Encoded(canonical bytes, derived dependencies)` or `Rejected(code, document-relative path, public message, hint)`.
  - `ExtensionDocumentCodecs` is an immutable (owner, schemaVersion) registry; duplicates are rejected.
  - H0 still never interprets payload bytes.
- **Composition.** The host passes the admitted codecs when it creates the authoring session:
  - `TransactionalWorldAuthoringSession(state, codecs)`;
  - `PortableWorldAuthoringSession(state, codecs)`;
  - `CanonicalWorldContract.ComposeEmptyPortableSession(worldId, contributions, codecs)`.

  Codecs acquire no capability, route or write authority. The portable session hands them to the inner sessions created by snapshot import and replay, so documents keep working after a restore.
- **Grammar.** A `put-extension` operation carries **exactly one** of `payloadBase64` or `document` (a JSON object). With `document`:
  - `dependencies` may not be supplied, because the codec derives them;
  - the codec encodes inside plan, dry-run, validate and apply;
  - derived dependencies go through the same `ParseReferences` rules as caller input;
  - the byte limit applies to the encoded payload.

  The resulting `MutationOperation` is exactly the one the `payloadBase64` form would produce.
- **Equivalence by construction.** The request fingerprint is computed over parsed canonical operations, and the journal stores the normalized request, whose `put-extension` always has `payloadBase64`. A document-authored extension therefore has the same state, content hash, fingerprint and journal entry as the payload-authored one. Replay and portability need no codec, and an idempotency key replays across the two forms.
- **Diagnostics.**
  - Absent or duplicate payload source: `world.change.invalid_request` with `oneOfFields`.
  - No codec for the owner and version: `world.change.extension_document_unsupported` with `supportedDocumentOwners`.
  - Codec rejection: `world.change.invalid_extension_document`. The path is `$.operations[i].document` plus the codec's path, and the context carries `extensionMachineCode` and `extensionPath`, with the codec's hint.
  - A codec that throws or derives invalid dependencies is contained as a structured `extension.codec-fault`.
  - A non-object `document` is refused earlier by the published request schema (`contract.invalid_request`).
- **Discovery.** The published operation schema gains an optional `document` object property.
- **Unchanged.** The Base64 path, including its accepted validation order (payload, then limit, then subject, then dependencies), is untouched.

## Not changed

- Canonical world state, hashing, journal schema and entries, snapshot and replay semantics, and the admission policy are unchanged.
- Existing codes, messages and paths on the `payloadBase64` path are unchanged. The only exception is the formerly `missing-or-not-a-string` absent payload, which now reports the one-of rule.
- No engine-specific code enters H0. The Unity binding codec and host registration are the separate WP-H1-01 reopen.

## Proof

- `Hk04TypedExtensionDocumentTests`:
  - The document form and the payload form give the same plan data, content hash and journal. An idempotent replay works across forms.
  - Every diagnostic path has the exact code, path and context: unsupported owner, codec rejection with mapped path and hint, both or neither sources, supplied dependencies, non-object document at the schema layer, and codec faults (throwing, or invalid derived dependency). The state is unchanged throughout.
  - Codecs survive snapshot import (new local lineage).
  - `document` is present in the discovered request schema.
- The HK-05 reopen-2 test is updated for the absent-payload rule.
- Local `Arkus.Harness.Tests` passes 455/455.
- `scripts/hk04-observe/verify-exact-sha.sh` now run the new test class explicitly, and tolerate exactly the Candidate Validation receipt files. They were previously RED in CI whatever the candidate contained, for the same reason as HK-05.
