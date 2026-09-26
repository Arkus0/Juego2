#!/usr/bin/env python3
"""WP-H1-11 representative real-source slice: derivation, verification and mount.

The representative slice only *selects* further items from the two Quaternius
distributions already adopted by WP-H1-04 and already stored in the pinned
private vault. It never adopts a new source, version or licence.

Authorities and their roles:

* ``Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json`` stays the admission record.
  Every representative item must appear there as an ``approved-source`` slice
  with the same asset path, source ID and content hash.
* ``Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json`` is the H1-11 manifest.
  It binds every selected item to its exact distribution entry, original
  Unity ``.meta`` sidecar (bytes + GUID), logical catalogue IDs and the import
  facts that are derived from the source bytes themselves (FBX axis/unit
  settings, node hierarchy, geometry bounds and material slots).

The derived facts are an independent oracle for Unity's effective import: they
are recomputed here from the source bytes and never read back from Unity.

Commands:

``derive``  recompute the manifest's source-derived fields from the vault and
            print or write the manifest (used to author it; CI uses ``check``).
``check``   recompute every source-derived field from the vault and require
            byte-for-byte equality with the committed manifest.
``static``  public-only cross-check of manifest, adoption record and mapping
            (no private bytes required; runs anywhere).
``mount``   verify and mount the representative distribution entries into the
            ignored Unity ``SourceSlice`` (fail closed, never overwrites).
``sources`` require the mounted SourceSlice to hold exactly the admitted files
            with their admitted bytes (before and after the Unity flow).
``self-test`` synthetic negative controls for the parser and the barriers.
"""

from __future__ import annotations

import argparse
import copy
import hashlib
import io
import json
import re
import struct
import sys
import tempfile
import zipfile
import zlib
from pathlib import Path, PurePosixPath

MANIFEST_SCHEMA = "arkus.h1-11-representative-slice@1"
MANIFEST_RELATIVE = Path("Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json")
ADOPTION_RELATIVE = Path("Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json")
MAPPING_RELATIVE = Path("Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json")
VAULT_MANIFEST_RELATIVE = Path("h1/H1_SOURCE_SLICE_MANIFEST.json")
SOURCE_SLICE_RELATIVE = Path("Unity/ArkusUnity/Assets/Arkus/H1/SourceSlice")
SOURCE_SLICE_ASSET_ROOT = "Assets/Arkus/H1/SourceSlice/"
GUID_RE = re.compile(r"(?m)^guid:\s*([0-9a-f]{32})\s*$")
SHA256_RE = re.compile(r"^[0-9a-f]{64}$")
PROVENANCES = {"distribution-entry", "h1-04-source-slice"}
# The four WP-H1-04 slices are admitted by H1-04 itself; H1-11 only reuses them.
H104_SLICE_PATHS = {
    "Assets/Arkus/H1/SourceSlice/Wall_Plaster_Window_Wide_Flat.fbx",
    "Assets/Arkus/H1/SourceSlice/MI_Plaster.mat",
    "Assets/Arkus/H1/SourceSlice/FacadeImportedMaterial.mat",
    "Assets/Arkus/H1/SourceSlice/UAL1.fbx",
}


class SliceError(Exception):
    pass


def die(message: str) -> "NoReturn":
    raise SliceError(message)


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    try:
        with path.open("rb") as stream:
            for chunk in iter(lambda: stream.read(1024 * 1024), b""):
                digest.update(chunk)
    except FileNotFoundError:
        die(f"missing required file: {path}")
    return digest.hexdigest()


def load_json(path: Path) -> dict:
    try:
        return json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError:
        die(f"missing required JSON: {path}")
    except json.JSONDecodeError as exc:
        die(f"invalid JSON {path}: {exc}")


def safe_relative(value: str) -> Path:
    posix = PurePosixPath(value)
    if not value or posix.is_absolute() or ".." in posix.parts:
        die(f"unsafe relative path: {value!r}")
    return Path(*posix.parts)


def meta_guid(text: str, label: str) -> str:
    match = GUID_RE.search(text)
    if not match:
        die(f"Unity meta sidecar has no canonical guid line: {label}")
    return match.group(1)


