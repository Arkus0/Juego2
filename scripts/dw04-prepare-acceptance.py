#!/usr/bin/env python3
"""Materialize the DW-04 acceptance freeze or its post-freeze context assembly.

Neither mode calls a model. Freeze mode is result-independent over the untouched
acceptance pool. Assembly mode may run only after TASK_SELECTION_FREEZE exists;
it replays the actual DW-02/03 read-only query adapter plus exact source fallback.
"""

import argparse
import hashlib
import json
import pathlib
import statistics
import subprocess
import sys

from importlib.machinery import SourceFileLoader

ROOT = pathlib.Path(__file__).resolve().parents[1]
trial = SourceFileLoader("dw04_trial", str(ROOT / "scripts/dw04-trial.py")).load_module()
PRE_COMMIT = "9cbed950a3469897cf286c9a90624c240701528f"
ACCEPTANCE_ORACLE_COMMIT = "5c15ce90734247b53841868e5cac50dfefe9889d"
PRE = ROOT / trial.PRE
CAL_RESULTS = ROOT / trial.CAL_RESULTS
SOURCE_ORACLES = ROOT / trial.SOURCE_ORACLES
PROTOCOL = ROOT / trial.CAL_PROTOCOL
FREEZE = ROOT / trial.FREEZE
RETRIEVAL_PROJECT = "tools/Arkus.Dw04.Retrieval"


def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, stderr=subprocess.PIPE)


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def load(path):
    return json.loads(path.read_text(encoding="utf-8"))


def write_new(path, value):
    path = pathlib.Path(path)
    if path.exists():
        raise trial.ProtocolError(f"refusing to overwrite {path}")
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def introduction_commit(path):
    raw = git("log", "--diff-filter=A", "--format=%H", "--", path).decode().splitlines()
    if not raw:
        raise trial.ProtocolError(f"no introduction commit for {path}")
    return raw[-1]


def response_contract(task_id, oracle):
    fact_types = {
        "A-CITY-01": {"importance":"importance","spatial_depth":"spatial_depth","interior":"interior"},
        "A-CITY-02": {"importance":"importance","spatial_depth":"spatial_depth","interior":"interior","x6_access":"token","x6_cart_authorized":"boolean","x6_bicycle_authorized":"boolean"},
        "A-CITY-03": {"importance":"importance","spatial_depth":"spatial_depth","interior":"interior","access_layers":"token"},
        "A-PA-01": {"actor_visible_stale_label":"token","automatic_truth_belief_sync":"token","nc02_behavior_neutral":"requirement"},
        "A-PA-02": {"recursive_hidden_lineage":"token","nc02_invariance":"requirement","receiver_epistemic_owner":"token"},
        "A-PA-03": {"DL-11":"token","DL-12":"token","DL-13":"token","DL-14":"token"},
    }
    global_blockers = [
        "importance_mismatch","interior_mismatch","x6_capacity_invented",
        "privileged_metadata_leak","automatic_truth_sync","hidden_lineage_as_epistemic_evidence"
    ]
    global_evidence = [
        "CITY02:§4:loc.casco.shared_court","CITY02:§4:loc.puerto.landing","CITY01:§5.3:X6","CITY01:§4.3:AX6",
        "CITY02:§4:loc.plaza.ayuntamiento","PA04:§11:actor-visible isTrue/isFalse/isStale",
        "PA04:§11:automatic truth→belief synchronization","PA04:§13:NC-02",
        "PA05:§6:Recursive hidden lineage embedded in ActorBelief","PA05-FIXTURES:§6:NC-02",
        "PA05-FIXTURES:§1:ownership","PA01:§3:DL-11..DL-14"
    ]
    keys = list(oracle["facts"])
    types = fact_types[task_id]
    if set(keys) != set(types):
        raise trial.ProtocolError(f"fact-type map drift for {task_id}")
    return {
        "fact_keys": keys,
        "fact_value_types": types,
        "allowed_blockers": global_blockers,
        "allowed_verdicts": ["REPORT","REJECT"],
        "evidence_ids": global_evidence,
    }


