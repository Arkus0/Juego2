# WP-H1-03 predecessor contract check

Base reconstructed from `main` at `88b6da2cff0d113c6e4769ec21e535b4fd34ac62`.

## Direct predecessors

### WP-H1-01 — portable Unity authoring contract

- reviewed frozen candidate: `385ce2466190003d18c849c8d944b881d184e1c7`
- independent PASS review: `#5265525323`
- implementation merge: `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`
- inherited boundary: public Unity binding semantics remain portable and engine-neutral; public requests carry logical identities, never native host paths or Unity runtime/editor types.

### WP-H1-02 — Unity project/toolchain baseline

- reviewed frozen candidate: `d86a08e644f542e9515f5e54fd4061f61e251c70`
- canonical PR: `#152`
- implementation merge: `faa42a3d58ab26b0dc2547f9b6b7fc49a604219d`
- Candidate Validation: Actions `35888922206` GREEN
- inherited boundary: exact Unity `6000.3.24f1 (4e7b9b5b6244)`, fixed project root, Force Text, Visible Meta Files, effective EditMode baseline and Unity-free H0 assembly direction.

## Foundational H0 containment

### WP-HK-09A — host capability containment

- reviewed frozen candidate: `acb1ccc341aec5131dc2ef979bd322e40e208b53`
- independent PASS review: `#5260496498`
- exact-SHA validation: Actions `35508529666`, artifact `10604463322`
- implementation merge: `614ad941881fdefa83fd46a1a8db989cfaba2cbb`
- inherited boundary: generic `ContractComposer` remains authority-neutral; `arkus.neutral-projection@1` enforces H0 admission below every transport. H0 exposes no generic shell/process, ambient network, caller-selected filesystem path or runtime-type activation authority.

## Binding H1 architecture

`Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md` v1.2 is accepted/binding from reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`, PASS `#5263596722`, merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`.

For H1-03 the fixed order is:

`reference JSONL / MCP -> arkus.neutral-projection@1 -> H1 policy + canonical composed handler -> fixed project-bound invocation envelope -> pinned Unity batch worker`.

H1-03 owns admission/project authority only. Process launch, lease/cancellation/crash/result reconciliation remain H1-03A.

## Remote effective-Unity substrate

Accepted `WP-H1-UNITY-CI` permits H1-03 end-to-end remote execution because its claim is machine-verifiable:

- accepted candidate: `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf`
- independent PASS: `#5299167558`
- implementation merge: `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`
- accepted Unity pilot: Actions `35936805407` / run #11
- effective editor: `6000.3.24f1 (4e7b9b5b6244)`

No physical owner-PC execution is required for this workpack.

## Reopen decision

`WP-H1-03` does not alter `H0HostCapabilityPolicy`. The accepted public `NeutralProjectionService(ComposedContract)` constructor remains H0-enforced. H1 adds a separate versioned admission profile and only an internal constructor consuming an opaque admitted-contract token. A raw `ComposedContract` with Unity Editor host effects therefore still fails the accepted H0 path.
