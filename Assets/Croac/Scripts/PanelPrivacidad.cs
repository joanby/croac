using UnityEngine;

namespace Croac
{
    /// <summary>
    /// La pregunta de consentimiento: sale la primera vez, antes de cualquier anuncio o evento, y se puede cambiar
    /// después con el botón «Privacidad» del menú. Aceptar y rechazar cuestan lo mismo: un toque.
    /// </summary>
    public class PanelPrivacidad : MonoBehaviour
    {
        [SerializeField] GameObject panel;

        void Start() => panel.SetActive(Guardado.Cargar().consentimiento == Consentimiento.SinPreguntar);

        /// <summary>Lo llama el botón «Privacidad» del menú.</summary>
        public void Abrir() => panel.SetActive(true);

        public void Aceptar() => Responder(Consentimiento.Acepta);

        public void Rechazar() => Responder(Consentimiento.Rechaza);

        void Responder(Consentimiento respuesta)
        {
            Servicios.Consentir(respuesta);
            panel.SetActive(false);
        }
    }
}
