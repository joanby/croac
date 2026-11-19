using System;

namespace Croac
{
    /// <summary>
    /// Anuncios recompensados, sin saber de qué red: LevelPlay (<see cref="AnunciosLevelPlay"/>) o ninguna
    /// (<see cref="AnunciosFalsos"/>). Los tres eventos pueden llegar en cualquier orden entre sí.
    /// </summary>
    public interface IAds
    {
        /// <summary>El anuncio ocupa la pantalla: hay que parar el juego y el audio.</summary>
        event Action Mostrado;

        /// <summary>El jugador se ha ganado la recompensa.</summary>
        event Action Recompensa;

        /// <summary>El anuncio se ha ido, con recompensa o sin ella.</summary>
        event Action Cerrado;

        bool Listo { get; }

        void Mostrar();
    }
}
