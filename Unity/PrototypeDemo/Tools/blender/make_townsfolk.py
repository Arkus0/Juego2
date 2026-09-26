"""Clothed townsfolk derived from Quaternius Universal Base Characters (CC0).

    blender -b --factory-startup --python Unity/PrototypeDemo/Tools/blender/make_townsfolk.py [-- only=<name>]

Garments are carved out of the body mesh by dominant bone (so they keep the skin weights and animate with the
UAL humanoid clips), pushed out a few millimetres, and the covered body faces are removed so nothing pokes
through. Hair comes from the "Rigged to Head Bone" hairstyles. Output: Assets/_Derived/Generated/Characters/*.fbx
(regenerable; not versioned because it contains third-party geometry).
"""
import bpy, bmesh, math, os, sys
from mathutils import Vector

SRC = r"C:\Juego2-Assets\Base Characters"
BODY_FBX = SRC + r"\Base Characters\Exports\Unity\{}.fbx"
HAIR_FBX = SRC + r"\Hairstyles\Rigged to Head Bone\FBX (Unity)\{}.fbx"
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "Assets", "_Derived", "Generated", "Characters")

BASES = {"male": "Regular_Male_FullBody", "female": "Regular_Female_FullBody",
         "teen_m": "Teen_Male_FullBody", "teen_f": "Teen_Female_FullBody"}

# name, base, skin tone, garments [(kind, hex)], hair [files], hair hex
VARIANTS = [
    ("Forastero",   "male",   "Light", [("shirt_long", "D8D2C4"), ("jacket", "2E4A4E"), ("trousers", "3A3530"), ("shoes", "4A3222")], ["Male/Hair_Buzzed"], "3A2A20"),
    ("Paisano",     "male",   "Light", [("jersey", "3F5A3A"), ("trousers", "5A4632"), ("shoes", "2A211B"), ("beret", "22222A")], ["Male/Hair_Balding", "Male/Hair_Beard"], "6E6A64"),
    ("Vecino",      "male",   "Light", [("shirt_long", "C9D3DC"), ("vest", "3A3A40"), ("trousers", "2E3440"), ("shoes", "1E1B18")], ["Male/Hair_SimpleParted", "Male/Hair_Moustache"], "2A1E18"),
    ("Abuelo",      "male",   "Light", [("shirt_long", "E6E0D2"), ("jacket", "55524C"), ("trousers", "3E3C38"), ("shoes", "1E1B18"), ("beret", "1C1C22")], ["Male/Hair_Balding"], "BDB8AE"),
    ("Camarero",    "male",   "Light", [("shirt_long", "EEEAE0"), ("vest", "1E1E22"), ("trousers", "1E1E22"), ("apron_long", "E4DED0"), ("shoes", "141210")], ["Male/Hair_SlickBack", "Male/Hair_Beard"], "231A14"),
    ("Joven",       "male",   "Dark",  [("shirt_short", "8A3A2E"), ("trousers", "35465E"), ("shoes", "D6D0C4")], ["Male/Hair_Dreads"], "1A1410"),
    ("Vecina",      "female", "Light", [("jersey", "6A2E36"), ("trousers", "2A2626"), ("skirt", "2F2E36"), ("apron", "3E5A7A"), ("shoes", "2A211B")], ["Female/Hair_Buns"], "4A3222"),
    ("Moza",        "female", "Light", [("jersey", "B08A3E"), ("trousers", "3A4A62"), ("shoes", "4A3222")], ["Female/Hair_Ponytail_2"], "5A3A22"),
    ("Abuela",      "female", "Light", [("shirt_long", "3A3434"), ("jacket", "1F1C1C"), ("trousers", "2E2A2A"), ("skirt_long", "2A2828"), ("shoes", "141210")], ["Female/Hair_Bob"], "C2BDB4"),
    ("Tendera",     "female", "Dark",  [("shirt_long", "B7C8D6"), ("trousers", "3A3530"), ("skirt", "5A4632"), ("apron", "EDE6D6"), ("shoes", "2A211B")], ["Female/Hair_Bob"], "1E1612"),
    ("Paisana",     "female", "Light", [("jersey", "4E6A48"), ("jacket", "6A4A2E"), ("trousers", "2E2A26"), ("skirt_long", "4A3E30"), ("shoes", "2A211B")], ["Female/Hair_Long"], "2E2018"),
    ("Chaval",      "teen_m", "Light", [("shirt_short", "2F5A8A"), ("trousers", "3E4A5E"), ("shoes", "E0DAD0")], ["Male/Hair_Buzzed_Teen"], "4A3222"),
    ("Chavala",     "teen_f", "Light", [("jersey", "2E3A5A"), ("trousers", "5A5A5E"), ("shoes", "E0DAD0")], ["Female/Hair_Long_Teen"], "7A5A36"),
]

