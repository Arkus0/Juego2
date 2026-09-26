# WP-H1-01 — Reopen 1: Unity binding document codec

Status: **CORRECTION / OWNER-DIRECTED (independent review waived by owner decision)**

## Trigger and owner decision

This is the second half of the owner-selected structural fix ("option 3", 2026-09-26), taken after WP-H1-GATE fresh-agent trials 4 and 5 (PR `#238`). A model-driven client cannot reliably transcribe the opaque ~476-character `payloadBase64` that `unity.binding.compile` returns into `authoring.change.*`.

WP-HK-04 reopen 1 (PR `#244`) added the engine-agnostic typed-document port to the authoring kernel. This reopen gives the `arkus.unity-binding` extension a codec for that port, and registers the codec in the public production hosts. The owner chose a direct reopen with independent review waived.

## PREDECESSOR_CONTRACT_CHECK

This reopen corrects WP-H1-01 itself. It consumes, unchanged:

- the accepted H1-00 logical-reference and identity rules;
- H0 object-scoped extension semantics (HK-02A);
- the HK-04 reopen 1 port: `IExtensionDocumentCodec`, `ExtensionDocumentEncoding` and `ExtensionDocumentCodecs` in `Arkus.Harness.Protocol`, and the `put-extension` `document` form;
- H1-01's own binding schema `arkus.unity-binding@1`, its normalization, its canonical encoding (`AUB1`), and its dependency derivation.

Reopen condition checked: effective evidence shows that the accepted public path (compile, then transcribe the payload into authoring) is not usable by a fresh model-driven client. No H1-00 or H0 guarantee is contradicted, and the binding format and its identities are unchanged.

## Correction

- **Codec.** `UnityBindingDocumentCodec` (owner `arkus.unity-binding`, version 1) canonicalizes a binding document with the compiler's **own** normalization, encoding and canonical-dependency derivation, through the shared `UnityBindingProducer.CanonicalizeForExtension`. A document-authored binding is therefore byte-identical to the compiled payload.
- **Codec diagnostics.**
  - Compiler errors keep their `unity.binding.*` code. Their path is re-rooted from `$.binding…` to the document (`$…`), so the kernel reports `$.operations[i].document…`. The hint points back to `unity.binding.compile`.
  - A missing `subjectId` gives `unity.binding.missing-subject`.
- **`unity.binding.compile`** and `inspect` additionally return `documentMutation`: `{kind: put-extension, owner, schemaVersion, subjectId, document: <normalized binding>}`. It is additive and optional in the success schema. `extensionMutation` and `payloadBase64` are unchanged, and the result identity stays `arkus.unity-binding-result@1`, as pinned by `H1UnityAuthoringContractPinTests`.
- **Hosts.** `UnityAuthoringProvider.CreateDocumentCodecs()` is passed to the authoring session wherever the Unity authoring provider is composed:
  - `ProductionHarnessHost`, the default JSONL/MCP host;
  - `ProductionH1UnityHost`;
  - `ProductionH1ProjectCheckpointHost`.

  Codecs acquire no capability, route or write authority, and they survive snapshot import (checkpoint restore).

## Not changed

- The binding schema and its identities, `AUB1` payload bytes, dependency derivation, compiler codes, and the `extensionMutation` / `payloadBase64` outputs are unchanged.
- Canonical state, hashing and journal are unchanged. Documents are journaled in their canonical `payloadBase64` form (HK-04 reopen 1).
- Unity/Editor code, projection, catalogue, lifecycle and checkpoint semantics are unchanged.

## Proof

- `H1UnityAuthoringProducerTests.CompiledDocumentMutationAppliesByteIdenticallyToTheCompiledPayload` covers:
  - applying `documentMutation` and `extensionMutation` in parallel sessions gives the same content hash, identical stored payload bytes and identical dependencies;
  - a bad `source.kind` gives `world.change.invalid_extension_document` at `$.operations[0].document.source.kind`, with `extensionMachineCode = unity.binding.invalid-source-kind` and a hint naming `unity.binding.compile`;
  - a missing subject gives `unity.binding.missing-subject`.
- `H1UnityAuthoringTransportTests.ProductionHostsApplyTheCompiledDocumentMutationAcrossJsonlAndMcp`: the real production reference (JSONL) and MCP host processes compile, then apply the returned `documentMutation` together with its subject objects, with equivalent results.
- Local `Arkus.Harness.Tests` passes 457/457.
- `scripts/h1-01-observe/verify-exact-sha.sh` now tolerate exactly the Candidate Validation receipt files.
- Effective public proof: the WP-H1-GATE rerun (the driver authors through `documentMutation` and asserts parity with the payload path) and the final fresh-agent trial.