# --------------------------------------------------------------------------- FBX


def _fbx_nodes(data: bytes):
    if data[:21] != b"Kaydara FBX Binary  \x00":
        die("source is not a binary FBX document")
    version = struct.unpack_from("<I", data, 23)[0]
    wide = version >= 7500
    header = 25 if wide else 13

    def read(offset: int):
        if wide:
            end, count, _ = struct.unpack_from("<QQQ", data, offset)
        else:
            end, count, _ = struct.unpack_from("<III", data, offset)
        name_length = data[offset + header - 1]
        if end == 0:
            return None, offset + header
        cursor = offset + header
        name = data[cursor:cursor + name_length].decode("ascii")
        cursor += name_length
        properties = []
        for _ in range(count):
            kind = chr(data[cursor])
            cursor += 1
            if kind == "Y":
                value = struct.unpack_from("<h", data, cursor)[0]; cursor += 2
            elif kind == "C":
                value = data[cursor]; cursor += 1
            elif kind == "I":
                value = struct.unpack_from("<i", data, cursor)[0]; cursor += 4
            elif kind == "F":
                value = struct.unpack_from("<f", data, cursor)[0]; cursor += 4
            elif kind == "D":
                value = struct.unpack_from("<d", data, cursor)[0]; cursor += 8
            elif kind == "L":
                value = struct.unpack_from("<q", data, cursor)[0]; cursor += 8
            elif kind in "fdlib":
                length, encoding, compressed = struct.unpack_from("<III", data, cursor)
                cursor += 12
                raw = data[cursor:cursor + compressed]
                cursor += compressed
                if encoding == 1:
                    raw = zlib.decompress(raw)
                elif encoding != 0:
                    die("unsupported FBX array encoding")
                code = {"f": "f", "d": "d", "l": "q", "i": "i", "b": "b"}[kind]
                value = struct.unpack("<%d%s" % (length, code), raw)
            elif kind in "SR":
                length = struct.unpack_from("<I", data, cursor)[0]; cursor += 4
                value = data[cursor:cursor + length]; cursor += length
                if kind == "S":
                    value = value.decode("utf-8", "replace")
            else:
                die(f"unsupported FBX property type {kind!r}")
            properties.append(value)
        children = []
        while cursor < end:
            child, cursor = read(cursor)
            if child is None:
                break
            children.append(child)
        return (name, properties, children), end

    nodes = []
    cursor = 27
    while cursor < len(data):
        node, cursor = read(cursor)
        if node is None:
            break
        nodes.append(node)
    return version, nodes


def _child(node, name):
    for child in node[2]:
        if child[0] == name:
            return child
    return None


def _object_name(raw: str) -> str:
    return raw.split("\x00", 1)[0]


def _round(value: float) -> float:
    return round(float(value), 4)


