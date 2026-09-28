"""Noncanonical ART-01 source-recovery visualization. Run in Blender 5.2.1 with
blender --background --factory-startup --python Tools/art01_recovery_diagnostic.py -- <accepted Medieval Source zip> <diagnostic output directory>
No source model or texture byte is written inside the repository.
"""
import bpy, bmesh, math, pathlib, mathutils, json, sys, zipfile, hashlib, tempfile
from mathutils import Matrix, Vector
args=sys.argv[sys.argv.index('--')+1:]
if len(args)!=2: raise SystemExit('expected accepted source archive and output directory')
archive=pathlib.Path(args[0])
out=pathlib.Path(args[1])
if not out.is_absolute(): raise SystemExit('diagnostic output directory must be absolute')
out.mkdir(parents=True,exist_ok=True)
survey=json.loads((out.parent/'RECOVERY_SOURCE_SURVEY.json').read_text(encoding='utf-8'))
if hashlib.sha256(archive.read_bytes()).hexdigest()!=survey['archiveSha256']:
    raise SystemExit('archive SHA-256 does not match accepted recovery survey')
temp_source=tempfile.TemporaryDirectory(prefix='art01-recovery-')
root=pathlib.Path(temp_source.name)
with zipfile.ZipFile(archive) as z:
    for row in survey['files']:
        data=z.read(row['archiveMember'])
        if hashlib.sha256(data).hexdigest()!=row['sha256']:
            raise SystemExit('source member mismatch: '+row['name'])
        (root/(row['name']+'.fbx')).write_bytes(data)

def reset():
    bpy.ops.object.select_all(action='SELECT'); bpy.ops.object.delete(use_global=False)
    for d in list(bpy.data.materials): bpy.data.materials.remove(d)
    bpy.context.scene.render.engine='BLENDER_WORKBENCH'
    bpy.context.scene.display.shading.color_type='MATERIAL'
    bpy.context.scene.display.shading.light='STUDIO'
    bpy.context.scene.display.shading.show_shadows=True
    bpy.context.scene.display.shading.show_cavity=True
    bpy.context.scene.display.shading.cavity_type='BOTH'
    bpy.context.scene.render.resolution_x=1500
    bpy.context.scene.render.resolution_y=900
    bpy.context.scene.render.resolution_percentage=100
    bpy.context.scene.render.image_settings.file_format='PNG'
    bpy.context.scene.camera=None

def camera(pos,target,lens=35):
    bpy.ops.object.camera_add(location=pos)
    c=bpy.context.object
    c.rotation_euler=(Vector(target)-c.location).to_track_quat('-Z','Y').to_euler()
    c.data.lens=lens
    bpy.context.scene.camera=c

def mats():
    for m in bpy.data.materials:
        n=m.name.lower()
        if 'plaster' in n: c=(0.66,0.65,0.59,1)
        elif 'unevenbrick' in n or 'brick' in n or 'rocktrim' in n: c=(0.42,0.45,0.43,1)
        elif 'woodtrim_wear' in n: c=(0.25,0.22,0.18,1)
        elif 'woodtrim' in n: c=(0.31,0.25,0.19,1)
        elif 'flattile' in n or 'roundtile' in n: c=(0.23,0.23,0.25,1)
        elif 'windowglass' in n: c=(0.22,0.30,0.32,1)
        elif 'vine' in n: c=(0.25,0.35,0.25,1)
        else: c=(0.47,0.47,0.44,1)
        m.diffuse_color=c

def plain_mat(name,color):
    m=bpy.data.materials.get(name) or bpy.data.materials.new(name)
    m.diffuse_color=(*color,1)
    return m

def load(name):
    before=set(bpy.context.scene.objects)
    bpy.ops.import_scene.fbx(filepath=str(root/(name+'.fbx')))
    new=[o for o in bpy.context.scene.objects if o not in before and o.type=='MESH']
    for o in new: o.hide_render=True; o.hide_viewport=True
    return [(o,o.matrix_world.copy()) for o in new]

sources={}
def put(name,loc,rot=0,scale=(1,1,1),label=None):
    if name not in sources: sources[name]=load(name)
    objs=[]
    T=Matrix.Translation(Vector(loc)) @ Matrix.Rotation(math.radians(rot),4,'Z') @ Matrix.Diagonal((scale[0],scale[1],scale[2],1))
    for original,base in sources[name]:
        o=original.copy(); o.data=original.data.copy(); bpy.context.collection.objects.link(o)
        o.hide_render=False; o.hide_viewport=False
        o.name=(label or name)
        o.matrix_world=T @ base
        objs.append(o)
    return objs

