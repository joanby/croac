# Croac

Arcade de una mano para móvil, en vertical: una rana salta de nenúfar en nenúfar. Es el proyecto guía del curso **Juegos Móviles con Unity 6** de Frogames Formación. «Croac» y su estudio, Estudio Nenúfar, son ficticios.

## Cómo abrirlo

1. Instala **Unity 6000.6.8f1** con los módulos Android, iOS y Mac Build Support.
2. Clona el repo y cambia a la rama del módulo que vas a seguir (`git switch m01-start`).
3. Abre la carpeta del proyecto desde Unity Hub y luego la escena `Assets/Croac/Scenes/Menu.unity`.

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