def describe_fbx(data: bytes) -> dict:
    """Source-derived import facts. Only exact binary structure is read."""
    version, top = _fbx_nodes(data)
    by_name = {node[0]: node for node in top}
    for required in ("GlobalSettings", "Objects", "Connections"):
        if required not in by_name:
            die(f"FBX document has no {required} section")
    settings = {}
    properties = _child(by_name["GlobalSettings"], "Properties70")
    for row in properties[2] if properties else []:
        settings[row[1][0]] = row[1][-1]
    for required in ("UnitScaleFactor", "UpAxis"):
        if required not in settings:
            die(f"FBX GlobalSettings has no {required}")

    models, geometries, materials = {}, {}, {}
    for obj in by_name["Objects"][2]:
        identifier, name = obj[1][0], _object_name(obj[1][1])
        if obj[0] == "Model":
            models[identifier] = (name, obj[1][2])
        elif obj[0] == "Geometry" and obj[1][2] == "Mesh":
            vertices = _child(obj, "Vertices")
            points = vertices[1][0] if vertices else ()
            if len(points) < 3 or len(points) % 3:
                die(f"FBX geometry {name!r} has no complete vertex list")
            xs, ys, zs = points[0::3], points[1::3], points[2::3]
            geometries[identifier] = {
                "min": [_round(min(xs)), _round(min(ys)), _round(min(zs))],
                "max": [_round(max(xs)), _round(max(ys)), _round(max(zs))],
            }
        elif obj[0] == "Material":
            materials[identifier] = name

    parent, geometry_of, materials_of = {}, {}, {}
    for connection in by_name["Connections"][2]:
        if connection[1][0] != "OO":
            continue
        child, owner = connection[1][1], connection[1][2]
        if child in models and (owner in models or owner == 0):
            parent[child] = owner
        elif child in geometries and owner in models:
            if owner in geometry_of:
                die("FBX model has more than one geometry")
            geometry_of[owner] = child
        elif child in materials and owner in models:
            materials_of.setdefault(owner, []).append(materials[child])

    def path(identifier):
        names = []
        while identifier:
            names.append(models[identifier][0])
            identifier = parent.get(identifier, 0)
        return "/".join(reversed(names))

    nodes = sorted(({"path": path(i), "type": t} for i, (_, t) in models.items()), key=lambda n: n["path"])
    if len({node["path"] for node in nodes}) != len(nodes):
        die("FBX node paths are ambiguous")
    meshes = []
    for identifier in sorted(geometry_of, key=path):
        meshes.append({
            "node": path(identifier),
            "boundsSource": geometries[geometry_of[identifier]],
            "materials": materials_of.get(identifier, []),
        })
    return {
        "version": version,
        "unitScaleFactor": _round(settings["UnitScaleFactor"]),
        "upAxis": int(settings["UpAxis"]),
        "nodes": nodes,
        "meshes": meshes,
    }


# ------------------------------------------------------------------ distribution


class Vault:
    """Read-only view of the verified private vault payloads."""

    def __init__(self, vault_root: Path):
        self.root = vault_root.resolve()
        manifest = load_json(self.root / VAULT_MANIFEST_RELATIVE)
        self.distributions = {item["sourceId"]: item for item in manifest.get("sourceDistributions", [])}
        self.slice_files = {PurePosixPath(item["path"]).name: item for item in manifest.get("files", [])}
        self._zip = None

    def payload_path(self, source_id: str) -> Path:
        if source_id not in self.distributions:
            die(f"vault has no distribution for {source_id}")
        return self.root / safe_relative(self.distributions[source_id]["vaultPath"])

    def require_distribution(self, source_id: str, expected_sha256: str) -> Path:
        payload = self.payload_path(source_id)
        observed = sha256_file(payload)
        if observed != expected_sha256:
            die(f"distribution hash mismatch for {source_id}: expected={expected_sha256} observed={observed}")
        return payload

    def archive(self, source_id: str, expected_sha256: str) -> zipfile.ZipFile:
        if self._zip is None:
            payload = self.require_distribution(source_id, expected_sha256)
            try:
                self._zip = zipfile.ZipFile(payload)
            except zipfile.BadZipFile:
                die(f"distribution {source_id} is not the adopted archive")
        return self._zip

    def entry(self, source_id: str, expected_sha256: str, name: str) -> bytes:
        archive = self.archive(source_id, expected_sha256)
        try:
            return archive.read(name)
        except KeyError:
            die(f"distribution {source_id} has no entry {name!r}")

    def slice_bytes(self, file_name: str) -> tuple[bytes, bytes]:
        item = self.slice_files.get(file_name)
        if item is None:
            die(f"vault SourceSlice has no {file_name}")
        return ((self.root / safe_relative(item["path"])).read_bytes(),
                (self.root / safe_relative(item["metaPath"])).read_bytes())


def adoption_sources(public_root: Path) -> dict:
    adoption = load_json(public_root / ADOPTION_RELATIVE)
    return {item["sourceId"]: item for item in adoption.get("sources", [])}