def block(name,loc,dimensions,material):
    bpy.ops.mesh.primitive_cube_add(size=1,location=loc)
    o=bpy.context.object; o.name=name; o.dimensions=dimensions
    bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
    o.data.materials.append(material)
    return o

def gable(xc,y,zbase,width,peak,material):
    verts=[(xc-width/2,y-.16,zbase),(xc+width/2,y-.16,zbase),(xc,y-.16,peak),(xc-width/2,y+.16,zbase),(xc+width/2,y+.16,zbase),(xc,y+.16,peak)]
    faces=[(0,1,2),(5,4,3),(0,3,4,1),(1,4,5,2),(2,5,3,0)]
    mesh=bpy.data.meshes.new('gable.geometry'); mesh.from_pydata(verts,[],faces); mesh.update()
    o=bpy.data.objects.new('Derived gable infill',mesh); bpy.context.collection.objects.link(o); o.data.materials.append(material)

def facade(xc,yfront,z,kind,upper=False):
    family='Wall_Plaster' if kind=='plaster' else 'Wall_WoodBrick'
    for i,dx in enumerate((-2,0,2)):
        if upper: module='Window_Wide_Flat' if i!=1 else 'Straight'
        else: module='Door_Flat' if i==1 else 'Window_Wide_Flat'
        put(family+'_'+module,(xc+dx,yfront,z))
        if module=='Window_Wide_Flat': put('Window_Wide_Flat1',(xc+dx,yfront,z))
        if module=='Door_Flat':
            put('DoorFrame_Flat_Brick',(xc+dx,yfront,z))
            put('Door_3_Flat',(xc+dx+.5,yfront-.1,z))

def house(xc,yc,ground_family,roof_name,front_offset=0):
    front=yc-5; back=yc+5
    story=3.1227
    for level in (0,1):
        z=level*story
        fam=ground_family if level==0 else 'plaster'
        facade(xc,front,z,fam,level==1)
        for dx in (-2,0,2): put('Wall_Plaster_Straight',(xc+dx,back,z),180)
        for side,rot in ((-3,-90),(3,90)):
            for dy in (-4,-2,0,2,4):
                module='Wall_WoodBrick_Straight' if fam=='woodbrick' and level==0 else 'Wall_Plaster_Straight'
                put(module,(xc+side,yc+dy,z),rot)
    eave=2*story
    roofscale=(.86,.94,.45) if roof_name=='Roof_RoundTiles_6x10' else (.77,.90,.43)
    put(roof_name,(xc,yc,eave+.34),scale=roofscale)
    stone=plain_mat('derived.stone.base',(.35,.38,.38))
    gable(xc,front,eave,6,eave+2.15,stone); gable(xc,back,eave,6,eave+2.15,stone)
    block('plinth.front.left',(xc-1.86,front-.05,.08),(2.28,.60,.28),stone)
    block('plinth.front.right',(xc+1.86,front-.05,.08),(2.28,.60,.28),stone)
    block('landing',(xc,front-.95,.045),(1.38,1.8,.09),stone)
    block('plinth.back',(xc,back,.08),(6.4,.65,.28),stone)
    for xx in (xc-3.18,xc+3.18): block('plinth.side',(xx,yc,.08),(.28,10.2,.28),stone)

reset()
put('Wall_Plaster_Window_Wide_Flat',(0,0,0))
mats()
# Source-facing palette approximates its original warm plaster/timber split, no texturing claim.
for m in bpy.data.materials:
    if 'Plaster' in m.name: m.diffuse_color=(0.76,.67,.51,1)
    if 'WoodTrim' in m.name: m.diffuse_color=(.46,.27,.13,1)
block('reference floor',(0,0,-.07),(3,2,.08),plain_mat('source.floor',(.5,.5,.48)))
camera((3.2,-6.5,2.1),(0,0,1.6),50)
bpy.context.scene.render.filepath=str(out/'source_plaster_window.png')
bpy.ops.render.render(write_still=True)

reset(); sources={}
house(0,0,'plaster','Roof_RoundTiles_6x10')
house(7.3,.85,'plaster','Roof_FlatTiles_6x10')
# Real source component extraction: the WoodBrick material partition supplies a separate timber frieze/wainscot donor.
raw=load('Wall_WoodBrick_Straight')
donor=[]
for original,base in raw:
    o=original.copy(); o.data=original.data.copy()
    keep=[i for i,m in enumerate(o.data.materials) if m and m.name.startswith('MI_WoodTrim')]
    bm=bmesh.new(); bm.from_mesh(o.data)
    bmesh.ops.delete(bm,geom=[f for f in bm.faces if f.material_index not in keep],context='FACES')
    bm.to_mesh(o.data); bm.free(); o.data.update()
    donor.append((o,base))
