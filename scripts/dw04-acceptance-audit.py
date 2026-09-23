#!/usr/bin/env python3
"""Deterministic final audit for the frozen DW-04 acceptance campaign."""
import argparse, json, pathlib, statistics, subprocess, sys
from importlib.machinery import SourceFileLoader
ROOT=pathlib.Path(__file__).resolve().parents[1]
trial=SourceFileLoader("dw04_trial",str(ROOT/"scripts/dw04-trial.py")).load_module()
assembler=SourceFileLoader("dw04_asm",str(ROOT/"scripts/dw04-acceptance-assemble.py")).load_module()
PRE="Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json"; FREEZE="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"; ASSEMBLY="Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"; CAL="Docs/evidence/WP-DW-04/CALIBRATION_RESULTS.json"; CAL_ORACLES="Docs/evidence/WP-DW-04/CALIBRATION_ORACLES.json"; SOURCE_ORACLES="Docs/evidence/WP-DW-04/ACCEPTANCE_SOURCE_ORACLES.json"
class ProtocolError(ValueError): pass
def req(c,m):
    if not c: raise ProtocolError(m)
def git(*a): return subprocess.check_output(["git",*a],cwd=ROOT,stderr=subprocess.PIPE)
def blob(c,p): return git("rev-parse",f"{c}:{p}").decode().strip()
def load(c,p): return json.loads(git("show",f"{c}:{p}"))
def ancestor(a,b):
    try: subprocess.check_call(["git","merge-base","--is-ancestor",a,b],cwd=ROOT,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
    except subprocess.CalledProcessError as e: raise ProtocolError(f"chronology violation: {a} !<= {b}") from e
def check_calibration(f):
    cc=f["calibration_commit"]; ancestor(f["calibration_oracle_commit"],cc); ancestor(cc,f["freeze_parent"])
    req(blob(cc,CAL)==f["calibration_receipt_blob"]==blob("HEAD",CAL),"calibration receipt blob drift")
    r=load(cc,CAL); p=load(r["calibration_protocol_commit"],"Docs/evidence/WP-DW-04/CALIBRATION_PROTOCOL.json"); o=load(f["calibration_oracle_commit"],CAL_ORACLES); ctx=load(r["calibration_protocol_commit"],"Docs/evidence/WP-DW-04/CALIBRATION_CONTEXT.json")
    req(r["readiness"]=="READY" and len(r["runs"])==8 and not r["invalid_attempts"],"calibration not READY/complete")
    req(r["model_config"]==p["model_config"]==f["model_config"],"acceptance model/config differs from READY calibration")
    expected=[{"task":t,"run":i,"route":"CTX"} for t in load(f["precalibration_commit"],PRE)["calibration_policy"]["run_order"] for i in (1,2)]
    req([x["slot"] for x in r["runs"]]==expected,"calibration slot order drift")
    for x in r["runs"]:
        t=x["slot"]["task"]; task=p["tasks"][t]
        req(x["resolved_provider"]=="OpenAI" and x["provider_request_id"],"calibration provider identity missing")
        trial.check_request(x["request"],f["model_config"],x["slot"],task["semantic_question"],task["response_contract"],p["system_prompt"],None)
        req(x["request"]["context_fragments"]==ctx["contexts"][t],"calibration context drift")
        req(trial.score(o["tasks"][t]["oracle"],x["answer"])["pass"],f"calibration oracle miss: {t}")
def check_structure(f,assembly_commit):
    ancestor(f["freeze_commit"],assembly_commit)
    req(blob(assembly_commit,ASSEMBLY)==blob("HEAD",ASSEMBLY),"assembly changed after anchor")
    a=load(assembly_commit,ASSEMBLY); req(a["freeze_commit"]==f["freeze_commit"] and a["freeze_blob"]==f["freeze_blob"],"assembly/freeze identity mismatch")
    pre=load(f["precalibration_commit"],PRE); complete=True
    for t in f["selected"]:
        for route in ("CTX","DW"):
            frags=a["contexts"][t][route]; plan=f["context_plan"][t][route]; req(len(frags)==len(plan),"context plan cardinality drift")
            for item,frag in zip(plan,frags):
                rebuilt=assembler.materialize(item,pre["baseline_sha"],pre["authority_blobs"]); req(rebuilt==frag,f"context replay mismatch: {t}/{route}/{item['id']}")
            for needed in f["tasks"][t]["required_context"]:
                if not any(x["source_path"]==needed["source_path"] and needed["literal"] in x["text"] for x in frags): complete=False
            if route=="CTX":
                for path in {x["source_path"] for x in frags}:
                    supplied=sum(len(x["text"].encode()) for x in frags if x["source_path"]==path); available=len(git("show",f"{pre['baseline_sha']}:{path}")); req(supplied<available/2,f"CTX baseline inflated: {t}/{path}")
    req(complete,"required source/fallback completeness RED"); return a
def audit(freeze_commit, transcript):
    f=load(freeze_commit,FREEZE); f["freeze_commit"]=freeze_commit; f["freeze_blob"]=blob(freeze_commit,FREEZE)
    req(f["schema"]=="dw04-acceptance-v2","freeze schema")
    req(f["selected"]==["A-CITY-01","A-CITY-02","A-CITY-03","A-PA-01","A-PA-02","A-PA-03"],"selection drift")
    req(blob(freeze_commit,"scripts/dw04-trial.py")==f["scorer_blob"]==blob("HEAD","scripts/dw04-trial.py"),"scorer blob drift")
    for key,path in (("assembler_blob","scripts/dw04-acceptance-assemble.py"),("executor_blob","scripts/dw04-acceptance-execute.py"),("audit_blob","scripts/dw04-acceptance-audit.py"),("adapter_blob","scripts/dw04-openrouter-luna-adapter.py")):
        req(blob(freeze_commit,path)==f[key]==blob("HEAD",path),f"{key} drift")
    pre=load(f["precalibration_commit"],PRE); req(trial.precheck(pre,pre["baseline_sha"])==f["selected"],"preselection drift")
    req(blob(f["acceptance_oracle_commit"],SOURCE_ORACLES)==blob("HEAD",SOURCE_ORACLES),"source oracle drift")
    so=load(f["acceptance_oracle_commit"],SOURCE_ORACLES)
    for t in f["selected"]:
        req(f["tasks"][t]["oracle"]==so["tasks"][t]["oracle"] and f["tasks"][t]["required_context"]==so["tasks"][t]["required_context"],f"oracle drift {t}")
    common={(tuple(v["response_contract"]["allowed_blockers"]),tuple(v["response_contract"]["allowed_verdicts"]),tuple(v["response_contract"]["evidence_ids"])) for v in f["tasks"].values()}; req(len(common)==1,"task-specific formatting vocabulary leak")
    req(f["slots"]==trial.make_slots(f["selected"]) and f["correctness"]=="all_36" and f["context_reduction_min"]==0.30,"proof policy drift")
    check_calibration(f)
    req(len(transcript)==36 and [x["slot"] for x in transcript]==f["slots"],"acceptance execution count/order drift")
    assemblies={x["assembly_commit"] for x in transcript}; req(len(assemblies)==1,"multiple/missing assembly commits"); ac=next(iter(assemblies)); assembly=check_structure(f,ac)
    pairs={}; ctxb=[]; dwb=[]; ids=[]
    for rec in transcript:
        s=rec["slot"]; t=s["task"]; route=s["route"]; pair=s["pair"]; task=f["tasks"][t]; request=rec["request"]
        trial.check_request(request,f["model_config"],s,task["prompt"],task["response_contract"],f["system_prompt"],f["pair_seeds"][pair]); req(request["context_fragments"]==assembly["contexts"][t][route],"effective context drift")
        b=sum(len(x["text"].encode()) for x in request["context_fragments"]); req(rec["injected_source_bytes"]==b,"byte accounting drift"); (ctxb if route=="CTX" else dwb).append(b)
        resp=rec["response"]; req(resp.get("provider_request_id") and resp.get("resolved_provider")=="OpenAI" and resp.get("model")==f["model_config"]["model"],"provider/model identity drift"); ids.append(resp["provider_request_id"])
        result=trial.score(task["oracle"],resp.get("raw",{}).get("answer")); pairs.setdefault((t,pair),{})[route]=result
    req(len(set(ids))==36,"provider request inventory not unique")
    rows=[{"task":t,"pair":p,"CTX":v["CTX"],"DW":v["DW"]} for (t,p),v in sorted(pairs.items())]; req(len(rows)==18,"pair completeness")
    mc=statistics.median(ctxb); md=statistics.median(dwb); saving=1-md/mc if mc else 0; disposition=trial.decide(rows,True,saving)
    return {"disposition":disposition,"pairs":rows,"median_ctx_bytes":mc,"median_dw_bytes":md,"saving":saving,"structural":True,"provider_request_count":len(ids),"assembly_commit":ac,"freeze_commit":freeze_commit}
def main():
    p=argparse.ArgumentParser(); p.add_argument("--freeze-commit",required=True); p.add_argument("--transcript",required=True); a=p.parse_args(); rows=[json.loads(x) for x in pathlib.Path(a.transcript).read_text(encoding="utf-8").splitlines() if x.strip()]; print(json.dumps(audit(a.freeze_commit,rows),ensure_ascii=False,indent=2))
if __name__=="__main__":
    try: main()
    except (ProtocolError,trial.ProtocolError,assembler.ProtocolError,subprocess.CalledProcessError,OSError,KeyError,ValueError) as e:
        print("DW04 ACCEPTANCE AUDIT BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