TORSO = {"spine_01", "spine_02", "spine_03", "clavicle_l", "clavicle_r"}
UPPER = {"upperarm_l", "upperarm_r"}
LOWER = {"lowerarm_l", "lowerarm_r"}
PELVIS = {"pelvis", "root"}
LEGS = {"thigh_l", "thigh_r", "calf_l", "calf_r"}
FEET = {"foot_l", "foot_r", "ball_l", "ball_r"}
OFFSET = {"shirt_long": 0.011, "shirt_short": 0.011, "jersey": 0.013, "vest": 0.019, "jacket": 0.024,
          "trousers": 0.008, "shoes": 0.009}
# Laplacian passes: soften the sculpted anatomy so cloth reads as cloth, not paint
SMOOTH = {"shirt_long": 3, "shirt_short": 3, "jersey": 5, "vest": 4, "jacket": 6, "trousers": 2, "shoes": 1}
GENERATED = {"skirt", "skirt_long", "apron", "apron_long", "beret"}
BASE_LAYER = {"shirt_long", "shirt_short", "jersey", "jacket", "trousers", "shoes"}


def world_bone_z(arm, name):
    return (arm.matrix_world @ arm.data.bones[name].head_local).z


def garment_faces(obj, kind, dom, levels):
    waist, knee, ankle = levels
    mw = obj.matrix_world
    nmat = mw.to_3x3()
    sel = set()
    for f in obj.data.polygons:
        bones = [dom[v] for v in f.vertices]
        z = (mw @ f.center).z
        ny = (nmat @ f.normal).normalized().y
        def all_in(s):
            return all(b in s for b in bones)
        top_torso = all_in(TORSO) or (all_in(TORSO | PELVIS) and z > waist - 0.04)
        ok = False
        if kind in ("shirt_long", "jersey"):
            ok = top_torso or all_in(TORSO | UPPER | LOWER) and not all_in(PELVIS)
        elif kind == "shirt_short":
            ok = top_torso or all_in(TORSO | UPPER) and not all_in(PELVIS)
        elif kind == "jacket":
            ok = all_in(TORSO | UPPER | LOWER) or (all_in(TORSO | PELVIS) and z > waist - 0.13)
        elif kind == "vest":
            ok = all_in(TORSO) or (all_in(TORSO | PELVIS) and z > waist - 0.06)
        elif kind == "trousers":
            ok = (all_in(PELVIS | LEGS | {"spine_01"}) and z < waist + 0.04) and not (all_in(LEGS | FEET) and z < ankle)
        elif kind == "shoes":
            ok = all_in(FEET) or (all_in(LEGS | FEET) and z < ankle)
        if ok:
            sel.add(f.index)
    return sel


def make_material(name, hexcol):
    m = bpy.data.materials.get(name) or bpy.data.materials.new(name)
    r, g, b = (int(hexcol[i:i + 2], 16) / 255 for i in (0, 2, 4))
    m.diffuse_color = (r, g, b, 1)
    m.use_nodes = True
    bsdf = m.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (r ** 2.2, g ** 2.2, b ** 2.2, 1)
        bsdf.inputs["Roughness"].default_value = 0.8
    return m


