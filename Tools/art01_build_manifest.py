"""Emit ART-owned composition metadata from pinned source bytes and Unity measurements.

The manifest is a companion to H1 catalogue identity, never a mutation of H1.
Only the bounded ART benchmark's admitted pieces receive KEEPER_READY.

Every tagged benchmark instance carries an ``Art01Piece.kitId`` that must resolve to one
``pieces`` or ``assemblies`` entry here; ``Art01StructuralAudit`` enforces that closure, the
declared ``unityAssets``/``sourceLockDest`` provenance, BOX-form declarations and the
explicit ``dimensionalExceptions`` against ART_01_DIMENSIONAL_PROFILE.md.

Materials are recorded only as *family intent*. Concrete shader/material assets are
render-pipeline work and are re-made for URP before the CANDIDATE checkpoint.
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
MESHES = "Assets/Arkus/ART/Derived/Meshes/"
EXTERNAL = "Assets/Arkus/ART/External/"
BUILDER = "Unity/ArkusUnity/Assets/Arkus/ART/Editor/Art01BenchmarkBuilder.cs"
STOREY = 3.122689
WALL_OUT, WALL_IN = 0.0924, 0.3141
MATERIAL_NOTE = ("family intent only; concrete materials are render-pipeline work "
                 "(built-in Standard now, explicit URP remap before CANDIDATE)")

USED = {
    "Corner_Exterior_Brick", "Door_3_Flat", "DoorFrame_Flat_Brick",
    "Roof_RoundTiles_6x10", "Roof_RoundTiles_8x14",
    "Wall_UnevenBrick_Door_Flat", "Wall_UnevenBrick_Straight",
    "Wall_UnevenBrick_Window_Wide_Flat", "Window_Wide_Flat1",
    "WindowShutters_Wide_Flat_Open", "WindowShutters_Wide_Flat_Closed",
    "Bush_Common", "Bush_Common_Flowers",
    "CommonTree_1", "Fern_1", "Rock_Medium_1", "Barrel", "Bench",
    "Chair_1", "Crate_Wooden", "Lantern_Wall", "Mug", "Stool", "Table_Large",
}
SOURCE_ONLY = {
    "Roof_RoundTiles_6x10", "Roof_RoundTiles_8x14", "Regular_Male_FullBody",
    "Hair_Buzzed", "UAL1", "Balcony_Simple_Straight", "Floor_RoundRocks",
    "Prop_Chimney", "Grass_Common_Short",
}
REJECT = {"Wall_UnevenBrick_Window_Thin_Round"}

WINDOW_EXCEPTIONS = [
    {"measure": "WINDOW_OPENING_WIDTH", "min": 1.40, "max": 1.62,
     "reason": "reviewed source-insert exception: Quaternius Window_Wide_Flat1 is 1.61 m; "
               "the host opening is sized to that insert rather than stretching it"},
    {"measure": "WINDOW_OPENING_HEIGHT", "min": 1.55, "max": 1.60,
     "reason": "reviewed source-insert exception: Window_Wide_Flat1 is 1.58 m tall"},
]
DOOR_EXCEPTIONS = [
    {"measure": "PUBLIC_DOOR_CLEAR_WIDTH", "min": 1.10, "max": 1.16,
     "reason": "reviewed source-insert exception: DoorFrame_Flat_Brick leaves 1.14 m clear; "
               "stylized low-poly proportion accepted for the first slice, never narrower than the band"},
    {"measure": "PRIVATE_DOOR_CLEAR_WIDTH", "min": 1.00, "max": 1.16,
     "reason": "same single source door frame for private doors; one frame family in the first slice"},
]


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
    path = row["path"].removeprefix(EXTERNAL)
    family = path.split("/")[0]
    name = Path(path).stem
    locked = SOURCE[path]
    current_role = role(name, family)
    readiness = ("REJECT_FOR_KEEPER" if name in REJECT else
                 "SOURCE_ONLY" if name in SOURCE_ONLY or name not in USED else
                 "KEEPER_READY")
    is_modular = current_role in {"FACADE_HOST", "CORNER", "HOSTED_INSERT"}
    exceptions: list[dict] = []
    notes: list[str] = []
    if current_role == "FACADE_HOST":
        hosts = ["art01.derived.plinth.segment.v1 (ground storey)", "lower storey host (upper storey)"]
        connections = [f"SUPPORTED_BY plinth top / lower storey top at host y=0",
                       "MEETS adjacent host at x=+/-1.00 m", "corner notch closed by art01.derived.facade.corner_closure.v1",
                       "HOSTS real cut opening if Door/Window variant"]
        collision = "scene-generated jambs/lintel or full wall BoxColliders; no source mesh collider"
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
        connections = ["ROOTED_IN scenic ground at its measured height; CLEAR_OF route and structure"]
        collision = "none in benchmark"
    elif current_role == "DRESSING":
        hosts = ["floor", "ground", "wall host" if name == "Lantern_Wall" else "countertop"]
        connections = ["ATTACHED_TO facade host face" if name == "Lantern_Wall" else "SUPPORTED_BY named host; never seam cover"]
        collision = "none in bounded visual specimen"
    else:
        hosts = []; connections = []; collision = "none"
    if family == "Props": notes.append("FBX root scale x100 and X~270 deg; preserve prefab root and multiply local scale")
    if family in {"Medieval", "Nature", "Props"}:
        notes.append("raw FBX centimetres normalized by importer globalScale=0.01")
    if name.startswith("Roof_"): notes.append("source steep/chalet roof cannot be used unmodified")
    if name == "Fern_1": notes.append("source bounds 9.05x2.69x8.49 m; benchmark placement scale 0.12-0.16")
    if name == "Wall_UnevenBrick_Window_Thin_Round": notes.append("round head incompatible with rectangular retained benchmark")
    if name == "Wall_UnevenBrick_Window_Wide_Flat":
        notes.append("measured through-opening 1.20 x 1.275 m, sill 1.05 m: within profile; the Window_Wide_Flat1 frame overlaps the stone reveal")
    if name == "DoorFrame_Flat_Brick": exceptions = DOOR_EXCEPTIONS
    if name == "Door_3_Flat": notes.append("leaf is placed 0.10 m behind the frame face (reveal 0.18 m)")
    if name == "WindowShutters_Wide_Flat_Open":
        notes.append("open leaves reach +/-1.24 m from the module centre: admitted only when both neighbouring "
                     "modules on the same facade/storey are plain and the module is not a facade end")
    if name == "WindowShutters_Wide_Flat_Closed":
        notes.append("fits inside the 2 m module; used where open leaves would clash with a neighbouring opening or overhang a corner")
    if name == "Corner_Exterior_Brick":
        notes.append("3.02 m tall on a 3.12 m storey; the corner notch above it is closed by corner_closure")
    return {
        "id": f"art01.quaternius.{family.lower()}.{name.lower()}",
        "kind": "source_model", "readiness": readiness, "role": current_role,
        "provenance": {"sourceId": locked["source"], "sourceLockDest": path,
                       "sha256": locked["sha256"], "license": LOCK["licenseId"]},
        "effectiveBoundsM": {"size": row["boundsSize"], "center": row["boundsCenter"]},
        "import": {"globalScale": row["importerGlobalScale"],
                   "rootScale": row["rootScale"], "rootRotationEuler": row["rootRotationEuler"]},
        "pivot": "FBX root local origin; facade base y=0; insert shares source host origin" if is_modular else
                 "FBX root local origin at base; place on measured support height",
        "facing": "+Z local = exterior normal for normalized facade family" if is_modular else
                  "preserve source root rotation, then author yaw in wrapper",
        "allowedHosts": hosts, "connections": connections,
        "repeatSnap": "x=2.00 m module, 0.25 m assembly snap, 0.05 m trim correction" if is_modular else
                      "no automatic repeat; placement-specific uniform scale recorded in the scene digest",
        "materialFamily": MATERIAL_NOTE,
        "collisionTraversal": collision, "dimensionalExceptions": exceptions,
        "knownIncompatibilities": notes,
    }


def derived(id: str, role: str, dimensions: dict, lineage: list[str], hosts: list[str],
            connections: list[str], collision: str, *, assets: list[str] | None = None,
            box: bool = False, issues: list[str] | None = None, exceptions: list[dict] | None = None,
            repeat: str = "0.25 m massing snap; 0.05 m contact correction", extra: dict | None = None) -> dict:
    piece = {"id": id, "kind": "juego2_derivative", "readiness": "KEEPER_READY", "role": role,
             "provenance": {"lineage": lineage, "implementation": BUILDER,
                            "license": "Juego2-owned derivative of CC0 source where listed",
                            "unityAssets": assets or []},
             "formPrimitive": "BOX" if box else "MESH",
             "dimensionsM": dimensions,
             "pivot": "local datum at support plane unless connection says otherwise",
             "facing": "+Z local outward; wrapper/house yaw supplies street orientation",
             "allowedHosts": hosts, "connections": connections, "repeatSnap": repeat,
             "materialFamily": MATERIAL_NOTE, "collisionTraversal": collision,
             "dimensionalExceptions": exceptions or [], "knownIncompatibilities": issues or []}
    if extra: piece.update(extra)
    return piece


wall = {"width": 2.0, "height": STOREY, "outerFaceFromModuleLine": WALL_OUT, "innerFaceFromModuleLine": WALL_IN,
        "depth": round(WALL_OUT + WALL_IN, 4)}
derived_pieces = [
    derived("art01.derived.render_host.straight.v1", "FACADE_HOST", wall,
            ["quaternius-medieval-source:Wall_UnevenBrick_Straight (faces)", "T_Plaster_BaseColor"],
            ["lower storey host", "adjacent 2 m host"], ["SUPPORTED_BY lower storey", "MEETS adjacent host flush",
            "outer/inner faces identical to the stone source wall so storeys are flush"],
            "full wall BoxCollider", assets=[MESHES + "art01.facade.render_host.straight.v1.asset"]),
    derived("art01.derived.render_host.window_wide_flat.v1", "FACADE_HOST",
            {**wall, "apertureWidth": 1.61, "apertureHeight": 1.59, "sillAboveStoreyDatum": 0.94},
            ["quaternius-medieval-source:Wall_UnevenBrick_Window_Wide_Flat", "Window_Wide_Flat1"],
            ["lower storey host", "matching wide flat window insert"],
            ["HOSTS true through-opening", "sill/jamb/lintel extruded through full wall depth"],
            "two jamb, sill and lintel BoxColliders; no full-window block",
            assets=[MESHES + "art01.facade.render_host.window_wide_flat.v1.asset"], exceptions=WINDOW_EXCEPTIONS),
    derived("art01.derived.facade.corner_closure.v1", "CORNER_CLOSURE",
            {"width": WALL_OUT, "height": STOREY, "depth": WALL_OUT},
            ["measured source wall outer-face offset"], ["two facade modules meeting at a corner"],
            ["fills the WALL_OUT x WALL_OUT notch left where two module lines meet; coplanar-adjacent to both outer faces"],
            "none", box=True, issues=["never enlarged to stand in for a wall; stone storeys also carry a Corner_Exterior_Brick quoin"]),
    derived("art01.derived.roof.lowpitch.6x10.v1", "ROOF",
            {"sourceBounds": [8.25, 5.67, 11.85], "appliedScale": [0.86, 0.45, 0.94], "span": 6.0, "length": 10.0},
            ["quaternius-medieval-source:Roof_RoundTiles_6x10"], ["6x10 m two-storey wall assembly"],
            ["measured underside placed on the outer wall-top line (eave)", "gables fitted to the measured underside"],
            "none", assets=[EXTERNAL + "Medieval/Models/Roof_RoundTiles_6x10.fbx"],
            issues=["source chalet pitch vetoed; only named flattened assembly is admitted"],
            extra={"roofPitchMinDeg": 28.0, "roofPitchMaxDeg": 36.0}),
    derived("art01.derived.roof.lowpitch.8x14.v1", "ROOF",
            {"sourceBounds": [9.95, 6.78, 15.71], "appliedScale": [0.91, 0.45, 0.96], "span": 8.0, "length": 14.0},
            ["quaternius-medieval-source:Roof_RoundTiles_8x14"], ["8x14 m two-storey wall assembly"],
            ["measured underside placed on the outer wall-top line (eave)", "gables fitted to the measured underside"],
            "none", assets=[EXTERNAL + "Medieval/Models/Roof_RoundTiles_8x14.fbx"],
            issues=["source chalet pitch vetoed; do not generalize scaling to other roof models"],
            extra={"roofPitchMinDeg": 28.0, "roofPitchMaxDeg": 36.0}),
    derived("art01.derived.gable.stone.v1", "GABLE",
            {"halfWidth": "house half width + 0.0924", "rise": "fitted per roof family from measured underside + 0.03",
             "outerFace": WALL_OUT, "innerFace": WALL_IN},
            ["quaternius-medieval-source:roof family"], ["matching 6x10 or 8x14 roof"],
            ["MEETS top-storey wall face flush", "top edge at/above measured roof underside, below roof top surface"],
            "none", assets=[MESHES + "art01.gable.6.v1.asset", MESHES + "art01.gable.8.v1.asset"],
            issues=["mismatched roof width/pitch creates a visible slit"]),
    derived("art01.derived.eave.timber_plate.v1", "EAVE",
            {"width": 0.14, "height": 0.12, "length": "house depth + 1.08"},
            ["quaternius-medieval-source:T_WoodTrim_BaseColor"], ["eave-side outer wall face"],
            ["ATTACHED_TO outer wall face", "top MEETS measured roof underside at its outer edge"], "none", box=True),
    derived("art01.derived.plinth.segment.v1", "PLINTH",
            {"bottom": -0.15, "top": "storey datum (0.18 house / 0.30 public room)", "projectionBeyondWallFace": 0.10,
             "innerFace": "wall inner face"},
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["site datum -0.05 (embedded 0.10)"],
            ["continuous ring SUPPORTS every ground-storey host", "private door sill = plinth top (single 0.17 m rise)",
             "interrupted only by a public threshold of equal width"],
            "non-traversable visual base", box=True, issues=["never interrupted without a threshold/landing filling the gap"]),
    derived("art01.derived.threshold.public.v1", "THRESHOLD",
            {"width": 1.70, "landingDepthFromWallFace": 0.96, "stepTread": 0.30, "stepTop": 0.1775, "landingTop": 0.30},
            ["quaternius-medieval-source:T_Brick_BaseColor"], ["door frame", "street", "public floor"],
            ["street -> step (0.12-0.155 m) -> landing (0.1225 m) -> public floor level"],
            "step/landing single local support owner", box=True,
            issues=["CITY-07 must still validate final access role/elevation"]),
    derived("art01.derived.threshold.lean_to.v1", "PORCH_ROOF",
            {"width": 3.60, "projection": 1.65, "undersideAtWall": 3.03, "undersideAtEdge": 2.75, "thickness": 0.07},
            ["quaternius-medieval-source:T_RoundTiles_BaseColor"], ["F01 facade", "two porch posts"],
            ["back edge embedded 0.02 m in facade under first floor", "SUPPORTED_BY porch posts"],
            "none", assets=[MESHES + "art01.roof.threshold.lean_to.v1.asset"], issues=["not an independent floating roof"]),
    derived("art01.derived.threshold.porch_frame.v1", "PORCH_POST",
            {"postSection": 0.17, "footSection": 0.32, "footBottom": -0.15, "footTop": 0.25,
             "postSetbackFromWallFace": 1.40, "postHalfSpan": 1.65, "fasciaHeight": 0.21},
            ["quaternius-medieval-source:T_WoodTrim_BaseColor"], ["site datum", "lean-to canopy"],
            ["foot embedded in site", "post top MEETS canopy underside", "fascia CAPS canopy edge"],
            "post/foot BoxColliders outside the public step width", box=True),
    derived("art01.derived.interior.public_room_shell.v1", "INTERIOR_FLOOR",
            {"floorThickness": 0.13, "ceilingThickness": 0.20, "clearHeight": round(STOREY - 0.2, 4)},
            ["quaternius-medieval-source:T_WoodTrim_BaseColor"], ["F01 walls"],
            ["floor continues landing level at the door line", "ceiling CAPS public room"],
            "floor BoxCollider only", box=True),
    derived("art01.derived.interior.counter.v1", "INTERIOR_FURNITURE",
            {"height": 1.02, "depth": 0.92, "length": 3.8},
            ["quaternius-props-standard:T_Trim_Furniture_BaseColor"], ["public room floor"],
            ["carcass SUPPORTED_BY floor", "top SUPPORTED_BY carcass"], "none", box=True),
    derived("art01.derived.street.crowned.v1", "STREET",
            {"widthClasses": {"X1": 5.5, "W12": 2.8, "casco_reveal_specimen": 6.0, "micro_route_B": 2.4},
             "crownRise": 0.045, "edgeHeight": 0.01, "channel": {"centreFromRightEdge": 0.20, "width": 0.20, "depth": 0.03}},
            ["quaternius-medieval-source:T_RoundRocks_BaseColor"], ["site datum", "X1 arch span"],
            ["one continuous crowned mesh and MeshCollider", "integrated right-side drainage channel submesh",
             "MEETS kerbs, parapets and the F01 plinth face"],
            "one continuous MeshCollider traversable owner",
            assets=[MESHES + "art01.street.crowned.s02_w12_casco.v1.asset"],
            issues=["ART specimen stations are not CITY-07 coordinates or elevation decisions",
                    "6.0 m Casco pocket is an ART composition specimen, not an accepted CITY widening"]),
    derived("art01.derived.street.edge_drain.v1", "EDGE",
            {"width": 0.20, "top": 0.13, "bottom": -0.15, "riseAboveRoadEdge": 0.12},
            ["quaternius-medieval-source:T_Brick_BaseColor"], ["street edge outside the traversable surface"],
            ["continuous mitred kerb MEETS road edge", "ends MEET parapet ends and the F01 plinth",
             "drainage is the channel modelled inside street.crowned.v1"],
            "none on path", assets=[MESHES + "art01.street.kerb.-1.v1.asset", MESHES + "art01.street.kerb.1.v1.asset"]),
    derived("art01.derived.retaining.capped.v1", "RETAINING",
            {"bodyWidth": 0.42, "bodyTop": 0.48, "bottom": -0.15, "capWidth": 0.53, "capHeight": 0.09},
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["site datum", "kerb outer edge"],
            ["SUPPORTED_BY site (embedded)", "cap CAPS wall", "MEETS house wall / frontage plinth line"],
            "static wall collider outside path", box=True),
    derived("art01.derived.bridge.arch_span.v1", "BRIDGE_SPAN",
            {"halfWidth": 3.30, "deckTop": 0.0, "archSpan": 7.9, "archRise": 1.5, "crownUnderside": -0.70, "waterLevel": -2.2},
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["river embankment abutment"],
            ["SUPPORTS X1 road and parapets", "spandrel faces flush with parapet outer faces", "SPRINGS_FROM embankment"],
            "deck MeshCollider below the street surface", assets=[MESHES + "art01.bridge.arch_span.x1.v1.asset"],
            issues=["isolated bridgehead sample; the south end is a declared specimen cut, not CITY-07 bridge geometry"]),
    derived("art01.derived.bridge.parapet_capped.v1", "PARAPET",
            {"width": 0.55, "top": 1.08, "bottom": -0.10, "capWidth": 0.68, "capHeight": 0.12},
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["arch span deck", "bridgehead ground"],
            ["inner face MEETS road edge", "end MEETS S02 kerb start"], "static parapet collider", box=True),
    derived("art01.derived.retaining.embankment.v1", "RETAINING",
            {"depth": 0.60, "bottom": -2.4, "top": -0.05, "capDepth": 0.70, "capTop": 0.05},
            ["quaternius-medieval-source:T_UnevenBrick_BaseColor"], ["river bed", "S02 bank datum"],
            ["RETAINS bank above river", "MEETS arch span abutment"], "static wall collider", box=True),
    derived("art01.derived.site.scenic_bank.v1", "SCENIC",
            {"datumBesideFeatures": -0.05, "flatApron": 1.5, "blendDistance": 5.0, "riverEdgeZ": -26.5},
            ["quaternius-nature-standard:wet valley palette"], ["outside traversable road"],
            ["flat local datum beside route, buildings and edges; valley relief beyond", "ROOTS vegetation at measured height"],
            "none; road owns traversal",
            assets=[MESHES + "art01.site.scenic_bank.-1.v1.asset", MESHES + "art01.site.scenic_bank.1.v1.asset"],
            issues=["not CITY heightfield/elevation authority"]),
    derived("art01.derived.site.river_water.v1", "SCENIC_WATER", {"level": -2.2, "thickness": 0.02},
            ["quaternius-nature-standard:wet valley palette"], ["river bed below X1 arch"],
            ["visible below arch and embankment"], "none", box=True),
    derived("art01.derived.sign.bar.v1", "SIGN",
            {"width": 1.40, "height": 0.42, "depth": 0.08, "letterHeightApprox": 0.27},
            [], ["plain F01 facade host"], ["back flush on host face", "one readable face toward the public approach"],
            "none", box=True, issues=["word BAR is a specimen label, not a canonized venue name"]),
    derived("art01.derived.clothed_scale.forastero.v1", "SCALE_REFERENCE", {"standingHeight": 1.8},
            ["quaternius-base-characters-source:Regular_Male_FullBody", "Hair_Buzzed",
             "UAL1:Idle_Loop", "diagnostic #233 garment-carving technique"], ["road", "public floor"],
            ["SUPPORTED_BY route surface", "FACES travel direction after documented 180 deg visual adjustment"],
            "inspection CharacterController only; no NPC/gameplay",
            assets=["Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx"],
            issues=["raw T-pose/mannequin is PROXY_VISUAL"]),
]

assemblies = [
    {"id": "art01.assembly.benchmark.retained_chain.v1", "kind": "assembly", "readiness": "KEEPER_READY",
     "role": "BENCHMARK", "layers": ["X1 bridgehead", "S02 pause", "W12 lane", "Casco reveal", "micro-route B", "F01 threshold"],
     "routeWidthSource": "Unity/ArkusUnity/Assets/Arkus/CITY/City04Layout.json",
     "routeSegments": [
         {"route": "X1", "z0": -35.5, "z1": -26.5},
         {"route": "W12", "z0": -22.5, "z1": 6.5},
         {"route": "casco.micro.B", "z0": 22.5, "z1": 24.5},
     ],
     "specimenSegments": [
         {"id": "casco.reveal_pocket", "z0": 13.0, "z1": 18.0, "width": 6.0,
          "note": "ART composition specimen, not an accepted CITY widening; not checked against CITY"},
     ],
     "note": "local specimen order only; not CITY coordinates, grades or keeper placement"},
    {"id": "art01.assembly.house.two_storey_6x10.v1", "kind": "assembly", "readiness": "KEEPER_READY",
     "role": "BUILDING_ASSEMBLY",
     "layers": ["site datum -0.05", "plinth ring -0.15..0.18", "stone ground storey (source walls + quoins)",
                "render upper storey (render hosts)", "corner closures", "hosted openings/inserts",
                "low-pitch roof on measured eave", "fitted stone gables", "eave timber plates"],
     "members": ["art01.quaternius.medieval.wall_unevenbrick_*", "art01.derived.render_host.*", "art01.derived.plinth.segment.v1",
                 "art01.derived.facade.corner_closure.v1", "art01.derived.roof.lowpitch.6x10.v1", "art01.derived.gable.stone.v1",
                 "art01.derived.eave.timber_plate.v1"],
     "placementRule": "plinth outer face meets the kerb outer edge; accepted route width is never narrowed"},
    {"id": "art01.assembly.house.bar_8x14.v1", "kind": "assembly", "readiness": "KEEPER_READY",
     "role": "BUILDING_ASSEMBLY",
     "layers": ["site datum -0.05", "plinth ring -0.15..0.30 interrupted by public threshold", "stone ground storey",
                "render upper storey", "corner closures", "hosted openings/inserts", "public threshold + lean-to porch",
                "public room shell", "low-pitch roof on measured eave", "fitted stone gables", "eave timber plates",
                "flush wall sign and wall lantern"],
     "members": ["as two_storey_6x10 plus art01.derived.threshold.*", "art01.derived.interior.*", "art01.derived.sign.bar.v1"],
     "placementRule": "public door module centred on micro-route B; lane terminates on the plinth face"},
    {"id": "art01.inspection.walk_rig.v1", "kind": "assembly", "readiness": "INSPECTION_ONLY",
     "role": "INSPECTION_ONLY", "note": "third-person inspection instrument; no game identity"},
]

pieces = [source_piece(row) for row in AUDIT["rows"] if row["path"].endswith(".fbx")]
pieces += derived_pieces
human_fbx = ROOT / "Unity/ArkusUnity/Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx"
if not human_fbx.is_file():
    raise FileNotFoundError("ART01_DERIVED_HUMAN_MISSING")
for piece in pieces:
    if piece["id"] == "art01.derived.clothed_scale.forastero.v1":
        piece["provenance"]["artifactSha256"] = hashlib.sha256(human_fbx.read_bytes()).hexdigest()
ids = [piece["id"] for piece in pieces] + [a["id"] for a in assemblies]
if len(ids) != len(set(ids)):
    raise ValueError("duplicate ART logical ID")
manifest = {
    "schema": "juego2.art01.kit-composition@2", "wp": "WP-ART-01",
    "authority": "ART-owned composition companion; does not alter H0/H1/CITY contracts",
    "unityVersion": AUDIT["unityVersion"],
    "sourceLockSha256": hashlib.sha256((EVIDENCE / "SOURCE_LOCK.json").read_bytes()).hexdigest(),
    "metricPolicy": "Docs/workpacks/ART/ART_01_DIMENSIONAL_PROFILE.md",
    "renderPipelineStatus": "STRUCTURAL checkpoint: geometry/metadata are renderer-independent; materials are provisional "
                            "built-in Standard until the post-H1-GATE URP adoption",
    "presentationStates": {
        "KEEPER_READY": "admitted composition with required host/support/metric truth",
        "PROXY_VISUAL": "deliberate visual placeholder; cannot satisfy required benchmark",
        "COVERAGE_BLOCKED": "required composition cannot be produced from admitted kit",
    },
    "pieces": pieces,
    "assemblies": assemblies,
}
out = EVIDENCE / "KIT_COMPOSITION_MANIFEST.json"
out.write_text(json.dumps(manifest, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
print(f"ART01_MANIFEST_WRITTEN pieces={len(pieces)} assemblies={len(assemblies)} "
      f"keeper={sum(p['readiness'] == 'KEEPER_READY' for p in pieces)}")