def item_bytes(item: dict, vault: Vault, sources: dict) -> tuple[bytes, bytes]:
    provenance = item.get("provenance")
    if provenance == "distribution-entry":
        source = sources.get(item["sourceId"])
        if source is None:
            die(f"{item['assetPath']} names an unadopted source {item['sourceId']}")
        entry = item["distributionEntry"]
        return (vault.entry(item["sourceId"], source["distributionSha256"], entry),
                vault.entry(item["sourceId"], source["distributionSha256"], entry + ".meta"))
    if provenance == "h1-04-source-slice":
        return vault.slice_bytes(PurePosixPath(item["assetPath"]).name)
    die(f"unknown provenance {provenance!r} for {item.get('assetPath')}")


def derived_fields(item: dict, content: bytes, meta: bytes) -> dict:
    fields = {
        "contentSha256": sha256_bytes(content),
        "metaSha256": sha256_bytes(meta),
        "unityGuid": meta_guid(meta.decode("utf-8"), item["assetPath"] + ".meta"),
    }
    if item["assetPath"].endswith(".fbx"):
        fields["fbx"] = describe_fbx(content)
    return fields


def derive(manifest: dict, vault: Vault, sources: dict) -> dict:
    result = copy.deepcopy(manifest)
    for item in result["items"]:
        content, meta = item_bytes(item, vault, sources)
        item.update(derived_fields(item, content, meta))
    return result


def render(manifest: dict) -> str:
    return json.dumps(manifest, indent=2, ensure_ascii=False) + "\n"


# ---------------------------------------------------------------- static checks


def static_check(public_root: Path, manifest: dict | None = None, catalogue_binding: bool = True) -> dict:
    """Admission checks always; catalogue binding checks unless mounting (the mapping is
    derived from Unity's effective import of the mounted bytes, so it cannot gate the mount)."""
    manifest = manifest if manifest is not None else load_json(public_root / MANIFEST_RELATIVE)
    adoption = load_json(public_root / ADOPTION_RELATIVE)
    mapping = load_json(public_root / MAPPING_RELATIVE)
    if manifest.get("schemaId") != MANIFEST_SCHEMA:
        die("unexpected representative-slice manifest schema")
    items = manifest.get("items")
    if not isinstance(items, list) or not 10 <= len(items) <= 15:
        die("representative slice must select 10-15 source pieces")

    sources = {item["sourceId"]: item for item in adoption.get("sources", [])}
    slices = {}
    for row in adoption.get("slices", []):
        if row["assetPath"] in slices:
            die(f"duplicate adoption slice {row['assetPath']}")
        slices[row["assetPath"]] = row
    catalogue = {}
    for row in mapping.get("entries", []):
        catalogue.setdefault(row["logicalId"], row)

    seen_paths, seen_ids = set(), set()
    for item in items:
        path = item.get("assetPath", "")
        if not path.startswith(SOURCE_SLICE_ASSET_ROOT) or "/" in path[len(SOURCE_SLICE_ASSET_ROOT):]:
            die(f"representative item escapes the reviewed SourceSlice root: {path!r}")
        if path in seen_paths:
            die(f"representative item selected twice: {path}")
        seen_paths.add(path)
        if item.get("provenance") not in PROVENANCES:
            die(f"representative item has no admitted provenance: {path}")
        if item["sourceId"] not in sources:
            die(f"representative item names an unadopted source: {path}")
        if (item["provenance"] == "h1-04-source-slice") != (path in H104_SLICE_PATHS):
            die(f"representative item provenance disagrees with the H1-04 slice set: {path}")
        if item["provenance"] == "distribution-entry" and not item.get("distributionEntry"):
            die(f"representative item has no distribution entry: {path}")
        for key in ("contentSha256", "metaSha256"):
            if not SHA256_RE.fullmatch(str(item.get(key, ""))):
                die(f"representative item {path} has no {key}")
        admitted = slices.get(path)
        if admitted is None:
            die(f"representative item is not in the H1-04 adoption record: {path}")
        if (admitted["sourceId"], admitted["adoptionStatus"], admitted["contentSha256"]) != (
                item["sourceId"], "approved-source", item["contentSha256"]):
            die(f"representative item disagrees with its adoption slice: {path}")
        for kind, logical_id in sorted(item.get("catalogue", {}).items()):
            if "/" in logical_id or logical_id.startswith("Assets"):
                die(f"catalogue identity is an asset path, not a logical ID: {logical_id}")
            if logical_id in seen_ids:
                die(f"logical ID reused by two representative items: {logical_id}")
            seen_ids.add(logical_id)
            if not catalogue_binding:
                continue
            row = catalogue.get(logical_id)
            if row is None:
                die(f"representative catalogue identity is unmapped: {logical_id}")
            if row["kind"] != kind or row["sourceId"] != item["sourceId"] or row["contentSha256"] != item["contentSha256"]:
                die(f"representative catalogue identity is bound to another item: {logical_id}")
        if not item.get("catalogue"):
            die(f"representative item has no catalogue identity: {path}")

    # Completeness in the other direction: every admitted slice that is not an
    # original H1-04 slice must be a representative item (no silent widening).
    extension = {path for path in slices if path not in H104_SLICE_PATHS}
    selected = {item["assetPath"] for item in items if item["provenance"] == "distribution-entry"}
    if extension != selected:
        die("adoption record and representative manifest disagree: "
            f"unlisted={sorted(extension - selected)} unadmitted={sorted(selected - extension)}")

    clips = manifest.get("clips", [])
    roles = sorted(clip.get("role") for clip in clips)
    if roles != ["idle", "sit", "walk"]:
        die(f"representative humanoid needs exactly idle/walk/sit clip roles, found {roles}")
    for clip in clips if catalogue_binding else ():
        row = catalogue.get(clip.get("logicalId", ""))
        if row is None or row["kind"] != "animation-clip" or row["sourceId"] != "quaternius-ual1-source":
            die(f"representative clip is not a catalogued UAL1 clip: {clip}")
    return manifest


