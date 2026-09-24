# Owner-decision timeout amendment

Status: BINDING PROCESS_ONLY maintenance amendment after merge to `main`.
Date: 2026-09-24

This amendment changes only the liveness policy for bounded **soft owner preferences** requested by Worker/repair roles through `scripts/request_owner_decision.py`.

## Rule

A soft owner-preference request waits at most **180 seconds** for a valid bot-attested owner choice.

- If an authenticated owner choice is available before the wait closes, the Worker continues with that exact selected option.
- If no valid choice is available after 180 seconds, the request is closed locally as abandoned with reason `owner-timeout-delegated-to-worker` and the helper returns this instruction to the same Worker:

  `El usuario indica que elijas la opción que creas más conveniente.`

- The Worker then chooses autonomously among the same 2–3 alternatives it already declared valid and in-scope. The timeout does not mint an `OWNER_DECISION` attestation and does not pretend that the owner selected an option.
- Because the timed-out request is marked abandoned, `/status` and advertisement no longer count it as pending and an old Telegram button cannot be accepted afterward.

## Safety boundary

This fallback applies only to decisions that were already eligible for the soft owner-decision mechanism. It does **not** apply to Reviewer verdicts, acceptance/proof waivers, security/authentication failures, unavailable mandatory evidence, fourth FAIL, `REVIEW_BLOCKED`, required physical-PC observations, or any other canonical hard-stop condition.

Where older owner-console/autopilot documentation says that soft owner decisions wait indefinitely, this 180-second rule supersedes that text. All other trust-boundary and exact-SHA rules remain unchanged.