def plan_fragment(fid, source_path, origin, selector, query_args=None):
    return {"id":fid,"source_path":source_path,"origin":origin,"selector":selector,"query_args":query_args}


def context_plan():
    city2 = "Docs/production/CITY_LOCATION_PROGRAMME.md"
    city1 = "Docs/production/CITY_MOBILITY_TOPOLOGY.md"
    pa1 = "Docs/research/living-world/results/PA-01.md"
    pa4 = "Docs/research/living-world/results/PA-04.md"
    pa5 = "Docs/research/living-world/results/PA-05.md"
    pa5f = "Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md"
    return {
        "A-CITY-01": {
            "CTX": [plan_fragment("ctx-table-city02-shared", city2, "source_excerpt", "loc.casco.shared_court")],
            "DW": [plan_fragment("dw-query-city02-shared", city2, "dw_query", "loc.casco.shared_court", ["city","loc.casco.shared_court"])],
        },
        "A-CITY-02": {
            "CTX": [
                plan_fragment("ctx-table-city02-puerto", city2, "source_excerpt", "loc.puerto.landing"),
                plan_fragment("ctx-table-city01-x6", city1, "source_excerpt", "`X6` | `W.LANDING ↔ O.PUERTO_UP`"),
                plan_fragment("ctx-line-city01-ax6", city1, "source_excerpt", "`AX6` — State-1 ferry"),
            ],
            "DW": [
                plan_fragment("dw-query-city02-puerto", city2, "dw_query", "loc.puerto.landing", ["city","loc.puerto.landing"]),
                plan_fragment("dw-fallback-table-city01-x6", city1, "source_excerpt", "`X6` | `W.LANDING ↔ O.PUERTO_UP`"),
                plan_fragment("dw-fallback-line-city01-ax6", city1, "source_excerpt", "`AX6` — State-1 ferry"),
            ],
        },
        "A-CITY-03": {
            "CTX": [plan_fragment("ctx-table-city02-townhall", city2, "source_excerpt", "loc.plaza.ayuntamiento")],
            "DW": [plan_fragment("dw-query-city02-townhall", city2, "dw_query", "loc.plaza.ayuntamiento", ["city","loc.plaza.ayuntamiento"])],
        },
        "A-PA-01": {
            "CTX": [
                plan_fragment("ctx-table-pa04-stale", pa4, "source_excerpt", "actor-visible `isTrue/isFalse/isStale`"),
                plan_fragment("ctx-table-pa04-truth-sync", pa4, "source_excerpt", "automatic truth→belief synchronization"),
                plan_fragment("ctx-section-pa04-nc02", pa4, "source_excerpt", "NC-02"),
            ],
            "DW": [
                plan_fragment("dw-query-pa04-nc02", pa4, "dw_query", "NC-02", ["pa-fixture","pa04","NC-02"]),
                plan_fragment("dw-fallback-table-pa04-stale", pa4, "source_excerpt", "actor-visible `isTrue/isFalse/isStale`"),
                plan_fragment("dw-fallback-table-pa04-truth-sync", pa4, "source_excerpt", "automatic truth→belief synchronization"),
            ],
        },
        "A-PA-02": {
            "CTX": [
                plan_fragment("ctx-table-pa05-lineage", pa5, "source_excerpt", "Recursive hidden lineage embedded in ActorBelief"),
                plan_fragment("ctx-section-pa05-nc02", pa5f, "source_excerpt", "NC-02 — HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE"),
            ],
            "DW": [
                plan_fragment("dw-query-pa05-nc02", pa5f, "dw_query", "NC-02", ["pa-fixture","pa05","NC-02"]),
                plan_fragment("dw-fallback-table-pa05-lineage", pa5, "source_excerpt", "Recursive hidden lineage embedded in ActorBelief"),
            ],
        },
        "A-PA-03": {
            "CTX": [
                plan_fragment("ctx-table-pa01-dl11", pa1, "source_excerpt", "| DL-11 |"),
                plan_fragment("ctx-table-pa01-dl12", pa1, "source_excerpt", "| DL-12 |"),
                plan_fragment("ctx-table-pa01-dl13", pa1, "source_excerpt", "| DL-13 |"),
                plan_fragment("ctx-table-pa01-dl14", pa1, "source_excerpt", "| DL-14 |"),
            ],
            "DW": [
                plan_fragment("dw-query-pa01-dl11", pa1, "dw_query", "DL-11", ["pa-disposition","pa01","DL-11"]),
                plan_fragment("dw-query-pa01-dl12", pa1, "dw_query", "DL-12", ["pa-disposition","pa01","DL-12"]),
                plan_fragment("dw-query-pa01-dl13", pa1, "dw_query", "DL-13", ["pa-disposition","pa01","DL-13"]),
                plan_fragment("dw-query-pa01-dl14", pa1, "dw_query", "DL-14", ["pa-disposition","pa01","DL-14"]),
            ],
        },
    }


