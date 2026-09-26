"""Emit ART-owned composition metadata from pinned source bytes and Unity measurements.

The manifest is a companion to H1 catalogue identity, never a mutation of H1.
Only the bounded ART benchmark's admitted pieces receive KEEPER_READY.
"""

from __future__ import annotations

import hashlib
import json
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
EVIDENCE = ROOT / "Docs/evidence/WP-ART-01"
LOCK = json.loads((EVIDENCE / "SOURCE_LOCK.json").read_text(encoding="utf-8"))
AUDIT = json.loads((EVIDENCE / "UNITY_SOURCE_AUDIT_NORMALIZED.json").read_text(encoding="utf-8"))
SOURCE = {row["dest"]: row for row in LOCK["files"]}

USED = {
    "Corner_Exterior_Brick", "Door_3_Flat", "DoorFrame_Flat_Brick",
    "Roof_RoundTiles_6x10", "Roof_RoundTiles_8x14",
    "Wall_UnevenBrick_Door_Flat", "Wall_UnevenBrick_Straight",
    "Wall_UnevenBrick_Window_Wide_Flat", "Window_Wide_Flat1",
    "WindowShutters_Wide_Flat_Open", "Bush_Common", "Bush_Common_Flowers",
    "CommonTree_1", "Fern_1", "Rock_Medium_1", "Barrel", "Bench",
    "Chair_1", "Crate_Wooden", "Lantern_Wall", "Mug", "Stool", "Table_Large",
}
SOURCE_ONLY = {
    "Roof_RoundTiles_6x10", "Roof_RoundTiles_8x14", "Regular_Male_FullBody",
    "Hair_Buzzed", "UAL1", "Balcony_Simple_Straight", "Floor_RoundRocks",
    "Prop_Chimney", "WindowShutters_Wide_Flat_Closed", "Grass_Common_Short",
}
REJECT = {"Wall_UnevenBrick_Window_Thin_Round"}


def role(name: str, family: str) -> str:
    if name.startswith("Wall_"): return "FACADE_HOST"
    if name.startswith("Corner_"): return "CORNER"
    if name.startswith("Roof_"): return "ROOF_SOURCE"
    if name.startswith("WindowShutters") or name.startswith("Window_") or name.startswith("Door"): return "HOSTED_INSERT"
    if family == "Nature": return "NATURE"
    if family == "Props": return "DRESSING"
    if family in {"Characters", "UAL"}: return "SCALE_SOURCE_ONLY"
    return "SOURCE_REFERENCE"


