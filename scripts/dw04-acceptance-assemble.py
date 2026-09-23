#!/usr/bin/env python3
"""Build the post-freeze DW-04 acceptance context assembly deterministically."""
import argparse, hashlib, json, pathlib, subprocess, sys
ROOT=pathlib.Path(__file__).resolve().parents[1]
FREEZE="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"; PLAN="Docs/evidence/WP-DW-04/ACCEPTANCE_CONTEXT_PLAN.json"; PRE="Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json"
class ProtocolError(ValueError): pass
def require(c,m):
    if not c: raise ProtocolError(m)
def git(*a): return subprocess.check_output(["git",*a],cwd=ROOT,stderr=subprocess.PIPE)
def load(c,p): return json.loads(git("show",f"{c}:{p}"))
def blob(c,p): return git("rev-parse",f"{c}:{p}").decode().strip()
def source(c,p): return git("show",f"{c}:{p}").decode("utf-8")
def sha(s): return hashlib.sha256(s.encode()).hexdigest()
def extract(text,spec):
    if spec["mode"]=="line_contains":
        m=[x for x in text.splitlines() if spec["needle"] in x]; require(len(m)==1,f"selector not unique: {spec['needle']}"); return m[0]
    if spec["mode"]=="between":
        a=text.find(spec["start"]); require(a>=0,f"start missing: {spec['start']}"); b=text.find(spec["end"],a+len(spec["start"])); require(b>a,f"end missing: {spec['end']}"); return text[a:b].rstrip()
    raise ProtocolError("unknown extraction mode")
def query(args):
    cmd=["dotnet","run","--project","tools/Arkus.Dw04.Retrieval","-c","Release","--no-build","--",str(ROOT),*args]
    a=subprocess.check_output(cmd,cwd=ROOT).decode().strip(); b=subprocess.check_output(cmd,cwd=ROOT).decode().strip(); require(a==b,f"nondeterministic query: {args}"); return a,json.loads(a)
def materialize(item,baseline,authority):
    path=item["source_path"]; require(authority.get(path)==blob(baseline,path),f"authority drift: {path}")
    if item["origin"]=="source_excerpt":
        text=extract(source(baseline,path),item["extract"]); require(item["selector"] in text,f"selector absent: {item['id']}"); return {"id":item["id"],"source_path":path,"source_blob":authority[path],"origin":"source_excerpt","selector":item["selector"],"text":text}
    require(item["origin"]=="dw_query","unknown origin"); text,obj=query(item["query_args"]); prov=obj.get("Provenance") or {}; src=source(baseline,path); require(prov.get("SourcePath")==path and prov.get("SourceDigest")==sha(src),f"query provenance mismatch: {item['id']}"); return {"id":item["id"],"source_path":path,"source_blob":authority[path],"source_sha256":sha(src),"origin":"dw_query","selector":item["selector"],"text":text,"query_receipt":{"args":item["query_args"],"output_sha256":sha(text),"provenance":prov}}
def build(fc):
    f=load(fc,FREEZE); require(blob(fc,FREEZE)==blob("HEAD",FREEZE),"freeze drift"); require(blob(fc,PLAN)==f["context_plan_blob"]==blob("HEAD",PLAN),"context plan drift")
    p=load(fc,PLAN); pre=load(f["precalibration_commit"],PRE); require(p["selected"]==f["selected"],"plan selection drift")
    contexts={}
    for t in f["selected"]:
        contexts[t]={r:[materialize(i,pre["baseline_sha"],pre["authority_blobs"]) for i in p["context_plan"][t][r]] for r in ("CTX","DW")}
    return {"schema":"dw04-context-assembly-v3","freeze_commit":fc,"freeze_blob":blob(fc,FREEZE),"context_plan_blob":f["context_plan_blob"],"baseline_sha":pre["baseline_sha"],"contexts":contexts}
def main():
    q=argparse.ArgumentParser(); q.add_argument("--freeze-commit",required=True); q.add_argument("--output",required=True); a=q.parse_args(); out=pathlib.Path(a.output); require(not out.exists(),"output exists"); v=build(a.freeze_commit); out.parent.mkdir(parents=True,exist_ok=True); out.write_text(json.dumps(v,ensure_ascii=False,indent=2)+"\n"); print("DW04_ACCEPTANCE_ASSEMBLY_READY")
if __name__=="__main__":
    try: main()
    except (ProtocolError,subprocess.CalledProcessError,OSError,KeyError,ValueError) as e: print("DW04 ASSEMBLY BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