def keep_faces(obj, faces, offset, material, smooth=0):
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bm.faces.ensure_lookup_table()
    kill = [f for f in bm.faces if f.index not in faces]
    bmesh.ops.delete(bm, geom=kill, context="FACES")
    loose = [v for v in bm.verts if not v.link_faces]
    bmesh.ops.delete(bm, geom=loose, context="VERTS")
    inner = [v for v in bm.verts if not v.is_boundary]
    for _ in range(smooth):
        bmesh.ops.smooth_vert(bm, verts=inner, factor=0.5, use_axis_x=True, use_axis_y=True, use_axis_z=True)
    bm.normal_update()
    for v in bm.verts:
        v.co += v.normal * offset
    bm.to_mesh(obj.data)
    bm.free()
    obj.data.materials.clear()
    obj.data.materials.append(material)
    for p in obj.data.polygons:
        p.material_index = 0


def remove_faces(obj, faces):
    bm = bmesh.new()
    bm.from_mesh(obj.data)
    bm.faces.ensure_lookup_table()
    bmesh.ops.delete(bm, geom=[bm.faces[i] for i in faces], context="FACES")
    loose = [v for v in bm.verts if not v.link_faces]
    bmesh.ops.delete(bm, geom=loose, context="VERTS")
    bm.to_mesh(obj.data)
    bm.free()


# ---------------------------------------------------------------- generated garments (skirts, aprons, berets)
def skinned(name, arm, verts, faces, weights, material, both_sides=None):
    """World-space mesh parented to the armature, skinned with explicit per-vertex bone weights.
    both_sides(v) -> inward-shifted copy used for a reversed back shell (cloth seen from inside)."""
    if both_sides:
        n = len(verts)
        verts = verts + [both_sides(v) for v in verts]
        weights = weights + weights
        faces = faces + [tuple(i + n for i in reversed(f)) for f in faces]
    # same set-up as the body: identity local transform under the armature, vertices in armature space
    # (the FBX exporter does not keep a parent-inverse matrix on skinned meshes)
    to_arm = arm.matrix_world.inverted()
    me = bpy.data.meshes.new(name)
    me.from_pydata([tuple(to_arm @ v) for v in verts], [], faces)
    me.update()
    o = bpy.data.objects.new(name, me)
    bpy.context.scene.collection.objects.link(o)
    o.parent = arm
    groups = {}
    for i, w in enumerate(weights):
        for b, x in w.items():
            if x <= 0: continue
            g = groups.get(b) or o.vertex_groups.new(name=b)
            groups[b] = g
            g.add([i], x, "REPLACE")
    o.modifiers.new("Armature", "ARMATURE").object = arm
    me.materials.append(material)
    bpy.context.view_layer.update()
    return o


def hull2d(pts):
    pts = sorted(set((round(p[0], 4), round(p[1], 4)) for p in pts))
    if len(pts) < 3: return pts
    def cross(o, a, b): return (a[0] - o[0]) * (b[1] - o[1]) - (a[1] - o[1]) * (b[0] - o[0])
    lo, hi = [], []
    for p in pts:
        while len(lo) >= 2 and cross(lo[-2], lo[-1], p) <= 0: lo.pop()
        lo.append(p)
    for p in reversed(pts):
        while len(hi) >= 2 and cross(hi[-2], hi[-1], p) <= 0: hi.pop()
        hi.append(p)
    return lo[:-1] + hi[:-1]


