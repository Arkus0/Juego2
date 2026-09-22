#!/usr/bin/env python3
"""Measure the mandatory cold-start reading cost of a Juego2 role session.

NON-CONTRACTUAL measurement tooling for the token-savings work. It does not
define, weaken or reinterpret any acceptance criterion.

Tokenizer: the Claude tokenizer bundled in older `anthropic` wheels.
    pip install tokenizers
    pip download anthropic==0.21.3 --no-deps -d /tmp/a
    unzip -o -j /tmp/a/*.whl anthropic/tokenizer.json -d scripts/token-budget/
Counts are within a few percent of current Claude models, not exact.

Usage:  python3 scripts/token-budget/measure.py [--root .] [--set worker-foundational]
"""
import argparse, glob, json, os, sys

# Reading sets are derived from AGENTS.md "Mandatory predecessor contract check"
# plus .agents/skills/{implement,validate}-workpack/SKILL.md preconditions.
BOOT = ['CLAUDE.md', 'AGENTS.md', 'Docs/ROADMAP.md', 'Docs/workpacks/README.md',
        'Docs/engineering/WORKER_REVIEW_PROTOCOL.md',
        'Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md']
PROOF = ['Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md']
H1 = {
    'skill/worker': ['.agents/skills/implement-workpack/SKILL.md'],
    'skill/reviewer': ['.agents/skills/validate-workpack/SKILL.md'],
    'wp+plan': ['Docs/workpacks/H1/WP-H1-02.md', 'Docs/workpacks/H1/README.md'],
    'predecessor': ['Docs/workpacks/HK/WP-HK-GATE.md', 'Docs/evidence/WP-HK-GATE/*.md'],
    'architecture': ['Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md',
                     'Docs/architecture/ADR-H1-*.md'],
    'inherited/hk00': ['Docs/engineering/PRODUCT_ARCHITECTURE.md',
                       'Docs/engineering/DEPENDENCY_IP_POLICY.md'],
}
PA = {
    'skill/worker': ['.agents/skills/implement-workpack/SKILL.md'],
    'skill/reviewer': ['.agents/skills/validate-workpack/SKILL.md'],
    'wp+plan': ['Docs/workpacks/PA/WP-PA-03.md', 'Docs/workpacks/PA/README.md',
                'Docs/research/living-world/PA_ROADMAP.md'],
    'predecessor': ['Docs/research/living-world/results/PA-02.md',
                    'Docs/evidence/WP-PA-02/*.md'],
    'amendments': ['Docs/research/living-world/LIVING_WORLD_CROSSCUTTING_AMENDMENT_01_*.md',
                   'Docs/research/living-world/PA-12_AMENDMENT_01_*.md'],
}
SETS = {
    'worker-foundational':   (BOOT + PROOF, H1, ['skill/worker', 'wp+plan', 'predecessor',
                                                 'architecture', 'inherited/hk00']),
    'reviewer-foundational': (BOOT + PROOF, H1, ['skill/reviewer', 'wp+plan', 'predecessor',
                                                 'architecture', 'inherited/hk00']),
    'worker-harvest':        (BOOT, PA, ['skill/worker', 'wp+plan', 'predecessor', 'amendments']),
    'reviewer-harvest':      (BOOT, PA, ['skill/reviewer', 'wp+plan', 'predecessor', 'amendments']),
}


def load_tokenizer(here):
    from tokenizers import Tokenizer
    path = os.path.join(here, 'tokenizer.json')
    if not os.path.exists(path):
        sys.exit(f'tokenizer.json not found at {path} — see the module docstring.')
    return Tokenizer.from_file(path)


def count(tok, root, patterns):
    rows, total = [], 0
    for pat in patterns:
        matches = sorted(glob.glob(os.path.join(root, pat))) if any(c in pat for c in '*?[') \
            else [os.path.join(root, pat)]
        for fp in matches:
            if not os.path.exists(fp):
                rows.append((None, os.path.relpath(fp, root)))
                continue
            n = len(tok.encode(open(fp, encoding='utf-8', errors='replace').read()).ids)
            total += n
            rows.append((n, os.path.relpath(fp, root)))
    return total, rows


def main():
    here = os.path.dirname(os.path.abspath(__file__))
    ap = argparse.ArgumentParser()
    ap.add_argument('--root', default=os.path.join(here, '..', '..'))
    ap.add_argument('--set', dest='which', choices=list(SETS) + ['all'], default='all')
    ap.add_argument('--json', action='store_true')
    ap.add_argument('--diff', help='extra file (e.g. a candidate diff) to include')
    args = ap.parse_args()
    root = os.path.abspath(args.root)
    tok = load_tokenizer(here)
    out = {}
    for name in ([args.which] if args.which != 'all' else list(SETS)):
        boot, groups, keys = SETS[name]
        total, detail = count(tok, root, boot)
        parts = {'boot': total}
        for k in keys:
            t, rows = count(tok, root, groups[k])
            parts[k] = t
            detail += rows
            total += t
        if args.diff:
            t, rows = count(tok, root, [args.diff])
            parts['diff'] = t
            total += t
            detail += rows
        out[name] = {'total': total, 'parts': parts}
        if not args.json:
            print(f'\n=== {name} ===')
            for n, f in sorted(detail, key=lambda r: -(r[0] or 0)):
                print(f'  {"MISSING":>9}  {f}' if n is None else f'  {n:>9,}  {f}')
            for k, v in parts.items():
                print(f'  {v:>9,}  [{k}]')
            print(f'  {total:>9,}  TOTAL cold-start reading')
    if args.json:
        print(json.dumps(out, indent=2))


if __name__ == '__main__':
    main()
