using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// Dónde viven la tienda y los anuncios: los crea el primero que los pide y, sin recarga de dominio,
    /// <c>[AutoStaticsCleanup]</c> los vacía al entrar en Play para que cada sesión empiece con los suyos. Lo que la
    /// tienda entrega se guarda aquí.
    /// </summary>
    public static partial class Servicios
    {
        [AutoStaticsCleanup]
        static IStore tienda;

        [AutoStaticsCleanup]
        static IAds anuncios;

        /// <summary>La clave de la app y el bloque del anuncio salen del panel de LevelPlay, con cuenta.</summary>
        const string ClaveLevelPlay = "";
        const string BloqueRevivir = "";

        public static IStore Tienda => tienda ??= CrearTienda();

        public static IAds Anuncios => anuncios ??= CrearAnuncios();

        static IStore CrearTienda()
        {
            var nueva = new TiendaUnityIap();
            nueva.Entregar += Entregar;
            return nueva;
        }

        /// <summary>
        /// En el editor, los anuncios simulados de LevelPlay (con Android o iOS como plataforma activa), con cualquier
        /// clave. En el móvil, LevelPlay solo con su clave; sin ella, y fuera de Android e iOS, <see cref="AnunciosFalsos"/>.
        /// </summary>
        static IAds CrearAnuncios()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Application.isEditor) return new AnunciosLevelPlay("editor", "editor-revivir");
            if (ClaveLevelPlay.Length > 0) return new AnunciosLevelPlay(ClaveLevelPlay, BloqueRevivir);
#endif
            return new AnunciosFalsos();
        }

        static void Entregar(string producto, string pedido)
        {
            var guardado = Guardado.Cargar();
            if (Catalogo.Entregar(guardado, producto, pedido)) guardado.Guardar();
        }
    }
}
