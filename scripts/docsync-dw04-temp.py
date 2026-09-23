#!/usr/bin/env python3
import json
from pathlib import Path

SOURCE_MAIN = 'f4a2f145101bc735388b682b20a7913cd720cd33'
CANDIDATE = 'c9e5e26e5780b05fe26c99c4a98a59d63ad269e5'
REVIEW = '#5291762220'
PR = '#150'
VALIDATION = '35868280099'


def replace_once(path, old, new):
    p = Path(path)
    s = p.read_text(encoding='utf-8')
    count = s.count(old)
    if count != 1:
        raise SystemExit(f'{path}: expected one match, got {count}: {old!r}')
    p.write_text(s.replace(old, new, 1), encoding='utf-8')


wp = Path('Docs/workpacks/DW/WP-DW-04.md')
s = wp.read_text(encoding='utf-8')
if s.count('Status: PLANNED / NOT_STARTED') != 1:
    raise SystemExit('WP-DW-04 status shape drift')
s = s.replace('Status: PLANNED / NOT_STARTED', 'Status: COMPLETE / ACCEPTED', 1)
anchor = 'Blocks: `WP-DW-05`\n'
if s.count(anchor) != 1 or '\nAcceptance:' in s:
    raise SystemExit('WP-DW-04 acceptance insertion shape drift')
acceptance = f"\nAcceptance: frozen candidate `{CANDIDATE}`; independent PASS review `{REVIEW}`; PR `{PR}`; merge `{SOURCE_MAIN}`; final fully GREEN exact-SHA validation Actions run `{VALIDATION}`.\n"
wp.write_text(s.replace(anchor, anchor + acceptance, 1), encoding='utf-8')

replace_once('Docs/workpacks/DW/README.md', 'Status: ACTIVE / `WP-DW-03` COMPLETE', 'Status: ACTIVE / `WP-DW-04` COMPLETE')
replace_once('Docs/workpacks/DW/README.md', 'Next default workpack: `WP-DW-04 — Structured-context quality and token trial` (`REMOTE_OK`), dependency-valid because DW-03 and CTX-03 are accepted, but `NOT_STARTED` until a human explicitly starts its Worker.', f'`WP-DW-04` is **COMPLETE**. Frozen candidate `{CANDIDATE}` passed independent review `{REVIEW}` in PR `{PR}`, final fully GREEN exact-SHA validation run `{VALIDATION}` was GREEN, and the accepted candidate merged as `{SOURCE_MAIN}`. Post-PASS reconciliation is `Docs/evidence/WP-DW-04/DOCSYNC.md`.\n\nNext default workpack: `WP-DW-05 — Generic-boundary stress and H2 impact assessment` (`REMOTE_OK`), dependency-valid because DW-04 is accepted, but `NOT_STARTED` until a human explicitly starts its Worker.')
replace_once('Docs/workpacks/DW/README.md', '| 5 | `WP-DW-04` | structured retrieval can reduce context on a pre-tuning frozen task universe while paired same-config agents preserve required task/review facts, blockers and verdicts under deterministic scoring | `REMOTE_OK` |', '| 5 | `WP-DW-04` ✅ | structured retrieval can reduce context on a pre-tuning frozen task universe while paired same-config agents preserve required task/review facts, blockers and verdicts under deterministic scoring | `REMOTE_OK` |')
replace_once('Docs/workpacks/DW/README.md', 'The DW programme plan is accepted after PR `#117` PASS + merge + DocSync, `WP-DW-00` is accepted after PR `#126` PASS + merge + DocSync, `WP-DW-01` is accepted after PR `#136` PASS + merge + DocSync, `WP-DW-02` is accepted after PR `#139` PASS + merge + DocSync, and `WP-DW-03` is accepted after PR `#143` PASS + merge + DocSync. `WP-DW-04 — Structured-context quality and token trial` is the sole default next DW workpack and remains `NOT_STARTED` until a human explicitly starts its Worker. No later DW workpack is implicitly authorized. H1, PA and CTX continue under their own accepted DAGs.', 'The DW programme plan is accepted after PR `#117` PASS + merge + DocSync, `WP-DW-00` is accepted after PR `#126` PASS + merge + DocSync, `WP-DW-01` is accepted after PR `#136` PASS + merge + DocSync, `WP-DW-02` is accepted after PR `#139` PASS + merge + DocSync, `WP-DW-03` is accepted after PR `#143` PASS + merge + DocSync, and `WP-DW-04` is accepted after PR `#150` PASS + merge + DocSync. `WP-DW-05 — Generic-boundary stress and H2 impact assessment` is the sole default next DW workpack and remains `NOT_STARTED` until a human explicitly starts its Worker. No later DW workpack is implicitly authorized. H1, PA and CTX continue under their own accepted DAGs.')

replace_once('Docs/workpacks/README.md', '`WP-DW-04 — Structured-context quality and token trial` is now the next default dependency-valid DW workpack because both DW-03 and its CTX-03 side prerequisite are accepted, and remains `NOT_STARTED` until a human explicitly starts its Worker.', f'`WP-DW-04 — Structured-context quality and token trial` is **COMPLETE** on frozen candidate `{CANDIDATE}` (independent PASS review `{REVIEW}`, PR `{PR}`, implementation merge `{SOURCE_MAIN}`, final fully GREEN exact-SHA validation `{VALIDATION}`). `WP-DW-05 — Generic-boundary stress and H2 impact assessment` is now the next default dependency-valid DW workpack because DW-04 is accepted, and remains `NOT_STARTED` until a human explicitly starts its Worker.')

