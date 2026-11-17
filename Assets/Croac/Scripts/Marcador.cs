using System.Linq;
using TMPro;
using UnityEngine;

namespace Croac
{
    /// <summary>Los marcadores de la partida: puntos, moscas a la vista y tiempo.</summary>
    public class Marcador : MonoBehaviour
    {
        [SerializeField] Juego juego;
        [SerializeField] TMP_Text texto;
        [SerializeField] TMP_Text textoMoscas;
        [SerializeField] TMP_Text textoTiempo;

        void Update()
        {
            texto.SetText("Puntos: " + juego.Puntos);
            var moscas = FindObjectsByType<Nenufar>().Where(n => n.TieneMosca).ToList();
            textoMoscas.SetText("Moscas: " + moscas.Count);
            textoTiempo.SetText("Tiempo: " + Time.timeSinceLevelLoad.ToString("0.00") + " s");
        }
    }
}