# ------------------------------------------------------------------------ mount


def mount(public_root: Path, vault: Vault, manifest: dict | None = None, write: bool = True) -> list[dict]:
    """Verify every representative distribution entry; write it only when ``write``."""
    manifest = static_check(public_root, manifest, catalogue_binding=False)
    sources = adoption_sources(public_root)
    target = public_root / SOURCE_SLICE_RELATIVE
    if write and not target.is_dir():
        die("the accepted H1-04 SourceSlice must be mounted before the representative extension")
    mounted = []
    staged = []
    for item in manifest["items"]:
        if item["provenance"] != "distribution-entry":
            continue
        content, meta = item_bytes(item, vault, sources)
        observed = derived_fields(item, content, meta)
        for key in ("contentSha256", "metaSha256", "unityGuid"):
            if observed[key] != item[key]:
                die(f"{key} mismatch for {item['assetPath']}: expected={item[key]} observed={observed[key]}")
        name = PurePosixPath(item["assetPath"]).name
        for path in (target / name, target / (name + ".meta")):
            if write and path.exists():
                die(f"refusing to mount over existing SourceSlice file: {path.name}")
        staged.append((name, content, meta))
        mounted.append({"assetPath": item["assetPath"], "sha256": observed["contentSha256"], "unityGuid": observed["unityGuid"]})
    for name, content, meta in staged if write else ():
        (target / name).write_bytes(content)
        (target / (name + ".meta")).write_bytes(meta)
    return mounted


def verify_for_vault(public_root: Path, vault_root: Path, write: bool) -> list[dict]:
    """Entry point used by the accepted H1 asset-vault verifier.

    Every admitted slice beyond the four H1-04 slices must be carried by the
    representative manifest; a missing manifest can therefore never hide an
    admitted-but-unverified item.
    """
    manifest_path = public_root / MANIFEST_RELATIVE
    adoption = load_json(public_root / ADOPTION_RELATIVE)
    extension = [row["assetPath"] for row in adoption.get("slices", []) if row.get("assetPath") not in H104_SLICE_PATHS]
    if not manifest_path.exists():
        if extension:
            die(f"admitted slices have no representative manifest: {sorted(extension)}")
        return []
    return mount(public_root, Vault(vault_root), load_json(manifest_path), write=write)


