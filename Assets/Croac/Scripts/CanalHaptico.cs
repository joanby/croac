using System;
using UnityEngine;

namespace Croac
{
    /// <summary>Lo fuerte que vibra el móvil: un aterrizaje, una mosca o el plof.</summary>
    public enum Haptico
    {
        Ligero,
        Medio,
        Fuerte,
    }

    /// <summary>
    /// Un canal de eventos en un asset: quien quiere una vibración la emite aquí y quien sabe vibrar la escucha, sin que
    /// ninguno de los dos conozca al otro. <see cref="Juego"/> emite y <see cref="Vibrador"/> escucha.
    /// </summary>
    [CreateAssetMenu(menuName = "Croac/Canal háptico", fileName = "CanalHaptico")]
    public class CanalHaptico : ScriptableObject
    {
        public event Action<Haptico> Pedido;

        public void Emitir(Haptico haptico) => Pedido?.Invoke(haptico);
    }
}
