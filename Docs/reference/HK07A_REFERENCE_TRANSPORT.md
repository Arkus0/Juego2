# HK07A neutral projection and reference transport

HK07A exposes the accepted canonical Arkus H0 contract through two deliberately separate layers:

1. `arkus.neutral-projection@1` defines transport-independent request, discovery, outcome, cancellation and completeness semantics.
2. `arkus.reference.jsonl@1` frames that neutral contract as strict UTF-8 JSON Lines over standard input/output.

The composed canonical inventory remains the only capability and schema authority. The projection and JSONL adapter do not maintain a command registry. Clients start with `system.describe` and select a published canonical version range.

> **HK09A containment note.** HK07A originally proved a `--file PATH` one-shot framing convenience. The H0 production executable no longer exposes that mode: HK09A removes caller-controlled filesystem authority from the production host. Use stdin/stdout (`--stream` or `--once`). The neutral projection and JSONL protocol semantics are unchanged.

## Build and launch

From a clean checkout with the SDK pinned by `global.json`:

```bash
dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --configuration Release --no-restore
dotnet src/Arkus.Harness.Cli/bin/Release/net8.0/Arkus.Harness.Cli.dll --stream
```

Supported production launch modes are:

| Invocation | Behavior |
|---|---|
| no mode option or `--stream` | Read zero or more JSONL request frames until EOF and write one response for every frame. |
| `--once` | Read exactly one request from stdin, require EOF after it, write one response and exit. |
| `--diagnostics` | Add lifecycle diagnostics on stderr only; combinable with a supported mode. |

`--file` is rejected as an H0 usage error before the framing host can open any caller-provided path. Unknown, duplicate or incompatible options likewise fail closed before any protocol output. The local host needs no Unity/editor state, network access or interactive prompt. Its initial process-local authored session is explicit and fixed: world ID `world.arkus.session`, revision `0`, and no objects. Ambient `ARKUS_*` or other environment values do not select a world, registry or capability; the .NET host/runtime environment itself remains part of the documented execution substrate.

## Request frame

Each input frame is one JSON object:

```json
{
  "protocol": "arkus.reference.jsonl@1",
  "request": {
    "projectionVersion": "arkus.neutral-projection@1",
    "requestId": "client-1",
    "capability": "system.describe",
    "acceptedVersions": {
      "major": 1,
      "minimumMinor": 0,
      "maximumMinor": 0
    },
    "arguments": {},
    "timeoutMilliseconds": 5000
  }
}
```

`timeoutMilliseconds` is optional; all other shown fields are required. Envelopes reject missing, duplicate or unknown fields. `requestId` is opaque non-empty client correlation. `arguments` is the portable JSON object validated by the selected canonical capability's discovered request schema.

The neutral request is exactly:

| Field | Meaning |
|---|---|
| `projectionVersion` | Neutral semantic contract identifier; exactly `arkus.neutral-projection@1`. |
| `requestId` | Correlation echoed unchanged in the outcome. |
| `capability` | Canonical capability name from composed discovery. |
| `acceptedVersions` | Canonical major and inclusive minor-version range. |
| `arguments` | Canonical portable request data, with no transport framing fields. |
| `timeoutMilliseconds` | Optional non-negative admission deadline in milliseconds. |

## Response frame

Every ordinary request-level outcome, including malformed input, is a JSONL response frame:

```json
{
  "protocol": "arkus.reference.jsonl@1",
  "response": {
    "projectionVersion": "arkus.neutral-projection@1",
    "requestId": "client-1",
    "capability": "system.describe",
    "status": "success",
    "result": {}
  }
}
```

An error response has `status: "error"`, a `failureKind`, and the accepted structured-error shape in `error`:

| `failureKind` | Authority and meaning |
|---|---|
| `canonical` | The composed canonical dispatcher rejected or failed the invocation. Its error data is preserved. |
| `cancelled` | The cancellation token was reached before canonical dispatch began; machine code `projection.cancelled`. |
| `timed-out` | The admission deadline expired before canonical dispatch began; machine code `projection.timeout`. |
| `transport` | The JSONL adapter could not form a valid neutral request. |

Object properties in emitted JSON are sorted with ordinal comparison; array order remains canonical order. Identical process state plus identical input produces byte-identical stdout.

## Admission cancellation and timeout

Cancellation and timeout are deliberately bounded to admission. An already cancelled request or a request with `timeoutMilliseconds: 0` is never canonically dispatched. A positive deadline bounds waiting for the process-local dispatch gate. Once synchronous canonical dispatch begins, the canonical success or structured failure is authoritative: the adapter never reports cancellation for an effect that may already have committed.

`Ctrl+C` supplies the process cancellation token to neutral admission. It is not an unsafe thread-abort mechanism and does not promise to interrupt a canonical handler after dispatch begins. Interruptible canonical execution and operational time/resource enforcement are not HK07A claims.

## Framing and failure behavior

- Input is strict UTF-8; invalid byte sequences return `transport.invalid_utf8`.
- One frame is bounded to 1,048,576 bytes before the line terminator; a larger frame returns `transport.frame_too_large` after the remainder of that line is consumed.
- LF and CRLF delimit frames. A final valid JSON value at EOF is accepted without a newline.
- An incomplete JSON value and empty one-shot input return `transport.truncated_frame`; other invalid strict JSON returns `transport.malformed_json`.
- JSON comments, trailing commas, duplicate properties, non-finite/unrepresentable numbers and non-object envelopes are rejected with stable transport/projection diagnostics.
- Multiple frames in one-shot mode return `transport.multiple_frames` and dispatch neither frame.
- Canonical validation, version, unknown-capability and domain failures retain their canonical machine codes and repair data under `failureKind: "canonical"`.
- Protocol frames are written only to stdout. Usage, lifecycle and fatal diagnostics are written only to stderr.

## Exit codes

| Code | Meaning |
|---:|---|
| `0` | Stream reached clean EOF, or one-shot request succeeded. Stream request errors are framed outcomes and do not terminate a healthy stream. |
| `2` | One-shot input produced a framed request, projection, canonical or transport error. |
| `64` | Invalid or forbidden launch options (including production `--file`); no protocol frame is written. |
| `70` | Unexpected host/software failure; diagnostic is written to stderr. |

## Discovery and completeness

The production composition currently publishes 18 accepted canonical capabilities. That count is an observation, not a registry: clients and adapters must consume `system.describe`, and projection completeness is evaluated against `ComposedContract.Definitions`. Scoped contributions therefore appear automatically. Omitting a composed capability, inventing an adapter-only capability or emitting a duplicate is a projection-conformance failure.

The runtime kernel has no dependency on the projection executable, stdin/stdout, JSONL or `System.Text.Json`. The neutral projection likewise has no CLI or JSON dependency. JSON conversion and framing terminate at the reference adapter boundary.

## State and trust boundary

State is process-local. To reproduce authored state in a fresh process, use the accepted snapshot import and/or journal replay capabilities through the public contract over stdin/stdout. HK07A does not add durable storage, crash recovery, authentication, networking, multi-process coordination, MCP, batching/pagination, or gameplay/runtime-state semantics. HK09A additionally constrains the production H0 host so transports cannot gain generic external/elevated authority and the CLI does not obtain caller-selected filesystem authority.