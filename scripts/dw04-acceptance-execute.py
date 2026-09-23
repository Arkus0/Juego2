#!/usr/bin/env python3
"""Execute the frozen 36-call DW-04 acceptance schedule exactly once."""
import argparse, datetime, hashlib, json, os, pathlib, subprocess, sys
from importlib.machinery import SourceFileLoader
ROOT=pathlib.Path(__file__).resolve().parents[1]
trial=SourceFileLoader("dw04_trial",str(ROOT/"scripts/dw04-trial.py")).load_module()
audit=SourceFileLoader("dw04_audit",str(ROOT/"scripts/dw04-acceptance-audit.py")).load_module()
FREEZE="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"; ASSEMBLY="Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"
class ProtocolError(ValueError): pass
def req(c,m):
    if not c: raise ProtocolError(m)
def git(*a): return subprocess.check_output(["git",*a],cwd=ROOT,stderr=subprocess.PIPE)
def load(c,p): return json.loads(git("show",f"{c}:{p}"))
def blob(c,p): return git("rev-parse",f"{c}:{p}").decode().strip()
def now(): return datetime.datetime.now(datetime.timezone.utc).isoformat()
def main():
    p=argparse.ArgumentParser(); p.add_argument("--freeze-commit",required=True); p.add_argument("--assembly-commit",required=True); p.add_argument("--output",required=True); a=p.parse_args()
    campaign=os.environ.get("DW04_CAMPAIGN_ID"); req(campaign,"DW04_CAMPAIGN_ID required")
    f=load(a.freeze_commit,FREEZE); f["freeze_commit"]=a.freeze_commit; f["freeze_blob"]=blob(a.freeze_commit,FREEZE)
    req(blob(a.freeze_commit,"scripts/dw04-acceptance-execute.py")==f["executor_blob"]==blob("HEAD","scripts/dw04-acceptance-execute.py"),"executor drift")
    req(blob(a.freeze_commit,"scripts/dw04-openrouter-luna-adapter.py")==f["adapter_blob"]==blob("HEAD","scripts/dw04-openrouter-luna-adapter.py"),"adapter drift")
    assembly=audit.check_structure(f,a.assembly_commit)
    out=pathlib.Path(a.output); req(not out.exists(),"transcript already exists")
    adapter=ROOT/"scripts/dw04-openrouter-luna-adapter.py"; records=[]; fd=os.open(out,os.O_WRONLY|os.O_CREAT|os.O_EXCL,0o600)
    with os.fdopen(fd,"w",encoding="utf-8") as log:
        for slot in f["slots"]:
            task=f["tasks"][slot["task"]]; ctx=assembly["contexts"][slot["task"]][slot["route"]]
            request=dict(f["model_config"]); request.update({"task_prompt":task["prompt"],"response_contract":task["response_contract"],"system_prompt":f["system_prompt"],"slot":slot,"context_fragments":ctx,"seed":f["pair_seeds"][slot["pair"]]})
            trial.check_request(request,f["model_config"],slot,task["prompt"],task["response_contract"],f["system_prompt"],f["pair_seeds"][slot["pair"]])
            started=now(); cp=subprocess.run([sys.executable,str(adapter)],input=trial.canonical(request),cwd=ROOT,stdout=subprocess.PIPE,stderr=subprocess.PIPE,timeout=f["timeout_seconds"],check=False); ended=now()
            if cp.returncode or not cp.stdout:
                rec={"slot":slot,"state":"RUN_INVALID","started":started,"ended":ended,"exit_code":cp.returncode,"stderr_sha256":hashlib.sha256(cp.stderr).hexdigest(),"request":request,"request_sha256":trial.digest(trial.canonical(request)),"freeze_commit":a.freeze_commit,"freeze_blob":f["freeze_blob"],"assembly_commit":a.assembly_commit,"assembly_blob":blob(a.assembly_commit,ASSEMBLY)}; log.write(json.dumps(rec,ensure_ascii=False)+"\n"); log.flush(); raise ProtocolError("provider invalid during no-retry acceptance campaign; partial transcript preserved")
            response=json.loads(cp.stdout); req(response.get("provider_request_id") and response.get("resolved_provider")=="OpenAI" and response.get("model")==f["model_config"]["model"],"provider response identity drift")
            rec={"slot":slot,"request":request,"request_sha256":trial.digest(trial.canonical(request)),"response":response,"injected_source_bytes":sum(len(x["text"].encode()) for x in ctx),"started":started,"ended":ended,"freeze_commit":a.freeze_commit,"freeze_blob":f["freeze_blob"],"assembly_commit":a.assembly_commit,"assembly_blob":blob(a.assembly_commit,ASSEMBLY)}; records.append(rec); log.write(json.dumps(rec,ensure_ascii=False)+"\n"); log.flush()
    req(len(records)==36,"acceptance campaign incomplete"); print("DW04_ACCEPTANCE_36_RECORDED")
if __name__=="__main__":
    try: main()
    except (ProtocolError,trial.ProtocolError,audit.ProtocolError,subprocess.CalledProcessError,subprocess.TimeoutExpired,OSError,KeyError,ValueError,json.JSONDecodeError) as e:
        print("DW04 ACCEPTANCE EXECUTION BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