def source_piece(row: dict) -> dict:
    path = row["path"].removeprefix("Assets/Arkus/ART/External/")
    family = path.split("/")[0]
    name = Path(path).stem
    dest = path
    locked = SOURCE[dest]
    current_role = role(name, family)
    readiness = ("REJECT_FOR_KEEPER" if name in REJECT else
                 "SOURCE_ONLY" if name in SOURCE_ONLY or name not in USED else
                 "KEEPER_READY")
    is_modular = current_role in {"FACADE_HOST", "CORNER", "HOSTED_INSERT"}
    if current_role == "FACADE_HOST":
        hosts = ["art01.derived.plinth.segment.v1", "adjacent 2.00 m facade host"]
        connections = ["SUPPORTED_BY plinth at y=0", "MEETS adjacent host at x=±1.00 m",
                       "HOSTS named real cut opening if Door/Window variant"]
        collision = "scene-generated jambs/lintel or full wall; no competing source mesh collider"
    elif current_role == "HOSTED_INSERT":
        hosts = ["matching 2.00 m facade opening only"]
        connections = ["FILLS matching cut host at common source pivot; never plain wall sticker"]
        collision = "none; host owns structural collision"
    elif current_role == "ROOF_SOURCE":
        hosts = ["art01.derived.roof.lowpitch.* only"]
        connections = ["source geometry remeasured and recapped by named derivative"]
        collision = "none"
    elif current_role == "NATURE":
        hosts = ["site soil outside traversable route"]
        connections = ["ROOTED_IN scenic bank; maintain route clearance"]
        collision = "none in benchmark"
    elif current_role == "DRESSING":
        hosts = ["floor", "ground", "wall host" if name == "Lantern_Wall" else "countertop"]
        connections = ["SUPPORTED_BY named host; never seam cover"]
        collision = "none in bounded visual specimen"
    else:
        hosts = []; connections = []; collision = "none"
    exceptions = []
    if family == "Props": exceptions.append("FBX root scale x100 and X≈270°; preserve prefab root and multiply local scale")
    if family in {"Medieval", "Nature", "Props"}:
        exceptions.append("raw FBX centimetres normalized by importer globalScale=0.01")
    if name.startswith("Roof_"): exceptions.append("source steep/chalet roof cannot be used unmodified")
    if name == "Fern_1": exceptions.append("source bounds 9.05×2.69×8.49 m; benchmark placement scale 0.12–0.16")
    if name == "Window_Wide_Flat1": exceptions.append("source 1.61 m overall opening exceeds 1.40 m typical band; reviewed source-module exception")
    if name == "Wall_UnevenBrick_Window_Thin_Round": exceptions.append("round head incompatible with rectangular retained benchmark")
    return {
        "id": f"art01.quaternius.{family.lower()}.{name.lower()}",
        "kind": "source_model", "readiness": readiness, "role": current_role,
        "provenance": {"sourceId": locked["source"], "sourceLockDest": dest,
                       "sha256": locked["sha256"], "license": LOCK["licenseId"]},
        "effectiveBoundsM": {"size": row["boundsSize"], "center": row["boundsCenter"]},
        "import": {"globalScale": row["importerGlobalScale"],
                   "rootScale": row["rootScale"], "rootRotationEuler": row["rootRotationEuler"]},
        "pivot": "FBX root local origin; facade base y=0; insert shares source host origin" if is_modular else
                 "FBX root local origin; use audited bounds/contact plane",
        "facing": "+Z exterior normal for normalized facade family; verify mirrored corner/door leaf" if is_modular else
                  "preserve source root rotation, then author yaw in wrapper",
        "allowedHosts": hosts, "connections": connections,
        "repeatSnap": "x=2.00 m module, 0.25 m assembly snap, 0.05 m trim correction" if is_modular else
                      "no automatic repeat; placement-specific scale and clearance",
        "materialFamily": "ART01 Standard/built-in explicit remap; URP consumer requires separate explicit remap",
        "collisionTraversal": collision, "knownIncompatibilities": exceptions,
    }


def derived(id: str, role: str, dimensions: str, source_ids: list[str], hosts: list[str],
            connections: list[str], collision: str, issues: list[str] | None = None,
            repeat: str = "0.25 m massing snap; 0.05 m contact correction") -> dict:
    return {"id": id, "kind": "juego2_derivative", "readiness": "KEEPER_READY",
            "role": role, "provenance": {"lineage": source_ids,
            "implementation": "Unity/ArkusUnity/Assets/Arkus/ART/Editor/Art01BenchmarkBuilder.cs",
            "license": "Juego2-owned derivative of CC0 source where listed"},
            "effectiveDimensionsM": dimensions,
            "pivot": "local datum at support plane unless connection says otherwise",
            "facing": "+Z local outward; wrapper yaw supplies street orientation",
            "allowedHosts": hosts, "connections": connections,
            "repeatSnap": repeat,
            "materialFamily": "ART01 built-in Standard explicit material IDs; overcast target",
            "collisionTraversal": collision, "knownIncompatibilities": issues or []}


