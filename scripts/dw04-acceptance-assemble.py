#!/usr/bin/env python3
"""Build the post-freeze DW-04 acceptance context assembly deterministically."""
import argparse, hashlib, json, pathlib, subprocess, sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
ASSEMBLY_PATH = "Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"
FREEZE_PATH = "Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"

class ProtocolError(ValueError): pass

def require(c, m):
    if not c: raise ProtocolError(m)

def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, stderr=subprocess.PIPE)

def load_at(commit, path):
    return json.loads(git("show", f"{commit}:{path}"))

def blob_at(commit, path):
    return git("rev-parse", f"{commit}:{path}").decode().strip()

def source_at(commit, path):
    return git("show", f"{commit}:{path}").decode("utf-8")

def sha(data):
    return hashlib.sha256(data.encode("utf-8")).hexdigest()

def extract_source(text, spec):
    mode = spec["mode"]
    if mode == "line_contains":
        matches = [line for line in text.splitlines() if spec["needle"] in line]
        require(len(matches) == 1, f"source selector not unique: {spec['needle']}")
        return matches[0]
    if mode == "between":
        start = text.find(spec["start"])
        require(start >= 0, f"source start missing: {spec['start']}")
        end = text.find(spec["end"], start + len(spec["start"]))
        require(end > start, f"source end missing: {spec['end']}")
        return text[start:end].rstrip()
    raise ProtocolError(f"unknown extraction mode: {mode}")

def query(args):
    command = ["dotnet", "run", "--project", "tools/Arkus.Dw04.Retrieval", "-c", "Release", "--no-build", "--", str(ROOT), *args]
    a = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    b = subprocess.check_output(command, cwd=ROOT).decode("utf-8").strip()
    require(a == b, f"DW query is nondeterministic: {args}")
    parsed = json.loads(a)
    return a, parsed

def materialize(item, baseline_sha, authority_blobs):
    path = item["source_path"]
    require(authority_blobs.get(path) == blob_at(baseline_sha, path), f"authority blob drift: {path}")
    if item["origin"] == "source_excerpt":
        text = extract_source(source_at(baseline_sha, path), item["extract"])
        require(item["selector"] in text, f"selector absent from excerpt: {item['id']}")
        return {"id":item["id"],"source_path":path,"source_blob":authority_blobs[path],"origin":"source_excerpt","selector":item["selector"],"text":text}
    require(item["origin"] == "dw_query", f"unknown context origin: {item['origin']}")
    text, parsed = query(item["query_args"])
    provenance = parsed.get("Provenance") or {}
    require(provenance.get("SourcePath") == path, f"DW query provenance source mismatch: {item['id']}")
    return {"id":item["id"],"source_path":path,"source_blob":authority_blobs[path],"origin":"dw_query","selector":item["selector"],"text":text,"query_receipt":{"args":item["query_args"],"output_sha256":sha(text),"provenance":provenance}}

def build(freeze_commit):
    freeze = load_at(freeze_commit, FREEZE_PATH)
    require(blob_at(freeze_commit, FREEZE_PATH) == blob_at("HEAD", FREEZE_PATH), "acceptance freeze changed after anchor")
    pre = load_at(freeze["precalibration_commit"], "Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json")
    require(freeze["selected"] == ["A-CITY-01","A-CITY-02","A-CITY-03","A-PA-01","A-PA-02","A-PA-03"], "selected suite drift")
    contexts = {}
    for task in freeze["selected"]:
        contexts[task] = {}
        for route in ("CTX","DW"):
            contexts[task][route] = [materialize(item, pre["baseline_sha"], pre["authority_blobs"]) for item in freeze["context_plan"][task][route]]
    return {"schema":"dw04-context-assembly-v2","freeze_commit":freeze_commit,"freeze_blob":blob_at(freeze_commit,FREEZE_PATH),"baseline_sha":pre["baseline_sha"],"contexts":contexts}

def main():
    p=argparse.ArgumentParser(); p.add_argument("--freeze-commit",required=True); p.add_argument("--output",required=True); a=p.parse_args()
    out=pathlib.Path(a.output); require(not out.exists(), "assembly output already exists")
    value=build(a.freeze_commit); out.parent.mkdir(parents=True,exist_ok=True); out.write_text(json.dumps(value,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
    print("DW04_ACCEPTANCE_ASSEMBLY_READY")
if __name__=="__main__":
    try: main()
    except (ProtocolError,subprocess.CalledProcessError,OSError,KeyError,ValueError) as e:
        print("DW04 ASSEMBLY BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
