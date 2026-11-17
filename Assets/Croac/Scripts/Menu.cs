using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Croac
{
    /// <summary>La pantalla de inicio: el récord y un botón por nivel.</summary>
    public class Menu : MonoBehaviour
    {
        [SerializeField] Nivel[] niveles;
        [SerializeField] TMP_Text textoRecord;

        void Start() => textoRecord.SetText("Récord: {0}", Guardado.Cargar().record);

        /// <summary>Lo llaman los botones del menú con el índice de su nivel.</summary>
        public void Jugar(int indice)
        {
            Seleccion.Nivel = niveles[indice];
            SceneManager.LoadScene("Juego");
        }
    }
}
