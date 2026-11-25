# Entrega final de «Croac» (proyecto final)

Solución de referencia del proyecto final del curso (06.2). «Croac» y Estudio Nenúfar son ficticios.

| Pieza | Dónde | Prueba |
|---|---|---|
| Build | `Croac.aab`, fuera de git (`.gitignore`: `*.aab`, `*.keystore`) | `jarsigner -verify` → `jar verified.`; 7 `.so.sym` dentro del AAB |
| Informe de rendimiento | `informe-rendimiento.md` y `datos/` | Tres medidas de la misma partida en la misma sesión |
| Métricas | `metricas/` | Definiciones, panel y una decisión (datos ficticios) |
| Ficha de tienda | `../Store/` (Lab 5) | Límites de Google Play y App Store Connect |

## La build de referencia
AAB de release de esta rama, compilado con el perfil **Google Play** activo (Switch Profile, como en el Lab 5): sin Development Build, con Build App Bundle, Debug Symbols en «Public (Symbol_Table)» (así lo rotula la ventana en Unity 6000.6.8f1; la documentación lo llama Public) con salida App Bundle y firmado con la clave de subida del keystore `{dedicated}: croac.keystore` (alias `croac`). Pesa 67,47 MB y lleva 7 ficheros `.so.sym` en `BUNDLE-METADATA/com.android.tools.build.debugsymbols/arm64-v8a/`. Sin el perfil activo, con los ajustes de plataforma clásicos, el AAB sale sin símbolos y pesa unos 44 MB.

Para repetirla:
1. File › Build Profiles › Google Play › Switch Profile.
2. Build, con el keystore de la carpeta dedicada y sus dos contraseñas.
3. `jarsigner -verify Croac.aab` (el del OpenJDK de Unity) y `unzip -l Croac.aab | grep debugsymbols`.

El proyecto de iOS sale con la firma automática y el Signing Team ID vacío (05.2): sin cuenta, Xcode no lo compila para un iPhone.

Versión 1.0.0, Bundle Version Code 1.
