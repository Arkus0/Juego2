# Evidence sheets: isolated sprites (1x & 4x, no filters), atlas, 3x3 tiling checks, palette
from PIL import Image; import os, json, random
R=os.path.dirname(os.path.dirname(os.path.abspath(__file__))); E=os.path.join(R,'evidence'); os.makedirs(E,exist_ok=True)
BG=(48,44,52)
def up(im,k): return im.resize((im.width*k,im.height*k),Image.NEAREST)
at=Image.open(f'{R}/assets/atlas/world_atlas.png'); bg=Image.new('RGBA',at.size,BG+(255,)); bg.alpha_composite(at)
bg.save(f'{E}/atlas_1x.png'); up(bg,2).save(f'{E}/atlas_x2.png')
# isolated sprites row at 1x with scale ruler (30px provisional human)
files=sorted(os.listdir(f'{R}/assets/props'))
ims=[Image.open(f'{R}/assets/props/{f}') for f in files]+[Image.open(f'{R}/assets/decals/{f}') for f in sorted(os.listdir(f'{R}/assets/decals'))]+[Image.open(f'{R}/assets/facades/{f}') for f in ('residencial_full.png','chigre_full.png','ultramarinos_full.png','backwall_full.png')]
W=sum(i.width for i in ims)+6*len(ims)+30; H=max(i.height for i in ims)+8
sh=Image.new('RGBA',(W,H),BG+(255,)); x=4
ruler=Image.new('RGBA',(10,30),(200,200,210,255)); sh.alpha_composite(ruler,(x,H-4-30)); x+=18
for i in ims: sh.alpha_composite(i,(x,H-4-i.height)); x+=i.width+6
sh.save(f'{E}/isolated_sprites_1x.png'); up(sh,3).save(f'{E}/isolated_sprites_x3.png')
# 3x3 tiling for every metatile (iteration 2 layout)
ts=Image.open(f'{R}/assets/tilesets/pavement_16.png')
tiles=[(f'cobble_{k}',(k*32,0,k*32+32,32)) for k in range(4)]+[('curb_h',(128,0,160,32)),('slab',(160,0,224,32))]
sheet=Image.new('RGB',(sum((b[2]-b[0])*3+8 for _,b in tiles)+4,96+8),BG); x=4
for n,b in tiles:
    t=ts.crop(b); w=b[2]-b[0]
    for i in range(3):
        for j in range(3): sheet.paste(t,(x+i*w,4+j*32))
    x+=w*3+8
sheet.save(f'{E}/tiling_3x3_1x.png'); up(sheet,3).save(f'{E}/tiling_3x3_x3.png')
random.seed(7); mix=Image.new('RGB',(12*32,8*32))
for i in range(12):
    for j in range(8):
        cn=random.randrange(4); mix.paste(ts.crop((cn*32,0,cn*32+32,32)),(i*32,j*32))
mix.save(f'{E}/tiling_mix_1x.png'); up(mix,3).save(f'{E}/tiling_mix_x3.png')
print('ok')
