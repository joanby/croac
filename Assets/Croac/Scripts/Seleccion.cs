using UnityEngine;

namespace Croac
{
    /// <summary>El nivel que se eligió en el menú. Se vacía al entrar en Play (el proyecto no recarga el dominio).</summary>
    public static class Seleccion
    {
        public static Nivel Nivel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Vaciar() => Nivel = null;
    }
}
