namespace Croac
{
    /// <summary>Cuándo se puede ofrecer el anuncio de revivir: con la partida terminada, la rana quieta y una vez por partida.</summary>
    public static class MomentoAnuncio
    {
        public static bool Permitido(bool terminada, bool enElAire, bool yaRevivida) => terminada && !enElAire && !yaRevivida;
    }
}
