namespace Croac
{
    /// <summary>
    /// Analítica de la partida, sin saber adónde va: a un fichero del móvil (<see cref="AnaliticaLocal"/>) o a ninguna
    /// parte, si el jugador no ha dado su consentimiento (<see cref="AnaliticaApagada"/>).
    /// </summary>
    public interface IAnalytics
    {
        /// <summary>Apunta el evento. La analítica pone la fecha, el usuario y la sesión.</summary>
        void Registrar(Evento evento);
    }
}