def make_freeze():
    if FREEZE.exists():
        raise trial.ProtocolError("TASK_SELECTION_FREEZE already exists")
    pre = load(PRE)
    selected = trial.precheck(pre, pre["baseline_sha"])
    results = load(CAL_RESULTS)
    oracles = load(SOURCE_ORACLES)
    protocol = load(PROTOCOL)
    if results.get("readiness") != "READY":
        raise trial.ProtocolError("calibration is not READY")
    if list(oracles["tasks"]) != selected:
        raise trial.ProtocolError("acceptance source-oracle order differs from deterministic selection")
    calibration_commit = introduction_commit(trial.CAL_RESULTS)
    if calibration_commit != "98bd04679fa79e00d1b45f46b41cdf27ab979c25":
        raise trial.ProtocolError("unexpected calibration result introduction commit")
    cal_raw = git("show", f"{calibration_commit}:{trial.CAL_RESULTS}")
    question = {t["id"]:t["question"] for t in pre["eligible_tasks"]}
    tasks = {}
    for task_id in selected:
        source = oracles["tasks"][task_id]
        tasks[task_id] = {
            "prompt": question[task_id],
            "oracle": source["oracle"],
            "required_context": source["required_context"],
            "response_contract": response_contract(task_id, source["oracle"]),
        }
    model_config = results["model_config"]
    if model_config != protocol["model_config"]:
        raise trial.ProtocolError("READY calibration model config differs from protocol")
    slots = trial.make_slots(selected)
    plan = context_plan()
    if list(plan) != selected:
        raise trial.ProtocolError("context-plan order differs from selection")
    freeze = {
        "schema":"dw04-acceptance-v1",
        "precalibration_commit":PRE_COMMIT,
        "acceptance_oracle_commit":ACCEPTANCE_ORACLE_COMMIT,
        "calibration_oracle_commit":results["calibration_oracle_commit"],
        "calibration_commit":calibration_commit,
        "calibration_status":"READY",
        "calibration_receipt_digest":sha256(cal_raw),
        "selected":selected,
        "tasks":tasks,
        "model_config":model_config,
        "system_prompt":"You are executing a frozen repository context-evaluation task. The supplied context fragments are the only semantic authority for this run: do not use memory, outside knowledge, web search, tools or unstated assumptions. Return only the canonical structured answer. Facts with typed A/S/I, boolean, requirement or fixture forms must use those canonical forms. Facts whose type is token must copy the exact canonical token or exact source phrase required by the task from the supplied authority; do not paraphrase, expand or add parentheticals. Verdict policy: REJECT when the proposition, assumption or promise under review is contradicted by the authority or a causal blocker is established against it; REPORT only for neutral recovery/reporting when no proposition is rejected. blockers and evidence contain only identifiers allowed by the response contract. Optional prose is forbidden.",
        "provider_adapter":{
            "command":["python3","scripts/dw04-openrouter-luna-adapter.py"],
            "script_path":"scripts/dw04-openrouter-luna-adapter.py",
            "script_sha256":sha256((ROOT/"scripts/dw04-openrouter-luna-adapter.py").read_bytes()),
        },
        "scorer_sha256":sha256((ROOT/"scripts/dw04-trial.py").read_bytes()),
        "pair_seeds":{"R1":None,"R2":None,"R3":None},
        "matched_run_policy":"R1 CTX→DW; R2 DW→CTX; R3 CTX→DW. Stable provider seed is not asserted; null run identities are frozen and both sides use identical effective settings.",
        "slots":slots,
        "context_plan":plan,
        "context_reduction_min":0.30,
        "correctness":"all_36",
        "timeout_seconds":120,
        "invalid_run_policy":"Each designated acceptance slot may receive at most one replacement only when the adapter exits 3 for an objective provider/transport failure before a scorable structured answer. Exit 2, parseable answers and semantic oracle misses are never retryable. Invalid receipts remain attached to that slot.",
        "proof_budget":{"tasks":6,"pairs_per_task":3,"scorable_executions":36,"semantic_reruns":0,"objective_invalid_replacements_per_slot":1},
        "inconclusive_policy":"STOP, frozen objective-invalid completion only, or one independently reviewed full restart under WP-DW-04; no selective semantic reruns or task replacement.",
        "normalization":"Exact canonical facts; blockers/evidence compared as sets; exact verdict; prose absent/unscored.",
    }
    return freeze