idxp = Path('Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json')
idx = json.loads(idxp.read_text(encoding='utf-8'))
if idx.get('generated_from_main_sha') != '10bbbb0cb64bcfab476281c089812332af136bb1':
    raise SystemExit('accepted-state index source-main drift')
dw = idx['tracks']['DW']
if dw.get('accepted_state_hint') != 'THROUGH_WP-DW-03' or dw.get('next_contract_hint') != 'WP-DW-04':
    raise SystemExit('accepted-state DW shape drift')
idx['generated_from_main_sha'] = SOURCE_MAIN
idx['generated_on'] = '2026-09-23'
dw['accepted_state_hint'] = 'THROUGH_WP-DW-04'
dw['accepted_workpacks_hint'] = ['WP-DW-00','WP-DW-01','WP-DW-02','WP-DW-03','WP-DW-04']
dw['next_contract_hint'] = 'WP-DW-05'
src = dw['sources']
if 'Docs/workpacks/DW/WP-DW-05.md' not in src:
    src.insert(src.index('Docs/evidence/WP-DW-00/DOCSYNC.md'), 'Docs/workpacks/DW/WP-DW-05.md')
if 'Docs/evidence/WP-DW-04/DOCSYNC.md' not in src:
    src.insert(src.index('Docs/engineering/DW_DESIGN_WORLD_ARCHITECTURE.md'), 'Docs/evidence/WP-DW-04/DOCSYNC.md')
idx['cross_track_contract_hints'] = [h for h in idx['cross_track_contract_hints'] if h.get('consumer') != 'WP-DW-04']
idxp.write_text(json.dumps(idx, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')

lines = [
    '# WP-DW-04 — Post-PASS DocSync', '',
    'DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  ',
    'MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  ',
    'DATE: 2026-09-23', '',
    '## Accepted result', '',
    f'- Frozen candidate SHA: `{CANDIDATE}`',
    '- Independent Reviewer verdict: **PASS**',
    f'- Review: `{REVIEW}`', f'- PR: `{PR}`', f'- Merge commit: `{SOURCE_MAIN}`',
    f'- Final fully GREEN exact-SHA validation: Actions run `{VALIDATION}`', '',
    '## Accepted claim', '',
    'DW-04 establishes, on its frozen six-task CITY+PA acceptance universe, that the DW structured-context route preserved the required Worker-like/Reviewer-like facts, blockers and verdicts across all 18 matched CTX-vs-DW pairs / 36 designated executions while reducing median injected source-context bytes from 2258.5 to 1380.5 (38.875%). Structural required-context completeness remained GREEN. Provider token/cost observations remain diagnostic only; no universal token- or cost-saving claim is accepted.', '',
    'The accepted evidence chain binds the scored transcript to the campaign receipt and preserved provider records, including provider response content/metadata and recomputed provider request-body hashes. The owner-authorized repaired acceptance generation is the operative DW-04 attempt; the earlier failed campaign remains preserved as historical evidence and is not treated as a terminal one-shot blocker.', '',
    '## DocSync actions', '',
    '1. Marked `WP-DW-04` COMPLETE / ACCEPTED and recorded the exact frozen candidate, independent PASS, final fully GREEN exact-SHA validation and implementation merge.',
    '2. Advanced the accepted DW spine to `WP-DW-05 — Generic-boundary stress and H2 impact assessment` as the next default workpack.',
    '3. Recorded DW-05 as dependency-valid after this DocSync because its sole prerequisite is accepted `WP-DW-04` PASS + merge + DocSync.',
    '4. Updated the DW track summary and root workpack index so DW-04 is accepted predecessor truth rather than an active/pending action.',
    f'5. Refreshed `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` from exact source main `{SOURCE_MAIN}` under `docsync-first-parent-v1`, with DW accepted through `WP-DW-04` and `WP-DW-05` as the next contract.',
    '6. Preserved authority and scope boundaries: CTX remains process-context owner; CITY/PA accepted sources remain semantic authority; DW-04\'s result is bounded to the frozen trial universe; H1 proceeds independently; final H2 public/external-boundary acceptance retains the accepted DW-GATE interlock.', '',
    '## Boundary', '',
    'This DocSync changes documentation/current-state projection only. It does not modify campaign answers, provider records, oracle, scorer, retrieval semantics, CTX policy, CITY/PA truth, H0/H1 runtime contracts or the accepted experimental result. It does not generalize the measured 38.875% source-byte reduction beyond the frozen DW-04 acceptance suite.', '',
    '## Next action', '',
    'Next default DW workpack: `WP-DW-05 — Generic-boundary stress and H2 impact assessment` (`REMOTE_OK`).', '',
    '`WP-DW-05` is dependency-valid after this DocSync because its direct prerequisite `WP-DW-04` is accepted. It remains `NOT_STARTED` until a human explicitly starts its Worker and must complete its own foundational Worker → exact-SHA proof → independent Reviewer PASS → merge → DocSync cycle before `WP-DW-GATE` begins.', '',
    '`DOCSYNC_COMPLETE`',
]
out = Path('Docs/evidence/WP-DW-04/DOCSYNC.md')
if out.exists():
    raise SystemExit('DW-04 DOCSYNC already exists')
out.write_text('\n'.join(lines) + '\n', encoding='utf-8')
