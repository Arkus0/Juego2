"""POC-2D-EASTWARD-00 art pipeline — ITERATION 2.
Sources: AI-generated original images (Grok Bot), source/*.jpg on magenta.
Formula: Eastward meets Shenmue meets Asturias.
Steps: IP repaint (brands/flags/crest/'1992' removed) -> adaptive magenta key + despill + erosion
-> premultiplied downscale -> halo cleanup -> shared 48-colour palette (42 median-cut + 6 reserved cool accents)
-> pavement: diamond lattice REBUILT from painted stones (perfect wrap) + curb strip rebuilt per band
-> facades (4) split roof/upper/base -> props -> decals -> tileset, atlas, Tiled map, manifest.
Run: python3 tools/pipeline.py    (iteration 1 kept in tools/pipeline_v1.py)
"""
from PIL import Image
import numpy as np, json, os, hashlib
from scipy import ndimage as nd
ROOT=os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
S=lambda *p: os.path.join(ROOT,*p)
PROV="AI image generation by Grok Bot (original, generated for this project; no game assets copied, traced or extracted)"
rng=np.random.default_rng(1992)

def load(name): return np.asarray(Image.open(S('source',name+'.jpg')).convert('RGB')).astype(np.float32)

# ---------------- IP repaint (done at source resolution, before keying) ----------------
def ip_repaint(a):
    a=a.copy()
    # vending header: '1992' + flag -> blank cream panel
    cream=np.median(a[20:30,500:540].reshape(-1,3),0)
    a[17:72,326:549]=cream+rng.normal(0,4,(55,223,1))
    a[17:19,326:549]=cream*0.75
    # vending cans: replace branded cans with generic unlabeled cans
    win=(346,86,479,246); dark=np.array([40,30,28])
    a[win[1]:win[3],win[0]:win[2]]=dark+rng.normal(0,3,(win[3]-win[1],win[2]-win[0],1))
    cols=[(150,60,50),(200,120,50),(80,120,60),(60,80,120)]
    k=0
    for cy in (116,196):
        for cx in (379,446):
            c=np.array(cols[k],float); k+=1
            for y in range(cy-28,cy+28):
                for x in range(cx-18,cx+18):
                    if (abs(x-cx)<16 or abs(y-cy)<24):
                        sh=1.15-0.45*((x-(cx-18))/36)
                        col=c*sh
                        if abs(y-cy)<5: col=np.array([225,215,195])*sh
                        if abs(y-cy)>=26: col=np.array([150,150,150])*sh
                        a[y,x]=col
            a[cy+30:cy+33,cx-30:cx+30]=np.array([90,80,70])
    # trash bin: municipal crest + text + '1992' -> plain green (column-wise shading copied from above)
    src=a[466:476,716:864].mean(0)
    a[478:655,716:864]=src[None]+rng.normal(0,3,(177,148,1))
    # butane plate 'BUTANO 1992' -> blank plate
    plate=np.median(a[152:156,128:190].reshape(-1,3),0)
    a[157:189,132:188]=plate*0.9+rng.normal(0,3,(32,56,1))
    return np.clip(a,0,255)

def key_out(rgb):
    k=np.median(np.concatenate([rgb[:8].reshape(-1,3),rgb[-8:].reshape(-1,3),rgb[:,:8].reshape(-1,3)]),0)
    d=np.abs(rgb-k).sum(-1)
    spill=np.clip(np.minimum(rgb[...,0],rgb[...,2])-rgb[...,1],0,None)
    bg=(d<140)|((spill>90)&(rgb[...,1]<80)&(rgb[...,0]>150))
    bg=nd.binary_opening(bg,iterations=1)
    fg=nd.binary_erosion(~bg,iterations=3)
    edge=nd.binary_dilation(~fg,iterations=8)&fg
    out=rgb.copy()
    sp=np.clip(np.minimum(out[...,0],out[...,2])-out[...,1]-10,0,None)
    for ch in (0,2): out[...,ch]=np.where(edge,out[...,ch]-sp,out[...,ch])
    return np.clip(out,0,255),fg.astype(np.float32)