def ray_hull(h, c, a):
    """Distance from c along angle a to the convex polygon h."""
    dx, dy = math.cos(a), math.sin(a)
    best = 0.0
    for i in range(len(h)):
        p, q = h[i], h[(i + 1) % len(h)]
        ex, ey = q[0] - p[0], q[1] - p[1]
        den = dx * ey - dy * ex
        if abs(den) < 1e-9: continue
        t = ((p[0] - c[0]) * ey - (p[1] - c[1]) * ex) / den
        u = ((p[0] - c[0]) * dy - (p[1] - c[1]) * dx) / den
        if t > 0 and -1e-6 <= u <= 1 + 1e-6: best = max(best, t)
    return best


def section(ws, z, band=0.025):
    return [(w.x, w.y) for w in ws if abs(w.z - z) < band]


def leg_weights(t, x, cx, xl, share=0.75):
    s = share * t ** 1.2
    wl = min(1.0, max(0.0, 0.5 + 0.5 * (x - cx) / (xl - cx)))
    return {"pelvis": 1 - s, "thigh_l": s * wl, "thigh_r": s * (1 - wl)}


def make_skirt(name, arm, ws, bone, top, hem, material, N=28, M=9):
    cx, cy = 0.0, bone("pelvis").y
    xl = bone("thigh_l").x
    rings, weights, prev = [], [], None
    for j in range(M):
        t = j / (M - 1)
        z = top + (hem - top) * t
        h = hull2d(section(ws, z))
        r = [ray_hull(h, (cx, cy), 2 * math.pi * i / N) for i in range(N)]
        if prev: r = [max(a, b) for a, b in zip(r, prev)]           # cloth hangs: never tucks back in
        prev = r
        ring = []
        for i in range(N):
            a = 2 * math.pi * i / N
            rr = r[i] * (1 + 0.14 * t) + 0.009 + 0.04 * t
            v = Vector((cx + rr * math.cos(a), cy + rr * math.sin(a), z))
            ring.append(v); weights.append(leg_weights(t, v.x, cx, xl))
        rings.append(ring)
    verts = [v for ring in rings for v in ring]
    faces = [(j * N + i, j * N + (i + 1) % N, (j + 1) * N + (i + 1) % N, (j + 1) * N + i) for j in range(M - 1) for i in range(N)]
    inward = lambda v: Vector((cx + (v.x - cx) * 0.985, cy + (v.y - cy) * 0.985, v.z))
    o = skinned(name, arm, verts, faces, weights, material, inward)
    fix_normals_out(o, Vector((cx, cy, 0)))
    return o


def facing(bone):
    """+1 if the character looks down +Y in this scene (toes ahead of the ankle), else -1."""
    return 1.0 if bone("ball_l").y > bone("foot_l").y else -1.0


def make_apron(name, arm, ws, bone, top, hem, material, width, over=None, C=9, M=8):
    cx = 0.0
    f = facing(bone)
    xl = bone("thigh_l").x
    hw = width / 2
    rows, weights, front = [], [], None
    for j in range(M):
        t = j / (M - 1)
        z = top + (hem - top) * t
        ys = [f * w.y for w in ws if abs(w.z - z) < 0.03 and abs(w.x - cx) < hw]
        if over: ys += [f * v.y for v in over if v.z > z - 0.03 and abs(v.x - cx) < hw]
        y = max(ys) if ys else front
        front = y if front is None else max(front, y)                  # hangs straight from the belly
        row = []
        for i in range(C):
            u = -1 + 2 * i / (C - 1)
            x = cx + u * hw * (1 + 0.1 * t)
            v = Vector((x, f * (front + 0.014 + 0.02 * t - 0.05 * u * u), z))
            row.append(v); weights.append(leg_weights(t, x, cx, xl, 0.6))
        rows.append(row)
    verts = [v for row in rows for v in row]
    faces = [(j * C + i, (j + 1) * C + i, (j + 1) * C + i + 1, j * C + i + 1) for j in range(M - 1) for i in range(C - 1)]
    o = skinned(name, arm, verts, faces, weights, material, lambda v: Vector((v.x, v.y - f * 0.004, v.z)))
    fix_normals_front(o, f)
    return o


