using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Croac
{
    /// <summary>
    /// Lee los dedos con Enhanced Touch y los traduce a gestos: un deslizamiento horizontal (cambiar de carril) y un
    /// pellizco con dos dedos (zoom). No es un MonoBehaviour para poder probarlo con toques simulados: quien lo usa llama
    /// a <see cref="Actualizar"/> una vez por frame y a <see cref="Dispose"/> al terminar.
    /// </summary>
    public class Gestos : IDisposable
    {
        readonly float umbral;
        float distanciaAnterior;

        /// <param name="umbral">Recorrido, en píxeles, a partir del cual un dedo que se mueve ya no es un toque.</param>
        public Gestos(float umbral)
        {
            this.umbral = umbral;
            EnhancedTouchSupport.Enable();
        }

        /// <summary>6 mm de recorrido, o 100 píxeles si el sistema no sabe los puntos por pulgada (Screen.dpi vale 0).</summary>
        public static float UmbralEnPixeles(float dpi) => dpi > 0f ? dpi * 6f / 25.4f : 100f;

        /// <summary>-1 o 1 en el frame en que termina un deslizamiento hacia la izquierda o la derecha; 0 en los demás.</summary>
        public int Deslizamiento { get; private set; }

        /// <summary>Algún dedo ha recorrido más que el umbral desde que tocó: lo que hace no es un toque ni mantener.</summary>
        public bool Moviendose { get; private set; }

        /// <summary>Cuánto ha cambiado la distancia entre dos dedos desde el frame anterior; 1 si no hay pellizco.</summary>
        public float Pellizco { get; private set; } = 1f;

        public void Actualizar()
        {
            Deslizamiento = 0;
            Moviendose = false;
            Pellizco = 1f;
            var toques = Touch.activeTouches;
            foreach (var toque in toques)
            {
                var recorrido = toque.screenPosition - toque.startScreenPosition;
                if (recorrido.magnitude > umbral) Moviendose = true;
                bool horizontal = Mathf.Abs(recorrido.x) > umbral && Mathf.Abs(recorrido.x) > Mathf.Abs(recorrido.y);
                if (toque.phase == TouchPhase.Ended && horizontal) Deslizamiento = recorrido.x > 0f ? 1 : -1;
            }
            if (toques.Count != 2)
            {
                distanciaAnterior = 0f;
                return;
            }
            float distancia = Vector2.Distance(toques[0].screenPosition, toques[1].screenPosition);
            if (distanciaAnterior > 0f) Pellizco = distancia / distanciaAnterior;
            distanciaAnterior = distancia;
        }

        public void Dispose() => EnhancedTouchSupport.Disable();
    }
}