derived_pieces = [
    derived("art01.derived.render_host.straight.v1", "FACADE_HOST", "2.00 W × 3.122689 H × 0.44 D",
            ["quaternius-medieval-source:Wall_UnevenBrick_Straight", "T_Plaster_BaseColor"],
            ["plinth", "adjacent 2 m host"], ["SUPPORTED_BY plinth", "MEETS adjacent host", "CAPS at eave"],
            "full wall BoxCollider"),
    derived("art01.derived.render_host.window_wide_flat.v1", "FACADE_HOST",
            "2.00 W × 3.122689 H × 0.44 D; aperture 1.61 W × 1.59 H, sill 0.94",
            ["quaternius-medieval-source:Wall_UnevenBrick_Window_Wide_Flat", "Window_Wide_Flat1"],
            ["plinth", "matching wide flat window insert"],
            ["HOSTS true through-opening", "sill/jamb/lintel extruded 0.44 m"],
            "four jamb/sill/lintel colliders; no full-window block",
            ["overall opening 1.61 m exceeds 1.40 m typical profile; accepted source insert exception"]),
    derived("art01.derived.render_host.door_flat.v1", "FACADE_HOST",
            "2.00 W × 3.122689 H × 0.44 D; opening 1.61 W × 2.39 H at ground",
            ["quaternius-medieval-source:Wall_UnevenBrick_Door_Flat", "DoorFrame_Flat_Brick"],
            ["plinth gap", "public/private door insert"],
            ["HOSTS real clear opening", "MEETS landing and interior floor"],
            "two jambs plus lintel, no doorway floor blocker"),
    derived("art01.derived.roof.lowpitch.6x10.v1", "ROOF", "source 8.25×5.67×11.85; applied scale 0.86×0.45×0.94",
            ["quaternius-medieval-source:Roof_RoundTiles_6x10"], ["6×10 m two-storey wall assembly"],
            ["eave derived from 0.30+2×3.122689 m wall top", "gable penetrates roof underside by ≤0.25 m"],
            "none", ["source chalet pitch vetoed; only named flattened assembly is admitted"]),
    derived("art01.derived.roof.lowpitch.8x14.v1", "ROOF", "source 9.95×6.78×15.71; applied scale 0.91×0.45×0.96",
            ["quaternius-medieval-source:Roof_RoundTiles_8x14"], ["8×14 m two-storey wall assembly"],
            ["eave derived from supported wall top", "gable penetrates roof underside by ≤0.25 m"],
            "none", ["source chalet pitch vetoed; do not generalize scaling to other roof models"]),
    derived("art01.derived.gable.stone.v1", "GABLE", "span 6 or 8 m; rise derives from matching lowpitch roof bounds",
            ["quaternius-medieval-source:roof family"], ["matching 6×10 or 8×14 roof"],
            ["MEETS upper wall", "CAPS under roof by 0.25 m overlap"], "none",
            ["mismatched roof width/pitch creates visible gap"]),
    derived("art01.derived.plinth.segment.v1", "PLINTH", "0.30 H × 0.52 D, 2 m facade repeat; interrupted at door",
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["site datum", "2 m facade host"],
            ["SUPPORTED_BY site", "SUPPORTS facade", "MEETS threshold and street edge"],
            "non-traversable visual base", ["never block public door gap"]),
    derived("art01.derived.threshold.lean_to.v1", "PORCH_ROOF", "3.60 W × 1.65 projection; 3.10 wall rise to 2.82 eave",
            ["quaternius-medieval-source:T_RoundTiles_BaseColor"], ["F01 facade", "two 2.65 m posts"],
            ["MEETS facade beneath upper floor", "SUPPORTED_BY posts", "CAPS threshold"],
            "posts only; roof visual", ["not an independent floating roof"]),
    derived("art01.derived.threshold.public.v1", "THRESHOLD", "landing 1.20 W × 0.96 D × 0.11; step rise 0.15",
            ["quaternius-medieval-source:T_Brick_BaseColor"], ["door frame", "street", "interior floor"],
            ["TRANSITIONS_TO street, landing and interior floor"], "landing/step single local support owner",
            ["CITY-07 must still validate final access role/elevation"]),
    derived("art01.derived.street.crowned.v1", "STREET", "variable width X1 5.5 → W12 2.8 → micro B 2.4 m; crown +0.045",
            ["quaternius-medieval-source:T_RoundRocks_BaseColor"], ["site datum"],
            ["SUPPORTED_BY site", "MEETS named edge/drain/threshold"], "one continuous MeshCollider traversable owner",
            ["ART specimen stations are not CITY-07 coordinates or elevation decisions"]),
    derived("art01.derived.street.edge_drain.v1", "EDGE_DRAIN", "kerb 0.18 W × 0.13 rise; drain visual groove 0.12 W",
            ["quaternius-medieval-source:T_Brick_BaseColor"], ["street side"],
            ["MEETS road edge", "DRAINS_TO visual groove outside sole path collider"], "none on path"),
    derived("art01.derived.retaining.capped.v1", "RETAINING", "0.42 W × 0.56 H garden; 0.53 W × 0.09 cap",
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["site bank/shoulder"],
            ["SUPPORTED_BY ground", "CAPS wall", "MEETS road shoulder"], "static wall collider outside path"),
    derived("art01.derived.site.scenic_bank.v1", "SCENIC", "local sample ±48 m width, irregular rise 0–2.5 m",
            ["quaternius-nature-standard:wet valley palette"], ["outside traversable road"],
            ["MEETS road visually below edge", "ROOTS vegetation"], "none; road owns traversal",
            ["not CITY heightfield/elevation authority"]),
    derived("art01.derived.sign.bar.v1", "SIGN", "1.55 W × 0.48 H × 0.10 D; two independently oriented text faces",
            [], ["F01 facade host"], ["ATTACHED_TO host", "FACES both directions without mirrored text"],
            "none", ["word BAR is specimen label, not canonized venue name"]),
    derived("art01.derived.clothed_scale.forastero.v1", "SCALE_REFERENCE", "1.8 m standing human; no behavior",
            ["quaternius-base-characters-source:Regular_Male_FullBody", "Hair_Buzzed",
             "UAL1:Idle_Loop", "diagnostic #233 garment-carving technique"], ["road", "public floor"],
            ["SUPPORTED_BY route surface", "FACES travel direction after documented 180° visual adjustment"],
            "inspection CharacterController only; no NPC/gameplay", ["raw T-pose/mannequin is PROXY_VISUAL"]),
]