def make_beret(name, arm, ws, dom, bone, material):
    head = [w for w, d in zip(ws, dom) if d == "Head"]
    ztop = max(w.z for w in head)
    crown = [w for w in head if w.z > ztop - 0.06]
    cx = sum(w.x for w in crown) / len(crown); cy = sum(w.y for w in crown) / len(crown) - 0.012 * facing(bone)   # sits back a little
    zb = ztop - 0.06
    band = section(head, zb, 0.012)
    hr = max(math.hypot(x - cx, y - cy) for x, y in band) + 0.006
    N = 24
    profile = [(hr, zb), (hr + 0.035, ztop - 0.03), (hr + 0.03, ztop - 0.005), (0.075, ztop + 0.018), (0.0, ztop + 0.026)]
    verts, faces = [], []
    tilt = math.radians(7)
    for k, (r, z) in enumerate(profile):
        for i in range(N):
            a = 2 * math.pi * i / N
            x, y = r * math.cos(a), r * math.sin(a)
            verts.append(Vector((cx + x, cy + y, z + math.sin(tilt) * x)))   # worn slightly to one side
    for k in range(len(profile) - 1):
        for i in range(N):
            faces.append((k * N + i, k * N + (i + 1) % N, (k + 1) * N + (i + 1) % N, (k + 1) * N + i))
    o = skinned(name, arm, verts, faces, [{"Head": 1.0}] * len(verts), material)
    fix_normals_out(o, Vector((cx, cy, 0)), whole=True)
    return o


