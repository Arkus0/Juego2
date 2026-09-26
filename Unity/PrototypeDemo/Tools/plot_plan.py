"""Quick plan plot of the generated layout (debug aid)."""
import re, sys
from PIL import Image, ImageDraw
S = 4  # px per metre
U0, V0, U1, V1 = -30, -90, 255, 150
img = Image.new("RGB", (int((U1-U0)*S), int((V1-V0)*S)), (235, 235, 228))
d = ImageDraw.Draw(img)
def X(p): return ((p[0]-U0)*S, (V1-p[1])*S)
src = open("Assets/_Prototype/Editor/Build/Seed.cs", encoding="utf-8").read()
def pts(name):
    m = re.search(name + r" = Pts\(([^;]*)\);", src)
    nums = [float(x.strip().rstrip("f")) for x in m.group(1).split(",")]
    return [(nums[i], nums[i+1]) for i in range(0, len(nums), 2)]
d.polygon([X(p) for p in pts("Hard")], outline=(0,0,0))
for n in ("Rio", "Arroyo"): d.polygon([X(p) for p in pts(n)], fill=(120,160,170))
for m in re.finditer(r'new Route\("(\w+)", "[^"]*", ([\d.]+)f, Surf\.\w+, new\[\] \{[^}]*\}, ([^)]*)\)', src):
    nums = [float(x.strip().rstrip("f")) for x in m.group(3).split(",")]
    P = [(nums[i], nums[i+1]) for i in range(0, len(nums), 2)]
    d.line([X(p) for p in P], fill=(150,120,90), width=int(float(m.group(2))*S))
for line in open("Captures/Debug/houses.tsv", encoding="utf-8"):
    f = line.rstrip("\n").split("\t")
    poly = [tuple(map(float, q.split(","))) for q in f[2].split()]
    col = (90,160,90) if f[1] == "Garden" else (200,80,60) if f[1] != "Filler" else (60,60,60)
    d.polygon([X(p) for p in poly], outline=col, fill=None if f[1]=="Garden" else col)
img.save("Captures/Debug/plan.png")
print("ok", img.size)
