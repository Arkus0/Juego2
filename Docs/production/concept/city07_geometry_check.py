# Planning-geometry validation for CITY P8 lanes and P9 frontage regions (seed U/V metres).
# Run: python3 city07_geometry_check.py  (requires shapely)
from shapely.geometry import Polygon, LineString, Point
from shapely.ops import unary_union
H=Polygon([(-20,-60),(70,-80),(205,-48),(245,22),(220,95),(150,145),(70,132),(-20,55)])
RIO=Polygon([(-20,-8),(0,-10),(20,-24),(40,-42),(60,-46),(95,-45),(140,-42),(185,-40),(205,-38),(205,-48),(185,-52),(140,-57),(95,-61),(60,-62),(40,-58),(20,-43),(0,-30),(-20,-24)])
ARR=Polygon([(-20,-8),(0,10),(20,28),(40,48),(60,68),(78,90),(88,111),(108,132),(120,140.125),(100,136.875),(99,130),(90,118),(82,106),(70,86),(50,60),(30,40),(8,20),(-20,4)])
X5=Polygon([(90.5,107.8),(88.5,121.8),(91.5,122.2),(93.5,108.2)])
ST=[Polygon(p) for p in ([(84,99),(99,99),(96,108),(87,108)],[(86,119),(93,124),(94,130),(84,128)])]
nb=unary_union([RIO.buffer(3),ARR.buffer(2)]).difference(unary_union([X5]+ST))
R=lambda a,b,c,d:Polygon([(a,c),(b,c),(b,d),(a,d)])
REG={'F01':R(88,96,45,60),'F02':R(148,166,66,88),'F03':R(188,198,76,94),'F04':R(62,69,40,54),'F05':R(98,105,24,36),'F06':R(126,138,62,78),'F07':R(170,178,74,90),'F08':R(50,57,3,16),'S01':Polygon([(132,35),(150,40),(148,52),(130,47)]),'S03':R(70,83,55,68)}
L={
'W18 (P8a-1)':([(104,41),(110,22)],3.0),
'W19 (P8a-2)':([(110,22),(128,6),(155,8)],3.0),
'W20 (P8a-3)':([(155,8),(172,28),(172,45),(168,57)],3.0),
'W21 (P8b)':([(68,8),(88,12),(110,22)],2.8),
'W22 (P8c-1)':([(168,65),(168,93),(160,110),(150,114),(128,106)],2.6),
'W23 (P8c-2)':([(128,106),(110,96),(96,84),(88,79)],2.8),
'W24 (P8d)':([(88,79),(104,72),(116,62),(121,48.5)],2.6),
'W25 (P8f)':([(110,22),(125,28),(140,24),(155,8)],1.8),
'W26 (P8g)':([(128,106),(114,116),(100,108),(92,108)],1.8),
'E06 (P8e)':([(90,122),(84,132),(60,112),(35,88),(12,64)],3.4),
}
for k,(pts,w) in L.items():
  ls=LineString(pts);b=ls.buffer(w/2,cap_style=2)
  issues=[]
  if not H.contains(b): issues.append('outside hard')
  wi=b.intersection(unary_union([RIO,ARR])).area
  if wi>0.01: issues.append(f'water {wi:.1f}')
  bi=b.intersection(nb).area
  if bi>0.05: issues.append(f'bank {bi:.1f}')
  for n,r in REG.items():
    a=b.intersection(r).area
    if a>0.05: issues.append(f'{n} {a:.1f}')
  print(f'{k}: len={ls.length:.1f} m  {"OK" if not issues else issues}')
W={'W05':[(96,40),(104,41),(118,47),(121,48.5),(132,54),(150,58)],'W12':[(80,35),(74,22),(68,8),(62,-6),(56,-18),(50,-32)],'W13':[(80,35),(84,44),(86,58),(87,72),(88,79),(89,86),(90,97),(92,108)]}
from shapely.ops import substring
def split(pts,at):
  ls=LineString(pts);ds=[ls.project(Point(p)) for p in at];return ls.length,ds
