"""POC-2D-EASTWARD-00 art pipeline.
Source: AI-generated original images (Grok Bot) in source/*.jpg on magenta.
Steps: magenta key (hard alpha + despill + 1px erode) -> premultiplied downscale to the pixel grid
-> shared palette (median cut over ALL pieces, 48 colours) -> nearest-colour quantize (no dither)
-> seamless pavement tiles (wrap crossfade) -> modular facades -> props -> tileset, atlas, Tiled map, manifest.
Run:  python3 tools/pipeline.py
"""
from PIL import Image
import numpy as np, json, os, hashlib
from scipy import ndimage as nd
ROOT=os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
S=lambda *p: os.path.join(ROOT,*p)
KEY=np.array([248,0,198])
T=16  # grid tile size
PROV="AI image generation by Grok Bot (original, generated for this project; no game assets copied, traced or extracted)"

def load(name): return np.asarray(Image.open(S('source',name+'.jpg')).convert('RGB')).astype(np.float32)

def key_out(rgb):
    d=np.abs(rgb-KEY).sum(-1)
    spill=np.clip(np.minimum(rgb[...,0],rgb[...,2])-rgb[...,1],0,None)
    bg=(d<170)|((spill>110)&(rgb[...,1]<70))
    bg=nd.binary_opening(bg,iterations=1)
    fg=~bg; fg=nd.binary_fill_holes(fg) if False else fg
    fg=nd.binary_erosion(fg,iterations=2)   # remove 2px JPEG fringe at source res
    # despill near the matte edge only
    edge=nd.binary_dilation(~fg,iterations=6)&fg
    out=rgb.copy()
    sp=np.clip(np.minimum(out[...,0],out[...,2])-out[...,1],0,None)
    for ch in (0,2): out[...,ch]=np.where(edge,out[...,ch]-sp*0.8,out[...,ch])
    return np.clip(out,0,255),fg.astype(np.float32)

def downscale(rgb,alpha,size):
    """premultiplied area resample, then hard alpha at 50%"""
    w,h=size
    pm=np.dstack([rgb*alpha[...,None],alpha*255]).astype(np.uint8)
    im=Image.fromarray(pm,'RGBA')
    # Image.BOX after a LANCZOS pre-pass to ~2x keeps micro-contrast without ringing
    im=im.resize((w*2,h*2),Image.LANCZOS).resize((w,h),Image.BOX)
    a=np.asarray(im).astype(np.float32); al=a[...,3]/255
    col=np.where(al[...,None]>0.01,a[...,:3]/np.maximum(al[...,None],1e-3),0)
    return np.clip(col,0,255),(al>0.5).astype(np.float32)

PIECES={}   # name -> (rgb, alpha)  pre-palette
def add(name,rgb,al): PIECES[name]=(rgb,al)

# ---------------- pavement ----------------
pav=load('pavement')
boxes=[(55,70,313,327),(359,70,617,327),(663,70,921,327),(967,70,1225,327),(55,372,313,631),(359,372,616,631),(663,372,921,631),(967,372,1225,631)]
pnames=['cobble_a','cobble_b','cobble_c','cobble_moss','cobble_puddle','cobble_worn_path','cobble_gutter','cobble_to_slab']
M=32   # each pavement variant becomes a 32x32 metatile (2x2 grid tiles); cobble ~5-6 px
OV=8   # crossfade overlap in px (at target res)
def seamless(rgb):
    h,w,_=rgb.shape
    out=rgb[:M+OV,:M+OV].copy()
    # x wrap: blend the extra right strip into the left start
    wgt=((np.arange(OV)+0.5)/OV)[None,:,None]
    out[:,:OV]=out[:,M:M+OV]*(1-wgt)+out[:,:OV]*wgt
    out=out[:,:M]
    wgt=((np.arange(OV)+0.5)/OV)[:,None,None]
    out[:OV]=out[M:M+OV]*(1-wgt)+out[:OV]*wgt
    return out[:M]