def verify_mounted_sources(public_root: Path) -> int:
    """The mounted SourceSlice holds exactly the admitted slices with their admitted bytes.

    Run before and after the effective Unity flow: a bridge write to purchased upstream
    bytes, an unadmitted extra file or an omitted admitted file all fail here.
    """
    adoption = load_json(public_root / ADOPTION_RELATIVE)
    expected = {PurePosixPath(row["assetPath"]).name: row["contentSha256"] for row in adoption.get("slices", [])}
    target = public_root / SOURCE_SLICE_RELATIVE
    if not target.is_dir():
        die("SourceSlice is not mounted")
    observed = {path.name: sha256_file(path) for path in target.iterdir() if path.is_file() and path.suffix != ".meta"}
    if set(observed) != set(expected):
        die(f"mounted SourceSlice universe differs: extra={sorted(set(observed) - set(expected))} missing={sorted(set(expected) - set(observed))}")
    changed = sorted(name for name in expected if observed[name] != expected[name])
    if changed:
        die(f"mounted upstream source bytes differ from the admitted hashes: {changed}")
    return len(observed)


# -------------------------------------------------------------------- self-test


def _synthetic_fbx(nodes: list[tuple[int, str, str, int]], geometry: dict[int, list[float]], materials: dict[int, list[str]]) -> bytes:
    """Build a tiny FBX 7400 binary document for negative controls."""

    def prop(value):
        if isinstance(value, str):
            raw = value.encode("utf-8")
            return b"S" + struct.pack("<I", len(raw)) + raw
        if isinstance(value, float):
            return b"D" + struct.pack("<d", value)
        if isinstance(value, int):
            return b"L" + struct.pack("<q", value)
        if isinstance(value, tuple):
            raw = struct.pack("<%dd" % len(value), *value)
            return b"d" + struct.pack("<III", len(value), 0, len(raw)) + raw
        raise TypeError(value)

    def node(name, props, children, offset):
        body = b"".join(prop(value) for value in props)
        head_len = 13 + len(name)
        cursor = offset + head_len + len(body)
        encoded = []
        for child in children:
            data = child(cursor)
            encoded.append(data)
            cursor += len(data)
        if children:
            cursor += 13
        blob = struct.pack("<III", cursor, len(props), len(body)) + bytes([len(name)]) + name.encode() + body
        blob += b"".join(encoded)
        if children:
            blob += b"\x00" * 13
        return blob

    def leaf(name, props, children=()):
        return lambda offset: node(name, props, list(children), offset)

    material_ids = {}
    objects, connections = [], []
    for identifier, name, kind, owner in nodes:
        objects.append(leaf("Model", [identifier, name + "\x00\x01Model", kind]))
        connections.append(leaf("C", ["OO", identifier, owner]))
        if identifier in geometry:
            gid = identifier + 1000
            objects.append(leaf("Geometry", [gid, "g\x00\x01Geometry", "Mesh"], [leaf("Vertices", [tuple(geometry[identifier])])]))
            connections.append(leaf("C", ["OO", gid, identifier]))
        for material in materials.get(identifier, []):
            mid = material_ids.setdefault(material, 5000 + len(material_ids))
            connections.append(leaf("C", ["OO", mid, identifier]))
    for material, mid in material_ids.items():
        objects.append(leaf("Material", [mid, material + "\x00\x01Material", ""]))
    settings = leaf("GlobalSettings", [], [leaf("Properties70", [], [
        leaf("P", ["UpAxis", "int", "Integer", "", 1]),
        leaf("P", ["UnitScaleFactor", "double", "Number", "", 1.0]),
    ])])
    sections = [settings, leaf("Objects", [], objects), leaf("Connections", [], connections)]
    out = bytearray(b"Kaydara FBX Binary  \x00\x1a\x00" + struct.pack("<I", 7400))
    for section in sections:
        out += section(len(out))
    out += b"\x00" * 13
    return bytes(out)


