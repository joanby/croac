using TMPro;
using UnityEngine;

namespace Croac
{
    /// <summary>Los puntos de la partida en pantalla.</summary>
    public class Marcador : MonoBehaviour
    {
        [SerializeField] TMP_Text texto;

        public void Mostrar(int puntos) => texto.SetText("{0}", puntos);
    }
}