for (x0,y0,x1,y1),n in zip(boxes,pnames):
    inset=int((x1-x0)*0.05)
    crop=pav[y0+inset:y1-inset,x0+inset:x1-inset]
    # map interior so that cobble size ~ 5-6 px; source interior ~232 px holds ~5.5 cobbles
    src=Image.fromarray(crop.astype(np.uint8))
    tgt=M+OV
    small=np.asarray(src.resize((tgt*2,tgt*2),Image.LANCZOS).resize((tgt,tgt),Image.BOX)).astype(np.float32)
    if n in('cobble_worn_path','cobble_gutter','cobble_to_slab'):
        # directional transition pieces: only wrap along their run direction to keep the feature centred
        small=np.asarray(src.resize((M*2,M*2),Image.LANCZOS).resize((M,M),Image.BOX)).astype(np.float32)
        if n=='cobble_gutter':  # runs horizontally -> wrap x only
            ext=np.concatenate([small,small[:,:OV]],1); wgt=((np.arange(OV)+0.5)/OV)[None,:,None]
            small=small.copy(); small[:,:OV]=small[:,:OV]*wgt+np.roll(small,OV,1)[:,:OV]*(1-wgt)
        else:                   # runs vertically -> wrap y only
            wgt=((np.arange(OV)+0.5)/OV)[:,None,None]; small=small.copy(); small[:OV]=small[:OV]*wgt+np.roll(small,OV,0)[:OV]*(1-wgt)
        add('pav_'+n,small,np.ones(small.shape[:2],np.float32))
    else:
        t=seamless(small); add('pav_'+n,t,np.ones(t.shape[:2],np.float32))

# tonal normalisation: match mean/std of every pavement piece to cobble_a so variants and transitions blend (no visible grid)
ref=PIECES['pav_cobble_a'][0]; rm,rs=ref.reshape(-1,3).mean(0),ref.reshape(-1,3).std(0)
for n in list(PIECES):
    if n.startswith('pav_') and n not in('pav_cobble_a','pav_cobble_puddle'):
        r,a=PIECES[n]; m,sd=r.reshape(-1,3).mean(0),r.reshape(-1,3).std(0)
        k=0.85 if n!='pav_cobble_moss' else 0.6
        PIECES[n]=(np.clip((r-m)/np.maximum(sd,1)*(sd*(1-k)+rs*k)+(m*(1-k)+rm*k),0,255),a)
# ---------------- facades ----------------
def facade(name,box,size,splits):
    x0,y0,x1,y1=box
    rgb,al=key_out(load(name))
    rgb,al=rgb[y0:y1,x0:x1],al[y0:y1,x0:x1]
    r,a=downscale(rgb,al,size)
    add('fac_'+name+'_full',r,a)
    return r,a,splits
FAC={}
FAC['residencial']=facade('facade_res',(337,11,942,698),(160,182),{'roof':0.0,'upper':0.205,'base':0.655})
FAC['chigre']=facade('chigre',(352,33,1006,642),(128,119),{'roof':0.0,'upper':0.17,'base':0.44})
# ---------------- props ----------------
props_src=key_out(load('props'))
PROPS=[('barrel_table',(100,7,268,246),24),('fish_crates',(675,18,907,236),20),('butane',(1040,29,1131,217),14),('stool',(410,63,552,215),12),
('vending',(888,216,1054,469),36),('bicycle',(344,258,567,449),20),('wall_lantern',(114,266,211,409),15),('lamp_post',(153,422,227,691),50),
('hydrangea',(496,468,678,681),20),('bench',(822,478,1099,699),20)]
for n,(x0,y0,x1,y1),hgt in PROPS:
    rgb,al=props_src[0][y0:y1,x0:x1],props_src[1][y0:y1,x0:x1]
    w=max(4,round((x1-x0)*hgt/(y1-y0)))
    r,a=downscale(rgb,al,(w,hgt)); add('prop_'+n,r,a)

# ---------------- shared palette ----------------
allpx=np.concatenate([r[a>0.5] for r,a in PIECES.values()])
sample=Image.fromarray(allpx[None].astype(np.uint8))
NCOL=48
# 42 colours median-cut over everything + 6 reserved cool accents (hydrangea / butane blues) so focal colour is not averaged away
palimg=sample.quantize(colors=NCOL-6,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE)
pal=np.array(palimg.getpalette()[:(NCOL-6)*3]).reshape(-1,3)
acc=np.concatenate([PIECES[n][0][PIECES[n][1]>0.5] for n in ('prop_hydrangea','prop_butane')])
acc=acc[(acc[:,2]-acc[:,0])>15]
accimg=Image.fromarray(acc[None].astype(np.uint8)).quantize(colors=6,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE)
pal=np.concatenate([pal,np.array(accimg.getpalette()[:18]).reshape(-1,3)])
# order palette by luminance for documentation
pal=pal[np.argsort(pal@[0.299,0.587,0.114])]
def quant(rgb):
    flat=rgb.reshape(-1,3)
    idx=np.argmin(((flat[:,None,:]-pal[None])**2).sum(-1),1)
    return pal[idx].reshape(rgb.shape).astype(np.uint8)
