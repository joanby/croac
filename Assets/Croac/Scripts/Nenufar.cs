using UnityEngine;

namespace Croac
{
    /// <summary>Un nenúfar que va de lado a lado de la charca y puede llevar una mosca.</summary>
    public class Nenufar : MonoBehaviour
    {
        [SerializeField] GameObject mosca;

        float velocidad;
        float limite;

        public bool TieneMosca => mosca.activeSelf;

        public void Configurar(Fila fila, float limite)
        {
            velocidad = fila.Velocidad;
            this.limite = limite;
            mosca.SetActive(fila.ConMosca);
        }

        public void ComerMosca() => mosca.SetActive(false);

        void Update()
        {
            var posicion = transform.position;
            posicion.x += velocidad * Time.deltaTime;
            if (Mathf.Abs(posicion.x) > limite)
            {
                posicion.x = Mathf.Sign(posicion.x) * limite;
                velocidad = -velocidad;
            }
            transform.position = posicion;
        }
    }
}
