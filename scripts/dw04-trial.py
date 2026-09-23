#!/usr/bin/env python3
"""DW-04 experiment guards. No model answer is an expected-answer oracle.

The untrusted transcript is scored against a Git-anchored, pre-result freeze. This
module does not create model results; actual provider calls and route assembly are
separate evidence and must be present before an acceptance disposition is possible.
"""

import argparse
import hashlib
import json
import pathlib
import statistics
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
PRE = "Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json"
CAL_ORACLES = "Docs/evidence/WP-DW-04/CALIBRATION_ORACLES.json"
CAL_CONTEXT = "Docs/evidence/WP-DW-04/CALIBRATION_CONTEXT.json"
CAL_PROTOCOL = "Docs/evidence/WP-DW-04/CALIBRATION_PROTOCOL.json"
SOURCE_ORACLES = "Docs/evidence/WP-DW-04/ACCEPTANCE_SOURCE_ORACLES.json"
CAL_RESULTS = "Docs/evidence/WP-DW-04/CALIBRATION_RESULTS.json"
FREEZE = "Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"
ASSEMBLY = "Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"


class ProtocolError(ValueError):
    pass


def canonical(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"), ensure_ascii=False).encode("utf-8")


def digest(value):
    return hashlib.sha256(value).hexdigest()


def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, stderr=subprocess.PIPE)


def require(condition, reason):
    if not condition:
        raise ProtocolError(reason)


def frozen_file(commit, path):
    require(len(commit) == 40 and all(c in "0123456789abcdef" for c in commit), "exact 40-character freeze commit required")
    try:
        raw = git("show", f"{commit}:{path}")
        frozen_blob = git("rev-parse", f"{commit}:{path}").decode().strip()
        current_blob = git("rev-parse", f"HEAD:{path}").decode().strip()
        git("merge-base", "--is-ancestor", commit, "HEAD")
    except subprocess.CalledProcessError as exc:
        raise ProtocolError("freeze must exist in candidate Git ancestry at canonical path") from exc
    require(current_blob == frozen_blob, f"freeze changed after committed anchor: {path}")
    return json.loads(raw), digest(raw)


def ancestor(earlier, later):
    try:
        git("merge-base", "--is-ancestor", earlier, later)
    except subprocess.CalledProcessError as exc:
        raise ProtocolError(f"freeze chronology violated: {earlier} is not an ancestor of {later}") from exc


def precheck(pre, source_commit):
    require(pre["schema"] == "dw04-precalibration-v1", "pre-freeze schema")
    tasks = pre["eligible_tasks"]
    ids = [task["id"] for task in tasks]
    require(len(ids) == len(set(ids)) == 12, "universe must contain 12 unique tasks")
    cal = sorted(task["id"] for task in tasks if task["partition"] == "calibration")
    pool = sorted(task["id"] for task in tasks if task["partition"] == "acceptance_pool")
    require(len(cal) == 4 and len(pool) == 8, "partition cardinality")
    require(cal == sorted(pre["calibration_policy"]["run_order"]), "calibration order/set drift")
    selected = sorted(x for x in pool if x.startswith("A-CITY-"))[:3] + sorted(x for x in pool if x.startswith("A-PA-"))[:3]
    require(selected == ["A-CITY-01", "A-CITY-02", "A-CITY-03", "A-PA-01", "A-PA-02", "A-PA-03"], "selection rule drift")
    require(pre["calibration_policy"]["runs_per_task"] == 2, "calibration budget drift")
    for path, expected in pre["authority_blobs"].items():
        require(path.startswith("Docs/") and ".." not in pathlib.PurePosixPath(path).parts, "authority path escapes repository")
        actual = git("rev-parse", f"{source_commit}:{path}").decode().strip()
        require(actual == expected, f"accepted authority blob changed: {path}")
        current = git("rev-parse", f"HEAD:{path}").decode().strip()
        require(current == expected, f"candidate substituted accepted authority bytes: {path}")
    for task in tasks:
        require(task["kind"] in ("worker", "reviewer") and task["anchors"], "task kind/anchors")
        for anchor in task["anchors"]:
            require(anchor.partition("#")[0] in pre["authority_blobs"] and anchor.partition("#")[2], f"unbound authority: {anchor}")
    return selected