FINAL={}
for n,(r,a) in PIECES.items():
    q=quant(r); FINAL[n]=np.dstack([q,(a*255).astype(np.uint8)])
# palette files
pim=Image.new('RGB',(NCOL,1)); pim.putdata([tuple(int(v) for v in p) for p in pal]); pim.save(S('assets','palette','palette.png'))
pim.resize((NCOL*12,12),Image.NEAREST).save(S('assets','palette','palette_x12.png'))
with open(S('assets','palette','poc2d.gpl'),'w') as f:
    f.write('GIMP Palette\nName: POC2D-Eastward-00\nColumns: 12\n#\n')
    for p in pal: f.write('%3d %3d %3d #%02x%02x%02x\n'%(*p,*p))
# ---------------- write pieces ----------------
MAN={'project':'POC-2D-EASTWARD-00','grid_tile':T,'palette':{'file':'assets/palette/palette.png','gpl':'assets/palette/poc2d.gpl','colors':NCOL,'hex':['#%02x%02x%02x'%tuple(p) for p in pal]},
     'provenance_default':PROV,'pipeline':'tools/pipeline.py','assets':[]}
def save(arr,path,meta):
    Image.fromarray(arr,'RGBA' if arr.shape[2]==4 else 'RGB').save(S(path))
    meta.update({'file':path,'w':arr.shape[1],'h':arr.shape[0],'sha1':hashlib.sha1(open(S(path),'rb').read()).hexdigest()[:12]})
    MAN['assets'].append(meta)
# pavement tileset: each metatile 32x32 -> 4 tiles of 16. tileset image 8 metatiles in a row (256x32) + extra slab tile row
pav_list=['pav_'+n for n in pnames]
ts=np.zeros((64,256,4),np.uint8)
for i,n in enumerate(pav_list): ts[0:32,i*32:i*32+32]=FINAL[n]
# derived: pure slab metatile (from right half of cobble_to_slab, mirrored to fill), horizontal curb (rotated transition)
slab=FINAL['pav_cobble_to_slab'][:,20:32]; slabm=np.concatenate([slab,slab[:,::-1],slab,slab[:,::-1][:,:8]],1)[:,:32]
ts[32:64,0:32]=slabm
rot=np.rot90(FINAL['pav_cobble_to_slab'],1)   # slab on top, cobble below (for sidewalk along north facades)
ts[32:64,32:64]=rot
save(ts,'assets/tilesets/pavement_16.png',{'id':'tileset_pavement','kind':'tileset','tile':16,'metatile':32,
 'layout':'row0: '+', '.join(pnames)+' (each 2x2 tiles). row1: slab_fill (derived), curb_horizontal (cobble_to_slab rotated 90deg, PROVISIONAL)',
 'seamless':'wrap crossfade 8px on both axes for cobble_a/b/c/moss/puddle; single-axis wrap for path (y), gutter (x), cobble_to_slab (y)',
 'source':'source/pavement.jpg','reuse':'any street; 3 base variants + moss/puddle decals + path/gutter/curb transitions'})
for n in pav_list: save(FINAL[n],f'assets/tilesets/{n}.png',{'id':n,'kind':'metatile','source':'source/pavement.jpg','reuse':'see tileset'})
# facades modular split
for key,(r,a,sp) in FAC.items():
    full=FINAL['fac_'+('facade_res' if key=='residencial' else 'chigre')+'_full']; h=full.shape[0]
    rows={k:round(v*h) for k,v in sp.items()}
    parts={'roof':(0,rows['upper']),'upper':(rows['upper'],rows['base']),'base':(rows['base'],h)}
    save(full,f'assets/facades/{key}_full.png',{'id':f'fac_{key}_full','kind':'facade_macrosprite','anchor':'bottom-center (foot line = last opaque row)',
      'source':f"source/{'facade_res' if key=='residencial' else 'chigre'}.jpg",'reuse':'macro-sprite; pieces below allow re-stacking (base/upper/roof)',
      'notes':'chigre: left side wall + side roof hip CROPPED to keep a single front-on 3/4 grammar' if key=='chigre' else ''})
    for pn,(a0,a1) in parts.items():
        save(full[a0:a1],f'assets/facades/{key}_{pn}.png',{'id':f'fac_{key}_{pn}','kind':'facade_piece','offset_y_from_top':a0,
          'layer':'occlusion/above-actors' if pn=='roof' else 'world (depth by foot line)','anchor':'bottom-center of full facade',
          'source':f"source/{'facade_res' if key=='residencial' else 'chigre'}.jpg",'reuse':'stack base+upper(+upper)+roof; change base to vary ground floor'})
