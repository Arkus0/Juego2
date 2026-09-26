"""Build one ART-01 clothed scale model from local CC0 Quaternius source.

Bounded adaptation of #233's diagnostic garment-carving technique. The outfit
is not an NPC system. Run with Blender 5.x:
  blender -b --factory-startup --python Tools/art01_make_scale_reference.py
Output is a versioned ART-owned derivative with source lineage in the kit ledger.
"""

from pathlib import Path

import bmesh
import bpy


ROOT = Path(__file__).resolve().parents[1]
VAULT = Path("C:/Juego2-Assets/Base Characters")
BODY = VAULT / "Base Characters/Exports/Unity/Regular_Male_FullBody.fbx"
HAIR = VAULT / "Hairstyles/Rigged to Head Bone/FBX (Unity)/Male/Hair_Buzzed.fbx"
OUT = ROOT / "Unity/ArkusUnity/Assets/Arkus/ART/Derived/Characters/Townsfolk_Forastero.fbx"

TORSO = {"spine_01", "spine_02", "spine_03", "clavicle_l", "clavicle_r"}
UPPER = {"upperarm_l", "upperarm_r"}
LOWER = {"lowerarm_l", "lowerarm_r"}
PELVIS = {"pelvis", "root"}
LEGS = {"thigh_l", "thigh_r", "calf_l", "calf_r"}
FEET = {"foot_l", "foot_r", "ball_l", "ball_r"}
GARMENTS = [
    ("shirt_long", "D8D2C4", 0.011, 3),
    ("jacket", "2E4A4E", 0.024, 6),
    ("trousers", "3A3530", 0.008, 2),
    ("shoes", "4A3222", 0.009, 1),
]


def material(name: str, rgb: str):
    mat = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    values = tuple(int(rgb[i:i + 2], 16) / 255 for i in (0, 2, 4))
    mat.diffuse_color = (*values, 1)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = tuple(v ** 2.2 for v in values) + (1,)
        bsdf.inputs["Roughness"].default_value = 0.8
    return mat


def selected_faces(body, kind, dominant, waist, ankle):
    chosen = set()
    world = body.matrix_world
    normal_matrix = world.to_3x3()
    for face in body.data.polygons:
        bones = [dominant[v] for v in face.vertices]
        z = (world @ face.center).z
        def all_in(group):
            return all(b in group for b in bones)
        torso = all_in(TORSO) or (all_in(TORSO | PELVIS) and z > waist - 0.04)
        if kind == "shirt_long":
            keep = torso or (all_in(TORSO | UPPER | LOWER) and not all_in(PELVIS))
        elif kind == "jacket":
            keep = all_in(TORSO | UPPER | LOWER) or (all_in(TORSO | PELVIS) and z > waist - 0.13)
        elif kind == "trousers":
            keep = (all_in(PELVIS | LEGS | {"spine_01"}) and z < waist + 0.04) and not (
                all_in(LEGS | FEET) and z < ankle)
        else:  # shoes
            keep = all_in(FEET) or (all_in(LEGS | FEET) and z < ankle)
        if keep:
            chosen.add(face.index)
    return chosen


def retain_and_push(obj, faces, offset, new_material, smooth_passes):
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bm.faces.ensure_lookup_table()
    bmesh.ops.delete(bm, geom=[f for f in bm.faces if f.index not in faces], context="FACES")
    bmesh.ops.delete(bm, geom=[v for v in bm.verts if not v.link_faces], context="VERTS")
    inside = [v for v in bm.verts if not v.is_boundary]
    for _ in range(smooth_passes):
        bmesh.ops.smooth_vert(bm, verts=inside, factor=0.5,
                             use_axis_x=True, use_axis_y=True, use_axis_z=True)
    bm.normal_update()
    for v in bm.verts:
        v.co += v.normal * offset
    bm.to_mesh(obj.data)
    bm.free()
    obj.data.materials.clear()
    obj.data.materials.append(new_material)
    for polygon in obj.data.polygons:
        polygon.material_index = 0