def table_excerpt(text, selector):
    lines = text.splitlines()
    matches = [i for i,line in enumerate(lines) if selector in line and line.lstrip().startswith("|")]
    if len(matches) != 1:
        raise trial.ProtocolError(f"table selector {selector!r} has {len(matches)} matches")
    i = matches[0]
    sep = None
    for j in range(i-1, -1, -1):
        stripped = lines[j].strip()
        if stripped.startswith("|") and "---" in stripped:
            sep = j
            break
        if stripped.startswith("#"):
            break
    if sep is None or sep == 0 or not lines[sep-1].lstrip().startswith("|"):
        raise trial.ProtocolError(f"table header not found for {selector}")
    return "\n".join([lines[sep-1], lines[sep], lines[i]])


def line_excerpt(text, selector):
    matches = [line for line in text.splitlines() if selector in line]
    if len(matches) != 1:
        raise trial.ProtocolError(f"line selector {selector!r} has {len(matches)} matches")
    return matches[0]


def section_excerpt(text, selector):
    lines = text.splitlines()
    starts = []
    for i,line in enumerate(lines):
        stripped = line.lstrip()
        if stripped.startswith("#") and selector in line:
            starts.append(i)
    if len(starts) != 1:
        raise trial.ProtocolError(f"section selector {selector!r} has {len(starts)} heading matches")
    start = starts[0]
    level = len(lines[start]) - len(lines[start].lstrip("#"))
    end = len(lines)
    for i in range(start+1, len(lines)):
        stripped = lines[i].lstrip()
        if stripped.startswith("#"):
            other = len(lines[i]) - len(lines[i].lstrip("#"))
            if other <= level:
                end = i
                break
    return "\n".join(lines[start:end]).strip()


def source_fragment(desc, pre):
    path = desc["source_path"]
    raw = (ROOT/path).read_text(encoding="utf-8")
    current_blob = git("rev-parse", f"HEAD:{path}").decode().strip()
    if current_blob != pre["authority_blobs"][path]:
        raise trial.ProtocolError(f"authority blob drift: {path}")
    fid = desc["id"]
    if "-table-" in fid:
        text = table_excerpt(raw, desc["selector"])
    elif "-line-" in fid:
        text = line_excerpt(raw, desc["selector"])
    elif "-section-" in fid:
        text = section_excerpt(raw, desc["selector"])
    else:
        raise trial.ProtocolError(f"unknown source-excerpt strategy: {fid}")
    return {"id":fid,"source_path":path,"source_blob":current_blob,"origin":"source_excerpt","selector":desc["selector"],"text":text}