for n,_,hgt in PROPS:
    arr=FINAL['prop_'+n]
    save(arr,f'assets/props/{n}.png',{'id':n,'kind':'prop','anchor':'bottom-center (feet/ground contact)','target_height_px':hgt,'source':'source/props.jpg',
      'reuse':'generic urban prop','collision':'footprint = bottom 25% of width x 6px' if n not in('wall_lantern',) else 'none (wall-mounted)'})
# ---------------- atlas (props + facades), simple shelf packer, JSON hash compatible with Phaser/TexturePacker ----------------
items=[('fac_residencial_full',FINAL['fac_facade_res_full']),('fac_chigre_full',FINAL['fac_chigre_full'])]+[(n,FINAL['prop_'+n]) for n,_,_ in PROPS]
for key in FAC:
    full=FINAL['fac_'+('facade_res' if key=='residencial' else 'chigre')+'_full']; h=full.shape[0]; sp=FAC[key][2]
    rows={k:round(v*h) for k,v in sp.items()}
    items+= [(f'fac_{key}_roof',full[:rows['upper']]),(f'fac_{key}_upper',full[rows['upper']:rows['base']]),(f'fac_{key}_base',full[rows['base']:])]
AW=512; x=y=rowh=0; frames={}; atlas=np.zeros((1024,AW,4),np.uint8)
for n,arr in sorted(items,key=lambda t:-t[1].shape[0]):
    h,w=arr.shape[:2]
    if x+w>AW: x=0; y+=rowh+2; rowh=0
    atlas[y:y+h,x:x+w]=arr; frames[n]={'frame':{'x':x,'y':y,'w':w,'h':h},'rotated':False,'trimmed':False,'spriteSourceSize':{'x':0,'y':0,'w':w,'h':h},'sourceSize':{'w':w,'h':h},'anchor':{'x':0.5,'y':1.0}}
    x+=w+2; rowh=max(rowh,h)
AH=1<<int(np.ceil(np.log2(y+rowh+1)))
atlas=atlas[:AH]
save(atlas,'assets/atlas/world_atlas.png',{'id':'atlas_world','kind':'atlas','format':'JSON hash (TexturePacker/Phaser compatible)'})
json.dump({'frames':frames,'meta':{'image':'world_atlas.png','size':{'w':AW,'h':AH},'scale':'1','app':'tools/pipeline.py'}},open(S('assets','atlas','world_atlas.json'),'w'),indent=1)
# ---------------- Tiled map (JSON, embedded tileset) ----------------
MW,MH=30,26
FOOT=200   # facade foot line y (px)
def mt(col,row,sub):  # metatile -> gid (tileset cols=16)
    return 1+(row*2+sub[1])*16+col*2+sub[0]
ground=[0]*(MW*MH)
rng=np.random.default_rng(1992)
base_variants=[0,1,2]
for ty in range(0,MH,2):
    for tx in range(0,MW,2):
        y=ty*16
        if y<FOOT: col,row=0,1      # under facades (hidden) -> slab
        elif y<FOOT+32: col,row=1,1  # horizontal curb strip along facades
        else:
            row=0; r=rng.random(); col=int(rng.choice(base_variants)) if r<0.9 else 3   # moss 10%
            if tx in(14,):col=5    # worn path down the middle (vertical)
            if ty==20: col=6       # gutter row
            if (tx,ty) in((8,14),(20,18)): col=4
        for sx in (0,1):
            for sy in (0,1):
                if ty+sy<MH and tx+sx<MW: ground[(ty+sy)*MW+tx+sx]=mt(col,row,(sx,sy))