def score(oracle, answer):
    """Compare canonical structured claims exactly; prose is never examined."""
    require(set(oracle) == {"facts", "blockers", "verdict", "evidence"}, "frozen oracle shape")
    if not isinstance(answer, dict):
        return {"pass": False, "errors": ["missing structured answer"]}
    errors = []
    facts = answer.get("facts")
    if not isinstance(facts, dict):
        errors.append("facts shape")
    else:
        for key, expected in oracle["facts"].items():
            if key not in facts or facts[key] != expected:
                errors.append("fact:" + key)
        for key in set(facts) - set(oracle["facts"]):
            errors.append("unfrozen fact:" + key)
    for field in ("blockers", "evidence"):
        actual = answer.get(field)
        expected = oracle[field]
        if not isinstance(actual, list) or len(actual) != len(set(map(str, actual))) or set(map(str, actual)) != set(expected):
            errors.append(field + " mismatch")
    if answer.get("verdict") != oracle["verdict"]:
        errors.append("verdict mismatch")
    return {"pass": not errors, "errors": errors}


def make_slots(tasks):
    return [{"task": task, "pair": f"R{pair}", "route": route}
            for task in tasks for pair in (1, 2, 3)
            for route in (("CTX", "DW") if pair != 2 else ("DW", "CTX"))]


def check_request(request, config, slot, prompt, response_contract, system_prompt, seed):
    common = {"provider", "model", "version", "temperature", "thinking", "tool_policy",
              "execution_budget", "run_policy", "provider_options"}
    require(set(config) == common, "model/configuration freeze has undeclared or missing dimensions")
    require(set(request) == common | {"task_prompt", "response_contract", "system_prompt", "slot", "context_fragments", "seed"},
            "provider request has undeclared or missing configuration")
    for field in common:
        require(request.get(field) == config.get(field), f"paired model/configuration/run policy drift: {field}")
    require(request["seed"] == seed, "matched seed/run identity mismatch")
    require(request["task_prompt"] == prompt and request["system_prompt"] == system_prompt,
            "system/task prompt drift")
    require(request["response_contract"] == response_contract, "response contract drift")
    require(request["slot"] == slot, "provider request slot drift")


def calibration_check(pre, pre_commit, freeze):
    oracle_commit = freeze["calibration_oracle_commit"]
    result_commit = freeze["calibration_commit"]
    ancestor(pre_commit, oracle_commit)
    ancestor(oracle_commit, result_commit)
    oracles, _ = frozen_file(oracle_commit, CAL_ORACLES)
    results, results_sha = frozen_file(result_commit, CAL_RESULTS)
    protocol_commit = results.get("calibration_protocol_commit")
    require(isinstance(protocol_commit, str), "calibration protocol commit missing from results")
    ancestor(oracle_commit, protocol_commit)
    ancestor(protocol_commit, result_commit)
    protocol, _ = frozen_file(protocol_commit, CAL_PROTOCOL)
    context, _ = frozen_file(protocol_commit, CAL_CONTEXT)
    require(oracles["precalibration_commit"] == pre_commit and oracles["route"] == "CTX_ONLY", "calibration oracle lineage/route")
    require(protocol["precalibration_commit"] == pre_commit and protocol["calibration_oracle_commit"] == oracle_commit,
            "calibration protocol lineage mismatch")
    require(context["precalibration_commit"] == pre_commit and context["route"] == "CTX", "calibration context lineage/route")
    task_ids = set(pre["calibration_policy"]["run_order"])
    require(set(oracles["tasks"]) == set(protocol["tasks"]) == set(context["contexts"]) == task_ids,
            "calibration task universe mismatch")
    require(results_sha == freeze["calibration_receipt_digest"], "calibration result changed after acceptance freeze")
    require(results.get("precalibration_commit") == pre_commit and results.get("calibration_oracle_commit") == oracle_commit,
            "calibration input freeze mismatch")
    require(results.get("calibration_protocol_commit") == protocol_commit, "calibration protocol identity mismatch")
    slots = [{"task": task, "run": i, "route": "CTX"}
             for task in pre["calibration_policy"]["run_order"] for i in (1, 2)]
    require([run.get("slot") for run in results["runs"]] == slots, "CTX calibration count/order/partition violation")
    require(results.get("readiness") == "READY", "calibration results are not READY")
    require(results["model_config"] == protocol["model_config"] == freeze["model_config"],
            "acceptance protocol differs from ready calibration")
    for run in results["runs"]:
        task = run["slot"]["task"]
        require(run.get("provider_request_id") and run.get("model") == freeze["model_config"]["model"],
                "calibration model call identity/model missing")
        require(run.get("route") == "CTX", "calibration used DW route")
        request = run.get("request", {})
        task_protocol = protocol["tasks"][task]
        check_request(request, freeze["model_config"], run["slot"], task_protocol["semantic_question"],
                      task_protocol["response_contract"], protocol["system_prompt"], None)
        require(request["context_fragments"] == context["contexts"][task],
                "calibration context differs from frozen CTX context")
        require(score(oracles["tasks"][task]["oracle"], run.get("answer"))["pass"],
                "CTX calibration did not meet all-eight readiness criterion")
    return results_sha


