#!/usr/bin/env python3
"""Deterministic final audit for the frozen DW-04 acceptance campaign."""
import argparse,json,pathlib,statistics,subprocess,sys
from importlib.machinery import SourceFileLoader
ROOT=pathlib.Path(__file__).resolve().parents[1]
trial=SourceFileLoader("dw04_trial",str(ROOT/"scripts/dw04-trial.py")).load_module(); assembler=SourceFileLoader("dw04_asm",str(ROOT/"scripts/dw04-acceptance-assemble.py")).load_module()
PRE="Docs/evidence/WP-DW-04/PRECALIBRATION_FREEZE.json"; FREEZE="Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json"; PROTOCOL="Docs/evidence/WP-DW-04/ACCEPTANCE_PROTOCOL.json"; PLAN="Docs/evidence/WP-DW-04/ACCEPTANCE_CONTEXT_PLAN.json"; ASSEMBLY="Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json"; CAL="Docs/evidence/WP-DW-04/CALIBRATION_RESULTS.json"; CAL_ORACLES="Docs/evidence/WP-DW-04/CALIBRATION_ORACLES.json"; SOURCE_ORACLES="Docs/evidence/WP-DW-04/ACCEPTANCE_SOURCE_ORACLES.json"
class ProtocolError(ValueError): pass
def req(c,m):
    if not c: raise ProtocolError(m)
