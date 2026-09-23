#!/usr/bin/env python3
"""Assemble frozen DW-04 acceptance CTX/DW contexts after TASK_SELECTION_FREEZE.

No model is called here. Recipes are fixed before acceptance execution and only
read accepted source bytes / the accepted typed retrieval adapter.
"""

import argparse
import hashlib
import json
import pathlib
import subprocess
import sys

from importlib.machinery import SourceFileLoader

ROOT = pathlib.Path(__file__).resolve().parents[1]
trial = SourceFileLoader("dw04_trial", str(ROOT / "scripts/dw04-trial.py")).load_module()

PROGRAMME = "Docs/production/CITY_LOCATION_PROGRAMME.md"
MOBILITY = "Docs/production/CITY_MOBILITY_TOPOLOGY.md"
PA01 = "Docs/research/living-world/results/PA-01.md"
PA04 = "Docs/research/living-world/results/PA-04.md"
PA05 = "Docs/research/living-world/results/PA-05.md"
PA05_FIX = "Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md"


def desc(id_, path, origin, selector, args=None):
    return {"id": id_, "source_path": path, "origin": origin, "selector": selector, "query_args": args}


PLANS = {
    "A-CITY-01": {
        "CTX": [desc("ctx-city01-programme", PROGRAMME, "source_excerpt",
                     "| `loc.casco.shared_court` | Casco Viejo | passage + shared court / stair relation |")],
        "DW": [desc("dw-city01-record", PROGRAMME, "dw_query", "loc.casco.shared_court",
                    ["city", "loc.casco.shared_court"])],
    },
    "A-CITY-02": {
        "CTX": [
            desc("ctx-city02-programme", PROGRAMME, "source_excerpt",
                 "| `loc.puerto.landing` | Puerto Fluvial | X6/X7 landing"),
            desc("ctx-city02-x6", MOBILITY, "source_excerpt",
                 "`AX6` — State-1 ferry: pedestrian/porter/carryable load only"),
        ],
        "DW": [
            desc("dw-city02-record", PROGRAMME, "dw_query", "loc.puerto.landing",
                 ["city", "loc.puerto.landing"]),
            desc("dw-city02-x6-fallback", MOBILITY, "source_excerpt",
                 "`AX6` — State-1 ferry: pedestrian/porter/carryable load only"),
        ],
    },
    "A-CITY-03": {
        "CTX": [desc("ctx-city03-programme", PROGRAMME, "source_excerpt",
                     "| `loc.plaza.ayuntamiento` | Plaza y Ayuntamiento | town hall / civic office |")],
        "DW": [desc("dw-city03-record", PROGRAMME, "dw_query", "loc.plaza.ayuntamiento",
                    ["city", "loc.plaza.ayuntamiento"])],
    },
    "A-PA-01": {
        "CTX": [desc("ctx-pa01-pa04-review-surface", PA04, "source_excerpt",
                     "actor-visible `isTrue/isFalse/isStale` | **REJECT**")],
        "DW": [
            desc("dw-pa01-nc02", PA04, "dw_query", "NC-02", ["pa-fixture", "pa04", "NC-02"]),
            desc("dw-pa01-dispositions-fallback", PA04, "source_excerpt",
                 "actor-visible `isTrue/isFalse/isStale` | **REJECT**"),
        ],
    },
    "A-PA-02": {
        "CTX": [
            desc("ctx-pa02-pa05-disposition", PA05, "source_excerpt",
                 "Recursive hidden lineage embedded in ActorBelief | `REJECT`"),
            desc("ctx-pa02-fixture-ownership", PA05_FIX, "source_excerpt",
                 "PA-04 RECEIVER EPISTEMIC BOUNDARY"),
        ],
        "DW": [
            desc("dw-pa02-nc02", PA05_FIX, "dw_query", "NC-02", ["pa-fixture", "pa05", "NC-02"]),
            desc("dw-pa02-pa05-disposition-fallback", PA05, "source_excerpt",
                 "Recursive hidden lineage embedded in ActorBelief | `REJECT`"),
            desc("dw-pa02-ownership-fallback", PA05_FIX, "source_excerpt",
                 "PA-04 RECEIVER EPISTEMIC BOUNDARY"),
        ],
    },
    "A-PA-03": {
        "CTX": [desc("ctx-pa03-disposition-section", PA01, "source_excerpt",
                     "| DL-11 | exact physical off-screen path simulation | **REJECT**")],
        "DW": [
            desc("dw-pa03-dl11", PA01, "dw_query", "DL-11", ["pa-finding", "pa01", "DL-11"]),
            desc("dw-pa03-dl11-14-fallback", PA01, "source_excerpt",
                 "| DL-11 | exact physical off-screen path simulation | **REJECT**"),
        ],
    },
}


def read(path):
    return (ROOT / path).read_text(encoding="utf-8")


def through_line(source, start_marker, end_marker):
    start = source.find(start_marker)
    trial.require(start >= 0, f"excerpt start marker missing: {start_marker}")
    end_start = source.find(end_marker, start)
    trial.require(end_start >= 0, f"excerpt end marker missing: {end_marker}")
    end = source.find("\n", end_start)
    if end < 0:
        end = len(source)
    else:
        end += 1
    return source[start:end].rstrip("\n")


def exact_lines(source, first_marker, last_marker):
    return through_line(source, first_marker, last_marker)


