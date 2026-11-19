namespace Croac
{
    /// <summary>Lo que vende «Croac»: un paquete de moscas (consumible) y quitar los anuncios (no consumible).</summary>
    public static class Catalogo
    {
        public const string Moscas = "com.frogames.croac.moscas50";
        public const string SinAnuncios = "com.frogames.croac.sinanuncios";
        public const int MoscasPorPaquete = 50;
        const int PedidosRecordados = 50;

        /// <summary>
        /// Da al jugador lo que ha comprado. Un consumible se entrega una sola vez por pedido: la tienda puede avisar
        /// dos veces del mismo (si la app se cerró antes de confirmarlo). Devuelve <c>false</c> si no ha cambiado nada.
        /// </summary>
        public static bool Entregar(Guardado guardado, string producto, string pedido)
        {
            if (producto == SinAnuncios)
            {
                if (guardado.sinAnuncios) return false;
                guardado.sinAnuncios = true;
                return true;
            }
            if (producto != Moscas || guardado.pedidos.Contains(pedido)) return false;
            guardado.pedidos.Add(pedido);
            if (guardado.pedidos.Count > PedidosRecordados) guardado.pedidos.RemoveAt(0);
            guardado.moscas += MoscasPorPaquete;
            return true;
        }
    }
}
