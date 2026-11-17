using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Croac
{
    /// <summary>Carga lo que necesita cada nivel. De momento, el fondo, que va en Addressables.</summary>
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] SpriteRenderer fondo;

        public async Awaitable Cargar(Nivel nivel)
        {
            fondo.sprite = await Addressables.LoadAssetAsync<Sprite>(nivel.fondo).Task;
        }
    }
}
