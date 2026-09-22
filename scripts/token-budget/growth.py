#!/usr/bin/env python3
"""Measure how the mandatory boot-set reading cost grows over project history.

NON-CONTRACTUAL measurement tooling. Companion to measure.py; same tokenizer
setup (see that file's docstring).

Walks every commit that touched Docs/ROADMAP.md, tokenizes the boot set at that
revision, and reports the two growth terms: the linear cost per accepted
workpack, and the step cost of accepting a new phase plan.

Usage:  python3 scripts/token-budget/growth.py [--ref origin/main]
"""
import argparse, json, os, re, subprocess, sys

BOOT = ['CLAUDE.md', 'AGENTS.md', 'Docs/ROADMAP.md', 'Docs/workpacks/README.md',
        'Docs/engineering/WORKER_REVIEW_PROTOCOL.md',
        'Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md',
        'Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md']


def main():
    here = os.path.dirname(os.path.abspath(__file__))
    ap = argparse.ArgumentParser()
    ap.add_argument('--ref', default='origin/main')
    ap.add_argument('--json', action='store_true')
    args = ap.parse_args()

    from tokenizers import Tokenizer
    tp = os.path.join(here, 'tokenizer.json')
    if not os.path.exists(tp):
        sys.exit(f'tokenizer.json not found at {tp} — see measure.py docstring.')
    tok = Tokenizer.from_file(tp)

    def git(*a):
        return subprocess.run(['git', *a], capture_output=True, text=True).stdout

    revs = [l.split('@@') for l in git('log', '--format=%H@@%ad', '--date=short',
                                       '--reverse', args.ref, '--', 'Docs/ROADMAP.md'
                                       ).strip().split('\n') if '@@' in l]
    rows = []
    for sha, date in revs:
        total = roadmap = 0
        for f in BOOT:
            blob = git('show', f'{sha}:{f}')
            if not blob:
                continue
            n = len(tok.encode(blob).ids)
            total += n
            if f.endswith('ROADMAP.md'):
                roadmap = n
        accepted = len(set(re.findall(r'`(WP-[A-Z0-9-]+)`\s*(?:✅\s*)?COMPLETE',
                                      git('show', f'{sha}:Docs/ROADMAP.md'))))
        rows.append({'date': date, 'sha': sha[:8], 'roadmap': roadmap,
                     'boot': total, 'accepted': accepted})

    if args.json:
        print(json.dumps(rows, indent=2))
        return

    print(f"{'date':<12}{'commit':<10}{'ROADMAP':>9}{'boot set':>10}{'accepted WPs':>14}")
    for r in rows:
        print(f"{r['date']:<12}{r['sha']:<10}{r['roadmap']:>9,}{r['boot']:>10,}{r['accepted']:>14}")

    # first occurrence of each count: a later revision with the same count is a
    # phase-plan step, which belongs to the step term, not the linear one.
    have = {}
    for r in rows:
        have.setdefault(r['accepted'], r)
    lo = min(k for k in have if k >= 1)
    hi = max(have)
    if hi > lo:
        a, b = have[lo], have[hi]
        span = hi - lo
        print(f"\nlinear term, WP {lo} -> {hi}:")
        print(f"  boot    +{(b['boot'] - a['boot']) / span:.0f} tokens per accepted workpack")
        print(f"  ROADMAP +{(b['roadmap'] - a['roadmap']) / span:.0f} tokens per accepted workpack")
    print("\nstep term (boot set rising with no new accepted workpack):")
    for prev, cur in zip(rows, rows[1:]):
        if cur['accepted'] == prev['accepted'] and cur['boot'] - prev['boot'] > 500:
            print(f"  {prev['sha']} -> {cur['sha']}  +{cur['boot'] - prev['boot']:,} "
                  f"at {cur['accepted']} accepted workpacks")
    print("  Each accepted phase also adds its binding architecture to the foundational "
          "read set, which this boot set does not include.")


if __name__ == '__main__':
    main()