def decide(pair_rows, structural, savings):
    if not structural or any(row["CTX"]["pass"] and not row["DW"]["pass"] for row in pair_rows):
        return "FAIL"
    if any(not row["CTX"]["pass"] or not row["DW"]["pass"] for row in pair_rows):
        return "INCONCLUSIVE"
    return "PASS" if savings >= 0.30 else "FAIL"


def context_check(freeze, assembly, pre, selected):
    """Replay the actual typed-query path and compare required source literals."""
    require(set(assembly["contexts"]) == set(selected), "context task universe changed")
    require(set(freeze["context_plan"]) == set(selected), "context-plan task universe changed")
    queries = {}
    complete = True
    task_index = {t["id"]: t for t in pre["eligible_tasks"]}
    for task in selected:
        anchors = {a.partition("#")[0] for a in task_index[task]["anchors"]}
        require(set(assembly["contexts"][task]) == set(freeze["context_plan"][task]) == {"CTX", "DW"},
                "missing or extra context route")
        for route in ("CTX", "DW"):
            context = assembly["contexts"][task][route]
            require(isinstance(context, list) and context, "empty route context")
            described = [{"id": f.get("id"), "source_path": f.get("source_path"),
                          "origin": f.get("origin"), "selector": f.get("selector"),
                          "query_args": f.get("query_receipt", {}).get("args")}
                         for f in context]
            require(described == freeze["context_plan"][task][route], "context source/query plan changed after freeze")
            require({f["source_path"] for f in context} <= anchors, "baseline inflated with unrelated authority")
            if route == "CTX":
                for path in anchors:
                    supplied = sum(len(f["text"].encode()) for f in context if f["source_path"] == path)
                    available = len(git("show", f"{pre['baseline_sha']}:{path}"))
                    require(supplied < available / 2, "CTX baseline inflated with a largely unrelated full document")
            for f in context:
                path = f["source_path"]
                require(path in pre["authority_blobs"] and f.get("source_blob") == pre["authority_blobs"][path]
                        and f.get("id") and isinstance(f.get("text"), str), "unbound source/context fragment")
                source = git("show", f"{pre['baseline_sha']}:{path}").decode("utf-8")
                if f.get("origin") == "source_excerpt":
                    require(f.get("selector") and f["selector"] in f["text"] and f["text"] in source,
                            "source excerpt is outside frozen selector or accepted source bytes")
                else:
                    require(f.get("origin") == "dw_query" and route == "DW" and f.get("query_receipt"),
                            "unverified non-source fragment")
                    query = f["query_receipt"]
                    argv = query.get("args", [])
                    require(argv and argv[0] in ("city", "pa-fixture", "pa-finding", "pa-disposition") and
                            digest(f["text"].encode()) == query.get("output_sha256"), "typed query receipt/context mismatch")
                    key = tuple(argv)
                    if key not in queries:
                        command = ["dotnet", "run", "--project", "tools/Arkus.Dw04.Retrieval", "-c", "Release",
                                   "--", str(ROOT), *argv]
                        try:
                            a = subprocess.check_output(command, cwd=ROOT).decode().strip()
                            b = subprocess.check_output(command, cwd=ROOT).decode().strip()
                        except (OSError, subprocess.CalledProcessError) as exc:
                            raise ProtocolError("real DW production query path unavailable; no structural PASS") from exc
                        require(a == b, "DW query changed across repeated assembly")
                        queries[key] = a
                    require(f["text"] == queries[key], "DW context does not equal accepted typed query output")
            for item in freeze["tasks"][task]["required_context"]:
                require(item["source_path"] in pre["authority_blobs"] and item["literal"] and
                        item["literal"] in git("show", f"{pre['baseline_sha']}:{item['source_path']}").decode(),
                        "expected context literal is not independent accepted source truth")
                if not any(f["source_path"] == item["source_path"] and item["literal"] in f["text"] for f in context):
                    complete = False
    return complete


