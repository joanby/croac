# Croac

Arcade de una mano para móvil, en vertical: una rana salta de nenúfar en nenúfar. Es el proyecto guía del curso **Juegos Móviles con Unity 6** de Frogames Formación. «Croac» y su estudio, Estudio Nenúfar, son ficticios.

## Cómo abrirlo

1. Instala **Unity 6000.6.8f1** con los módulos Android, iOS y Mac Build Support.
2. Clona el repo y cambia a la rama del módulo que vas a seguir (`git switch m01-start`).
3. Abre la carpeta del proyecto desde Unity Hub y luego la escena `Assets/Croac/Scenes/Menu.unity`.

## Ajustes de partida

Vertical, URP con Renderer 2D, solo Input System y Play Mode sin recarga de dominio. Desde `m01-finished`, Android compila con IL2CPP para ARM64, pide como mínimo la API 26 y apunta a la API 36, la que Google Play exige a las apps nuevas y a las actualizaciones desde el 31 de agosto de 2026 ([requisito de API objetivo de Google Play](https://developer.android.com/google/play/requirements/target-sdk)). iOS 15 como mínimo. Bundle id `com.frogames.croac`.

`m01-start` parte a propósito con Mono, ARMv7 y la API objetivo automática, y sin Build Profiles: lo arregla la lección 01.4. El fichero `.croac-ajustes` de cada rama dice qué ajustes trae distintos de los de arriba.

## Ramas

Cada módulo tiene una rama de partida y otra de solución:

| Rama | Qué es |
|---|---|
| `mNN-start` | El proyecto tal y como empieza el módulo NN (`m01-start` es el juego base). |
| `mNN-finished` | El proyecto con el módulo NN terminado. |
| `main` | La última rama terminada. |

## Qué hay dentro

- `Assets/Croac/`: scripts (`namespace Croac`), escenas, prefabs, niveles (ScriptableObject), sprites, sonidos, herramientas de Editor y tests EditMode.
- Los fondos de cada nivel se cargan con Addressables (grupo local `Fondos`).
- `tools/`: los scripts que generan el arte y los sonidos (`uv run tools/generar_sprites.py`, `uv run tools/generar_sonidos.py`).

## Licencia

MIT (a confirmar al publicar el repo).
