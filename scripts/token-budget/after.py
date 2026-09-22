#!/usr/bin/env python3
"""Measure the POST-CTX-01 cold-start cost by reading the authoritative
derived role profiles (Docs/engineering/context-bootstrap-profiles.json),
not by editing measure.py's hardcoded PRE-CTX arrays.

Two columns per role:
  MIN  = initial_reads + conditional_reads whose trigger is unambiguous
         from the exact WP header (Class/Depends on/Execution overlay).
  ESC  = MIN + the conditional/escalation sources the WP body actually binds
         (must_escalate_if: "the current claim binds an architecture/proof
         source outside the initial pack").
"""
import glob, json, os, sys
from tokenizers import Tokenizer

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(sys.argv[1] if len(sys.argv) > 1 else os.path.join(HERE, '..', '..'))
tok = Tokenizer.from_file(os.path.join(HERE, 'tokenizer.json'))

def n(patterns):
    total = 0
    for pat in patterns:
        fp = os.path.join(ROOT, pat)
        matches = sorted(glob.glob(fp)) if any(c in pat for c in '*?[') else [fp]
        for f in matches:
            if os.path.exists(f):
                total += len(tok.encode(open(f, encoding='utf-8', errors='replace').read()).ids)
    return total

# --- what the CTX-01 worker/reviewer profile literally names ---
PROFILE_INIT = ['AGENTS.md',
                'Docs/engineering/CONTEXT_BOOTSTRAP_V1.md',
                'Docs/engineering/context-bootstrap-profiles.json',
                'Docs/engineering/WORKER_REVIEW_PROTOCOL.md']
# session overhead the profile does NOT name but that is still paid
OVERHEAD = ['CLAUDE.md', 'Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md']

WP_H1_02   = ['Docs/workpacks/H1/WP-H1-02.md']
FOUND      = ['Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md']   # Class: FOUNDATIONAL
OVERLAY    = ['Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md']     # Execution overlay: named in WP
PREDEC     = ['Docs/workpacks/HK/WP-HK-GATE.md', 'Docs/evidence/WP-HK-GATE/*.md']
ARCH       = ['Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md', 'Docs/architecture/ADR-H1-*.md']
HK00       = ['Docs/engineering/PRODUCT_ARCHITECTURE.md', 'Docs/engineering/DEPENDENCY_IP_POLICY.md']
SKILL_W    = ['.agents/skills/implement-workpack/SKILL.md']
SKILL_R    = ['.agents/skills/validate-workpack/SKILL.md']
DIFF       = 32968   # measured mean candidate diff, held fixed (CLOSED, read in full)

rows = []
for role, skill in (('worker', SKILL_W), ('reviewer', SKILL_R)):
    base = n(PROFILE_INIT + OVERHEAD + WP_H1_02 + skill)
    mn   = base + n(FOUND + OVERLAY + PREDEC)
    esc  = mn + n(ARCH + HK00)
    rows.append((role + '-foundational', mn, esc))

print('=== POST-CTX-01 foundational cold start (WP-H1-02), main f7b4f1e ===')
print(f'{"role":<26}{"MIN":>9}{"ESC":>9}')
for r, mn, esc in rows:
    print(f'{r:<26}{mn:>9,}{esc:>9,}')
print()
print('--- component detail ---')
for lbl, pats in [('profile initial_reads', PROFILE_INIT), ('unnamed overhead', OVERHEAD),
                  ('exact WP', WP_H1_02), ('foundational_claim', FOUND),
                  ('h1_02 overlay', OVERLAY), ('direct_dependencies (predecessor)', PREDEC),
                  ('architecture (escalation)', ARCH), ('inherited hk00 (escalation)', HK00),
                  ('skill worker', SKILL_W), ('skill reviewer', SKILL_R)]:
    print(f'{n(pats):>9,}  {lbl}')
print()
print('--- reviewer incl. mean candidate diff (32,968) ---')
rm, re_ = rows[1][1], rows[1][2]
print(f'{rm+DIFF:>9,}  reviewer MIN + diff')
print(f'{re_+DIFF:>9,}  reviewer ESC + diff')
