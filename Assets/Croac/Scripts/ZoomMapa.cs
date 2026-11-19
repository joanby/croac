using UnityEngine;

namespace Croac
{
    /// <summary>Acerca y aleja el mapa de niveles del menú (el objeto que lleva este componente) con un pellizco.</summary>
    public class ZoomMapa : MonoBehaviour
    {
        [SerializeField] float minimo = 1f;
        [SerializeField] float maximo = 1.6f;

        Gestos gestos;

        void OnEnable() => gestos = new Gestos(Gestos.UmbralEnPixeles(Screen.dpi));

        void OnDisable() => gestos.Dispose();

        void Update()
        {
            gestos.Actualizar();
            if (gestos.Pellizco == 1f) return;
            float escala = Mathf.Clamp(transform.localScale.x * gestos.Pellizco, minimo, maximo);
            transform.localScale = new Vector3(escala, escala, 1f);
        }
    }
}