def fix_normals_out(o, c, whole=False):
    """Flip every face if the outer shell (first half of the faces) points towards the vertical axis through c."""
    me = o.data
    polys = list(me.polygons) if whole else list(me.polygons)[: len(me.polygons) // 2]
    mw, nm = o.matrix_world, o.matrix_world.to_3x3()
    score = 0.0
    for p in polys:
        n, q = nm @ p.normal, mw @ p.center
        score += n.x * (q.x - c.x) + n.y * (q.y - c.y) + (n.z * 0.2 if whole else 0)
    if score < 0:
        for p in me.polygons: p.flip()
        me.update()


def fix_normals_front(o, f):
    """Apron: the front shell must face the way the character looks."""
    me = o.data
    polys = list(me.polygons)[: len(me.polygons) // 2]
    nm = o.matrix_world.to_3x3()
    if f * sum((nm @ p.normal).y for p in polys) < 0:
        for p in me.polygons: p.flip()
        me.update()


def build(name, base, tone, garments, hair_files, hair_hex):
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=BODY_FBX.format(BASES[base]))
    arm = next(o for o in bpy.context.scene.objects if o.type == "ARMATURE")
    body = max((o for o in bpy.context.scene.objects if o.type == "MESH"), key=lambda o: len(o.data.vertices))
    groups = {g.index: g.name for g in body.vertex_groups}
    dom = []
    for v in body.data.vertices:
        best = max(v.groups, key=lambda g: g.weight, default=None)
        dom.append(groups.get(best.group) if best else None)
    waist = world_bone_z(arm, "spine_01") + 0.02
    knee = world_bone_z(arm, "calf_l")
    ankle = world_bone_z(arm, "foot_l") + 0.035
    covered = set()
    ws = [body.matrix_world @ v.co for v in body.data.vertices]
    bone = lambda b: arm.matrix_world @ arm.data.bones[b].head_local
    for kind, hexcol in garments:
        if kind in GENERATED:
            continue
        faces = garment_faces(body, kind, dom, (waist, knee, ankle))
        if not faces:
            print("[townsfolk] empty garment", name, kind); continue
        g = body.copy(); g.data = body.data.copy(); g.name = f"{name}_{kind}"
        bpy.context.scene.collection.objects.link(g)
        keep_faces(g, faces, OFFSET[kind], make_material(f"Cloth_{hexcol}", hexcol), SMOOTH.get(kind, 0))
        if kind in BASE_LAYER:
            covered |= faces
    skirt_verts = None
    for kind, hexcol in garments:
        mat = make_material(f"Cloth_{hexcol}", hexcol)
        if kind in ("skirt", "skirt_long"):
            hem = knee - 0.06 if kind == "skirt" else knee - 0.24
            sk = make_skirt(f"{name}_{kind}", arm, ws, bone, waist - 0.03, hem, mat)
            skirt_verts = [sk.matrix_world @ v.co for v in sk.data.vertices][: len(sk.data.vertices) // 2]   # outer shell
        elif kind in ("apron", "apron_long"):
            hem = knee - 0.02 if kind == "apron" else knee - 0.2
            make_apron(f"{name}_{kind}", arm, ws, bone, waist - 0.035, hem, mat, 0.36 if base == "male" else 0.34, skirt_verts)
        elif kind == "beret":
            make_beret(f"{name}_{kind}", arm, ws, dom, bone, mat)
    remove_faces(body, covered)
    for m in body.data.materials:
        if m: m.name = f"Skin_{BASES[base].replace('_FullBody', '')}_{tone}"
    hair_mat = make_material(f"Hair_{hair_hex}", hair_hex)
    for o in list(bpy.context.scene.objects):
        if o.type == "MESH" and o is not body and not o.name.startswith(name + "_"):
            for i, m in enumerate(o.data.materials):
                if m and m.name.startswith("MI_Hair"):
                    o.data.materials[i] = hair_mat
    for hf in hair_files:
        before = set(bpy.context.scene.objects)
        bpy.ops.import_scene.fbx(filepath=HAIR_FBX.format(hf))
        new = [o for o in bpy.context.scene.objects if o not in before]
        for o in new:
            if o.type == "MESH":
                mw = o.matrix_world.copy()
                o.parent = arm
                o.matrix_world = mw
                mod = next((m for m in o.modifiers if m.type == "ARMATURE"), None) or o.modifiers.new("Armature", "ARMATURE")
                mod.object = arm
                o.data.materials.clear(); o.data.materials.append(hair_mat)
                o.name = f"{name}_{os.path.basename(hf)}"
        for o in new:
            if o.type == "ARMATURE":
                bpy.data.objects.remove(o, do_unlink=True)
    # export like the Quaternius Unity FBX kit. Their importer turns the rig 180 deg about Z; undo it so the
    # re-exported character faces the same way as the original FBX (+Z in Unity), not backwards.
    arm.rotation_euler = (arm.rotation_euler.x, arm.rotation_euler.y, 0.0)
    bpy.context.view_layer.update()
    bpy.ops.object.select_all(action="DESELECT")
    for o in bpy.context.scene.objects:
        if o.type in ("ARMATURE", "MESH"):
            o.select_set(True)
    os.makedirs(OUT, exist_ok=True)
    path = os.path.join(OUT, f"Townsfolk_{name}.fbx")
    bpy.ops.export_scene.fbx(filepath=path, use_selection=True, object_types={"ARMATURE", "MESH"},
                             apply_scale_options="FBX_SCALE_ALL", axis_forward="-Y", axis_up="Z", apply_unit_scale=True,
                             use_space_transform=False, bake_space_transform=False, mesh_smooth_type="OFF",
                             use_mesh_modifiers=True, primary_bone_axis="Y", secondary_bone_axis="X",
                             armature_nodetype="NULL", use_armature_deform_only=True, add_leaf_bones=False, bake_anim=False)
    print(f"[townsfolk] {name}: garments={len(garments)} covered={len(covered)} -> {path}")


only = None
if "--" in sys.argv:
    for a in sys.argv[sys.argv.index("--") + 1:]:
        if a.startswith("only="): only = a[5:]
for v in VARIANTS:
    if only and v[0] != only: continue
    build(*v)
