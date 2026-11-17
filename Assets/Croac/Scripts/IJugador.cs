namespace Croac
{
    /// <summary>
    /// Lo mínimo que necesita alguien que juega a «Croac», sea una persona o un bot de pruebas.
    /// </summary>
    public interface IJugador
    {
        /// <summary>La rana está quieta en un nenúfar y la partida sigue.</summary>
        bool PuedeSaltar { get; }

        /// <summary>La partida ha terminado (la rana cayó al agua).</summary>
        bool Terminada { get; }

        /// <summary>Distancia horizontal, con signo, del nenúfar de la fila siguiente a la rana.</summary>
        float DesvioSiguiente { get; }

        void Saltar();

        void Reiniciar();
    }
}
