# Google Play: Data safety de «Croac» 1.0.0

«Croac» guarda en el móvil el récord, las moscas y su `eventos.jsonl` de analítica (módulo 04): no salen del dispositivo, así que no se declaran como recogidos. Lo que sí sale lo envían los SDK, y su respuesta se suma a la del juego. Fuentes: [Data safety de LevelPlay](https://docs.unity.com/en-us/grow/levelplay/platform/legal-resources/google-data-safety-questionnaire) (última actualización: 20-nov-2025) y [Data safety de Unity IAP](https://docs.unity.com/en-us/iap/privacy-and-consent/google-play-data-safety) (IAP 5.4 y posteriores). El adaptador de Unity Ads tiene su propia página en docs.unity.com: revísala con tu versión. Consultadas el 25-nov-2026.

## Recogida y seguridad

| Pregunta | Respuesta de «Croac» | Por qué |
|---|---|---|
| ¿Recoge o comparte alguno de los tipos de datos obligatorios? | Sí | LevelPlay y Unity IAP dicen que sí |
| ¿Se cifran en tránsito todos los datos recogidos? | Sí | Los dos SDK dicen que sí |
| ¿Pueden los usuarios pedir que se borren sus datos? | No, desde el juego | «Croac» no tiene cuentas; LevelPlay ofrece una vía propia, Unity IAP no |

## Tipos de datos

| Tipo | Recogido | Compartido | Para qué | De dónde sale |
|---|---|---|---|---|
| Ubicación aproximada | Sí | Sí | Analítica; fraude, seguridad y cumplimiento; publicidad | LevelPlay (Unity IAP la recoge sin compartir, de forma efímera) |
| Información personal: identificadores personales | Sí | No | Funcionalidad; analítica | Unity IAP |
| Información financiera: historial de compras | Sí | No | Funcionalidad; analítica | Unity IAP |
| Actividad en la app: otras acciones (interacción con anuncios) | Sí | Sí | Analítica; fraude, seguridad y cumplimiento; publicidad | LevelPlay |
| Información y rendimiento de la app: diagnósticos, registros de fallos | Sí | No | Funcionalidad; analítica; fraude y seguridad | LevelPlay y Unity IAP |
| Dispositivo u otros identificadores | Sí | Sí | Analítica; fraude, seguridad y cumplimiento; publicidad | LevelPlay (Unity IAP los recoge sin compartir) |

Todo lo demás (contactos, fotos, mensajes, salud, archivos, calendario, navegación): no.

**App Store (App Privacy)**: las mismas categorías, con las respuestas de Apple que publica LevelPlay ([cuestionario de privacidad de Apple](https://docs.unity.com/en-us/grow/levelplay/platform/legal-resources/apple-privacy-questionnaire)) y los privacy manifests de los SDK (módulo 04).
