"""Recompute the H1 catalogue fingerprint (tools/Arkus.H1.UnityHost/H1CatalogueModel.cs FingerprintOf) in Python.

Used by WP-H2F-02 to re-baseline the pinned value after the URP adoption added one dependency to one row of
Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json. Validation: on the pre-H2F-02 snapshot it reproduces the pinned
e1e92d98... exactly. Run from the repository root; prints the fingerprint of HEAD's and the working tree's snapshot.
"""
import json, hashlib, subprocess, sys
def fp(effective_text, mapping_text):
    eff=json.loads(effective_text); mp=json.loads(mapping_text)
    def nk(kind,guid,lfid,tn): return kind+"|"+(tn if kind=="component-schema" else guid+"|"+lfid)
    by={nk(r['kind'],r['nativeGuid'],r['localFileId'],r['typeName']):r for r in eff['rows']}
    entries=[]
    for m in mp['entries']:
        f=by[nk(m['kind'],m.get('nativeGuid',''),m.get('localFileId',''),m.get('typeName',''))]
        entries.append((m['logicalId'],m['kind'],f['name'],f['typeName'],f['dimensions'],m.get('sourceId',''),m.get('adoptionStatus',''),
                        'true' if f['compatible'] else 'false',f['path'],f['nativeGuid'],f['localFileId'],f['contentSha256'],
                        sorted(f['dependencies']), sorted(f['schemaFields'])))
    def key(s): return s.encode('utf-16-be')  # ordinal comparison of UTF-16 code units
    entries.sort(key=lambda e:key(e[0]))
    b=["arkus.h1-catalogue-snapshot@1"]
    ap=lambda v: b.append(f"{len(v.encode('utf-16-le'))//2}:{v}")
    for e in entries:
        for v in e[:12]: ap(v)
        for v in sorted(e[12], key=key): ap(v)
        for v in sorted(e[13], key=key): ap(v)
        ap("<end>")
    return hashlib.sha256("".join(b).encode('utf-8')).hexdigest()
if __name__ == "__main__":
    mapping = open('Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json', encoding='utf-8').read()
    old = subprocess.check_output(['git', 'show', 'HEAD:Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json']).decode('utf-8')
    new = open('Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json', encoding='utf-8').read()
    print('head', fp(old, mapping))
    print('worktree', fp(new, mapping))
