# Ficha de tienda de «Croac» (Lab 5)

Lo que pide Google Play Console (y su equivalente de App Store Connect) para publicar «Croac» 1.0.0. «Croac» y Estudio Nenúfar son ficticios: es la solución del Lab 5 del curso, no una ficha publicada.

| Fichero | Qué es | Requisito |
|---|---|---|
| `ficha-google-play.md` | Nombre, descripción breve y completa | 30, 80 y 4.000 caracteres |
| `ficha-app-store.md` | Nombre, subtítulo, palabras clave, texto promocional y descripción | 30, 30, 100 bytes, 170 y 4.000 |
| `icono-512.png` | Icono de la ficha | 512 × 512, PNG de 32 bits, 1 MB como máximo; cuadrado completo, sin esquinas redondeadas, sombra ni transparencia (Google Play aplica la máscara y la sombra) |
| `grafico-1024x500.png` | Gráfico de funciones | 1024 × 500, PNG de 24 bits sin alfa |
| `capturas/*.png` | Capturas de teléfono | 1080 × 1920 (9:16), PNG de 24 bits sin alfa; los juegos necesitan tres 9:16 de juego para optar a recomendaciones |
| `data-safety.md` | Respuestas de Data safety | Lo que recogen los SDK de LevelPlay y Unity IAP |
| `clasificacion-edad.md` | Respuestas del cuestionario de IARC y de Apple | Clasificación esperada |

Las capturas salen del emulador de Android (AVD Medium_Phone, build de desarrollo, porque la de release no descarga por HTTP del servidor local: a la del menú se le recortan los 50 píxeles de abajo, donde esa build pinta «Development Build») y del Device Simulator de Unity (iOS Notch Device). Las imágenes las genera `ficha_tienda.py` del repositorio del curso. Para el App Store, las capturas tienen que tener los tamaños exactos de cada pantalla de iPhone: no son estas.

Fuentes: [Google Play: especificaciones del icono](https://developer.android.com/distribute/google-play/resources/icon-design-specifications), [Google Play: recursos gráficos de la ficha](https://support.google.com/googleplay/android-developer/answer/9866151), [Google Play: buenas prácticas de la ficha](https://support.google.com/googleplay/android-developer/answer/13393723), [App Store Connect: información de la app](https://developer.apple.com/help/app-store-connect/reference/app-information/app-information), [App Store Connect: información de la versión](https://developer.apple.com/help/app-store-connect/reference/app-information/platform-version-information).
