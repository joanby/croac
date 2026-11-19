using System;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// La rana: salta hacia arriba hasta una altura, en recto o hacia un lado, y avisa al aterrizar; si cae al agua, se hunde.
    /// </summary>
    public class Rana : MonoBehaviour
    {
        [SerializeField] float duracionSalto = 0.22f;
        [SerializeField] float escalaSalto = 1.35f;
        [SerializeField] float duracionHundirse = 0.5f;

        public event Action Aterrizo;

        public bool EnElAire { get; private set; }

        float tiempo;
        float alturaInicio;
        float alturaDestino;
        float xInicio;
        float xDestino;
        bool hundiendose;

        /// <param name="desplazamiento">Cuánto se mueve hacia un lado durante el salto (negativo, a la izquierda).</param>
        public void Saltar(float altura, float desplazamiento = 0f)
        {
            transform.SetParent(null, true);
            alturaInicio = transform.position.y;
            alturaDestino = altura;
            xInicio = transform.position.x;
            xDestino = xInicio + desplazamiento;
            tiempo = 0f;
            EnElAire = true;
        }

        public void Hundirse()
        {
            tiempo = 0f;
            hundiendose = true;
        }

        void Update()
        {
            if (EnElAire) Volar();
            else if (hundiendose) Hundir();
        }

        void Volar()
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionSalto);
            var posicion = transform.position;
            posicion.y = Mathf.Lerp(alturaInicio, alturaDestino, t);
            posicion.x = Mathf.Lerp(xInicio, xDestino, t);
            transform.position = posicion;
            transform.localScale = Vector3.one * (1f + (escalaSalto - 1f) * Mathf.Sin(t * Mathf.PI));
            if (t < 1f) return;
            EnElAire = false;
            Aterrizo?.Invoke();
        }

        void Hundir()
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionHundirse);
            transform.localScale = Vector3.one * (1f - 0.7f * t);
            if (t >= 1f) hundiendose = false;
        }
    }
}
