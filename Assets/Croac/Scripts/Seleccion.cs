using Unity.Scripting.LifecycleManagement;

namespace Croac
{
    /// <summary>
    /// El nivel que se eligió en el menú. El proyecto entra en Play sin recargar el dominio, así que un campo estático
    /// guardaría el nivel de la partida anterior: <c>[AutoStaticsCleanup]</c> hace que Unity lo vacíe al entrar en Play.
    /// </summary>
    public static partial class Seleccion
    {
        [AutoStaticsCleanup]
        public static Nivel Nivel;
    }
}
