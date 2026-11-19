using Unity.Scripting.LifecycleManagement;

namespace Croac
{
    /// <summary>
    /// Dónde vive la tienda: la crea el primero que la pide y, sin recarga de dominio, <c>[AutoStaticsCleanup]</c> la
    /// vacía al entrar en Play para que cada partida empiece con la suya. Lo que la tienda entrega se guarda aquí.
    /// </summary>
    public static partial class Servicios
    {
        [AutoStaticsCleanup]
        static IStore tienda;

        public static IStore Tienda => tienda ??= CrearTienda();

        static IStore CrearTienda()
        {
            var nueva = new TiendaUnityIap();
            nueva.Entregar += Entregar;
            return nueva;
        }

        static void Entregar(string producto, string pedido)
        {
            var guardado = Guardado.Cargar();
            if (Catalogo.Entregar(guardado, producto, pedido)) guardado.Guardar();
        }
    }
}
