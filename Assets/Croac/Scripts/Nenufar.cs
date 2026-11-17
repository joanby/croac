using UnityEngine;

namespace Croac
{
    /// <summary>Un nenúfar que va de lado a lado de la charca y puede llevar una mosca.</summary>
    public class Nenufar : MonoBehaviour
    {
        const float PeriodoZumbido = 0.01f;

        [SerializeField] GameObject mosca;

        float velocidad;
        float limite;
        Vector3 posicionMosca;
        float hastaZumbido;
        Transform propio;

        public bool TieneMosca => mosca.activeSelf;

        public void Configurar(Fila fila, float limite)
        {
            velocidad = fila.Velocidad;
            this.limite = limite;
            mosca.SetActive(fila.ConMosca);
        }

        public void ComerMosca() => mosca.SetActive(false);

        void Awake()
        {
            propio = transform;
            posicionMosca = mosca.transform.localPosition;
        }

        /// <summary>
        /// Lo coloca en la charca. Usa el Transform guardado en <c>Awake</c>: el primer acceso a <c>transform</c> crea
        /// su objeto de C# y asigna memoria, y así eso pasa al llenar el pool, no en un salto.
        /// </summary>
        public void Colocar(Vector3 posicion) => propio.position = posicion;

        void Update()
        {
            Zumbar();
            var posicion = transform.position;
            posicion.x += velocidad * Time.deltaTime;
            if (Mathf.Abs(posicion.x) > limite)
            {
                posicion.x = Mathf.Sign(posicion.x) * limite;
                velocidad = -velocidad;
            }
            transform.position = posicion;
        }

        /// <summary>
        /// La mosca tiembla sobre el nenúfar. Es un temporizador y no una corrutina: el nenúfar sale de un pool y una
        /// corrutina habría que arrancarla en cada <c>OnEnable</c>, y <c>StartCoroutine</c> asigna memoria.
        /// </summary>
        void Zumbar()
        {
            hastaZumbido -= Time.deltaTime;
            if (hastaZumbido > 0f) return;
            hastaZumbido = PeriodoZumbido;
            mosca.transform.localPosition = posicionMosca + (Vector3)(Random.insideUnitCircle * 0.04f);
        }
    }
}