def downscale(rgb,alpha,size):
    w,h=size
    pm=np.dstack([rgb*alpha[...,None],alpha*255]).astype(np.uint8)
    im=Image.fromarray(pm,'RGBA').resize((w*2,h*2),Image.LANCZOS).resize((w,h),Image.BOX)
    a=np.asarray(im).astype(np.float32); al=a[...,3]/255
    col=np.where(al[...,None]>0.01,a[...,:3]/np.maximum(al[...,None],1e-3),0)
    al=(al>0.5)
    # halo cleanup: boundary pixels that are still magenta-ish are dropped
    ring=al&~nd.binary_erosion(al)
    mag=(np.minimum(col[...,0],col[...,2])-col[...,1]>35)&(col[...,0]>col[...,1]+50)
    al=al&~(ring&mag)
    return np.clip(col,0,255),al.astype(np.float32)

PIECES={}
def add(n,r,a): PIECES[n]=(r,a)

# ---------------- PAVEMENT v2: lattice rebuild ----------------
pav=load('pavement_v2')
CB=[(73,64,333,330),(364,64,624,330),(654,64,915,330),(945,64,1207,330)]
M=32; NP=2; PF=M/NP   # 2 diamond periods per 32 px metatile (stone ~12 px across the diagonal)
PX,PY=69,60.5         # measured lattice periods (tools/seam_analysis.py)
def diamond_mask(P,shrink=0.80,jx=0.0,jy=0.0):
    # rounded diamond (superellipse in 45deg-rotated coords) -> cobble with rounded corners
    r=int(np.ceil(P/2)); yy,xx=np.mgrid[-r:r+1,-r:r+1].astype(float); xx-=jx; yy-=jy
    u,v=(xx+yy)/2,(xx-yy)/2
    return ((np.abs(u)**5+np.abs(v)**5)**(1/5))/(P/4)<shrink, r
# stone texture pool: downscaled painted tiles with the mortar inpainted by neighbouring stone pixels
texs=[]; mortar=[]
for (x0,y0,x1,y1) in CB:
    t=pav[y0+12:y1-12,x0+12:x1-12]; h,w,_=t.shape
    sw,sh=round(w*PF/PX*1.0),round(h*PF/PY*1.0)
    s=np.asarray(Image.fromarray(t.astype(np.uint8)).resize((sw*2,sh*2),Image.LANCZOS).resize((sw,sh),Image.BOX)).astype(np.float32)
    L=s.mean(-1); dark=L<np.percentile(L,40)
    mortar.append(s[L<np.percentile(L,10)])
    idx=nd.distance_transform_edt(dark,return_distances=False,return_indices=True)
    texs.append(s[idx[0],idx[1]])
mcol=np.median(np.concatenate(mortar),0)
print('texture pool',[t.shape for t in texs])
def build_cobble(seed):
    g=np.random.default_rng(seed)
    out=np.zeros((M,M,3),np.float32)+mcol*1.05+g.normal(0,4,(M,M,1))
    msk,r=diamond_mask(PF)
    er=nd.binary_erosion(msk)
    tl=msk&~np.roll(msk,(1,1),(0,1))   # top-left rim (lit)
    br=msk&~np.roll(msk,(-1,-1),(0,1)) # bottom-right rim (shadow)
    for j in range(NP):
        for i in range(NP):
            for (dx,dy) in ((0,0),(0.5,0.5)):
                msk,r=diamond_mask(PF,g.uniform(0.86,0.97),g.uniform(-.6,.6),g.uniform(-.6,.6))
                tl=msk&~np.roll(msk,(1,1),(0,1)); br=msk&~np.roll(msk,(-1,-1),(0,1))
                T=texs[g.integers(len(texs))]; oy=g.integers(0,T.shape[0]-2*r-1); ox=g.integers(0,T.shape[1]-2*r-1)
                patch=T[oy:oy+2*r+1,ox:ox+2*r+1]*g.uniform(0.9,1.08)
                # slight per-stone jitter of shape: random 1px nibble on corners
                cx,cy=int(round((i+dx)*PF)),int(round((j+dy)*PF))
                for yy in range(-r,r+1):
                    for xx in range(-r,r+1):
                        if not msk[yy+r,xx+r]: continue
                        c=patch[yy+r,xx+r]
                        if tl[yy+r,xx+r]: c=c*1.16
                        elif br[yy+r,xx+r]: c=c*0.72
                        out[(cy+yy)%M,(cx+xx)%M]=c
    return out
