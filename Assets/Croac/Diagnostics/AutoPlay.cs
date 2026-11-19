using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Croac.Diagnostics
{
    public enum AccionBot
    {
        Esperar,
        Saltar,
        Reiniciar,
        CambiarNivel,
    }

    /// <summary>
    /// Lo que decide el bot en cada frame, sin escena: salta cuando el nenúfar de la fila siguiente está a menos de
    /// un umbral que sortea con su semilla en cada salto, reinicia si la rana cae al agua y pide otro nivel cada
    /// <c>saltosPorNivel</c> saltos. La misma semilla frente a la misma partida da los mismos saltos.
    /// </summary>
    public class Bot
    {
        readonly System.Random azar;
        readonly int saltosPorNivel;
        float umbral;
        int saltosEnNivel;

        public Bot(int semilla, int saltosPorNivel)
        {
            azar = new System.Random(semilla);
            this.saltosPorNivel = saltosPorNivel;
            umbral = SortearUmbral();
        }

        public AccionBot Decidir(IJugador jugador)
        {
            if (jugador.Terminada)
            {
                saltosEnNivel = 0;
                return AccionBot.Reiniciar;
            }
            if (!jugador.PuedeSaltar) return AccionBot.Esperar;
            if (saltosEnNivel >= saltosPorNivel)
            {
                saltosEnNivel = 0;
                return AccionBot.CambiarNivel;
            }
            if (Math.Abs(jugador.DesvioSiguiente) > umbral) return AccionBot.Esperar;
            saltosEnNivel++;
            umbral = SortearUmbral();
            return AccionBot.Saltar;
        }

        float SortearUmbral() => 0.2f + 0.3f * (float)azar.NextDouble();
    }

    /// <summary>
    /// Juega solo a «Croac» para que dos medidas de la misma rama sean comparables. Se activa si el juego arranca
    /// con <c>-croacAutoplay &lt;semilla&gt;</c>: desde el menú carga la escena de juego (<c>Juego</c>, o la que diga
    /// <c>-croacEscena &lt;nombre&gt;</c>) con el primer nivel y cambia al siguiente cada <see cref="SaltosPorNivel"/>
    /// saltos, en bucle. En una escena sin <see cref="IJugador"/> no hace nada más: así se mide, por ejemplo, el menú. Fija <c>Time.captureFramerate</c> a 60: el tiempo del juego
    /// avanza 1/60 s por frame, vaya el Mac rápido o lento, y la partida pasa por los mismos estados en cada medida.
    /// Solo necesita del juego <see cref="IJugador"/>, los <see cref="Nivel"/> que carga el menú (ordenados por
    /// semilla) y <see cref="Seleccion"/>.
    /// </summary>
    public class AutoPlay : MonoBehaviour
    {
        public const int SaltosPorNivel = 2;

        Bot bot;
        string escena;
        Nivel[] niveles;
        int indiceNivel;
        IJugador jugador;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Activar()
        {
            var args = LineaDeComandos.Leer();
            string semilla = FrameBudgetRecorder.Argumento(args, "-croacAutoplay");
            if (semilla == null) return;
            var go = new GameObject(nameof(AutoPlay));
            DontDestroyOnLoad(go);
            var autoPlay = go.AddComponent<AutoPlay>();
            autoPlay.bot = new Bot(int.Parse(semilla), SaltosPorNivel);
            autoPlay.escena = FrameBudgetRecorder.Argumento(args, "-croacEscena") ?? "Juego";
        }

        void Start()
        {
            Time.captureFramerate = 60;
            Application.runInBackground = true;
            niveles = Resources.FindObjectsOfTypeAll<Nivel>();
            Array.Sort(niveles, (a, b) => a.semilla.CompareTo(b.semilla));
            SceneManager.sceneLoaded += AlCargarEscena;
            Seleccion.Nivel = niveles[0];
            SceneManager.LoadScene(escena);
        }

        void OnDestroy() => SceneManager.sceneLoaded -= AlCargarEscena;

        void AlCargarEscena(Scene escena, LoadSceneMode _)
        {
            foreach (var raiz in escena.GetRootGameObjects())
            {
                jugador = raiz.GetComponentInChildren<IJugador>();
                if (jugador != null) return;
            }
        }

        void Update()
        {
            if (jugador == null) return;
            switch (bot.Decidir(jugador))
            {
                case AccionBot.Saltar:
                    jugador.Saltar();
                    break;
                case AccionBot.CambiarNivel:
                    indiceNivel = (indiceNivel + 1) % niveles.Length;
                    Seleccion.Nivel = niveles[indiceNivel];
                    Recargar();
                    break;
                case AccionBot.Reiniciar:
                    Recargar();
                    break;
            }
        }

        void Recargar()
        {
            var actual = jugador;
            jugador = null;
            actual.Reiniciar();
        }
    }
}
