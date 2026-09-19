# WP-HK-01 residual risk audit

## In-claim status

No known material defect class remains able to falsify HK01 acceptance inside the declared trust boundary.

The canonical surface currently contains only the H0 `system.describe` base capability; synthetic scoped providers prove that future reviewed engine/provider definitions and bindings can enter the same inventory without redesigning the kernel. World, mutation, validation, provenance implementation, transports and concrete Unity behavior remain owned by later workpacks.

## Non-blocking residual risks

- The schema model intentionally implements the Arkus-required subset of JSON Schema 2020-12, not every keyword in the standard. Canonical output identifies the 2020-12 dialect and uses standard object/string/number/array/required/enum/additionalProperties constructs plus the namespaced `x-arkus-reference-namespace` annotation. Expanding the supported schema vocabulary later must preserve projection/conformance semantics.
- Effective-route enumeration trusts normal .NET assembly loading/reflection behavior and the canonical build outputs, as permitted by the foundational trusted base. Arbitrary malicious compiler/runtime/CI subversion is out of claim.
- The production completeness oracle discovers current Arkus production projects under `src`. HK01 has no external plugin loader; when a later workpack introduces a reviewed external/provider loading mechanism, that mechanism must extend the independent universe rather than relying on provider registration metadata alone.
- Concrete MCP/JSONL projection parity is deliberately deferred to HK07. HK01 proves the canonical projection/conformance source that those adapters must consume; it does not implement a production transport.
- Concrete Unity/editor/runtime types and capabilities are deliberately absent. H1 must instantiate this scoped-provider boundary with portable canonical data and prove its own engine parity.
- HK01 defines policy/transaction/provenance declarations and ensures they cannot be omitted. Enforcement of later authoring transaction, validation and provenance behavior belongs to the corresponding downstream kernel workpacks.
- The current compatibility policy is conservative: only limited optional request evolution is considered additive within a higher minor version. Conservative false-breaking classifications are acceptable at this stage; silently accepting semantic breakage is not.

## Dependency/IP

HK01 adds no third-party package or shipped dependency. Existing pinned build/test dependencies remain governed by the accepted HK00 baseline.

## Proof budget

The final proof machinery is bounded to explicit HK01 acceptance, required self-attacks and defect classes discovered during Worker falsification. It does not attempt to prove arbitrary hostile behavior of the trusted .NET/Git/CI substrate.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