def query_fragment(desc, pre):
    args = desc["query_args"]
    command = ["dotnet","run","--project",RETRIEVAL_PROJECT,"-c","Release","--no-build","--",str(ROOT),*args]
    first = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    second = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    if first != second:
        raise trial.ProtocolError(f"DW query nondeterministic: {args}")
    data = json.loads(first)
    actual_path = data.get("Provenance",{}).get("SourcePath")
    if actual_path != desc["source_path"]:
        raise trial.ProtocolError(f"DW query provenance path drift for {args}: {actual_path}")
    blob = git("rev-parse", f"HEAD:{actual_path}").decode().strip()
    if blob != pre["authority_blobs"][actual_path]:
        raise trial.ProtocolError(f"DW query authority blob drift: {actual_path}")
    return {"id":desc["id"],"source_path":actual_path,"source_blob":blob,"origin":"dw_query","selector":desc["selector"],"text":first,"query_receipt":{"args":args,"output_sha256":sha256(first.encode("utf-8"))}}


def make_assembly(freeze_commit):
    freeze, freeze_digest = trial.frozen_file(freeze_commit, trial.FREEZE)
    pre, _ = trial.frozen_file(PRE_COMMIT, trial.PRE)
    selected = trial.precheck(pre, pre["baseline_sha"])
    if freeze["selected"] != selected:
        raise trial.ProtocolError("freeze selection drift")
    contexts = {}
    for task in selected:
        contexts[task] = {}
        for route in ("CTX","DW"):
            fragments = []
            for desc in freeze["context_plan"][task][route]:
                fragments.append(query_fragment(desc, pre) if desc["origin"] == "dw_query" else source_fragment(desc, pre))
            contexts[task][route] = fragments
    assembly = {"schema":"dw04-context-assembly-v1","precalibration_commit":PRE_COMMIT,"freeze_commit":freeze_commit,"freeze_sha256":freeze_digest,"contexts":contexts}
    structural = trial.context_check(freeze, assembly, pre, selected)
    ctx = [sum(len(x["text"].encode("utf-8")) for x in contexts[t]["CTX"]) for t in selected]
    dw = [sum(len(x["text"].encode("utf-8")) for x in contexts[t]["DW"]) for t in selected]
    med_ctx = statistics.median(ctx); med_dw = statistics.median(dw)
    saving = 1 - med_dw/med_ctx if med_ctx else 0
    assembly["pre_execution_measurement"] = {"structural":structural,"per_task_ctx_bytes":dict(zip(selected,ctx)),"per_task_dw_bytes":dict(zip(selected,dw)),"median_ctx_bytes":med_ctx,"median_dw_bytes":med_dw,"saving":saving,"threshold":freeze["context_reduction_min"]}
    if not structural:
        raise trial.ProtocolError("structural context completeness RED before model execution")
    return assembly


def main():
    parser = argparse.ArgumentParser()
    sub = parser.add_subparsers(dest="mode", required=True)
    p1=sub.add_parser("freeze"); p1.add_argument("--output",required=True)
    p2=sub.add_parser("assemble"); p2.add_argument("--freeze-commit",required=True); p2.add_argument("--output",required=True)
    args=parser.parse_args()
    value = make_freeze() if args.mode=="freeze" else make_assembly(args.freeze_commit)
    write_new(args.output,value)
    if args.mode=="assemble":
        m=value["pre_execution_measurement"]
        print(json.dumps(m,ensure_ascii=False))
        if m["saving"] < m["threshold"]:
            print("DW04 PRE-ACCEPTANCE RED: deterministic context reduction below frozen threshold; no model calls authorized", file=sys.stderr)
            return 4
    else:
        print(json.dumps({"selected":value["selected"],"slots":len(value["slots"]),"calibration_commit":value["calibration_commit"]}))
    return 0


if __name__=="__main__":
    try:
        raise SystemExit(main())
    except (trial.ProtocolError, subprocess.CalledProcessError, json.JSONDecodeError, OSError) as exc:
        print("DW04 PREP BLOCKED: "+str(exc), file=sys.stderr)
        raise SystemExit(1)