sources['Donor_WoodBrick']=donor
for dx in (-2,2): put('Donor_WoodBrick',(7.3+dx,-4.15,0),label='recovered timber wainscot donor')
for dy in (-4,-2,0,2,4): put('Donor_WoodBrick',(10.3,.85+dy,0),rot=90,label='recovered timber side cladding donor')
# Formerly deprioritized timber modules used as a supported upper gallery and shopfront detail.
put('Balcony_Cross_Straight',(7.3,-4.25,3.08))
put('Prop_Crate',(9.9,-5.1,.4),scale=(.47,.47,.47))
put('Prop_Vine1',(-2.7,-5.2,.45),scale=(.75,.75,.75))
road=plain_mat('diagnostic.street',(.33,.38,.39))
block('diagnostic road',(3.7,-10,-.13),(20,8,.16),road)
curb=plain_mat('diagnostic.kerb',(.45,.47,.45))
block('diagnostic left edge',(3.7,-6.1,-.03),(20,.22,.20),curb)
block('diagnostic far edge',(3.7,-13.9,-.03),(20,.22,.20),curb)
# Height-only third-person scale instrument, deliberately non-keeper.
proxy=plain_mat('diagnostic.human.scale',(.18,.23,.27))
bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=.22,depth=1.55,location=(2.1,-8.6,.78))
bpy.context.object.name='1.78m diagnostic scale proxy body'; bpy.context.object.data.materials.append(proxy)
bpy.ops.mesh.primitive_uv_sphere_add(segments=12,ring_count=8,radius=.18,location=(2.1,-8.6,1.63))
bpy.context.object.name='1.78m diagnostic scale proxy head'; bpy.context.object.data.materials.append(proxy)
mats()
camera((12.8,-17.5,1.65),(3.4,-1.0,2.6),28)
bpy.context.scene.render.filepath=str(out/'third_person_recovery_diagnostic_oblique.png')
bpy.ops.render.render(write_still=True)

# Show the exact source-to-donor partition separately. This does not create a
# production mesh or admit an additional source into the ART Unity project.
reset(); sources={}
src=put('Wall_WoodBrick_Straight',(-1.7,0,0))[0]
donor=src.copy(); donor.data=src.data.copy(); bpy.context.collection.objects.link(donor)
donor.name='Diagnostic WoodBrick timber donor'
donor.matrix_world=Matrix.Translation(Vector((3.4,0,0))) @ src.matrix_world
material_counts={m.name:sum(1 for p in src.data.polygons if p.material_index==i)
                 for i,m in enumerate(src.data.materials) if m}
keep_indices=[i for i,m in enumerate(donor.data.materials) if m and m.name.startswith('MI_WoodTrim')]
bm=bmesh.new(); bm.from_mesh(donor.data)
bmesh.ops.delete(bm,geom=[f for f in bm.faces if f.material_index not in keep_indices],context='FACES')
bm.to_mesh(donor.data); bm.free(); donor.data.update()
mats()
for_source=next(row for row in survey['files'] if row['name']=='Wall_WoodBrick_Straight')
camera((3,-8,2.6),(0,0,1.6),42)
bpy.context.scene.render.resolution_x=1400
bpy.context.scene.render.resolution_y=850
bpy.context.scene.render.filepath=str(out/'donor_timber_partition_probe.png')
bpy.ops.render.render(write_still=True)
report={
    'schema':'juego2.art01.recovery-donor-probe@1',
    'status':'PREFOUNDATION_DIAGNOSTIC_ONLY_NOT_UNITY_ADMISSION',
    'source':'Wall_WoodBrick_Straight',
    'archiveMember':for_source['archiveMember'],
    'sourceSha256':for_source['sha256'],
    'sourcePolygonCount':len(src.data.polygons),
    'materialPolygonCounts':material_counts,
    'keptMaterialIndices':keep_indices,
    'keptPolygonCount':len(donor.data.polygons),
    'method':'Blender 5.2.1: preserve faces whose imported source material starts with MI_WoodTrim; remove other material faces without altering source file',
    'remainingWork':'Assess disconnected parts and joints; create stable ART derivative ID and exact import/material/collider proof on accepted H2F baseline before keeper use',
    'capture':'recovery_diagnostic/donor_timber_partition_probe.png'
}
(out.parent/'RECOVERY_DONOR_PROBE.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
temp_source.cleanup()
print('ART01_RECOVERY_DIAGNOSTIC',out)
