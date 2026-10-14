# Objective tileability check for pavement_v2 cobble tiles: find lattice period by autocorrelation,
# then search crop (offset,size=k*period) minimising wrap error; compare with natural neighbour-column difference.
from PIL import Image; import numpy as np, json, sys
a=np.asarray(Image.open('source/pavement_v2.jpg').convert('L')).astype(float)
boxes=[(73,64,333,330),(364,64,624,330),(654,64,915,330),(945,64,1207,330)]
res=[]
for i,(x0,y0,x1,y1) in enumerate(boxes):
    t=a[y0+12:y1-12,x0+12:x1-12]; t=t-t.mean()
    F=np.fft.fft2(t); ac=np.real(np.fft.ifft2(F*np.conj(F))); ac/=ac[0,0]
    h,w=t.shape
    px=int(np.argmax(ac[0,20:w//2]))+20; py=int(np.argmax(ac[20:h//2,0]))+20
    natural=np.mean(np.abs(np.diff(t,axis=1)))
    best=None
    for k in (3,4,5):
        for N in range(k*px-4,k*px+5):
            for ox in range(0,max(1,w-N),2):
                if ox+N+1>w: break
                ex=np.mean(np.abs(t[:,ox]-t[:,ox+N]))
                if best is None or ex<best[0]: best=(ex,'x',k,N,ox)
    bx=best; best=None
    for k in (3,4,5):
        for N in range(k*py-4,k*py+5):
            for oy in range(0,max(1,h-N),2):
                if oy+N+1>h: break
                ey=np.mean(np.abs(t[oy]-t[oy+N]))
                if best is None or ey<best[0]: best=(ey,'y',k,N,oy)
    by=best
    res.append({'tile':i,'period_x':px,'period_y':py,'ac_peak_x':round(float(ac[0,px]),3),'ac_peak_y':round(float(ac[py,0]),3),
      'natural_col_diff':round(natural,1),'best_wrap_x':{'err':round(bx[0],1),'periods':bx[2],'size':bx[3],'off':bx[4]},
      'best_wrap_y':{'err':round(by[0],1),'periods':by[2],'size':by[3],'off':by[4]},
      'ratio_x':round(bx[0]/natural,2),'ratio_y':round(by[0]/natural,2)})
json.dump(res,open('evidence/seam_analysis_v2.json','w'),indent=1); print(json.dumps(res,indent=1))