def audit(freeze, transcript, pre, pre_commit, freeze_commit, freeze_digest):
    ancestor(pre_commit, freeze_commit)
    selected = precheck(pre, pre["baseline_sha"])
    require(freeze["schema"] == "dw04-acceptance-v1", "acceptance freeze schema")
    require(freeze["selected"] == selected and len(freeze["tasks"]) == 6, "selection changed after pre-freeze")
    require(list(freeze["tasks"]) == selected, "task manifest order/identity")
    ancestor(pre_commit, freeze["acceptance_oracle_commit"])
    ancestor(freeze["acceptance_oracle_commit"], freeze["calibration_commit"])
    source_oracles, _ = frozen_file(freeze["acceptance_oracle_commit"], SOURCE_ORACLES)
    require(source_oracles["precalibration_commit"] == pre_commit, "source oracle lineage")
    require(list(source_oracles["tasks"]) == selected, "pre-result source oracle task universe")
    for task in selected:
        for field in ("oracle", "required_context"):
            require(freeze["tasks"][task][field] == source_oracles["tasks"][task][field],
                    f"oracle or structural requirement modified after calibration/results: {task}/{field}")
        require("response_contract" in freeze["tasks"][task], f"missing frozen response contract: {task}")
    vocab = {(tuple(t["response_contract"]["allowed_blockers"]),
              tuple(t["response_contract"]["allowed_verdicts"]),
              tuple(t["response_contract"]["evidence_ids"])) for t in freeze["tasks"].values()}
    require(len(vocab) == 1, "task-specific acceptance response vocabulary leaks expected semantics")
    require(freeze["slots"] == make_slots(selected), "18 matched pairs/36 slot order changed")
    require(set(freeze["pair_seeds"]) == {"R1", "R2", "R3"}, "three matched seeds/identities required")
    require(freeze["precalibration_commit"] == pre_commit, "pre-calibration identity changed")
    require(freeze["scorer_sha256"] == digest((ROOT / "scripts/dw04-trial.py").read_bytes()), "scorer modified after freeze")
    require(freeze["context_reduction_min"] == 0.30 and freeze["correctness"] == "all_36", "threshold weakened")
    require(freeze["calibration_status"] == "READY" and freeze["calibration_receipt_digest"], "CTX calibration absent")
    calibration_check(pre, pre_commit, freeze)
    ancestor(freeze["calibration_commit"], freeze_commit)
    require(len(transcript) == 36, "missing, added or adaptively rerun acceptance execution")
    require([x.get("slot") for x in transcript] == freeze["slots"], "slot order or identity changed")
    assembly_commits = {x.get("assembly_commit") for x in transcript}
    require(len(assembly_commits) == 1 and next(iter(assembly_commits)), "acceptance contexts have multiple/missing assembly versions")
    assembly_commit = next(iter(assembly_commits))
    ancestor(freeze_commit, assembly_commit)
    assembly, assembly_sha = frozen_file(assembly_commit, ASSEMBLY)
    require(assembly["freeze_commit"] == freeze_commit and assembly["freeze_sha256"] == freeze_digest,
            "context assembly predates or disagrees with the task-selection freeze")
    structural = context_check(freeze, assembly, pre, selected)
    require(all(x.get("freeze_commit") == freeze_commit and x.get("freeze_sha256") == freeze_digest for x in transcript),
            "run not bound to immutable pre-result freeze")
    require(all(x.get("assembly_sha256") == assembly_sha for x in transcript), "context assembly modified after runs")
    pairs, bytes_ctx, bytes_dw = {}, [], []
    for record in transcript:
        slot = record["slot"]
        task, pair, route = slot["task"], slot["pair"], slot["route"]
        require(record.get("request") and record.get("response"), "missing actual request/response")
        request = record["request"]
        check_request(request, freeze["model_config"], slot, freeze["tasks"][task]["prompt"],
                      freeze["tasks"][task]["response_contract"], freeze["system_prompt"], freeze["pair_seeds"][pair])
        context = request.get("context_fragments")
        require(isinstance(context, list) and context, "no auditable context fragments")
        require(context == assembly["contexts"][task][route], "context changed after frozen assembly")
        context_bytes = sum(len(f["text"].encode("utf-8")) for f in context)
        require(record.get("injected_source_bytes") == context_bytes, "source-open bytes excluded or inflated")
        require(record.get("request_sha256") == digest(canonical(request)), "effective provider request differs from logged request")
        response = record["response"]
        require(response.get("provider_request_id") and response.get("raw"), "no provider execution identity/raw response")
        require(response.get("model") == request["model"], "provider returned a different model")
        answer = response["raw"].get("answer")
        result = score(freeze["tasks"][task]["oracle"], answer)
        pairs.setdefault((task, pair), {})[route] = result
        (bytes_ctx if route == "CTX" else bytes_dw).append(context_bytes)
    pair_rows = [{"task": t, "pair": p, "CTX": sides["CTX"], "DW": sides["DW"]}
                 for (t, p), sides in sorted(pairs.items())]
    require(len(pair_rows) == 18 and all(len(row) == 4 for row in pair_rows), "pair completeness")
    median_ctx, median_dw = statistics.median(bytes_ctx), statistics.median(bytes_dw)
    savings = 1 - median_dw / median_ctx if median_ctx else 0
    disposition = decide(pair_rows, structural, savings)
    return {"disposition": disposition, "pairs": pair_rows, "median_ctx_bytes": median_ctx,
            "median_dw_bytes": median_dw, "saving": savings, "structural": structural}


