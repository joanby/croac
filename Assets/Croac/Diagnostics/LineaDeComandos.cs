using System;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Croac.Diagnostics
{
    /// <summary>
    /// Los argumentos con los que se ha lanzado el juego. En iOS, <c>Environment.GetCommandLineArgs</c> devuelve un solo
    /// elemento vacío aunque el proceso los reciba (comprobado en el simulador con 6000.6.8f1 y
    /// <c>xcrun simctl launch … -croacAutoplay 7</c>), así que ahí se leen de <c>NSProcessInfo</c> con el plugin
    /// <c>CroacArgumentos.mm</c>. Lo usa AutoPlay; FrameBudgetRecorder (lección 02.2) sigue con <c>Environment.GetCommandLineArgs</c>.
    /// </summary>
    public static class LineaDeComandos
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern string CroacArgumentos();

        public static string[] Leer() => CroacArgumentos().Split('\n');
#else
        public static string[] Leer() => Environment.GetCommandLineArgs();
#endif
    }
}
