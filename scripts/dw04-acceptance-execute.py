#!/usr/bin/env python3
"""Execute the frozen 36-call DW-04 acceptance schedule exactly once."""
import argparse,datetime,hashlib,json,os,pathlib,subprocess,sys
from importlib.machinery import SourceFileLoader
ROOT=pathlib.Path(__file__).resolve().parents[1]
trial=SourceFileLoader("dw04_trial",str(ROOT/"scripts/dw04-trial.py")).load_module(); audit=SourceFileLoader("dw04_audit",str(ROOT/"scripts/dw04-acceptance-audit.py")).load_module()
FREEZE="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"; PROTOCOL="Docs/evidence/WP-DW-04/ACCEPTANCE_PROTOCOL.json"; PLAN="Docs/evidence/WP-DW-04/ACCEPTANCE_CONTEXT_PLAN.json"; ASSEMBLY="Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"; SOURCE_ORACLES="Docs/evidence/WP-DW-04/ACCEPTANCE_SOURCE_ORACLES.json"
class ProtocolError(ValueError): pass
def req(c,m):
    if not c: raise ProtocolError(m)
def git(*a): return subprocess.check_output(["git",*a],cwd=ROOT,stderr=subprocess.PIPE)
def load(c,p): return json.loads(git("show",f"{c}:{p}"))
def blob(c,p): return git("rev-parse",f"{c}:{p}").decode().strip()
def now(): return datetime.datetime.now(datetime.timezone.utc).isoformat()
def main():
    q=argparse.ArgumentParser(); q.add_argument("--freeze-commit",required=True); q.add_argument("--assembly-commit",required=True); q.add_argument("--output",required=True); a=q.parse_args(); req(os.environ.get("DW04_CAMPAIGN_ID"),"campaign id required")
    f=load(a.freeze_commit,FREEZE); f["freeze_commit"]=a.freeze_commit; f["freeze_blob"]=blob(a.freeze_commit,FREEZE); req(blob(a.freeze_commit,PROTOCOL)==f["protocol_blob"]==blob("HEAD",PROTOCOL),"protocol drift"); req(blob(a.freeze_commit,PLAN)==f["context_plan_blob"]==blob("HEAD",PLAN),"plan drift"); p=load(a.freeze_commit,PROTOCOL); plan=load(a.freeze_commit,PLAN); so=load(f["acceptance_oracle_commit"],SOURCE_ORACLES)
    req(blob(a.freeze_commit,"scripts/dw04-acceptance-execute.py")==p["script_blobs"]["executor"]==blob("HEAD","scripts/dw04-acceptance-execute.py"),"executor drift"); req(blob(a.freeze_commit,"scripts/dw04-openrouter-luna-adapter.py")==p["script_blobs"]["adapter"]==blob("HEAD","scripts/dw04-openrouter-luna-adapter.py"),"adapter drift")
    assembly=audit.check_structure(f,p,plan,a.assembly_commit); out=pathlib.Path(a.output); req(not out.exists(),"transcript exists"); adapter=ROOT/"scripts/dw04-openrouter-luna-adapter.py"; count=0; fd=os.open(out,os.O_WRONLY|os.O_CREAT|os.O_EXCL,0o600)
    with os.fdopen(fd,"w",encoding="utf-8") as log:
        for slot in p["slots"]:
            t=slot["task"]; task=p["tasks"][t]; rc=audit.response_contract(p,so,t); ctx=assembly["contexts"][t][slot["route"]]; request=dict(p["model_config"]); request.update({"task_prompt":task["prompt"],"response_contract":rc,"system_prompt":p["system_prompt"],"slot":slot,"context_fragments":ctx,"seed":p["pair_seeds"][slot["pair"]]}); trial.check_request(request,p["model_config"],slot,task["prompt"],rc,p["system_prompt"],p["pair_seeds"][slot["pair"]])
            started=now(); cp=subprocess.run([sys.executable,str(adapter)],input=trial.canonical(request),cwd=ROOT,stdout=subprocess.PIPE,stderr=subprocess.PIPE,timeout=p["timeout_seconds"],check=False); ended=now()
            base={"slot":slot,"request":request,"request_sha256":trial.digest(trial.canonical(request)),"started":started,"ended":ended,"freeze_commit":a.freeze_commit,"freeze_blob":f["freeze_blob"],"assembly_commit":a.assembly_commit,"assembly_blob":blob(a.assembly_commit,ASSEMBLY)}
            if cp.returncode or not cp.stdout:
                base.update({"state":"RUN_INVALID","exit_code":cp.returncode,"stderr_sha256":hashlib.sha256(cp.stderr).hexdigest()}); log.write(json.dumps(base,ensure_ascii=False)+"\n"); log.flush(); raise ProtocolError("provider invalid; acceptance has no replacement calls")
            response=json.loads(cp.stdout); req(response.get("provider_request_id") and response.get("resolved_provider")=="OpenAI" and response.get("model")==p["model_config"]["model"],"provider identity drift"); base.update({"response":response,"injected_source_bytes":sum(len(x["text"].encode()) for x in ctx)}); log.write(json.dumps(base,ensure_ascii=False)+"\n"); log.flush(); count+=1
    req(count==36,"acceptance campaign incomplete"); print("DW04_ACCEPTANCE_36_RECORDED")
if __name__=="__main__":
    try: main()
    except (ProtocolError,trial.ProtocolError,audit.ProtocolError,subprocess.CalledProcessError,subprocess.TimeoutExpired,OSError,KeyError,ValueError,json.JSONDecodeError) as e: print("DW04 ACCEPTANCE EXECUTION BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
