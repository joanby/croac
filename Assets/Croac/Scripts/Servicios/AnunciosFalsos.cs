using System;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// <see cref="IAds"/> sin red de anuncios, para donde LevelPlay no llega: el Mac y un móvil sin clave de LevelPlay.
    /// «Muestra» el anuncio al momento: avisa de que empieza, da la recompensa y lo cierra.
    /// </summary>
    public class AnunciosFalsos : IAds
    {
        public event Action Mostrado;
        public event Action Recompensa;
        public event Action Cerrado;

        public bool Listo => true;

        public void Mostrar()
        {
            Debug.Log("AnunciosFalsos: anuncio simulado, sin LevelPlay");
            Mostrado?.Invoke();
            Recompensa?.Invoke();
            Cerrado?.Invoke();
        }
    }
}
