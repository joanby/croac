using System.Linq;
using TMPro;
using UnityEngine;

namespace Croac
{
    /// <summary>Los marcadores de la partida: puntos, saltos y récord, moscas a la vista y tiempo.</summary>
    public class Marcador : MonoBehaviour
    {
        [SerializeField] Juego juego;
        [SerializeField] TMP_Text texto;
        [SerializeField] TMP_Text textoMoscas;
        [SerializeField] TMP_Text textoTiempo;

        Camera camara;
        int record;

        void Start()
        {
            camara = Camera.main;
            record = Guardado.Cargar().record;
        }

        void Update()
        {
            texto.SetText("Puntos <b>" + juego.Puntos + "</b> · saltos " + juego.Saltos + " · récord " + record);
            float arriba = camara.transform.position.y + camara.orthographicSize;
            var aLaVista = FindObjectsByType<Nenufar>().Where(n => n.TieneMosca && n.transform.position.y < arriba).ToList();
            textoMoscas.SetText("Moscas <b>" + aLaVista.Count + "</b>");
            textoTiempo.SetText("<b>" + Time.timeSinceLevelLoad.ToString("0.00") + "</b> s");
        }
    }
}
