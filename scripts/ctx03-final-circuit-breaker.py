#!/usr/bin/env python3
"""Independent CTX-03 final circuit-breaker controls for B1/B2 classes."""
from __future__ import annotations

import importlib.util
import json
import sys
import tempfile
from pathlib import Path

DOCSYNC = Path("scripts/ctx03-docsync-history-check.py")
DYNAMIC = Path("scripts/ctx03-dynamic-context-check.py")


def module(path: Path, name: str):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None: raise RuntimeError(f"cannot load {path}")
    m = importlib.util.module_from_spec(spec); spec.loader.exec_module(m); return m


def write(root: Path, rel: str, text: str) -> None:
    p = root / rel; p.parent.mkdir(parents=True, exist_ok=True); p.write_text(text, encoding="utf-8")


def state(accepted: list[str], next_wp: str | None) -> str:
    return json.dumps({"authority":"DERIVED_NAVIGATION_ONLY","tracks":{"CTX":{"accepted_workpacks_hint":accepted,"next_contract_hint":next_wp}}})


def closure(wp: str, review: int, pr: int) -> str:
    return f"WP: `{wp}`\nindependent PASS: review `#{review}`\nimplementation PR: `#{pr}`\nDOCSYNC_COMPLETE\n"


def b1_controls(d) -> list[str]:
    errors: list[str] = []
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        write(root,"Docs/workpacks/CTX/WP-CTX-01.md","Status: **COMPLETE**\n"); write(root,"Docs/workpacks/CTX/WP-CTX-02.md","Status: **COMPLETE**\n"); write(root,"Docs/workpacks/CTX/WP-CTX-03.md","Status: **FROZEN PLAN / NOT_STARTED**\n")
        write(root,"Docs/evidence/CTX-01/DOCSYNC.md",closure("WP-CTX-01",1,11)); write(root,"Docs/evidence/CTX-02/DOCSYNC.md",closure("WP-CTX-02",2,12))
        write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02"],"WP-CTX-03")); write(root,str(d.CTX_README),"The next CTX action is `WP-CTX-03`.\n"); write(root,str(d.ROOT_WP),"next dependency-valid CTX action is `WP-CTX-03`.\n")
        e,a,n=d.current_state_errors(root)
        if e or a != ["WP-CTX-01","WP-CTX-02"] or n != "WP-CTX-03": errors.append(f"B1 baseline fixture not GREEN: {e}")
        write(root,"Docs/workpacks/CTX/WP-CTX-03.md","Status: **COMPLETE**\n"); write(root,"Docs/evidence/CTX-03/DOCSYNC.md",closure("WP-CTX-03",3,121)); write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02","WP-CTX-03"],None)); write(root,str(d.CTX_README),"CTX programme complete.\n"); write(root,str(d.ROOT_WP),"CTX programme complete.\n")
        e,a,n=d.current_state_errors(root)
        if e or a != ["WP-CTX-01","WP-CTX-02","WP-CTX-03"] or n is not None: errors.append(f"B1 post-CTX-03 simulation not GREEN without oracle edit: {e}")
        write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02"],"WP-CTX-03")); e,_,_=d.current_state_errors(root)
        if not any("accepted list mismatch" in x for x in e): errors.append("B1 stale pre-CTX-03 index did not turn RED")
        write(root,"Docs/workpacks/CTX/WP-CTX-04.md","Status: **NOT_STARTED**\n"); write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02","WP-CTX-03"],"WP-CTX-99")); write(root,str(d.CTX_README),"The next CTX action is `WP-CTX-04`.\n"); write(root,str(d.ROOT_WP),"next dependency-valid CTX action is `WP-CTX-04`.\n"); e,_,_=d.current_state_errors(root)
        if not any("next contract mismatch" in x for x in e): errors.append("B1 wrong next_contract_hint did not turn RED")
        (root/"Docs/evidence/CTX-03/DOCSYNC.md").unlink(); write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02","WP-CTX-03"],"WP-CTX-04")); e,_,_=d.current_state_errors(root)
        if not any("no required DocSync closure" in x for x in e): errors.append("B1 removed accepted closure did not turn RED")
        write(root,"Docs/evidence/CTX-03/DOCSYNC.md",closure("WP-CTX-03",3,121)); write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02","WP-CTX-03","WP-CTX-99"],"WP-CTX-04")); e,_,_=d.current_state_errors(root)
        if not any("accepted list mismatch" in x for x in e): errors.append("B1 fictitious accepted index-only WP did not turn RED")
        write(root,str(d.STATE),state(["WP-CTX-01","WP-CTX-02","WP-CTX-03"],"WP-CTX-04")); e1,a1,n1=d.current_state_errors(root); write(root,str(d.HISTORY),"WP-CTX-99 ACCEPTED; next WP-CTX-88 (history prose only)\n"); e2,a2,n2=d.current_state_errors(root)
        if e1 != e2 or a1 != a2 or n1 != n2: errors.append("B1 history prose redefined expected current state")
        write(root,str(d.STATE),state([],"WP-CTX-01")); e,a,_=d.current_state_errors(root)
        if a != ["WP-CTX-01","WP-CTX-02","WP-CTX-03"] or not e: errors.append("B1 audited index narrowed checker-owned accepted discovery")
    return errors


