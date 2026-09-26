"""Generate the prototype's sign textures (own artwork, versioned).

    python Unity/PrototypeDemo/Tools/make_signs.py

Styles: painted wooden boards (hanging + facade), enamel street plaques, engraved stone plaque,
chalk blackboard and paper posters. Output: Assets/_Prototype/Signs/<id>.png (+ signs.json with sizes).
Deterministic (fixed seeds). Fonts come from C:/Windows/Fonts.
"""
from __future__ import annotations

import json
import math
import random
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont

OUT = Path(__file__).resolve().parents[1] / "Assets" / "_Prototype" / "Signs"
FONTS = Path("C:/Windows/Fonts")


def font(name: str, size: int) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype(str(FONTS / name), size)


def hexrgb(h: str):
    h = h.lstrip("#")
    return tuple(int(h[i:i + 2], 16) for i in (0, 2, 4))


def noise_layer(w, h, seed, scale=1.0, amount=18):
    rnd = random.Random(seed)
    small = Image.new("L", (max(2, int(w / 8 * scale)), max(2, int(h / 8 * scale))))
    small.putdata([128 + rnd.randint(-amount, amount) for _ in range(small.width * small.height)])
    return small.resize((w, h), Image.BICUBIC).filter(ImageFilter.GaussianBlur(2))


