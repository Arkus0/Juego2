# CTX-03 — Control matrix

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

| Claim | Independent universe / authority | Causal control | Required boundary |
|---|---|---|---|
| same-snapshot measurement | checker-owned representative routes + exact candidate bytes | redirect/drop representative route | RED |
| base profile universe | canonical profiles + checker-owned substitutions | drop profile / redirect substitution | RED |
| fixed conditional completeness | checker-owned fixed set + independently extracted fixed paths | new unreviewed fixed source / drop conditional budget | RED |
| fixed conditional growth | every canonical profile fixed superset | grow every applicable fixed source | RED over ceiling |
| representative route-effective growth | checker-owned H1/CITY/PA routes | grow H1/CITY/PA route-forced sources | RED over ceiling |
| dynamic slot classification | checker-owned slot map | introduce unknown placeholder | RED pending explicit review |
| dynamic slot surface completeness | all canonical `initial_reads` + `conditional_reads` | put unknown placeholder only in conditional surface | RED |
| future arbitrary WP | repository-wide `Docs/workpacks/**/WP-*.md` discovery | invent FUTURE route outside H1/CITY/PA | exact WP automatically counted |
| future exact-WP growth | dynamic per-source ceiling | grow invented WP | RED |
| new mandatory repository source | independent contract grammar | add required evidence source | automatically counted; growth over limit RED |
| caller cannot shrink dynamic route | exact contract / manifest remains authority | omit required source from caller binding | source still counted |
| repair Worker evidence | concrete repository/external classification | repository Worker evidence + external GitHub FAIL | repository evidence counted; external FAIL excluded explicitly |
| local manifest context | anchored repository manifest | omit caller-maintained named-file list | manifest-named repo file still counted |
| unrelated repository growth | resolved mandatory set | grow unrelated file | GREEN |
| dynamic ceiling evolution | prior config from base ref | raise limit without policy revision + justification | RED |
| B1 accepted-state discovery | numeric CTX contracts + required DocSync closures | stale/empty/fictitious derived index | RED while authoritative accepted set unchanged |
| B1 post-adoption transition | same unchanged checker | simulate CTX-03 COMPLETE + valid DocSync + updated projection | GREEN without oracle edit |
| B1 next state | numeric CTX contract sequence | wrong next hint / CTX-10 extension | RED for wrong hint; numeric next remains correct |
| B1 closure provenance | COMPLETE CTX contract | remove entire required DocSync closure | RED |
| history cannot define current state | authoritative CTX contracts/closures | edit history prose only | expected state unchanged |
| escalation completeness | canonical `must_escalate_if` | remove field / `[]` / `{}` / required predicate | RED |
| compact PA/CITY reachability | accepted capsule/source authority | remove material selector/read | quality replay RED |
| no semantic substitution | exact sources + independent Reviewer | mechanical/compact PASS | cannot establish semantic verdict |
| exact CLEAN identity | final candidate HEAD + GitHub issue comment | nonexistent/wrong PR/wrong SHA pointer | handoff RED |
| post-CLEAN mutation | live PR HEAD | mutate repository/evidence bytes | prior CLEAN invalid |
| missing REVIEW_READY | durable Automation V2 marker | all gates green but marker absent | REVIEW_BLOCKED |
| same-SHA retry | existing durable marker + later gate success | freeze RED → same SHA freeze GREEN | closure becomes reachable without duplicate marker |
| post-marker movement | final live HEAD read | move HEAD after marker | REVIEW_BLOCKED |
| history separation | normal role profiles vs `Docs/history/**` | history added to normal bootstrap | RED |

## Final exact-SHA validation surface

```text
python3 scripts/context-envelope-check.py --self-test
python3 scripts/ctx03-dynamic-context-check.py --self-test
python3 scripts/ctx03-dynamic-slot-controls.py --self-test
python3 scripts/mechanical-verifier-classifier.py --self-test
python3 scripts/review-ready-closure.py --self-test
python3 scripts/derive-worker-review-metadata.py --self-test
python3 scripts/validate-worker-handoff.py --self-test
python3 scripts/ctx03-quality-replay.py --self-test
python3 scripts/ctx03-process-controls.py --self-test
python3 scripts/ctx03-docsync-history-check.py --self-test
python3 scripts/context-envelope-check.py --audit --base-ref <BASE_SHA> --escalations Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json
python3 scripts/ctx03-dynamic-context-check.py --audit-policy --base-ref <BASE_SHA>
python3 scripts/ctx03-dynamic-slot-controls.py
python3 scripts/ctx03-quality-replay.py --negative-controls
python3 scripts/ctx03-process-controls.py
python3 scripts/ctx03-docsync-history-check.py
python3 scripts/ctx03-final-circuit-breaker.py
```

The PR workflow runs this causal surface and classifies its structured outcome. Accepted CTX-02 capsule validation remains independently binding; CTX-03 does not replace it.