for k in range(4): add(f'pav_cobble_{k}',build_cobble(100+k),np.ones((M,M),np.float32))
# curb strip: bands rebuilt from segmented painted blocks so width 32 wraps exactly
cs=pav[368:576,510:1208]
SC=M/ (cs.shape[0]*1.0)    # strip height -> 32 px
def band(y0,y1,nseg,hout):
    b=cs[y0:y1]; L=nd.uniform_filter1d(b.mean(-1).mean(0),5)
    # joints = local minima of column luminance
    mins=[x for x in range(3,len(L)-3) if L[x]==L[x-3:x+4].min() and L[x]<np.percentile(L,30)]
    segs=[(mins[i],mins[i+1]) for i in range(len(mins)-1) if mins[i+1]-mins[i]>15]
    pick=[segs[i] for i in np.linspace(0,len(segs)-1,nseg).astype(int)]
    widths=[M//nseg+(1 if i<M%nseg else 0) for i in range(nseg)]
    parts=[np.asarray(Image.fromarray(b[:,a0:a1].astype(np.uint8)).resize((wd*2,hout*2),Image.LANCZOS).resize((wd,hout),Image.BOX)).astype(np.float32) for (a0,a1),wd in zip(pick,widths)]
    return np.concatenate(parts,1)
curb=np.concatenate([band(0,92,2,14),band(92,152,3,9),band(152,208,5,9)],0)
add('pav_curb_h',curb,np.ones((M,M),np.float32))
slab=pav[360:606,73:473]
sl=np.asarray(Image.fromarray(slab[6:-6,6:-6].astype(np.uint8)).resize((128,64),Image.LANCZOS).resize((64,32),Image.BOX)).astype(np.float32)
add('pav_slab',sl,np.ones(sl.shape[:2],np.float32))
# ---------------- decals ----------------
dec=key_out(load('decals'))
for n,(x0,y0,x1,y1),wt in (('decal_puddle',(190,112,794,598),56),('decal_moss_a',(829,115,1066,312),20),('decal_moss_b',(832,400,1072,598),20)):
    r,a=dec[0][y0:y1,x0:x1],dec[1][y0:y1,x0:x1]; ht=round(wt*(y1-y0)/(x1-x0)); r,a=downscale(r,a,(wt,ht))
    if n=='decal_puddle':  # source reflects a blue midday sky; regrade to the dusk scene (dark slate water, amber sky glints)
        L=(r@np.array([0.299,0.587,0.114]))/255
        water=np.array([46,50,62])*(1-L[...,None])+np.array([214,160,104])*L[...,None]**1.6
        rim=(np.abs(r[...,2]-r[...,0])<25)  # brown stone rim keeps its colour
        r=np.where(rim[...,None],r,water)
    add(n,r,a)
# ---------------- facades ----------------
FAC={}
def facade(key,src,box,size,sp):
    rgb,al=key_out(load(src)); x0,y0,x1,y1=box
    r,a=downscale(rgb[y0:y1,x0:x1],al[y0:y1,x0:x1],size); add('fac_'+key,r,a); FAC[key]=(src,sp)
facade('residencial','facade_res',(337,11,942,698),(160,182),{'upper':0.205,'base':0.655})
facade('chigre','chigre',(352,33,1006,642),(128,119),{'upper':0.17,'base':0.44})
facade('ultramarinos','facade2',(410,21,870,694),(147,216),{'upper':0.085,'base':0.60})
facade('backwall','backwall',(231,133,1049,586),(256,141),{'upper':0.13,'base':0.60})
# ---------------- props ----------------
p1=key_out(load('props')); p2=key_out(ip_repaint(load('props2')))
PROPS=[('barrel_table',p1,(100,7,268,246),28),('stool',p1,(410,63,552,215),15),('lamp_post',p1,(153,422,227,691),58),('wall_lantern',p1,(114,266,211,409),18),
('hydrangea',p1,(496,468,678,681),24),('butane',p2,(68,28,252,340),19),('vending',p2,(321,11,576,361),44),('bicycle',p2,(595,65,912,332),26),
('bench',p2,(915,73,1261,336),26),('trash_bin',p2,(696,358,892,680),26),('fish_boxes',p2,(6,359,327,672),26),('prensa_sign',p2,(980,360,1201,687),32),('cider_crate',p2,(346,404,639,671),18)]
for n,src,(x0,y0,x1,y1),hgt in PROPS:
    r,a=src[0][y0:y1,x0:x1],src[1][y0:y1,x0:x1]
    w=max(4,round((x1-x0)*hgt/(y1-y0))); r,a=downscale(r,a,(w,hgt)); add('prop_'+n,r,a)
# ---------------- palette ----------------
NCOL=48
allpx=np.concatenate([r[a>0.5] for r,a in PIECES.values()])
pal=np.array(Image.fromarray(allpx[None].astype(np.uint8)).quantize(colors=NCOL-9,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE).getpalette()[:(NCOL-6)*3]).reshape(-1,3)
acc=np.concatenate([PIECES[n][0][PIECES[n][1]>0.5] for n in ('prop_hydrangea',)]); acc=acc[(acc[:,2]-acc[:,0])>15]
pal=np.concatenate([pal,np.array(Image.fromarray(acc[None].astype(np.uint8)).quantize(colors=6,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE).getpalette()[:18]).reshape(-1,3)])
gm=np.concatenate([PIECES[n][0][PIECES[n][1]>0.5] for n in ('decal_moss_a','decal_moss_b')]); gm=gm[(gm[:,1]>gm[:,0]+8)&(gm[:,1]>gm[:,2]+15)]
pal=np.concatenate([pal[:-3] if False else pal,np.array(Image.fromarray(gm[None].astype(np.uint8)).quantize(colors=3,method=Image.Quantize.MEDIANCUT,dither=Image.Dither.NONE).getpalette()[:9]).reshape(-1,3)])
pal=pal[np.argsort(pal@[0.299,0.587,0.114])]
def quant(rgb):
    f=rgb.reshape(-1,3); idx=np.argmin(((f[:,None,:]-pal[None])**2).sum(-1),1); return pal[idx].reshape(rgb.shape).astype(np.uint8)
FINAL={n:np.dstack([quant(r),(a*255).astype(np.uint8)]) for n,(r,a) in PIECES.items()}
os.makedirs(S('assets','palette'),exist_ok=True)
pim=Image.new('RGB',(NCOL,1)); pim.putdata([tuple(int(v) for v in p) for p in pal]); pim.save(S('assets','palette','palette.png'))
pim.resize((NCOL*12,12),Image.NEAREST).save(S('assets','palette','palette_x12.png'))
with open(S('assets','palette','poc2d.gpl'),'w') as f:
    f.write('GIMP Palette\nName: POC2D-Eastward-00 iter2\nColumns: 12\n#\n')
    for p in pal: f.write('%3d %3d %3d #%02x%02x%02x\n'%(*p,*p))
# ---------------- write ----------------
import shutil
for d in ('tilesets','props','facades','atlas','maps','decals'):
    shutil.rmtree(S('assets',d),ignore_errors=True); os.makedirs(S('assets',d))
MAN={'project':'POC-2D-EASTWARD-00','iteration':2,'style_formula':'Eastward meets Shenmue meets Asturias','grid_tile':16,
 'palette':{'file':'assets/palette/palette.png','gpl':'assets/palette/poc2d.gpl','colors':NCOL,'composition':'39 median-cut + 6 reserved blues (hydrangea) + 3 reserved moss greens','hex':['#%02x%02x%02x'%tuple(p) for p in pal]},
 'provenance_default':PROV,'ip_cleanup':'props2: brand cans (Coca-Cola/Fanta/Sprite/other), Spanish flag, city crest and all "1992" stamps repainted to blank/generic shapes before processing (tools/pipeline.py:ip_repaint)',
 'pipeline':'tools/pipeline.py','assets':[]}
def save(arr,path,meta):
    Image.fromarray(arr).save(S(path)); meta.update({'file':path,'w':arr.shape[1],'h':arr.shape[0],'sha1':hashlib.sha1(open(S(path),'rb').read()).hexdigest()[:12]}); MAN['assets'].append(meta)
ts=np.zeros((32,256,4),np.uint8)
order=['pav_cobble_0','pav_cobble_1','pav_cobble_2','pav_cobble_3','pav_curb_h']
for i,n in enumerate(order): ts[:,i*32:i*32+32]=FINAL[n]
ts[:,160:224]=FINAL['pav_slab']
save(ts,'assets/tilesets/pavement_16.png',{'id':'tileset_pavement','kind':'tileset','tile':16,'metatile':32,'source':'source/pavement_v2.jpg',
 'layout':'cols(32px): cobble_0..3 (diamond lattice, rebuilt from painted stones, perfect wrap both axes), curb_h (slabs over 2 cobble rows, rebuilt per band, wraps in x), slab (64x32, NOT wrap-verified)',
 'reuse':'any Cimavilla-type street; mix 4 cobble variants freely; curb along facade feet'})
for n in order+['pav_slab']: save(FINAL[n],f'assets/tilesets/{n}.png',{'id':n,'kind':'metatile','source':'source/pavement_v2.jpg'})
for n in ('decal_puddle','decal_moss_a','decal_moss_b'): save(FINAL[n],f'assets/decals/{n}.png',{'id':n,'kind':'ground_decal','anchor':'center','source':'source/decals.jpg','layer':'ground decals (above tiles, below actors)'})
SRCF={'residencial':'facade_res','chigre':'chigre','ultramarinos':'facade2','backwall':'backwall'}
items=[]
for key,(src,sp) in FAC.items():
    full=FINAL['fac_'+key]; h=full.shape[0]; ru,rb=round(sp['upper']*h),round(sp['base']*h)
    save(full,f'assets/facades/{key}_full.png',{'id':f'fac_{key}_full','kind':'facade_macrosprite','anchor':'bottom-center','source':f'source/{src}.jpg',
      'notes':{'chigre':'left side wall + side roof hip CROPPED (single front-on grammar)','backwall':'background block, placed set back (foot 40 px above street foot line)'}.get(key,'')})
    for pn,(a0,a1) in {'roof':(0,ru),'upper':(ru,rb),'base':(rb,h)}.items():
        save(full[a0:a1],f'assets/facades/{key}_{pn}.png',{'id':f'fac_{key}_{pn}','kind':'facade_piece','offset_y_from_top':a0,'layer':'occlusion' if pn=='roof' else 'world','source':f'source/{src}.jpg'})
        items.append((f'fac_{key}_{pn}',full[a0:a1]))
for n,_,_,hgt in PROPS:
    save(FINAL['prop_'+n],f'assets/props/{n}.png',{'id':n,'kind':'prop','anchor':'bottom-center','target_height_px':hgt,'source':'source/props2.jpg' if n in('butane','vending','bicycle','bench','trash_bin','fish_boxes','prensa_sign','cider_crate') else 'source/props.jpg',
      'collision':'none (wall-mounted)' if n=='wall_lantern' else 'footprint ~70% width x 6 px'})
    items.append((n,FINAL['prop_'+n]))
for n in ('decal_puddle','decal_moss_a','decal_moss_b'): items.append((n,FINAL[n]))
AW=512; x=y=rowh=0; frames={}; atlas=np.zeros((2048,AW,4),np.uint8)
for n,arr in sorted(items,key=lambda t:-t[1].shape[0]):
    h,w=arr.shape[:2]
    if x+w>AW: x=0; y+=rowh+2; rowh=0
    atlas[y:y+h,x:x+w]=arr; frames[n]={'frame':{'x':x,'y':y,'w':w,'h':h},'rotated':False,'trimmed':False,'spriteSourceSize':{'x':0,'y':0,'w':w,'h':h},'sourceSize':{'w':w,'h':h},'anchor':{'x':0.5,'y':1.0}}
    x+=w+2; rowh=max(rowh,h)
AH=1<<int(np.ceil(np.log2(y+rowh+1))); atlas=atlas[:AH]
save(atlas,'assets/atlas/world_atlas.png',{'id':'atlas_world','kind':'atlas','format':'JSON hash (TexturePacker/Phaser compatible)'})
json.dump({'frames':frames,'meta':{'image':'world_atlas.png','size':{'w':AW,'h':AH},'scale':'1','app':'tools/pipeline.py'}},open(S('assets','atlas','world_atlas.json'),'w'),indent=1)
# ---------------- Tiled map ----------------
MW,MH=30,26; FOOT=200
def gid(col,sx,sy): return 1+sy*16+col*2+sx
ground=[0]*(MW*MH)
for ty in range(0,MH,2):
    for tx in range(0,MW,2):
        y=ty*16
        if y+32<=FOOT: col=None                              # empty above the foot line: sky band shows where no building
        elif y<FOOT: col=5 if (tx//2)%2==0 else 6   # slab sidewalk at the facade feet
        elif y<FOOT+32: col=4                            # curb strip
        else: col=int(rng.integers(4))
        if col is None: continue
        for sx in (0,1):
            for sy in (0,1):
                if ty+sy<MH and tx+sx<MW: ground[(ty+sy)*MW+tx+sx]=gid(col,sx,sy)
objs=[]; oid=1
def obj(name,typ,x,y,w=0,h=0,props=None,point=False):
    global oid
    o={'id':oid,'name':name,'type':typ,'x':x,'y':y,'width':w,'height':h,'rotation':0,'visible':True}
    if point: o['point']=True
    if props: o['properties']=[{'name':k,'type':'string' if isinstance(v,str) else 'bool' if isinstance(v,bool) else 'float','value':v} for k,v in props.items()]
    objs.append(o); oid+=1
# facades (x = left edge, y = foot). backwall set back behind chigre; second backwall closes the east end
obj('backwall_behind_chigre','facade',96,FOOT-40,256,141,{'frame':'fac_backwall','flipX':False,'depthBias':-250.0})
obj('backwall_east','facade',434,FOOT,256,141,{'frame':'fac_backwall','flipX':True,'depthBias':-1.0})
obj('residencial','facade',0,FOOT,160,182,{'frame':'fac_residencial','flipX':False})
obj('chigre','facade',160,FOOT,128,119,{'frame':'fac_chigre','flipX':False})
obj('ultramarinos','facade',288,FOOT,147,216,{'frame':'fac_ultramarinos','flipX':False})
P=[('barrel_table',198,254),('stool',214,262),('barrel_table',258,252),('stool',242,260),('fish_boxes',112,230),('butane',148,214),('cider_crate',178,214),
   ('vending',448,222),('bicycle',404,216),('trash_bin',22,226),('prensa_sign',330,224),('hydrangea',60,214),('hydrangea',282,214),
   ('bench',96,334),('lamp_post',232,330),('lamp_post',462,300),('hydrangea',300,342)]
for n,x,y in P: obj(n,'prop',x,y,props={'frame':n},point=True)
obj('wall_lantern','prop_wall',168,150,props={'frame':'wall_lantern'},point=True)
obj('wall_lantern','prop_wall',282,150,props={'frame':'wall_lantern'},point=True)
for n,x,y in (('decal_puddle',330,300),('decal_puddle',120,380),('decal_moss_a',40,262),('decal_moss_b',470,250),('decal_moss_a',300,402)):
    obj(n,'decal',x,y,props={'frame':n},point=True)
for (x,y,r) in ((196,172,46),(258,172,46),(224,180,30),(168,146,40),(282,146,40),(232,268,56),(462,238,50)):
    obj('light','light',x,y,props={'radius':float(r)},point=True)
coll=[]
def co(x,y,w,h):
    global oid
    coll.append({'id':oid,'name':'','type':'solid','x':x,'y':y,'width':w,'height':h,'rotation':0,'visible':True}); oid+=1
co(0,0,480,FOOT+4)
for n,x,y in P:
    w=FINAL['prop_'+n].shape[1]; fw=6 if n=='lamp_post' else max(6,int(w*0.7)); co(x-fw//2,y-6,fw,6)
obj('door_chigre','trigger',214,FOOT-4,20,10,{'target':'chigre_interior (phase 3, NOT built)'})
obj('player_spawn','spawn',226,300,point=True)
tmap={'compressionlevel':-1,'height':MH,'width':MW,'infinite':False,'orientation':'orthogonal','renderorder':'right-down','tiledversion':'1.10.2','version':'1.10',
 'tilewidth':16,'tileheight':16,'type':'map','nextlayerid':4,'nextobjectid':oid,
 'properties':[{'name':'logical_resolution','type':'string','value':'480x270'},{'name':'foot_line_y','type':'int','value':FOOT}],
 'tilesets':[{'firstgid':1,'name':'pavement_16','image':'../tilesets/pavement_16.png','imagewidth':256,'imageheight':32,'tilewidth':16,'tileheight':16,'columns':16,'tilecount':32,'margin':0,'spacing':0}],
 'layers':[{'id':1,'name':'ground','type':'tilelayer','width':MW,'height':MH,'x':0,'y':0,'opacity':1,'visible':True,'data':ground},
  {'id':2,'name':'objects','type':'objectgroup','draworder':'topdown','x':0,'y':0,'opacity':1,'visible':True,'objects':objs},
  {'id':3,'name':'collision','type':'objectgroup','draworder':'topdown','x':0,'y':0,'opacity':1,'visible':False,'objects':coll}]}
json.dump(tmap,open(S('assets','maps','street_00.json'),'w'),indent=1)
MAN['assets'].append({'id':'map_street_00','kind':'tiled_map','file':'assets/maps/street_00.json','w':MW*16,'h':MH*16})
json.dump(MAN,open(S('assets','manifest.json'),'w'),indent=1,ensure_ascii=False)
print('ok',len(MAN['assets']))