pieces = [source_piece(row) for row in AUDIT["rows"] if row["path"].endswith(".fbx")]
pieces += derived_pieces
human_fbx = ROOT / "Unity/ArkusUnity/Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx"
if not human_fbx.is_file():
    raise FileNotFoundError("ART01_DERIVED_HUMAN_MISSING")
for piece in pieces:
    if piece["id"] == "art01.derived.clothed_scale.forastero.v1":
        piece["provenance"]["artifact"] = str(human_fbx.relative_to(ROOT)).replace("\\", "/")
        piece["provenance"]["artifactSha256"] = hashlib.sha256(human_fbx.read_bytes()).hexdigest()
ids = [piece["id"] for piece in pieces]
if len(ids) != len(set(ids)):
    raise ValueError("duplicate ART logical ID")
manifest = {
    "schema": "juego2.art01.kit-composition@1", "wp": "WP-ART-01",
    "authority": "ART-owned composition companion; does not alter H0/H1/CITY contracts",
    "unityVersion": AUDIT["unityVersion"],
    "sourceLockSha256": hashlib.sha256((EVIDENCE / "SOURCE_LOCK.json").read_bytes()).hexdigest(),
    "metricPolicy": "Docs/workpacks/ART/ART_01_DIMENSIONAL_PROFILE.md",
    "presentationStates": {
        "KEEPER_READY": "admitted composition with required host/support/metric truth",
        "PROXY_VISUAL": "deliberate visual placeholder; cannot satisfy required benchmark",
        "COVERAGE_BLOCKED": "required composition cannot be produced from admitted kit",
    },
    "pieces": pieces,
}
out = EVIDENCE / "KIT_COMPOSITION_MANIFEST.json"
out.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
print(f"ART01_MANIFEST_WRITTEN pieces={len(pieces)} keeper={sum(p['readiness']=='KEEPER_READY' for p in pieces)}")
