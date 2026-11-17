using System.Collections;
using UnityEngine;

namespace Croac
{
    /// <summary>Un nenúfar que va de lado a lado de la charca y puede llevar una mosca.</summary>
    public class Nenufar : MonoBehaviour
    {
        [SerializeField] GameObject mosca;

        float velocidad;
        float limite;
        Vector3 posicionMosca;

        public bool TieneMosca => mosca.activeSelf;

        public void Configurar(Fila fila, float limite)
        {
            velocidad = fila.Velocidad;
            this.limite = limite;
            mosca.SetActive(fila.ConMosca);
        }

        public void ComerMosca() => mosca.SetActive(false);

        void Awake() => posicionMosca = mosca.transform.localPosition;

        void OnEnable() => StartCoroutine(Zumbar());

        /// <summary>La mosca tiembla sobre el nenúfar.</summary>
        IEnumerator Zumbar()
        {
            while (true)
            {
                mosca.transform.localPosition = posicionMosca + (Vector3)(Random.insideUnitCircle * 0.04f);
                yield return new WaitForSeconds(0.05f);
            }
        }

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