def source_excerpt(task, frag_id):
    if frag_id == "ctx-city01-programme":
        return through_line(read(PROGRAMME), "### 3.1 Systemic importance — `A..D`",
                            "| `loc.casco.shared_court` | Casco Viejo | passage + shared court / stair relation |")
    if frag_id == "ctx-city02-programme":
        return through_line(read(PROGRAMME), "### 3.1 Systemic importance — `A..D`",
                            "| `loc.puerto.landing` | Puerto Fluvial | X6/X7 landing")
    if frag_id in ("ctx-city02-x6", "dw-city02-x6-fallback"):
        return through_line(read(MOBILITY), "- `AX6` — State-1 ferry: pedestrian/porter/carryable load only",
                            "| `X6` | `W.LANDING ↔ O.PUERTO_UP` | **State 1 only**")
    if frag_id == "ctx-city03-programme":
        return through_line(read(PROGRAMME), "### 3.1 Systemic importance — `A..D`",
                            "| `loc.plaza.ayuntamiento` | Plaza y Ayuntamiento | town hall / civic office |")
    if frag_id == "ctx-pa01-pa04-review-surface":
        return through_line(read(PA04), "| Mechanism | Status | Juego2 recommendation |",
                            "material decision = unchanged")
    if frag_id == "dw-pa01-dispositions-fallback":
        return exact_lines(read(PA04), "| actor-visible `isTrue/isFalse/isStale` | **REJECT**",
                           "| automatic truth→belief synchronization | **REJECT**")
    if frag_id == "ctx-pa02-pa05-disposition" or frag_id == "dw-pa02-pa05-disposition-fallback":
        return exact_lines(read(PA05), "| Recursive hidden lineage embedded in ActorBelief | `REJECT`",
                           "| Recursive hidden lineage embedded in ActorBelief | `REJECT`")
    if frag_id == "ctx-pa02-fixture-ownership":
        return through_line(read(PA05_FIX), "## 1. Ownership diagram",
                            "Engine/debug trace is allowed to differ and report the different root topology.")
    if frag_id == "dw-pa02-ownership-fallback":
        return exact_lines(read(PA05_FIX), "PA-04 RECEIVER EPISTEMIC BOUNDARY",
                           "PA-04 RECEIVER EPISTEMIC BOUNDARY")
    if frag_id == "ctx-pa03-disposition-section":
        return through_line(read(PA01), "## 3. Donor → Juego2 disposition",
                            "| DL-14 | donor M9/M10/`WorldState`/schema/API ownership | **REJECT** as authority")
    if frag_id == "dw-pa03-dl11-14-fallback":
        return exact_lines(read(PA01), "| DL-11 | exact physical off-screen path simulation | **REJECT**",
                           "| DL-14 | donor M9/M10/`WorldState`/schema/API ownership | **REJECT** as authority")
    raise trial.ProtocolError(f"unhandled source excerpt recipe: {task}/{frag_id}")


def make_source_fragment(pre, task, d):
    text = source_excerpt(task, d["id"])
    trial.require(d["selector"] in text, f"selector missing from source excerpt: {d['id']}")
    trial.require(text in read(d["source_path"]), f"excerpt not contiguous accepted source: {d['id']}")
    return {
        "id": d["id"], "source_path": d["source_path"], "source_blob": pre["authority_blobs"][d["source_path"]],
        "origin": "source_excerpt", "selector": d["selector"], "text": text,
    }


def make_query_fragment(pre, d):
    args = d["query_args"]
    command = ["dotnet", "run", "--project", "tools/Arkus.Dw04.Retrieval", "-c", "Release", "--no-build", "--",
               str(ROOT), *args]
    first = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    second = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    trial.require(first == second, f"typed query nondeterministic: {args}")
    payload = json.loads(first)
    provenance = payload.get("Provenance") or {}
    trial.require(provenance.get("SourcePath") == d["source_path"], f"typed query source mismatch: {args}")
    trial.require(d["source_path"] in pre["authority_blobs"], f"typed query source outside accepted authority: {args}")
    return {
        "id": d["id"], "source_path": d["source_path"], "source_blob": pre["authority_blobs"][d["source_path"]],
        "origin": "dw_query", "selector": d["selector"], "text": first,
        "query_receipt": {"args": args, "output_sha256": hashlib.sha256(first.encode("utf-8")).hexdigest()},
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--pre-commit", required=True)
    parser.add_argument("--freeze-commit", required=True)
    parser.add_argument("--output", required=True)
    args = parser.parse_args()

    pre, _ = trial.frozen_file(args.pre_commit, trial.PRE)
    freeze, freeze_sha = trial.frozen_file(args.freeze_commit, trial.FREEZE)
    trial.ancestor(args.pre_commit, args.freeze_commit)
    selected = trial.precheck(pre, pre["baseline_sha"])
    trial.require(selected == list(PLANS), "assembly recipe task order differs from independent selection")
    trial.require(freeze["context_plan"] == PLANS, "frozen context plan differs from reviewed assembly recipes")

    contexts = {}
    for task in selected:
        contexts[task] = {}
        for route in ("CTX", "DW"):
            built = []
            for d in PLANS[task][route]:
                built.append(make_query_fragment(pre, d) if d["origin"] == "dw_query" else make_source_fragment(pre, task, d))
            contexts[task][route] = built

    result = {
        "schema": "dw04-context-assembly-v1",
        "precalibration_commit": args.pre_commit,
        "freeze_commit": args.freeze_commit,
        "freeze_sha256": freeze_sha,
        "contexts": contexts,
    }
    trial.require(trial.context_check(freeze, result, pre, selected), "assembled contexts fail structural completeness")
    out = pathlib.Path(args.output)
    trial.require(not out.exists(), "context assembly output already exists")
    out.parent.mkdir(parents=True, exist_ok=True)
    out.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")

    sizes = {task: {route: sum(len(f["text"].encode("utf-8")) for f in contexts[task][route])
                    for route in ("CTX", "DW")} for task in selected}
    print(json.dumps({"structural": True, "bytes": sizes}, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, OSError, subprocess.CalledProcessError, json.JSONDecodeError) as exc:
        print("DW04 ASSEMBLY BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