objs=[]; oid=1
def obj(name,typ,x,y,w=0,h=0,props=None,point=False):
    global oid
    o={'id':oid,'name':name,'type':typ,'x':x,'y':y,'width':w,'height':h,'rotation':0,'visible':True}
    if point: o['point']=True
    if props: o['properties']=[{'name':k,'type':'string' if isinstance(v,str) else 'bool' if isinstance(v,bool) else 'float','value':v} for k,v in props.items()]
    objs.append(o); oid+=1
# facades: x = left edge, y = foot line
obj('residencial_A','facade',0,FOOT,160,182,{'frame':'fac_residencial','flipX':False})
obj('chigre','facade',162,FOOT,128,119,{'frame':'fac_chigre','flipX':False})
obj('residencial_A_mirror','facade',292,FOOT,160,182,{'frame':'fac_residencial','flipX':True,'note':'PROVISIONAL repetition until a second facade exists'})
# props (x,y = foot point)
P=[('barrel_table',200,252),('stool',214,258),('barrel_table',258,250),('stool',244,256),('fish_crates',112,226),('butane',150,212),
   ('vending',304,212),('bicycle',360,214),('hydrangea',40,214),('hydrangea',446,214),('bench',96,330),('lamp_post',232,330),('lamp_post',462,300),('hydrangea',300,340)]
for n,x,y in P: obj(n,'prop',x,y,props={'frame':n},point=True)
obj('wall_lantern','prop_wall',170,150,props={'frame':'wall_lantern'},point=True)
obj('wall_lantern','prop_wall',286,150,props={'frame':'wall_lantern'},point=True)
# lights
for (x,y,r) in ((196,172,46),(258,172,46),(226,180,30),(170,145,40),(286,145,40),(232,288,56),(462,258,50)):
    obj('light','light',x,y,props={'radius':float(r)},point=True)
coll=[]
def co(x,y,w,h):
    global oid
    coll.append({'id':oid,'name':'','type':'solid','x':x,'y':y,'width':w,'height':h,'rotation':0,'visible':True}); oid+=1
co(0,0,480,FOOT+4)
for n,x,y in P:
    w=FINAL['prop_'+n].shape[1]; fw=max(6,int(w*0.7)) if n!='lamp_post' else 6
    co(x-fw//2,y-6,fw,6)
obj('door_chigre','trigger',219,FOOT-4,20,10,{'target':'chigre_interior (phase 3, NOT built)'})
spawn={'id':oid,'name':'player_spawn','type':'spawn','x':226,'y':300,'width':0,'height':0,'point':True,'rotation':0,'visible':True}; oid+=1
tmap={'compressionlevel':-1,'height':MH,'width':MW,'infinite':False,'orientation':'orthogonal','renderorder':'right-down','tiledversion':'1.10.2','version':'1.10',
 'tilewidth':16,'tileheight':16,'type':'map','nextlayerid':5,'nextobjectid':oid,
 'properties':[{'name':'logical_resolution','type':'string','value':'480x270'},{'name':'foot_line_y','type':'int','value':FOOT}],
 'tilesets':[{'firstgid':1,'name':'pavement_16','image':'../tilesets/pavement_16.png','imagewidth':256,'imageheight':64,'tilewidth':16,'tileheight':16,'columns':16,'tilecount':64,'margin':0,'spacing':0}],
 'layers':[{'id':1,'name':'ground','type':'tilelayer','width':MW,'height':MH,'x':0,'y':0,'opacity':1,'visible':True,'data':ground},
  {'id':2,'name':'objects','type':'objectgroup','draworder':'topdown','x':0,'y':0,'opacity':1,'visible':True,'objects':objs+[spawn]},
  {'id':3,'name':'collision','type':'objectgroup','draworder':'topdown','x':0,'y':0,'opacity':1,'visible':False,'objects':coll}]}
json.dump(tmap,open(S('assets','maps','street_00.json'),'w'),indent=1)
MAN['assets'].append({'id':'map_street_00','kind':'tiled_map','file':'assets/maps/street_00.json','w':MW*16,'h':MH*16,'notes':'Tiled 1.10 JSON, embedded tileset; open directly in Tiled'})
json.dump(MAN,open(S('assets','manifest.json'),'w'),indent=1,ensure_ascii=False)
print('ok', len(MAN['assets']), 'assets; palette',NCOL)
