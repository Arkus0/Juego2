#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"; cd "${ROOT}"; actual="$(git rev-parse HEAD)"; expected="${1:-${CANDIDATE_SHA:-${actual}}}"; test "${actual}" = "${expected}"
freeze="$(git log -1 --format=%H -- Docs/evidence/WP-DW-04/TASK_SELECTION_FREEZE.json)"; test -n "${freeze}"; test ! -e scripts/dw04-execute.py
for x in Docs/evidence/WP-DW-04/CONTEXT_ASSEMBLY.json Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl Docs/evidence/WP-DW-04/ACCEPTANCE_PROVIDER_RAW.jsonl Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json Docs/evidence/WP-DW-04/TRIAL_RESULT.json; do test -f "$x"; done
tmp="$(mktemp)"; trap 'rm -f "${tmp}"' EXIT; python3 scripts/dw04-acceptance-audit.py --freeze-commit "${freeze}" --transcript Docs/evidence/WP-DW-04/ACCEPTANCE_TRANSCRIPT.jsonl > "${tmp}"
python3 - "${tmp}" <<'PY'
import json,pathlib,sys,hashlib
x=json.loads(pathlib.Path(sys.argv[1]).read_text()); y=json.load(open('Docs/evidence/WP-DW-04/TRIAL_RESULT.json')); assert x==y and x['disposition']=='PASS' and x['structural'] is True and x['saving']>=.30 and x['provider_request_count']==36
r=json.load(open('Docs/evidence/WP-DW-04/CAMPAIGN_RECEIPT.json')); assert r['execution_count']==36 and len(set(r['provider_request_ids']))==36; assert len([z for z in open('Docs/evidence/WP-DW-04/ACCEPTANCE_PROVIDER_RAW.jsonl') if z.strip()])==36
PY
echo 'DW-04 final canonical route GREEN'