def profiles_fixture() -> dict:
    return {"profiles":{"worker":{"initial_reads":["<EXACT_WP>"]},"repair_worker":{"initial_reads":["<EXACT_WP>","<CANONICAL_PR_LATEST_FAIL>","<ORIGINAL_WORKER_EVIDENCE>"]},"reviewer":{"initial_reads":["<EXACT_WP>","<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>","<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>"]},"planner_gate":{"initial_reads":["<EXACT_MILESTONE_OR_GATE_CONTRACT>","<RELEVANT_TRACK_README_OR_CONSTITUENT_WPS>"]},"docsync":{"initial_reads":["<ACCEPTED_PR_REVIEW_MERGE>","<EXACT_ACCEPTED_WP>"]},"h1_local_executor":{"initial_reads":["<DURABLE_EXTERNAL_HANDOFF_ANCHOR>","<ANCHORED_LOCAL_EXECUTION_MANIFEST>","<EXACT_H1_WP>","<MANIFEST_NAMED_FILES_AND_SCRIPTS>"]}}}


def b2_controls(m) -> list[str]:
    errors: list[str] = []
    with tempfile.TemporaryDirectory() as td:
        root=Path(td); write(root,str(m.PROFILES),json.dumps(profiles_fixture())); cfg={"process_envelope":{"dynamic_repository_envelope":{"per_source_ceiling_estimate":40,"aggregate_route_ceiling_estimate":120,"policy_revision":1,"rationale":"synthetic independent control"}}}
        future_wp="Docs/workpacks/FUTURE/WP-FUTURE-77.md"; proof="Docs/evidence/FUTURE-77/proof.md"; write(root,proof,"proof\n"); write(root,future_wp,f"# future\nMandatory repository source: {proof}\n")
        r=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp}); paths={x["path"] for x in r["sources"]}
        if r["errors"] or future_wp not in paths: errors.append(f"B2 future route exact WP not automatically budgeted: {r['errors']}")
        write(root,future_wp,"x"*200+f"\nMandatory repository source: {proof}\n"); r=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})
        if not any(future_wp in e and "per-source ceiling" in e for e in r["errors"]): errors.append("B2 future exact WP growth did not turn RED")
        write(root,future_wp,f"# future\nMandatory repository source: {proof}\n"); new_source="Docs/evidence/FUTURE-77/extra-evidence.md"; write(root,new_source,"extra\n"); write(root,future_wp,f"# future\nMandatory repository source: {proof}\nNew dependency evidence: {new_source}\n"); r=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})
        if new_source not in {x["path"] for x in r["sources"]} or r["errors"]: errors.append(f"B2 new mandatory source was not automatically counted: {r['errors']}")
        write(root,new_source,"n"*200); r=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})
        if not any(new_source in e and "per-source ceiling" in e for e in r["errors"]): errors.append("B2 new mandatory source growth did not turn RED")
        write(root,new_source,"extra\n"); r=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})
        if new_source not in {x["path"] for x in r["sources"]}: errors.append("B2 caller omission shrank mandatory repository universe")
        before=r["aggregate_estimate"]; write(root,"Docs/random/not-mandatory.md","z"*20000); after=m.audit_resolved_route(root,cfg,"worker",{"<EXACT_WP>":future_wp})
        if after["errors"] or after["aggregate_estimate"] != before: errors.append("B2 non-mandatory repository growth affected dynamic envelope")
        worker_evidence="Docs/evidence/FUTURE-77/worker-evidence.md"; write(root,worker_evidence,"worker\n"); rr=m.audit_resolved_route(root,cfg,"repair_worker",{"<EXACT_WP>":future_wp,"<CANONICAL_PR_LATEST_FAIL>":"external:review-5276314835","<ORIGINAL_WORKER_EVIDENCE>":worker_evidence})
        if rr["errors"] or worker_evidence not in {x["path"] for x in rr["sources"]}: errors.append(f"B2 repair_worker repository evidence escaped budget: {rr['errors']}")
        h1_wp="Docs/workpacks/H1/WP-H1-99.md"; manifest="Docs/evidence/H1-99/LOCAL_EXECUTION.json"; named="scripts/local-task.py"; write(root,h1_wp,"# h1 synthetic\n"); write(root,named,"print('ok')\n"); write(root,manifest,json.dumps({"script":named})); lr=m.audit_resolved_route(root,cfg,"h1_local_executor",{"<DURABLE_EXTERNAL_HANDOFF_ANCHOR>":"external:anchor","<ANCHORED_LOCAL_EXECUTION_MANIFEST>":manifest,"<EXACT_H1_WP>":h1_wp})
        if lr["errors"] or named not in {x["path"] for x in lr["sources"]}: errors.append(f"B2 local manifest-named file escaped budget: {lr['errors']}")
        data=profiles_fixture(); data["profiles"]["worker"]["initial_reads"].append("<SYNTHETIC_NEW_DYNAMIC_SLOT>"); write(root,str(m.PROFILES),json.dumps(data)); ue=m.slot_universe_errors(root)
        if not any("SYNTHETIC_NEW_DYNAMIC_SLOT" in e for e in ue): errors.append("B2 new dynamic mandatory slot did not fail closed for explicit oracle review")
    return errors


def main() -> int:
    repo=Path(".").resolve()
    try: errors=b1_controls(module(repo/DOCSYNC,"ctx03_docsync_target"))+b2_controls(module(repo/DYNAMIC,"ctx03_dynamic_target"))
    except Exception as exc:
        print(f"CTX03_FINAL_CIRCUIT_BREAKER: INFRA_ERROR\n{exc}",file=sys.stderr); return 23
    if errors:
        print("CTX03_FINAL_CIRCUIT_BREAKER: FAIL",file=sys.stderr)
        for e in errors: print(f"- {e}",file=sys.stderr)
        return 20
    print("CTX03_FINAL_CIRCUIT_BREAKER: PASS"); return 0

if __name__ == "__main__": raise SystemExit(main())
