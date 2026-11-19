namespace Croac
{
    /// <summary><see cref="IAnalytics"/> que no apunta nada: la que se usa sin el consentimiento del jugador.</summary>
    public class AnaliticaApagada : IAnalytics
    {
        public void Registrar(Evento evento) { }
    }
}