def git(*a): return subprocess.check_output(["git",*a],cwd=ROOT,stderr=subprocess.PIPE)
def blob(c,p): return git("rev-parse",f"{c}:{p}").decode().strip()
def load(c,p): return json.loads(git("show",f"{c}:{p}"))
def ancestor(a,b):
    try: subprocess.check_call(["git","merge-base","--is-ancestor",a,b],cwd=ROOT,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
    except subprocess.CalledProcessError as e: raise ProtocolError(f"chronology violation: {a} !<= {b}") from e
def response_contract(p,oracle,t):
    task=p["tasks"][t]; vocab=p["response_vocabulary"]; facts=oracle["tasks"][t]["oracle"]["facts"]
    req(set(task["fact_value_types"])==set(facts),f"fact type surface drift: {t}")
    return {"fact_keys":list(facts),"fact_value_types":task["fact_value_types"],"allowed_blockers":vocab["allowed_blockers"],"allowed_verdicts":vocab["allowed_verdicts"],"evidence_ids":vocab["evidence_ids"]}
def score_acceptance(oracle,answer):
    """Score exact claims while allowing additional frozen-authority evidence identifiers."""
    req(set(oracle)=={"facts","blockers","verdict","evidence"},"frozen oracle shape")
    if not isinstance(answer,dict): return {"pass":False,"errors":["missing structured answer"]}
    errors=[]; facts=answer.get("facts")
    if not isinstance(facts,dict): errors.append("facts shape")
    else:
        for key,expected in oracle["facts"].items():
            if key not in facts or facts[key]!=expected: errors.append("fact:"+key)
        for key in set(facts)-set(oracle["facts"]): errors.append("unfrozen fact:"+key)
    blockers=answer.get("blockers")
    if not isinstance(blockers,list) or len(blockers)!=len(set(map(str,blockers))) or set(map(str,blockers))!=set(oracle["blockers"]): errors.append("blockers mismatch")
    evidence=answer.get("evidence")
    if not isinstance(evidence,list) or len(evidence)!=len(set(map(str,evidence))) or not set(oracle["evidence"]).issubset(set(map(str,evidence))): errors.append("evidence mismatch")
    if answer.get("verdict")!=oracle["verdict"]: errors.append("verdict mismatch")
    return {"pass":not errors,"errors":errors}
def check_calibration(f,p):
    cc=f["calibration_commit"]; ancestor(f["calibration_oracle_commit"],cc); ancestor(cc,f["freeze_parent"]); req(blob(cc,CAL)==f["calibration_receipt_blob"]==blob("HEAD",CAL),"calibration receipt drift")
    r=load(cc,CAL); cp=load(r["calibration_protocol_commit"],"Docs/evidence/WP-DW-04/CALIBRATION_PROTOCOL.json"); o=load(f["calibration_oracle_commit"],CAL_ORACLES); ctx=load(r["calibration_protocol_commit"],"Docs/evidence/WP-DW-04/CALIBRATION_CONTEXT.json")
    req(r["readiness"]=="READY" and len(r["runs"])==8 and not r["invalid_attempts"],"calibration not READY"); req(r["model_config"]==cp["model_config"],"READY calibration model/config drift")
    for key in ("provider","model","version","temperature","thinking","tool_policy","execution_budget","provider_options"):
        req(r["model_config"][key]==p["model_config"][key],f"acceptance effective model/config differs from READY calibration: {key}")
    req(p["model_config"]["run_policy"]=="acceptance-no-replacement; objective invalid is terminal evidence; semantic misses are never rerun","acceptance run policy drift")
    pre=load(f["precalibration_commit"],PRE); expected=[{"task":t,"run":i,"route":"CTX"} for t in pre["calibration_policy"]["run_order"] for i in (1,2)]; req([x["slot"] for x in r["runs"]]==expected,"calibration slot drift")
    for x in r["runs"]:
        t=x["slot"]["task"]; task=cp["tasks"][t]; req(x["resolved_provider"]=="OpenAI" and x["provider_request_id"],"calibration provider identity missing"); trial.check_request(x["request"],cp["model_config"],x["slot"],task["semantic_question"],task["response_contract"],cp["system_prompt"],None); req(x["request"]["context_fragments"]==ctx["contexts"][t],"calibration context drift"); req(trial.score(o["tasks"][t]["oracle"],x["answer"])["pass"],f"calibration miss: {t}")
def check_structure(f,p,plan,assembly_commit):
    ancestor(f["freeze_commit"],assembly_commit); req(blob(assembly_commit,ASSEMBLY)==blob("HEAD",ASSEMBLY),"assembly drift"); a=load(assembly_commit,ASSEMBLY); req(a["freeze_commit"]==f["freeze_commit"] and a["freeze_blob"]==f["freeze_blob"] and a["context_plan_blob"]==f["context_plan_blob"],"assembly identity mismatch")
    pre=load(f["precalibration_commit"],PRE); so=load(f["acceptance_oracle_commit"],SOURCE_ORACLES); complete=True
    for t in f["selected"]:
        for route in ("CTX","DW"):
            frags=a["contexts"][t][route]; items=plan["context_plan"][t][route]; req(len(frags)==len(items),"context cardinality drift")
            for item,frag in zip(items,frags): req(assembler.materialize(item,pre["baseline_sha"],pre["authority_blobs"])==frag,f"context replay mismatch {t}/{route}/{item['id']}")
            for need in so["tasks"][t]["required_context"]:
                if not any(x["source_path"]==need["source_path"] and need["literal"] in x["text"] for x in frags): complete=False
            if route=="CTX":
                for path in {x["source_path"] for x in frags}:
                    supplied=sum(len(x["text"].encode()) for x in frags if x["source_path"]==path); available=len(git("show",f"{pre['baseline_sha']}:{path}")); req(supplied<available/2,f"CTX inflation: {t}/{path}")
    req(complete,"required source/fallback completeness RED"); return a
def audit(fc,transcript):
    f=load(fc,FREEZE); f["freeze_commit"]=fc; f["freeze_blob"]=blob(fc,FREEZE); req(f["schema"]=="dw04-acceptance-freeze-v3","freeze schema"); req(blob(fc,PROTOCOL)==f["protocol_blob"]==blob("HEAD",PROTOCOL),"protocol drift"); req(blob(fc,PLAN)==f["context_plan_blob"]==blob("HEAD",PLAN),"plan drift"); p=load(fc,PROTOCOL); plan=load(fc,PLAN)
    req(p["selected"]==plan["selected"]==f["selected"]==["A-CITY-01","A-CITY-02","A-CITY-03","A-PA-01","A-PA-02","A-PA-03"],"selection drift")
    req(p.get("canonical_executor")=="scripts/dw04-acceptance-execute.py" and p.get("canonical_workflow")==".github/workflows/dw04-campaign.yml","canonical route drift"); req(p.get("evidence_scoring_policy")=="required-frozen-evidence-subset; additional globally-frozen evidence allowed; blockers remain exact","evidence scoring policy drift")
    for path,key in (("scripts/dw04-trial.py","scorer"),("scripts/dw04-acceptance-assemble.py","assembler"),("scripts/dw04-acceptance-execute.py","executor"),("scripts/dw04-acceptance-audit.py","audit"),("scripts/dw04-openrouter-luna-adapter.py","adapter"),("tools/Arkus.Dw04.Retrieval/Program.cs","retrieval"),(".github/workflows/dw04-assemble.yml","assembly_workflow"),(".github/workflows/dw04-campaign.yml","workflow"),("scripts/dw04-verify-exact-sha.sh","verifier")): req(blob(fc,path)==p["script_blobs"][key]==blob("HEAD",path),f"{key} blob drift")
    pre=load(f["precalibration_commit"],PRE); req(trial.precheck(pre,pre["baseline_sha"])==f["selected"],"selection rule drift"); req(blob(f["acceptance_oracle_commit"],SOURCE_ORACLES)==f["acceptance_oracle_blob"]==blob("HEAD",SOURCE_ORACLES),"source oracle drift"); so=load(f["acceptance_oracle_commit"],SOURCE_ORACLES)
    req(p["slots"]==trial.make_slots(f["selected"]) and p["correctness"]=="all_36" and p["context_reduction_min"]==0.30,"proof policy drift"); check_calibration(f,p)
    req(len(transcript)==36 and [x["slot"] for x in transcript]==p["slots"],"execution count/order drift"); acs={x["assembly_commit"] for x in transcript}; req(len(acs)==1,"assembly inventory drift"); ac=next(iter(acs)); assembly=check_structure(f,p,plan,ac)
    pairs={}; ctxb=[]; dwb=[]; ids=[]
    for rec in transcript:
        s=rec["slot"]; t=s["task"]; route=s["route"]; task=p["tasks"][t]; rc=response_contract(p,so,t); request=rec["request"]; trial.check_request(request,p["model_config"],s,task["prompt"],rc,p["system_prompt"],p["pair_seeds"][s["pair"]]); req(request["context_fragments"]==assembly["contexts"][t][route],"effective context drift"); b=sum(len(x["text"].encode()) for x in request["context_fragments"]); req(rec["injected_source_bytes"]==b,"byte accounting drift"); (ctxb if route=="CTX" else dwb).append(b); resp=rec["response"]; req(resp.get("provider_request_id") and resp.get("resolved_provider")=="OpenAI" and resp.get("model")==p["model_config"]["model"],"provider/model drift"); ids.append(resp["provider_request_id"]); pairs.setdefault((t,s["pair"]),{})[route]=score_acceptance(so["tasks"][t]["oracle"],resp.get("raw",{}).get("answer"))
    req(len(set(ids))==36,"provider request IDs not unique"); rows=[{"task":t,"pair":r,"CTX":v["CTX"],"DW":v["DW"]} for (t,r),v in sorted(pairs.items())]; req(len(rows)==18,"pair completeness"); mc=statistics.median(ctxb); md=statistics.median(dwb); saving=1-md/mc if mc else 0; disposition=trial.decide(rows,True,saving); return {"disposition":disposition,"pairs":rows,"median_ctx_bytes":mc,"median_dw_bytes":md,"saving":saving,"structural":True,"provider_request_count":len(ids),"assembly_commit":ac,"freeze_commit":fc}
def main():
    q=argparse.ArgumentParser(); q.add_argument("--freeze-commit",required=True); q.add_argument("--transcript",required=True); a=q.parse_args(); rows=[json.loads(x) for x in pathlib.Path(a.transcript).read_text().splitlines() if x.strip()]; print(json.dumps(audit(a.freeze_commit,rows),ensure_ascii=False,indent=2))
if __name__=="__main__":
    try: main()
    except (ProtocolError,trial.ProtocolError,assembler.ProtocolError,subprocess.CalledProcessError,OSError,KeyError,ValueError) as e: print("DW04 ACCEPTANCE AUDIT BLOCKED: "+str(e),file=sys.stderr); sys.exit(1)
