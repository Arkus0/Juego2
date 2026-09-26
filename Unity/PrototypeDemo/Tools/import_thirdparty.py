"""Copy the Quaternius CC0 packs the prototype uses into Assets/ThirdParty (git-ignored).

Run from anywhere:  python Unity/PrototypeDemo/Tools/import_thirdparty.py [--vault C:/Juego2-Assets]

* Medieval Village MegaKit comes from the pack's own Unity URP project (keeps its .meta GUIDs).
* Nature / Props / UAL get deterministic .meta GUIDs (md5 of their project path), so the
  generated scene keeps resolving after a fresh copy on another machine.
Nothing is downloaded; the vault stays untouched.
"""

from __future__ import annotations

import argparse
import hashlib
import shutil
import zipfile
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[1]
DEST = PROJECT / "Assets" / "ThirdParty" / "Quaternius"

URP_ZIP = "Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip"
URP_PREFIX = "Medieval Village MegaKit[Unity URP]/Quaternius/Assets/Quaternius/Medieval Village MegaKit/"

LOOSE = [
    # (vault dir, glob, dest dir)
    ("Nature/FBX (Unity)", "*.fbx", "Nature/Models"),
    ("Nature/Textures", "*.png", "Nature/Textures"),
    ("Props (gratis)/Exports/FBX", "*.fbx", "Props/Models"),
    ("Props (gratis)/Textures", "*.png", "Props/Textures"),
    ("Animation/Unity", "UAL1.fbx", "UAL/Models"),
    ("Animation2/Unity", "UAL2.fbx", "UAL/Models"),
    ("Animation2/Female Mannequin/Unity", "Mannequin_F.fbx", "UAL/Models"),
]
LICENSES = [
    ("Medieval Village/License_Source.txt", "MedievalVillage/LICENSE_Quaternius.txt"),
    ("Nature/License_Standard.txt", "Nature/LICENSE_Quaternius.txt"),
    ("Props (gratis)/License_Standard.txt", "Props/LICENSE_Quaternius.txt"),
    ("Animation/License.txt", "UAL/LICENSE_Quaternius.txt"),
]


def guid_for(rel: str) -> str:
    return hashlib.md5(("juego2-prototype:" + rel.replace("\\", "/")).encode("utf-8")).hexdigest()


def write_meta(path: Path, folder: bool = False) -> None:
    meta = path.with_name(path.name + ".meta")
    if meta.exists():
        return
    rel = path.relative_to(PROJECT).as_posix()
    body = f"fileFormatVersion: 2\nguid: {guid_for(rel)}\n"
    if folder:
        body += "folderAsset: yes\n"
    meta.write_text(body, encoding="utf-8")


def ensure_dir(path: Path) -> None:
    missing = []
    p = path
    while not p.exists():
        missing.append(p)
        p = p.parent
    for d in reversed(missing):
        d.mkdir()
        write_meta(d, folder=True)


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--vault", default="C:/Juego2-Assets")
    args = ap.parse_args()
    vault = Path(args.vault)

    ensure_dir(DEST / "MedievalVillage")
    count = 0
    with zipfile.ZipFile(vault / URP_ZIP) as z:
        for name in z.namelist():
            if not name.startswith(URP_PREFIX) or name.endswith("/"):
                continue
            rel = name[len(URP_PREFIX):]
            target = DEST / "MedievalVillage" / rel
            ensure_dir(target.parent)
            if not target.exists():
                with z.open(name) as src, open(target, "wb") as dst:
                    shutil.copyfileobj(src, dst)
                count += 1
    print(f"MedievalVillage (URP project, original metas): {count} files")

    for src_dir, pattern, dst_dir in LOOSE:
        out = DEST / dst_dir
        ensure_dir(out)
        n = 0
        for f in sorted((vault / src_dir).glob(pattern)):
            target = out / f.name
            if not target.exists():
                shutil.copy2(f, target)
                n += 1
            # Models get deterministic GUIDs; textures need Unity's own TextureImporter meta
            # (a GUID-only meta makes Unity treat a PNG as a DefaultAsset).
            if target.suffix.lower() == ".fbx":
                write_meta(target)
        print(f"{dst_dir}: {n} new files")

    for src, dst in LICENSES:
        target = DEST / dst
        ensure_dir(target.parent)
        shutil.copy2(vault / src, target)
        write_meta(target)
    print("done ->", DEST)


if __name__ == "__main__":
    main()
