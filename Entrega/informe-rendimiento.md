# Informe de rendimiento: «Croac», m02-start frente a m06-finished

«Croac» y Estudio Nenúfar son ficticios. Es la solución del proyecto final del curso (06.2), con la plantilla de la 06.1.

## Método
- Máquina y build: un Mac, build de desarrollo para macOS en una ventana de 540 × 960 (medido en un Mac, build de desarrollo).
- Partida: el bot `AutoPlay` con semilla 7 en la escena Juego, 600 frames tras 120 de calentamiento, grabados con `FrameBudgetRecorder` (02.2). Argumentos del juego: `-croacAutoplay 7 -croacEscena Juego -croacBudget <csv> -croacFrames 600 -croacWarmup 120 -screen-width 540 -screen-height 960 -screen-fullscreen 0`.
- Las tres medidas, seguidas y en la misma sesión (25-nov-2026): el juego roto del principio del módulo 02, el juego optimizado al acabarlo y el juego final (el código de los módulos 03 a 05), con los fondos descargados del servidor. Justo después, una repetición del optimizado y del final.
- El juego final carga los fondos del grupo remoto de Addressables (05.4), servidos en local con `uv run python -m http.server 8000 --directory ServerData` durante la medida.

## Resultados
| Cifra | Roto (m02-start) | Optimizado (m02-finished) | Final (m06-finished) |
|---|---|---|---|
| Hilo principal, mediana (ms) | 2,16 | 0,86 | 0,59 |
| Hilo principal, mediana (ms), repetición | — | 0,63 | 0,64 |
| Hilo principal, p95 (ms) | 3,61 | 1,53 | 0,78 |
| Hilo principal, p95 (ms), repetición | — | 1,19 | 0,90 |
| Asignado por frame, mediana (B) | 1270 | 0 | 0 |
| Layout de la UI, mediana (ms) | 1,267 | 0,003 | 0,002 |
| Rebuilds de Canvas por frame, mediana | 1 | 0 | 0 |
| Llamadas de dibujo, mediana | 20 | 16 | 17 |
| Memoria de texturas, máximo (MB) | 214,1 | 18,1 | 18,1 |
| Memoria total, máximo (MB) | 406,4 | 309,9 | 221,1 |

## Presupuesto
La mediana del hilo principal baja de 2,16 ms a entre 0,59 y 0,64 ms en las dos medidas del juego final (de 3,4 a 3,7 veces menos). El p95 del final, entre 0,78 y 0,90 ms, usa como mucho el 8 % de 10,8 ms (el 65 % de 16,67 ms que aconseja Unity para 60 fps).

## Qué cambió y qué cifra movió cada cambio
- Texturas comprimidas, sin Read/Write y con Sprite Atlas (02.3): la memoria de texturas.
- Marcador sin cadenas nuevas, moscas contadas sin LINQ, pool de nenúfares y zumbido sin corrutina (02.4): lo asignado por frame.
- Handle del fondo liberado al salir del nivel (02.4): la memoria total ya no sube de nivel en nivel.
- La orla en un Canvas estático sin layouts ni Raycast Target (02.5): el layout de la UI y los rebuilds.
- Módulos 03 a 05 (toques, layout, hápticos, tienda, anuncios, consentimiento, analítica, firma y contenido remoto): la columna del juego final frente a la del optimizado, con los límites de abajo.

## Lo que no se puede afirmar
- Que el juego final vaya más rápido que el optimizado. En esta medida sale por debajo (0,59 frente a 0,86 ms de mediana), pero repetidas justo después, las dos dan 0,63 y 0,64 ms (`datos/rep-m02-finished.csv` y `datos/rep-m06-finished.csv`): es la variación entre ejecuciones de la misma partida, como la memoria total máxima. Lo que sí se puede afirmar es que el código de los módulos 03 a 05, en esta partida, no se ha comido lo ganado: lo asignado por frame y los rebuilds siguen en cero, el layout de la UI casi en cero y la memoria de texturas no cambia.
- Cuánto tarda el frame en un móvil: esto es un Mac. La proporción entre columnas sí vale; la cifra absoluta sale de medir en el móvil.
- Nada sobre fps: la build de desarrollo del Mac no se mide en fps, y el emulador va a 30 fps antes y después (02.9).
- Nada sobre lo que cuestan la compra, el anuncio y los eventos: la partida del bot reinicia al caer al agua, sin revivir, así que no compra ni ve anuncios (y en el Mac, fuera de Android e iOS, «Croac» usa `AnunciosFalsos`); y el Mac de la medida no tenía ninguna respuesta de consentimiento guardada, así que la analítica era `AnaliticaApagada` y no escribió nada.

## Pendiente
- La fila del móvil de gama baja.
- Los frames más lentos, en las tres medidas, son los de justo después de cambiar de nivel (el 443 y el 530; los cambios caen en el 438 y el 525): precargar el fondo del nivel siguiente.

## Ficheros
`datos/m02-start.csv`, `datos/m02-finished.csv` y `datos/m06-finished.csv`, y las dos repeticiones `datos/rep-*.csv`: una fila por frame, con las 14 columnas del grabador.
