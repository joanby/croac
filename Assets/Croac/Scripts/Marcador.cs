using TMPro;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// Los marcadores de la partida: puntos, saltos y récord, moscas a la vista y tiempo. Cada texto se reescribe
    /// solo cuando cambia lo que muestra, y con <c>SetText</c> y un formato, que no crea cadenas nuevas.
    /// </summary>
    public class Marcador : MonoBehaviour
    {
        [SerializeField] Juego juego;
        [SerializeField] TMP_Text texto;
        [SerializeField] TMP_Text textoMoscas;
        [SerializeField] TMP_Text textoTiempo;

        Camera camara;
        int record;
        int puntos = -1;
        int saltos = -1;
        int moscas = -1;
        int segundos = -1;

        void Start()
        {
            camara = Camera.main;
            record = Guardado.Cargar().record;
        }

        void Update()
        {
            if (juego.Puntos != puntos || juego.Saltos != saltos)
            {
                puntos = juego.Puntos;
                saltos = juego.Saltos;
                texto.SetText("Puntos <b>{0}</b> · saltos {1} · récord {2}", puntos, saltos, record);
            }
            int aLaVista = juego.MoscasPorDebajoDe(camara.transform.position.y + camara.orthographicSize);
            if (aLaVista != moscas)
            {
                moscas = aLaVista;
                textoMoscas.SetText("Moscas <b>{0}</b>", moscas);
            }
            int ahora = (int)Time.timeSinceLevelLoad;
            if (ahora != segundos)
            {
                segundos = ahora;
                textoTiempo.SetText("<b>{0}</b> s", segundos);
            }
        }
    }
}
