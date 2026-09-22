# CTX-01 — Predecessor contract check (repair cycle 1)

Status: **PASS / REPAIR_LINEAGE_RECONSTRUCTED**

## Exact repair lineage

- original canonical implementation PR: `#106`;
- failed frozen candidate: `d972db98eca5527e7c30596ea069866dd878069b`;
- independent FAIL: `#5271197524`;
- failure boundary: self-referential DocSync freshness predicate `generated_from_main_sha == current main SHA`;
- rollback: PR `#109`;
- restored accepted `main`: `a85954539e0ef397009e87af322eb735d58ccc0d`;
- repair branch: `repair/ctx-01-freshness-anchor`;
- `fail_cycle=1`.

Because PR #106 is merged/closed and then fully reverted, repair-in-place is impossible. This branch is the explicit successor implementation path; it does not silently create a second active CTX-01 implementation.

## Accepted predecessor guarantees consumed unchanged

The accepted CTX programme plan and current Worker/Reviewer process remain authoritative. CTX-01 continues to consume, not redesign: exact-WP ownership and one canonical implementation path; Worker patch/evidence/pre-review on its own branch; exact frozen candidate handoff to an independent Reviewer; PASS/merge/DocSync as distinct transitions; accepted predecessor semantics remain closed unless concrete evidence invalidates them; H1 local executor remains mechanically narrower than the remote Worker.

No accepted predecessor defect was found. The FAIL is entirely CTX-01-owned: its derived-index freshness rule could not survive persistence.

## Reopen trigger

Reopen predecessor boundaries only if authoritative accepted evidence shows that role ownership, Reviewer independence, DocSync transition ordering, or H1 local authority materially differs from the guarantees above. No such evidence is present in this repair.
