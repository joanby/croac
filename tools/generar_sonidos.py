# /// script
# requires-python = ">=3.11"
# ///
"""Genera los efectos de sonido de «Croac» en `Assets/Croac/Audio/` (WAV mono de 16 bits, 44,1 kHz).

    uv run tools/generar_sonidos.py

Síntesis sencilla y determinista: un «croac» para el salto y un pitido para la mosca.
"""
import math
import struct
import wave
from pathlib import Path

SALIDA = Path(__file__).resolve().parent.parent / "Assets/Croac/Audio"
FRECUENCIA = 44100


def escribir(nombre: str, muestras: list[float]):
    with wave.open(str(SALIDA / nombre), "wb") as wav:
        wav.setnchannels(1)
        wav.setsampwidth(2)
        wav.setframerate(FRECUENCIA)
        wav.writeframes(b"".join(struct.pack("<h", int(max(-1.0, min(1.0, m)) * 32000)) for m in muestras))


def croac() -> list[float]:
    """Dos pulsos graves con la frecuencia bajando, como una rana."""
    duracion = 0.22
    muestras = []
    for i in range(int(FRECUENCIA * duracion)):
        t = i / FRECUENCIA
        frecuencia = 180 - 260 * t
        envolvente = math.sin(math.pi * t / duracion) * (0.6 + 0.4 * math.sin(2 * math.pi * 28 * t))
        onda = 1.0 if math.sin(2 * math.pi * frecuencia * t) > 0 else -1.0
        muestras.append(0.35 * envolvente * onda)
    return muestras


def mosca() -> list[float]:
    """Un pitido corto que sube."""
    duracion = 0.12
    muestras = []
    for i in range(int(FRECUENCIA * duracion)):
        t = i / FRECUENCIA
        frecuencia = 880 + 2200 * t
        envolvente = 1 - t / duracion
        muestras.append(0.4 * envolvente * math.sin(2 * math.pi * frecuencia * t))
    return muestras


if __name__ == "__main__":
    SALIDA.mkdir(parents=True, exist_ok=True)
    escribir("salto.wav", croac())
    escribir("mosca.wav", mosca())
    print("\n".join(sorted(p.name for p in SALIDA.glob("*.wav"))))
