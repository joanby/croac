# Croac

Arcade de una mano para móvil, en vertical: una rana salta de nenúfar en nenúfar. Es el proyecto guía del curso **Juegos Móviles con Unity 6** de Frogames Formación. «Croac» y su estudio, Estudio Nenúfar, son ficticios.

## Cómo abrirlo

1. Instala **Unity 6000.6.8f1** con los módulos Android, iOS y Mac Build Support.
2. Clona el repo y cambia a la rama del módulo que vas a seguir:

   ```
   git clone https://github.com/joanby/croac.git
   cd croac
   git switch m01-start
   ```

3. Abre la carpeta del proyecto desde Unity Hub y luego la escena `Assets/Croac/Scenes/Menu.unity`.

## Ajustes de partida

Vertical, URP con Renderer 2D, solo Input System y Play Mode sin recarga de dominio. Desde `m01-finished`, Android compila con IL2CPP para ARM64, pide como mínimo la API 26 y apunta a la API 36, la que Google Play exige a las apps nuevas y a las actualizaciones desde el 31 de agosto de 2026 ([requisito de API objetivo de Google Play](https://developer.android.com/google/play/requirements/target-sdk)). iOS 15 como mínimo. Bundle id `com.frogames.croac`.

`m01-start` parte a propósito con Mono, ARMv7 y la API objetivo automática, y sin Build Profiles: lo arregla la lección 01.4. El fichero `.croac-ajustes`, que solo existe en `m01-start`, dice qué ajustes trae distintos de los de arriba.

## Ramas

Cada módulo tiene una rama de partida y otra de solución, en este orden: `m01-start`, `m01-finished`, `m02-start`, …, `m06-finished`. Cada rama parte de la anterior.

| Rama | Qué es |
|---|---|
| `mNN-start` | El proyecto tal y como empieza el módulo NN (`m01-start` es el juego base). |
| `mNN-finished` | El proyecto con el módulo NN terminado. |
| `main` | Lo mismo que `m06-finished`: el curso entero terminado. |

## Etiquetas

En los módulos 02 a 05, algunas lecciones tienen una etiqueta con el estado del proyecto al acabarlas: `mNN-NN.L` es el módulo y la lección (por ejemplo, `m03-03.2` es «Croac» al terminar la lección 03.2). Sirven para comparar tu código con el de una lección concreta (`git diff m03-03.1 m03-03.2`). Algunos arreglos llegaron a la rama después de su etiqueta, así que la referencia final es siempre `mNN-finished`.

Etiquetas: `m02-02.2`, `m03-03.1` a `m03-03.5`, `m04-04.3`, `m04-04.4`, `m04-04.6`, `m05-05.1`, `m05-05.2` y `m05-05.4`.

## Qué hay dentro

- `Assets/Croac/`: scripts (`namespace Croac`), escenas, prefabs, niveles (ScriptableObject), sprites, sonidos, herramientas de Editor y tests EditMode.
- Los fondos de cada nivel se cargan con Addressables (grupo local `Fondos`).
- `tools/`: los scripts que generan el arte y los sonidos (`uv run tools/generar_sprites.py`, `uv run tools/generar_sonidos.py`).

## Licencia

Todos los derechos reservados, ver [`LICENSE`](LICENSE), con una excepción: el código que escribes siguiendo las guías del curso puedes usarlo en tus propios juegos, también comerciales. El proyecto como conjunto, su arte y sus sonidos no se pueden redistribuir ni reutilizar. Los ficheros de terceros (Mobile Dependency Resolver, la fuente LiberationSans y los paquetes de Unity) conservan su licencia, que también se cita en `LICENSE`.