def remove_covered_skin(body, faces):
    bm = bmesh.new()
    bm.from_mesh(body.data)
    bm.faces.ensure_lookup_table()
    bmesh.ops.delete(bm, geom=[bm.faces[i] for i in faces], context="FACES")
    bmesh.ops.delete(bm, geom=[v for v in bm.verts if not v.link_faces], context="VERTS")
    bm.to_mesh(body.data)
    bm.free()


def main():
    if not BODY.is_file() or not HAIR.is_file():
        raise FileNotFoundError("ART-01 CC0 Base Characters source not found in local vault")
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=str(BODY))
    arm = next(o for o in bpy.context.scene.objects if o.type == "ARMATURE")
    body = max((o for o in bpy.context.scene.objects if o.type == "MESH"),
               key=lambda o: len(o.data.vertices))
    groups = {g.index: g.name for g in body.vertex_groups}
    dominant = []
    for vertex in body.data.vertices:
        best = max(vertex.groups, key=lambda g: g.weight, default=None)
        dominant.append(groups.get(best.group) if best else None)
    bone = lambda name: (arm.matrix_world @ arm.data.bones[name].head_local).z
    waist = bone("spine_01") + 0.02
    ankle = bone("foot_l") + 0.035
    covered = set()
    for kind, rgb, offset, smooth in GARMENTS:
        faces = selected_faces(body, kind, dominant, waist, ankle)
        if not faces:
            raise RuntimeError(f"ART-01 empty garment: {kind}")
        garment = body.copy()
        garment.data = body.data.copy()
        garment.name = f"Forastero_{kind}"
        bpy.context.scene.collection.objects.link(garment)
        retain_and_push(garment, faces, offset, material(f"Cloth_{rgb}", rgb), smooth)
        covered |= faces
    remove_covered_skin(body, covered)
    for mat in body.data.materials:
        if mat:
            mat.name = "Skin_Regular_Male_Light"

    hair_material = material("Hair_3A2A20", "3A2A20")
    before = set(bpy.context.scene.objects)
    bpy.ops.import_scene.fbx(filepath=str(HAIR))
    for obj in list(bpy.context.scene.objects):
        if obj in before:
            continue
        if obj.type == "MESH":
            world = obj.matrix_world.copy()
            obj.parent = arm
            obj.matrix_world = world
            mod = next((m for m in obj.modifiers if m.type == "ARMATURE"), None)
            if mod is None:
                mod = obj.modifiers.new("Armature", "ARMATURE")
            mod.object = arm
            obj.data.materials.clear()
            obj.data.materials.append(hair_material)
            obj.name = "Forastero_Hair_Buzzed"
        elif obj.type == "ARMATURE":
            bpy.data.objects.remove(obj, do_unlink=True)

    # The source Unity FBX rig has a 180-degree export correction around Z.
    arm.rotation_euler = (arm.rotation_euler.x, arm.rotation_euler.y, 0.0)
    bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    for obj in bpy.context.scene.objects:
        if obj.type in {"ARMATURE", "MESH"}:
            obj.select_set(True)
    OUT.parent.mkdir(parents=True, exist_ok=True)
    bpy.ops.export_scene.fbx(filepath=str(OUT), use_selection=True,
                             object_types={"ARMATURE", "MESH"},
                             apply_scale_options="FBX_SCALE_ALL", axis_forward="-Y", axis_up="Z",
                             apply_unit_scale=True, use_space_transform=False,
                             bake_space_transform=False, mesh_smooth_type="OFF",
                             use_mesh_modifiers=True, primary_bone_axis="Y",
                             secondary_bone_axis="X", armature_nodetype="NULL",
                             use_armature_deform_only=True, add_leaf_bones=False, bake_anim=False)
    print(f"ART01_CLOTHED_SCALE_DERIVED faces={len(covered)} output={OUT}")


main()
