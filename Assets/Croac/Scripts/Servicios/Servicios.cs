using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// Dónde viven la tienda, los anuncios y la analítica: los crea el primero que los pide y, sin recarga de dominio,
    /// <c>[AutoStaticsCleanup]</c> los vacía al entrar en Play para que cada sesión empiece con los suyos. Lo que la
    /// tienda entrega se guarda aquí, y los anuncios y la analítica miran antes el consentimiento del jugador.
    /// </summary>
    public static partial class Servicios
    {
        [AutoStaticsCleanup]
        static IStore tienda;

        [AutoStaticsCleanup]
        static IAds anuncios;

        [AutoStaticsCleanup]
        static IAnalytics analitica;

        /// <summary>La clave de la app y el bloque del anuncio salen del panel de LevelPlay, con cuenta.</summary>
        const string ClaveLevelPlay = "";
        const string BloqueRevivir = "";

        public static IStore Tienda => tienda ??= CrearTienda();

        public static IAds Anuncios => anuncios ??= CrearAnuncios();

        public static IAnalytics Analitica => analitica ??= CrearAnalitica();

        static bool Acepta => Guardado.Cargar().consentimiento == Consentimiento.Acepta;

        /// <summary>Guarda la respuesta del jugador. La analítica se vuelve a crear con ella la próxima vez que se pida.</summary>
        public static void Consentir(Consentimiento respuesta)
        {
            var guardado = Guardado.Cargar();
            guardado.consentimiento = respuesta;
            guardado.Guardar();
            analitica = null;
        }

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
            if (Application.isEditor) return new AnunciosLevelPlay("editor", "editor-revivir", Acepta);
            if (ClaveLevelPlay.Length > 0) return new AnunciosLevelPlay(ClaveLevelPlay, BloqueRevivir, Acepta);
#endif
            return new AnunciosFalsos();
        }

        /// <summary>Con consentimiento, a un fichero del móvil y con un usuario al azar que se crea la primera vez.</summary>
        static IAnalytics CrearAnalitica()
        {
            var guardado = Guardado.Cargar();
            if (guardado.consentimiento != Consentimiento.Acepta) return new AnaliticaApagada();
            if (string.IsNullOrEmpty(guardado.usuario))
            {
                guardado.usuario = System.Guid.NewGuid().ToString("N");
                guardado.Guardar();
            }
            return new AnaliticaLocal(AnaliticaLocal.RutaPorDefecto, guardado.usuario);
        }

        static void Entregar(string producto, string pedido)
        {
            var guardado = Guardado.Cargar();
            if (!Catalogo.Entregar(guardado, producto, pedido)) return;
            guardado.Guardar();
            Analitica.Registrar(new Evento { evento = "purchase", producto = producto, ingreso = Tienda.Importe(producto) });
        }
    }
}