def wood(w, h, base, seed, grain_vertical=False):
    """Painted-over plank texture: base colour + grain + plank seams + noise."""
    rnd = random.Random(seed)
    img = Image.new("RGB", (w, h), base)
    d = ImageDraw.Draw(img)
    planks = max(2, h // 90) if not grain_vertical else max(2, w // 90)
    for i in range(1, planks):
        if grain_vertical:
            x = int(i * w / planks) + rnd.randint(-4, 4)
            d.line([(x, 0), (x, h)], fill=tuple(max(0, c - 38) for c in base), width=3)
        else:
            y = int(i * h / planks) + rnd.randint(-3, 3)
            d.line([(0, y), (w, y)], fill=tuple(max(0, c - 38) for c in base), width=3)
    for _ in range((w * h) // 900):
        if grain_vertical:
            x = rnd.randint(0, w); y0 = rnd.randint(0, h); y1 = y0 + rnd.randint(20, 160)
            d.line([(x, y0), (x + rnd.randint(-2, 2), y1)], fill=tuple(max(0, c - rnd.randint(8, 22)) for c in base), width=1)
        else:
            y = rnd.randint(0, h); x0 = rnd.randint(0, w); x1 = x0 + rnd.randint(30, 220)
            d.line([(x0, y), (x1, y + rnd.randint(-2, 2))], fill=tuple(max(0, c - rnd.randint(8, 22)) for c in base), width=1)
    n = noise_layer(w, h, seed + 1)
    img = Image.composite(img, Image.new("RGB", (w, h), tuple(min(255, c + 12) for c in base)), n.point(lambda v: 255 if v > 118 else 200))
    return img


def weather(img, seed, strength=0.18):
    """Darken edges, add scuffs: paint that has lived outdoors in a humid valley."""
    w, h = img.size
    rnd = random.Random(seed)
    vign = Image.new("L", (w, h), 0)
    d = ImageDraw.Draw(vign)
    for i in range(24):
        d.rectangle([i, i, w - 1 - i, h - 1 - i], outline=int(255 * (1 - i / 24) * strength * 2.2))
    vign = vign.filter(ImageFilter.GaussianBlur(8))
    dark = Image.new("RGB", (w, h), (20, 16, 12))
    img = Image.composite(dark, img, vign)
    d = ImageDraw.Draw(img)
    for _ in range(int(w * h / 12000)):
        x, y = rnd.randint(0, w), rnd.randint(0, h)
        r = rnd.randint(1, 4)
        c = img.getpixel((min(w - 1, x), min(h - 1, y)))
        d.ellipse([x - r, y - r, x + r, y + r], fill=tuple(max(0, v - 30) for v in c))
    return img


def centered(draw, box, text, fnt, fill, shadow=None, spacing=0.0):
    x0, y0, x1, y1 = box
    if spacing:
        widths = [draw.textlength(ch, font=fnt) for ch in text]
        total = sum(widths) + spacing * (len(text) - 1)
        asc, desc = fnt.getmetrics()
        x = x0 + (x1 - x0 - total) / 2
        y = y0 + (y1 - y0 - (asc + desc)) / 2
        for ch, cw in zip(text, widths):
            if shadow: draw.text((x + 3, y + 3), ch, font=fnt, fill=shadow)
            draw.text((x, y), ch, font=fnt, fill=fill)
            x += cw + spacing
        return
    bb = draw.textbbox((0, 0), text, font=fnt)
    tw, th = bb[2] - bb[0], bb[3] - bb[1]
    x = x0 + (x1 - x0 - tw) / 2 - bb[0]
    y = y0 + (y1 - y0 - th) / 2 - bb[1]
    if shadow: draw.text((x + 3, y + 3), text, font=fnt, fill=shadow)
    draw.text((x, y), text, font=fnt, fill=fill)


def fit_font(draw, text, name, max_w, max_h, start=200):
    size = start
    while size > 10:
        f = font(name, size)
        bb = draw.textbbox((0, 0), text, font=f)
        if bb[2] - bb[0] <= max_w and bb[3] - bb[1] <= max_h:
            return f
        size -= 4
    return font(name, 10)


# ---------------------------------------------------------------- pictograms (simple, readable at 15 m)

def icon(draw, kind, cx, cy, s, col):
    t = max(3, int(s * 0.07))
    if kind == "jug":
        draw.rounded_rectangle([cx - s * .32, cy - s * .35, cx + s * .28, cy + s * .45], radius=int(s * .12), outline=col, width=t)
        draw.arc([cx + s * .12, cy - s * .2, cx + s * .52, cy + s * .25], 270, 90, fill=col, width=t)
        draw.line([cx - s * .32, cy - s * .18, cx + s * .28, cy - s * .18], fill=col, width=t)
    elif kind == "scissors":
        draw.ellipse([cx - s * .5, cy + s * .1, cx - s * .12, cy + s * .48], outline=col, width=t)
        draw.ellipse([cx + s * .12, cy + s * .1, cx + s * .5, cy + s * .48], outline=col, width=t)
        draw.line([cx - s * .2, cy + s * .15, cx + s * .25, cy - s * .5], fill=col, width=t)
        draw.line([cx + s * .2, cy + s * .15, cx - s * .25, cy - s * .5], fill=col, width=t)
    elif kind == "bottle":
        draw.rectangle([cx - s * .08, cy - s * .5, cx + s * .08, cy - s * .25], outline=col, width=t)
        draw.rounded_rectangle([cx - s * .22, cy - s * .25, cx + s * .22, cy + s * .48], radius=int(s * .1), outline=col, width=t)
        draw.line([cx - s * .22, cy + s * .05, cx + s * .22, cy + s * .05], fill=col, width=t)
    elif kind == "bread":
        draw.ellipse([cx - s * .5, cy - s * .25, cx + s * .5, cy + s * .3], outline=col, width=t)
        for k in (-1, 0, 1):
            draw.line([cx + k * s * .22 - s * .08, cy - s * .12, cx + k * s * .22 + s * .08, cy + s * .12], fill=col, width=t)
    elif kind == "cheese":
        draw.ellipse([cx - s * .5, cy - s * .05, cx + s * .5, cy + s * .35], outline=col, width=t)
        draw.line([cx - s * .5, cy + s * .15, cx - s * .5, cy - s * .22], fill=col, width=t)
        draw.line([cx + s * .5, cy + s * .15, cx + s * .5, cy - s * .22], fill=col, width=t)
        draw.ellipse([cx - s * .5, cy - s * .42, cx + s * .5, cy - s * .02], outline=col, width=t)
    elif kind == "key":
        draw.ellipse([cx - s * .5, cy - s * .2, cx - s * .12, cy + s * .18], outline=col, width=t)
        draw.line([cx - s * .12, cy, cx + s * .5, cy], fill=col, width=t)
        draw.line([cx + s * .3, cy, cx + s * .3, cy + s * .18], fill=col, width=t)
        draw.line([cx + s * .45, cy, cx + s * .45, cy + s * .22], fill=col, width=t)
    elif kind == "billiard":
        for k, (dx, dy) in enumerate([(-.2, .2), (.2, .2), (0, -.12)]):
            draw.ellipse([cx + dx * s - s * .18, cy + dy * s - s * .18, cx + dx * s + s * .18, cy + dy * s + s * .18], outline=col, width=t)
        draw.line([cx - s * .55, cy + s * .5, cx + s * .5, cy - s * .5], fill=col, width=t)
    elif kind == "plate":
        draw.ellipse([cx - s * .42, cy - s * .42, cx + s * .42, cy + s * .42], outline=col, width=t)
        draw.ellipse([cx - s * .25, cy - s * .25, cx + s * .25, cy + s * .25], outline=col, width=max(2, t // 2))
        draw.line([cx - s * .6, cy - s * .35, cx - s * .6, cy + s * .45], fill=col, width=t)
        draw.line([cx + s * .6, cy - s * .35, cx + s * .6, cy + s * .45], fill=col, width=t)
    elif kind == "horseshoe":
        draw.arc([cx - s * .42, cy - s * .45, cx + s * .42, cy + s * .4], 150, 30, fill=col, width=int(t * 1.6))
    elif kind == "basket":
        draw.polygon([(cx - s * .45, cy - s * .05), (cx + s * .45, cy - s * .05), (cx + s * .32, cy + s * .42), (cx - s * .32, cy + s * .42)], outline=col, width=t)
        draw.arc([cx - s * .32, cy - s * .5, cx + s * .32, cy + s * .2], 180, 360, fill=col, width=t)
        for k in (-1, 0, 1):
            draw.line([cx + k * s * .15, cy - s * .05, cx + k * s * .12, cy + s * .42], fill=col, width=max(2, t // 2))
    elif kind == "mug":
        draw.rounded_rectangle([cx - s * .3, cy - s * .38, cx + s * .2, cy + s * .42], radius=int(s * .06), outline=col, width=t)
        draw.arc([cx + s * .05, cy - s * .2, cx + s * .45, cy + s * .22], 270, 90, fill=col, width=t)
        draw.arc([cx - s * .38, cy - s * .62, cx + s * .28, cy - s * .2], 200, 340, fill=col, width=t)


# ---------------------------------------------------------------- styles

def painted_board(id_, title, sub, bg, fg, icon_kind, w=1024, h=384, seed=1, shaped=False, font_name="ROCKB.TTF"):
    img = wood(w, h, hexrgb(bg), seed)
    d = ImageDraw.Draw(img)
    fgc = hexrgb(fg)
    frame = tuple(max(0, c - 55) for c in hexrgb(bg))
    b = int(h * 0.07)
    d.rectangle([0, 0, w - 1, h - 1], outline=frame, width=b)
    d.rectangle([b + 10, b + 10, w - b - 11, h - b - 11], outline=fgc, width=4)
    d.rectangle([b + 20, b + 20, w - b - 21, h - b - 21], outline=fgc, width=1)
    x0 = b + 30
    if icon_kind:
        s = h * 0.46
        icon(d, icon_kind, x0 + s * 0.62, h / 2, s, fgc)
        x0 += int(s * 1.25)
        d.line([x0, b + 40, x0, h - b - 40], fill=fgc, width=2)
        x0 += 16
    tb = [x0, b + 26, w - b - 30, h - b - (26 if not sub else int(h * 0.30))]
    f = fit_font(d, title, font_name, tb[2] - tb[0] - 20, tb[3] - tb[1] - 6)
    centered(d, tb, title, f, fgc, shadow=tuple(max(0, c - 70) for c in hexrgb(bg)))
    if sub:
        sb = [x0, h - b - int(h * 0.30), w - b - 30, h - b - 24]
        f2 = fit_font(d, sub, "GARABD.TTF", sb[2] - sb[0] - 60, sb[3] - sb[1] - 4, start=60)
        centered(d, sb, sub, f2, fgc)
    img = weather(img, seed)
    if shaped:
        mask = Image.new("L", (w, h), 0)
        md = ImageDraw.Draw(mask)
        md.rounded_rectangle([0, 0, w - 1, h - 1], radius=int(h * 0.18), fill=255)
        img = img.convert("RGBA"); img.putalpha(mask)
    save(id_, img, "board")


def enamel(id_, line1, line2, w=900, h=300, seed=3):
    rnd = random.Random(seed)
    img = Image.new("RGB", (w, h), (238, 235, 225))
    d = ImageDraw.Draw(img)
    blue = (31, 56, 110)
    d.rounded_rectangle([6, 6, w - 7, h - 7], radius=36, outline=blue, width=14)
    d.rounded_rectangle([30, 30, w - 31, h - 31], radius=22, outline=blue, width=4)
    if line2:
        f1 = fit_font(d, line1, "GARABD.TTF", w - 140, h * 0.2, start=70)
        centered(d, [40, 40, w - 40, 40 + h * 0.26], line1, f1, blue)
        f2 = fit_font(d, line2, "BOOKOSB.TTF", w - 110, h * 0.4, start=150)
        centered(d, [40, 36 + h * 0.24, w - 40, h - 44], line2, f2, blue)
    else:
        f2 = fit_font(d, line1, "BOOKOSB.TTF", w - 110, h * 0.5, start=150)
        centered(d, [40, 40, w - 40, h - 40], line1, f2, blue)
    for _ in range(9):  # chipped enamel near the rim
        x = rnd.choice([rnd.randint(8, 40), rnd.randint(w - 40, w - 8)]); y = rnd.randint(10, h - 10)
        r = rnd.randint(3, 9)
        d.ellipse([x - r, y - r, x + r, y + r], fill=(40, 34, 30))
    img = img.filter(ImageFilter.SMOOTH)
    save(id_, img, "enamel")


def stone_plaque(id_, title, sub, w=1400, h=320, seed=5):
    img = Image.new("RGB", (w, h), (178, 172, 160))
    n = noise_layer(w, h, seed, 3, 26)
    img = Image.merge("RGB", [Image.blend(c, n, 0.35) for c in img.split()])
    d = ImageDraw.Draw(img)
    d.rectangle([0, 0, w - 1, h - 1], outline=(120, 114, 104), width=18)
    d.rectangle([34, 34, w - 35, h - 35], outline=(96, 90, 82), width=3)
    f = fit_font(d, title, "CASTELAR.TTF", w - 160, h * 0.5, start=150)
    box = [40, 40, w - 40, h * 0.68]
    centered(d, [box[0] + 2, box[1] + 2, box[2] + 2, box[3] + 2], title, f, (214, 208, 196))   # lit edge
    centered(d, box, title, f, (70, 64, 58))                                                   # incised
    if sub:
        f2 = fit_font(d, sub, "GARABD.TTF", w - 300, h * 0.2, start=60)
        centered(d, [40, h * 0.64, w - 40, h - 44], sub, f2, (80, 74, 66))
    save(id_, img, "stone")


def blackboard(id_, title, lines, w=640, h=900, seed=7):
    img = Image.new("RGB", (w, h), (44, 48, 45))
    n = noise_layer(w, h, seed, 2, 14)
    img = Image.merge("RGB", [Image.blend(c, n, 0.25) for c in img.split()])
    d = ImageDraw.Draw(img)
    rnd = random.Random(seed)
    for _ in range(40):  # ghost smudges of old chalk
        x, y = rnd.randint(0, w), rnd.randint(0, h)
        d.ellipse([x - 40, y - 12, x + 40, y + 12], fill=(56, 60, 57))
    img = img.filter(ImageFilter.GaussianBlur(1.5))
    d = ImageDraw.Draw(img)
    chalk = (232, 230, 220)
    ft = font("Inkfree.ttf", 92)
    centered(d, [0, 40, w, 170], title, ft, chalk)
    d.line([80, 185, w - 80, 190], fill=chalk, width=3)
    fl = font("Inkfree.ttf", 58)
    y = 230
    for line in lines:
        d.text((70, y), line, font=fl, fill=chalk)
        y += 92
    frame = Image.new("RGB", (w + 60, h + 60), (92, 66, 44))
    frame = wood(w + 60, h + 60, (92, 66, 44), seed + 3, grain_vertical=True)
    frame.paste(img, (30, 30))
    save(id_, frame, "board")


def poster(id_, head, title, sub, date, colors, w=600, h=840, seed=9):
    paper = (231, 222, 200)
    img = Image.new("RGB", (w, h), paper)
    d = ImageDraw.Draw(img)
    c1, c2 = hexrgb(colors[0]), hexrgb(colors[1])
    d.rectangle([0, 0, w, h * 0.16], fill=c1)
    d.rectangle([0, h * 0.84, w, h], fill=c2)
    centered(d, [20, 10, w - 20, h * 0.16 - 10], head, fit_font(d, head, "BERNHC.TTF", w - 60, h * 0.1, 90), paper)
    centered(d, [30, h * 0.2, w - 30, h * 0.45], title, fit_font(d, title, "BERNHC.TTF", w - 60, h * 0.22, 160), c1)
    for k in range(3):  # a few bunting triangles
        x = 80 + k * (w - 160) / 2
        d.polygon([(x - 45, h * 0.5), (x + 45, h * 0.5), (x, h * 0.58)], fill=[c1, c2, c1][k])
    centered(d, [30, h * 0.6, w - 30, h * 0.72], sub, fit_font(d, sub, "GARABD.TTF", w - 60, h * 0.1, 70), (40, 34, 30))
    centered(d, [30, h * 0.72, w - 30, h * 0.82], date, fit_font(d, date, "BOOKOSB.TTF", w - 60, h * 0.08, 60), c2)
    rnd = random.Random(seed)
    img = img.rotate(rnd.uniform(-0.6, 0.6), resample=Image.BICUBIC, fillcolor=paper)
    img = weather(img, seed, 0.12)
    save(id_, img, "paper")


SIZES = {}


def save(id_, img, kind):
    OUT.mkdir(parents=True, exist_ok=True)
    img.save(OUT / f"{id_}.png")
    SIZES[id_] = {"w": img.width, "h": img.height, "kind": kind, "alpha": img.mode == "RGBA"}


def main():
    # hanging shop signs (shaped boards, pictogram + name) — P9 places, shop and bar
    shops = [
        ("taberna", "TABERNA", "del Puente", "#5B2320", "#E8D9B0", "jug"),
        ("barberia", "BARBERÍA", "", "#23344A", "#E6DDC4", "scissors"),
        ("orujos", "ORUJOS", "del alambique", "#3B3A1F", "#E3D3A0", "bottle"),
        ("horno", "HORNO", "pan de leña", "#6A4A22", "#F0E2BE", "bread"),
        ("quesos", "QUESOS", "de Liébana", "#2F4A3E", "#EADFBE", "cheese"),
        ("ferreteria", "FERRETERÍA", "", "#2B2F33", "#DCD6C6", "key"),
        ("cafe_billar", "CAFÉ", "billar", "#3A2A45", "#E7DCC0", "billiard"),
        ("fonda", "FONDA", "comidas", "#4A3524", "#EADCB8", "plate"),
        ("herreria", "HERRERÍA", "", "#2A2622", "#D8C9A6", "horseshoe"),
        ("ultramarinos", "ULTRAMARINOS", "", "#3E4E2E", "#EFE3C2", "basket"),
        ("comestibles", "COMESTIBLES", "", "#2E4250", "#EDE2C4", "basket"),
        ("bar", "BAR", "El Puente", "#5E2B1F", "#EBD6A6", "mug"),
    ]
    for i, (id_, t, s, bg, fg, ic) in enumerate(shops):
        painted_board("hang_" + id_, t, s, bg, fg, ic, w=768, h=480, seed=10 + i, shaped=True)
    # estanco: the classic maroon board with yellow letters
    painted_board("hang_estanco", "TABACOS", "estanco", "#6E1B1B", "#E8C23A", None, w=768, h=480, seed=40, shaped=True, font_name="BERNHC.TTF")
    # facade name boards
    painted_board("facade_bar", "BAR  EL PUENTE", "vinos · comidas · orujos", "#4E2419", "#EBD39E", "mug", w=1400, h=300, seed=50)
    painted_board("facade_comestibles", "COMESTIBLES", "ultramarinos finos", "#2A3E4C", "#EFE4C6", "basket", w=1300, h=300, seed=51)
    painted_board("facade_taberna", "TABERNA DEL PUENTE", "", "#4F2320", "#E8D9B0", "jug", w=1300, h=280, seed=52)
    painted_board("facade_fonda", "FONDA DE LOS TINTES", "comidas y camas", "#46341F", "#EADCB8", "plate", w=1300, h=280, seed=53)
    painted_board("private", "PRIVADO", "", "#3A2E24", "#E0D4BA", None, w=640, h=220, seed=54, font_name="GARABD.TTF")
    # civic
    stone_plaque("plaque_ayuntamiento", "AYUNTAMIENTO", "casa consistorial", seed=60)
    stone_plaque("plaque_riadas", "RIADAS", "marcas de las crecidas", w=900, h=260, seed=63)
    stone_plaque("plaque_archivo", "ARCHIVO MUNICIPAL", "acceso restringido", w=1100, h=280, seed=61)
    painted_board("board_bandos", "BANDOS Y AVISOS", "", "#3B2F24", "#E4D8BC", None, w=1100, h=220, seed=62, font_name="GARABD.TTF")
    # street plaques (enamel)
    streets = {
        "calle_W04": ("CALLE", "DEL COMERCIO"), "calle_W05": ("SUBIDA", "A LA PLAZA"), "calle_W06": ("LA", "CUESTA"),
        "calle_W12": ("SUBIDA", "DEL PUENTE"), "calle_W13": ("CALLEJÓN", "BAJO"), "calle_P8a": ("CALLEJA", "DE LOS TINTES"),
        "calle_P8b": ("TRAVESÍA", "DEL HORNO"), "calle_P8c": ("CALLEJA", "ALTA"), "calle_P8d": ("PASADIZO", "DEL ARCO"),
        "calle_P8e": ("CAMINO", "DEL ENSANCHE"), "calle_plaza": ("PLAZA", "MAYOR"), "calle_casco": ("PLAZUELA", "DEL CASCO"),
        "calle_puente": ("PUENTE", "VIEJO"), "calle_landing": ("", "EMBARCADERO"),
    }
    for i, (id_, (a, b)) in enumerate(streets.items()):
        enamel(id_, a, b, seed=70 + i)
    # bar interior life
    blackboard("pizarra_bar", "Hoy", ["Cocido lebaniego", "Rabas", "Quesucos", "Tortilla", "Orujo de la casa"], seed=90)
    poster("poster_fiestas", "FIESTAS", "DEL PUENTE", "verbena · romería · bolos", "15 · 16 · 17 de agosto", ("#A33A2A", "#2F4A6B"), seed=91)
    poster("poster_bolos", "CONCURSO", "DE BOLOS", "en la Era alta", "domingo a las 5", ("#2F5A3E", "#7A5A2A"), seed=92)
    poster("poster_feria", "FERIA", "DE GANADO", "y mercado de quesos", "primer lunes de mes", ("#6B4A1E", "#3A3A5A"), seed=93)
    (OUT / "signs.json").write_text(json.dumps(SIZES, indent=1, ensure_ascii=False), encoding="utf-8")
    print(f"{len(SIZES)} signs -> {OUT}")


if __name__ == "__main__":
    main()
