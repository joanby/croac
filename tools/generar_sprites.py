# /// script
# requires-python = ">=3.11"
# dependencies = ["pillow"]
# ///
"""Genera el arte de «Croac» en `Assets/Croac/Sprites/`: estilo plano con la paleta del curso.

    uv run tools/generar_sprites.py

Es determinista: dos ejecuciones dan los mismos PNG. Los PNG se versionan.
"""
import random
from pathlib import Path

from PIL import Image, ImageDraw

CIAN = "#22d3ee"
LIMA = "#a3e635"
FONDO = "#0a0f14"
ROJO = "#f43f5e"
VERDE = "#4d7c0f"
VERDE_OSCURO = "#365314"
AGUA = "#0e3b46"
AGUA_NOCHE = "#0b1d2e"
BLANCO = "#f8fafc"

SALIDA = Path(__file__).resolve().parent.parent / "Assets/Croac/Sprites"
ESCALA = 4  # se dibuja a 4x y se reduce: bordes suaves sin perder el estilo plano


def lienzo(ancho: int, alto: int, fondo=None):
    """Imagen a escala y su `Draw`. Sin `fondo`, transparente; con `fondo`, opaca y los colores con alfa se mezclan."""
    if fondo is None:
        img = Image.new("RGBA", (ancho * ESCALA, alto * ESCALA), (0, 0, 0, 0))
        return img, ImageDraw.Draw(img)
    img = Image.new("RGB", (ancho * ESCALA, alto * ESCALA), fondo)
    return img, ImageDraw.Draw(img, "RGBA")


def caja(x0, y0, x1, y1):
    return [v * ESCALA for v in (x0, y0, x1, y1)]


def guardar(img: Image.Image, nombre: str, ancho: int, alto: int):
    img.resize((ancho, alto), Image.Resampling.LANCZOS).save(SALIDA / nombre, optimize=True)


def rana():
    img, d = lienzo(256, 256)
    for x in (40, 176):  # patas traseras
        d.ellipse(caja(x, 150, x + 60, 230), fill=VERDE)
    for x in (60, 156):  # patas delanteras
        d.ellipse(caja(x, 70, x + 40, 120), fill=VERDE)
    d.ellipse(caja(58, 72, 198, 222), fill=LIMA)
    d.ellipse(caja(88, 120, 168, 200), fill="#bef264")
    for x in (70, 138):  # ojos
        d.ellipse(caja(x, 44, x + 48, 92), fill=LIMA)
        d.ellipse(caja(x + 8, 52, x + 40, 84), fill=BLANCO)
        d.ellipse(caja(x + 16, 58, x + 32, 74), fill=FONDO)
    guardar(img, "rana.png", 256, 256)


def nenufar():
    img, d = lienzo(256, 256)
    d.ellipse(caja(8, 8, 248, 248), fill=VERDE_OSCURO)
    d.ellipse(caja(16, 16, 240, 240), fill=VERDE)
    d.pieslice(caja(16, 16, 240, 240), start=-100, end=-70, fill=(0, 0, 0, 0))
    d.pieslice(caja(8, 8, 248, 248), start=-100, end=-70, fill=(0, 0, 0, 0))
    for angulo in range(0, 360, 45):
        d.pieslice(caja(40, 40, 216, 216), start=angulo, end=angulo + 2, fill=VERDE_OSCURO)
    d.ellipse(caja(116, 116, 140, 140), fill=LIMA)
    guardar(img, "nenufar.png", 256, 256)


def mosca():
    img, d = lienzo(128, 128)
    d.ellipse(caja(14, 22, 62, 70), fill=CIAN + "b0")
    d.ellipse(caja(66, 22, 114, 70), fill=CIAN + "b0")
    d.ellipse(caja(44, 40, 84, 108), fill="#1e293b")
    d.ellipse(caja(48, 30, 80, 60), fill=ROJO)
    d.ellipse(caja(54, 36, 62, 44), fill=BLANCO)
    d.ellipse(caja(66, 36, 74, 44), fill=BLANCO)
    guardar(img, "mosca.png", 128, 128)


def fondo(nombre: str, agua: str, reflejo: str):
    """Fondo vertical de 1080x1920: franjas de agua y ondas."""
    ancho, alto = 1080, 1920
    img, d = lienzo(ancho, alto, agua)
    for i in range(0, alto, 240):
        d.rectangle(caja(0, i, ancho, i + 120), fill=FONDO + "40")
    azar = random.Random(nombre)  # semilla fija: el mismo fondo en cada ejecución
    for _ in range(22):
        x, y = azar.randrange(ancho), azar.randrange(alto)
        d.arc(caja(x - 90, y - 24, x + 90, y + 24), start=200, end=340, fill=reflejo, width=6 * ESCALA)
        d.arc(caja(x - 50, y - 10, x + 50, y + 14), start=200, end=340, fill=reflejo, width=4 * ESCALA)
    guardar(img, nombre, ancho, alto)


if __name__ == "__main__":
    SALIDA.mkdir(parents=True, exist_ok=True)
    rana()
    nenufar()
    mosca()
    fondo("fondo-charca.png", AGUA, CIAN + "90")
    fondo("fondo-remanso.png", AGUA_NOCHE, LIMA + "80")
    print("\n".join(sorted(p.name for p in SALIDA.glob("*.png"))))
