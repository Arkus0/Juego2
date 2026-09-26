"""Rebuild ART-01's bounded, ignored Unity input slice from the owner's local CC0 sources.

`lock` records exact selected bytes after manual license review. `import` verifies the
committed lock and copies only those files; it never writes the source vault.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import shutil
import zipfile
from pathlib import Path


REPO = Path(__file__).resolve().parents[1]
LOCK = REPO / "Docs/evidence/WP-ART-01/SOURCE_LOCK.json"
DEST = REPO / "Unity/ArkusUnity/Assets/Arkus/ART/External"
MEDIEVAL_ZIP = "Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip"
MEDIEVAL_SHA = "b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10"
MEDIEVAL_PREFIX = "Medieval Village MegaKit[Unity URP]/Quaternius/Assets/Quaternius/Medieval Village MegaKit/"

MEDIEVAL_MODELS = [
    "Balcony_Simple_Straight", "Corner_Exterior_Brick", "Door_3_Flat",
    "DoorFrame_Flat_Brick", "Floor_RoundRocks", "Prop_Chimney",
    "Wall_UnevenBrick_Door_Flat", "Wall_UnevenBrick_Straight",
    "Wall_UnevenBrick_Window_Thin_Round", "Wall_UnevenBrick_Window_Wide_Flat",
    "Window_Wide_Flat1", "WindowShutters_Wide_Flat_Closed",
    "WindowShutters_Wide_Flat_Open", "Roof_RoundTiles_6x10",
    "Roof_RoundTiles_8x14",
]
MEDIEVAL_TEXTURES = [
    "T_Brick_BaseColor", "T_Plaster_BaseColor", "T_RoundRocks_BaseColor",
    "T_RoundTiles_BaseColor", "T_UnevenBrick_BaseColor", "T_WoodTrim_BaseColor",
]
NATURE_MODELS = [
    "CommonTree_1", "Bush_Common", "Bush_Common_Flowers", "Fern_1",
    "Grass_Common_Short", "Rock_Medium_1",
]
NATURE_TEXTURES = [
    "Bark_NormalTree.png", "Leaves_NormalTree.png", "Leaves.png",
    "Flowers.png", "Grass.png", "Rocks_Diffuse.png",
]
PROPS_MODELS = [
    "Barrel", "Bench", "Chair_1", "Crate_Wooden", "Lantern_Wall",
    "Mug", "Stool", "Table_Large",
]
PROPS_TEXTURES = [
    "T_Trim_Furniture_BaseColor.png", "T_Trim_Metal_BaseColor.png",
    "T_Trim_Props_BaseColor.png", "T_Trim_Cloth_BaseColor.png",
]
CHARACTER_FILES = [
    ("Base Characters/Base Characters/Exports/Unity/Regular_Male_FullBody.fbx", "Characters/Regular_Male_FullBody.fbx"),
    ("Base Characters/Hairstyles/Rigged to Head Bone/FBX (Unity)/Male/Hair_Buzzed.fbx", "Characters/Hair_Buzzed.fbx"),
    ("Base Characters/Base Characters/Textures/T_Regular_Male_Light_BaseColor.png", "Characters/T_Regular_Male_Light_BaseColor.png"),
    ("Base Characters/Base Characters/Textures/T_Eye_Brown.png", "Characters/T_Eye_Brown.png"),
    ("Base Characters/Base Characters/Textures/T_Hair_1_BaseColor.png", "Characters/T_Hair_1_BaseColor.png"),
]


def sha(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def file_sha(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for chunk in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def source_rows(vault: Path) -> tuple[list[dict], dict]:
    archive = vault / MEDIEVAL_ZIP
    actual_archive_sha = file_sha(archive)
    if actual_archive_sha != MEDIEVAL_SHA:
        raise ValueError(f"H1-04 Medieval archive pin mismatch: {actual_archive_sha}")

    rows: list[dict] = []
    with zipfile.ZipFile(archive) as bundle:
        for name in MEDIEVAL_MODELS:
            member = MEDIEVAL_PREFIX + f"Modules/Source Models/{name}.fbx"
            rows.append({"source": "quaternius-medieval-source", "member": member,
                         "dest": f"Medieval/Models/{name}.fbx", "sha256": sha(bundle.read(member))})
        for name in MEDIEVAL_TEXTURES:
            member = MEDIEVAL_PREFIX + f"Materials/{name}.png"
            rows.append({"source": "quaternius-medieval-source", "member": member,
                         "dest": f"Medieval/Textures/{name}.png", "sha256": sha(bundle.read(member))})

    for name in NATURE_MODELS:
        rel = f"Nature/FBX (Unity)/{name}.fbx"
        rows.append({"source": "quaternius-nature-standard", "file": rel,
                     "dest": f"Nature/Models/{name}.fbx", "sha256": file_sha(vault / rel)})
    for name in NATURE_TEXTURES:
        rel = f"Nature/Textures/{name}"
        rows.append({"source": "quaternius-nature-standard", "file": rel,
                     "dest": f"Nature/Textures/{name}", "sha256": file_sha(vault / rel)})
    for name in PROPS_MODELS:
        rel = f"Props (gratis)/Exports/FBX/{name}.fbx"
        rows.append({"source": "quaternius-props-standard", "file": rel,
                     "dest": f"Props/Models/{name}.fbx", "sha256": file_sha(vault / rel)})
    for name in PROPS_TEXTURES:
        rel = f"Props (gratis)/Textures/{name}"
        rows.append({"source": "quaternius-props-standard", "file": rel,
                     "dest": f"Props/Textures/{name}", "sha256": file_sha(vault / rel)})
    for rel, dest in CHARACTER_FILES:
        rows.append({"source": "quaternius-base-characters-source", "file": rel,
                     "dest": dest, "sha256": file_sha(vault / rel)})
    ual = "Animation/Unity/UAL1.fbx"
    rows.append({"source": "quaternius-ual1-source", "file": ual,
                 "dest": "UAL/UAL1.fbx", "sha256": file_sha(vault / ual)})

    licenses = {
        "quaternius-medieval-source": {"file": "Medieval Village/License_Source.txt", "sha256": file_sha(vault / "Medieval Village/License_Source.txt")},
        "quaternius-nature-standard": {"file": "Nature/License_Standard.txt", "sha256": file_sha(vault / "Nature/License_Standard.txt")},
        "quaternius-props-standard": {"file": "Props (gratis)/License_Standard.txt", "sha256": file_sha(vault / "Props (gratis)/License_Standard.txt")},
        "quaternius-base-characters-source": {"file": "Base Characters/License_Source.txt", "sha256": file_sha(vault / "Base Characters/License_Source.txt")},
        "quaternius-ual1-source": {"file": "Animation/License.txt", "sha256": file_sha(vault / "Animation/License.txt")},
    }
    return rows, licenses


def check_lock(vault: Path, existing: dict) -> list[dict]:
    rows, licenses = source_rows(vault)
    expected = {"schema": "juego2.art01.source-lock@1", "medievalArchiveSha256": MEDIEVAL_SHA,
                "licenseId": "CC0-1.0", "licenses": licenses, "files": rows}
    if existing != expected:
        raise ValueError("Source lock mismatch: selected bytes, license or source list changed")
    return rows


def deterministic_meta(path: Path, relative: str) -> None:
    meta = path.with_name(path.name + ".meta")
    if not meta.exists():
        guid = sha(f"juego2-art01-external:{relative}".encode())[:32]
        meta.write_text(f"fileFormatVersion: 2\nguid: {guid}\n", encoding="utf-8")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("mode", choices=["lock", "verify", "import"])
    parser.add_argument("--assets-root", type=Path, default=Path("C:/Juego2-Assets"))
    args = parser.parse_args()
    vault = args.assets_root.resolve(strict=True)
    if args.mode == "lock":
        rows, licenses = source_rows(vault)
        value = {"schema": "juego2.art01.source-lock@1", "medievalArchiveSha256": MEDIEVAL_SHA,
                 "licenseId": "CC0-1.0", "licenses": licenses, "files": rows}
        LOCK.parent.mkdir(parents=True, exist_ok=True)
        LOCK.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        print(f"ART01_SOURCE_LOCK_WRITTEN files={len(rows)}")
        return
    rows = check_lock(vault, json.loads(LOCK.read_text(encoding="utf-8")))
    if args.mode == "verify":
        print(f"ART01_SOURCE_VERIFIED files={len(rows)}")
        return
    with zipfile.ZipFile(vault / MEDIEVAL_ZIP) as bundle:
        for row in rows:
            data = bundle.read(row["member"]) if "member" in row else (vault / row["file"]).read_bytes()
            if sha(data) != row["sha256"]:
                raise ValueError(f"Byte mismatch: {row['dest']}")
            target = DEST / row["dest"]
            target.parent.mkdir(parents=True, exist_ok=True)
            if not target.exists() or sha(target.read_bytes()) != row["sha256"]:
                target.write_bytes(data)
            deterministic_meta(target, row["dest"])
    print(f"ART01_SOURCE_IMPORTED files={len(rows)} target={DEST}")


if __name__ == "__main__":
    main()