def selftest():
    oracle = {"facts": {"f": "v"}, "blockers": ["b"], "verdict": "REJECT", "evidence": ["source"]}
    correct = {**oracle, "prose": "any writing style"}
    assert score(oracle, correct)["pass"]
    for mutation in ({**correct, "facts": {}}, {**correct, "blockers": []},
                     {**correct, "verdict": "ACCEPT"}, {**correct, "evidence": []},
                     {**correct, "facts": {"f": "wrong"}}):
        assert not score(oracle, mutation)["pass"]
    slots = make_slots(["A-CITY-01", "A-PA-01"])
    assert len(slots) == 12 and slots[2]["route"] == "DW"
    def pair(ctx, dw):
        return {"CTX": {"pass": ctx}, "DW": {"pass": dw}}
    assert decide([pair(True, False), pair(True, True), pair(True, True)], True, 0.8) == "FAIL"
    assert decide([pair(False, False)] * 3, True, 0.8) == "INCONCLUSIVE"
    assert decide([pair(True, True)] * 18, False, 0.8) == "FAIL"
    assert decide([pair(True, True)] * 18, True, 0.29) == "FAIL"
    assert decide([pair(True, True)] * 18, True, 0.30) == "PASS"
    config = {key: key for key in ("provider", "model", "version", "temperature", "thinking",
                                   "tool_policy", "execution_budget", "run_policy", "provider_options")}
    slot = {"task": "A-CITY-01", "pair": "R1", "route": "CTX"}
    contract = {"fact_keys": ["f"], "allowed_blockers": ["b"], "allowed_verdicts": ["REPORT", "REJECT"], "evidence_ids": ["e"]}
    request = {**config, "task_prompt": "prompt", "response_contract": contract, "system_prompt": "system", "slot": slot,
               "context_fragments": [], "seed": None}
    check_request(request, config, slot, "prompt", contract, "system", None)
    for field in config:
        changed = {**request, field: "different"}
        try:
            check_request(changed, config, slot, "prompt", contract, "system", None)
        except ProtocolError:
            pass
        else:
            raise AssertionError("unnoticed configuration drift: " + field)
    changed_contract = {**request, "response_contract": {**contract, "allowed_verdicts": ["REPORT"]}}
    try:
        check_request(changed_contract, config, slot, "prompt", contract, "system", None)
    except ProtocolError:
        pass
    else:
        raise AssertionError("unnoticed response-contract drift")
    print("DW04 scorer/slot causal controls: GREEN")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=("precheck", "audit", "selftest"))
    parser.add_argument("--pre-commit")
    parser.add_argument("--freeze-commit")
    parser.add_argument("--transcript")
    args = parser.parse_args()
    if args.mode == "selftest":
        selftest()
    elif args.mode == "precheck":
        require(args.pre_commit, "--pre-commit required")
        pre, sha = frozen_file(args.pre_commit, PRE)
        print(json.dumps({"selection": precheck(pre, pre["baseline_sha"]), "precalibration_sha256": sha}))
    else:
        require(args.pre_commit and args.freeze_commit and args.transcript, "both freezes and transcript required")
        pre, _ = frozen_file(args.pre_commit, PRE)
        freeze, sha = frozen_file(args.freeze_commit, FREEZE)
        raw = pathlib.Path(args.transcript).read_text()
        transcript = json.loads(raw) if raw.lstrip().startswith("[") else [json.loads(line) for line in raw.splitlines()]
        print(json.dumps(audit(freeze, transcript, pre, args.pre_commit, args.freeze_commit, sha), indent=2))


if __name__ == "__main__":
    try:
        main()
    except (ProtocolError, KeyError, TypeError, json.JSONDecodeError) as exc:
        print("DW04 RED: " + str(exc), file=sys.stderr)
        sys.exit(1)