def self_test() -> None:
    wall = _synthetic_fbx([(1, "Wall", "Mesh", 0)], {1: [-100.0, 0.0, -30.0, 100.0, 312.0, 9.0]}, {1: ["MI_WoodTrim", "MI_Plaster"]})
    described = describe_fbx(wall)
    assert described["unitScaleFactor"] == 1.0 and described["upAxis"] == 1, described
    assert described["nodes"] == [{"path": "Wall", "type": "Mesh"}], described
    assert described["meshes"][0]["boundsSource"] == {"min": [-100.0, 0.0, -30.0], "max": [100.0, 312.0, 9.0]}, described
    assert described["meshes"][0]["materials"] == ["MI_WoodTrim", "MI_Plaster"], described
    rig = _synthetic_fbx([(1, "Armature", "Null", 0), (2, "root", "LimbNode", 1), (3, "pelvis", "LimbNode", 2), (4, "Body", "Mesh", 0)],
                         {4: [0.0, 0.0, 0.0, 1.0, 1.0, 2.0]}, {})
    assert [n["path"] for n in describe_fbx(rig)["nodes"]] == ["Armature", "Armature/root", "Armature/root/pelvis", "Body"]
    for broken in (b"not an fbx", wall[:40]):
        try:
            describe_fbx(broken)
        except (SliceError, struct.error, IndexError):
            pass
        else:
            raise AssertionError("truncated/foreign FBX bytes must fail closed")

    with tempfile.TemporaryDirectory() as temp:
        root = Path(temp)
        public, vault_root = root / "public", root / "vault"
        entry = "Kit/Source Models/Wall.fbx"
        meta = b"fileFormatVersion: 2\nguid: 0123456789abcdef0123456789abcdef\n"
        payload = io.BytesIO()
        with zipfile.ZipFile(payload, "w") as archive:
            archive.writestr(entry, wall)
            archive.writestr(entry + ".meta", meta)
        (vault_root / "h1/distributions").mkdir(parents=True)
        (vault_root / "h1/distributions/medieval.payload").write_bytes(payload.getvalue())
        distribution = sha256_bytes(payload.getvalue())
        (vault_root / VAULT_MANIFEST_RELATIVE).write_text(json.dumps({"sourceDistributions": [
            {"sourceId": "src", "vaultPath": "h1/distributions/medieval.payload", "distributionSha256": distribution}], "files": []}))
        asset_path = SOURCE_SLICE_ASSET_ROOT + "Wall.fbx"
        items = []
        for index in range(11):
            items.append({"provenance": "distribution-entry", "sourceId": "src", "assetPath": asset_path if index == 0 else SOURCE_SLICE_ASSET_ROOT + f"Extra{index}.fbx",
                          "distributionEntry": entry, "catalogue": {"prefab": f"kit.prefab.item-{index}"}})
        base = {"schemaId": MANIFEST_SCHEMA, "items": items, "clips": [
            {"role": "idle", "logicalId": "clip.idle"}, {"role": "walk", "logicalId": "clip.walk"}, {"role": "sit", "logicalId": "clip.sit"}]}
        (public / ADOPTION_RELATIVE).parent.mkdir(parents=True)
        (public / MAPPING_RELATIVE).parent.mkdir(parents=True)
        (public / SOURCE_SLICE_RELATIVE).mkdir(parents=True)

        def publish(manifest, slices=None, mapping=None):
            derived = derive(manifest, Vault(vault_root), {"src": {"distributionSha256": distribution}})
            content_hash = derived["items"][0]["contentSha256"]
            (public / ADOPTION_RELATIVE).write_text(json.dumps({"sources": [{"sourceId": "src", "distributionSha256": distribution}], "slices": slices if slices is not None else [
                {"assetPath": item["assetPath"], "sourceId": "src", "adoptionStatus": "approved-source", "contentSha256": content_hash} for item in items]}))
            rows = mapping if mapping is not None else [
                {"logicalId": item["catalogue"]["prefab"], "kind": "prefab", "sourceId": "src", "contentSha256": content_hash} for item in items]
            rows += [{"logicalId": f"clip.{role}", "kind": "animation-clip", "sourceId": "quaternius-ual1-source", "contentSha256": "0" * 64} for role in ("idle", "walk", "sit")]
            (public / MAPPING_RELATIVE).write_text(json.dumps({"entries": rows}))
            return derived

        def expect_red(label, action):
            try:
                action()
            except SliceError:
                return
            raise AssertionError(f"negative control unexpectedly passed: {label}")

        good = publish(base)
        static_check(public, good)
        publish(base, slices=[])
        expect_red("selected item absent from the adoption record", lambda: static_check(public, good))
        publish(base)
        omitted = copy.deepcopy(good)
        omitted["items"].pop()
        expect_red("admitted item omitted from manifest", lambda: static_check(public, omitted))
        path_id = copy.deepcopy(good)
        path_id["items"][0]["catalogue"] = {"prefab": asset_path}
        expect_red("logical ID replaced by asset path", lambda: static_check(public, path_id))
        clips = copy.deepcopy(good)
        clips["clips"] = clips["clips"][:2]
        expect_red("missing clip role", lambda: static_check(public, clips))
        guid = copy.deepcopy(good)
        guid["items"][0]["unityGuid"] = "f" * 32
        expect_red("rebound Unity GUID", lambda: mount(public, Vault(vault_root), guid))
        content = copy.deepcopy(good)
        content["items"][0]["contentSha256"] = "e" * 64
        expect_red("content hash drift", lambda: mount(public, Vault(vault_root), content))
        substituted = copy.deepcopy(good)
        substituted["items"][0]["distributionEntry"] = "Kit/Source Models/Missing.fbx"
        expect_red("entry outside distribution", lambda: mount(public, Vault(vault_root), substituted))
        mounted = mount(public, Vault(vault_root), good)
        assert len(mounted) == 11 and (public / SOURCE_SLICE_RELATIVE / "Wall.fbx").read_bytes() == wall
        expect_red("mount over existing file", lambda: mount(public, Vault(vault_root), good))
        (vault_root / "h1/distributions/medieval.payload").write_bytes(payload.getvalue() + b"\0")
        expect_red("changed distribution byte", lambda: derive(good, Vault(vault_root), {"src": {"distributionSha256": distribution}}))
    print("H1_11_REPRESENTATIVE_SLICE_SELF_TEST_GREEN")


