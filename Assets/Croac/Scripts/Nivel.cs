using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Croac
{
    /// <summary>Los datos de un nivel: semilla, ritmo de la charca y fondo (cargado con Addressables).</summary>
    [CreateAssetMenu(menuName = "Croac/Nivel", fileName = "Nivel")]
    public class Nivel : ScriptableObject
    {
        public string titulo = "La charca";
        public int semilla = 1;
        [Tooltip("Distancia vertical entre filas, en unidades.")]
        public float separacionFilas = 2.4f;
        [Tooltip("Distancia máxima al centro a la que llega un nenúfar.")]
        public float limite = 3.2f;
        public float velocidadMinima = 0.8f;
        public float velocidadMaxima = 2f;
        [Range(0f, 1f)] public float probabilidadMosca = 0.3f;
        public AssetReferenceSprite fondo;
    }
}