for k,at in (('W05',[(104,41),(121,48.5)]),('W12',[(68,8)]),('W13',[(88,79)])):
  Lt,ds=split(W[k],at);print(k,round(Lt,1),[round(d,1) for d in ds])
print('--- fixed W18')
L['W18 (P8a-1)']=([(104,41),(108,38),(110,22)],3.0)
ls=LineString(L['W18 (P8a-1)'][0]);b=ls.buffer(1.5,cap_style=2);print(round(ls.length,1),[n for n,r in REG.items() if b.intersection(r).area>0.05],b.intersection(nb).area)
ACC={'W04':([(150,58),(162,61),(175,66),(186,68),(195,70)],5),'W05':(W['W05'],3.6),'W06':([(80,35),(70,30),(60,25),(46,22),(32,15),(16,7),(0,0)],3.4),'W12':(W['W12'],3.8),'W13':(W['W13'],3),'mA':([(80,35),(84,28),(90,27),(95,31),(96,40)],2.8),'mB':([(80,35),(83,41),(90,43),(96,40)],2.8)}
ALL=dict(ACC);ALL.update(L)
roads=unary_union([LineString(p).buffer(w/2+0.3,cap_style=2) for p,w in ALL.values()])
PLAZA=Polygon([(136,50),(150,53),(168,57),(168,65),(148,65),(136,61)]);CSQ=Polygon([(73,30),(84,28),(88,36),(83,41),(75,39)])
import math
def ring(c,r,n=9):
  return Polygon([(c[0]+math.cos(i/n*2*math.pi)*r*(.85+.3*((i*7919)%5)/5),c[1]+math.sin(i/n*2*math.pi)*r*(.85+.3*((i*7919)%5)/5)*1.1) for i in range(n)])
POCK=[ring((114,19),6.5),ring((171,27),6),ring((150,113),9,11),ring((115,118),4.2)]
block=unary_union([roads,nb,PLAZA,CSQ]+list(REG.values())+POCK+[Polygon(s) for s in ([(42,-40),(58,-40),(58,-22),(42,-22)],)])
P9=[('F09','taberna','W12',(58,-13),8,10),('F10','barberia','W12',(70,14),6,9),('F11','orujeria','W06',(54,23.5),8,10),
('F12','horno','W21 (P8b)',(98,17),7,9),('F13','queseria','W05',(112,44.5),7,9),('F14','ferreteria','W05',(127,51.5),7,9),
('F15','cafe_billar','W13',(87,66),8,10),('F16','fonda','W20 (P8a-3)',(172,38),9,12),('F17','herrero','W19 (P8a-2)',(140,7),9,11),
('F18','estanco','W22 (P8c-1)',(168,100),6,9.5),('F19','ultramarinos','E06 (P8e)',(45,98),9,16)]
out=[]
for fid,name,rt,near,w,d in P9:
  pts,rw=ALL[rt];ls=LineString(pts);s=ls.project(Point(near));
  p0=ls.interpolate(max(0,s-0.5));p1=ls.interpolate(min(ls.length,s+0.5));dx,dy=p1.x-p0.x,p1.y-p0.y;n=math.hypot(dx,dy);dx/=n;dy/=n;nx,ny=-dy,dx
  c=ls.interpolate(s);best=None
  for side in (1,-1):
    for extra in (0.4,0.8,1.2,1.6):
      off=rw/2+0.3+extra+d/2;cx,cy=c.x+nx*off*side,c.y+ny*off*side
      corners=[(cx+dx*w/2+nx*d/2,cy+dy*w/2+ny*d/2),(cx-dx*w/2+nx*d/2,cy-dy*w/2+ny*d/2),(cx-dx*w/2-nx*d/2,cy-dy*w/2-ny*d/2),(cx+dx*w/2-nx*d/2,cy+dy*w/2-ny*d/2)]
      corners=[(round(x*2)/2,round(y*2)/2) for x,y in corners];pg=Polygon(corners)
      if H.contains(pg) and pg.intersection(block).area<0.05:
        best=(side,corners,pg.distance(LineString(pts)));block=unary_union([block,pg]);break
    if best:break
  print(fid,name,rt,best if best else 'NO FIT')
