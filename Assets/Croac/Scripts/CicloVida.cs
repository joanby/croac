using System;

namespace Croac
{
    /// <summary>
    /// Decide cuándo guardar según el ciclo de vida de la app: al irse a segundo plano, al perder el foco
    /// y cuando el sistema avisa de que falta memoria. En cualquiera de los tres casos el sistema puede
    /// matar la app sin avisar otra vez, así que se guarda ya.
    /// No es un MonoBehaviour para poder probarlo sin escena: <see cref="Juego"/> le pasa los avisos de Unity.
    /// </summary>
    public class CicloVida
    {
        readonly Action guardar;

        /// <param name="guardar">Lo que hay que hacer para no perder el progreso.</param>
        public CicloVida(Action guardar) => this.guardar = guardar;

        /// <summary>Lo llama <c>OnApplicationPause</c>: <c>true</c> al irse a segundo plano.</summary>
        public void AlPausar(bool pausada)
        {
            if (pausada) guardar();
        }

        /// <summary>Lo llama <c>OnApplicationFocus</c>: <c>false</c> al perder el foco (una llamada, una notificación).</summary>
        public void AlCambiarFoco(bool conFoco)
        {
            if (!conFoco) guardar();
        }

        /// <summary>Lo llama <c>Application.lowMemory</c>.</summary>
        public void AlFaltarMemoria() => guardar();
    }
}