# ---------------------------------------------------------------------- command


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("command", choices=("derive", "check", "static", "mount", "sources", "self-test"))
    parser.add_argument("--public-root", type=Path, default=Path("."))
    parser.add_argument("--vault-root", type=Path)
    parser.add_argument("--output", type=Path)
    args = parser.parse_args()
    try:
        if args.command == "self-test":
            self_test()
            return 0
        public_root = args.public_root.resolve()
        if args.command == "sources":
            count = verify_mounted_sources(public_root)
            print(f"H1_11_MOUNTED_SOURCE_UNCHANGED_GREEN files={count}")
            return 0
        if args.command == "static":
            manifest = static_check(public_root)
            print(f"H1_11_REPRESENTATIVE_SLICE_STATIC_GREEN items={len(manifest['items'])}")
            return 0
        if args.vault_root is None:
            die("--vault-root is required")
        vault = Vault(args.vault_root)
        committed = load_json(public_root / MANIFEST_RELATIVE)
        if args.command == "derive":
            rendered = render(derive(committed, vault, adoption_sources(public_root)))
            if args.output:
                args.output.write_text(rendered, encoding="utf-8")
            else:
                sys.stdout.write(rendered)
            return 0
        if args.command == "check":
            static_check(public_root, committed, catalogue_binding=False)
            derived = derive(committed, vault, adoption_sources(public_root))
            if render(derived) != render(committed):
                die("committed representative manifest differs from facts recomputed from the adopted source bytes")
            print(f"H1_11_REPRESENTATIVE_SLICE_SOURCE_DERIVED_GREEN items={len(committed['items'])}")
            return 0
        mounted = mount(public_root, vault, committed)
        print(f"H1_11_REPRESENTATIVE_SLICE_MOUNTED items={len(mounted)}")
        return 0
    except SliceError as exc:
        print(f"H1_11_REPRESENTATIVE_SLICE_RED: {exc}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
