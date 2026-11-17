using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Croac
{
    /// <summary>
    /// Carga lo que necesita cada nivel. De momento, el fondo, que va en Addressables. Guarda el handle y lo libera
    /// al salir del nivel: si no, el fondo se queda en memoria y cada nivel nuevo suma el suyo.
    /// </summary>
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] SpriteRenderer fondo;

        AsyncOperationHandle<Sprite> handle;

        public async Awaitable Cargar(Nivel nivel)
        {
            handle = Addressables.LoadAssetAsync<Sprite>(nivel.fondo);
            fondo.sprite = await handle.Task;
        }

        void OnDestroy()
        {
            if (handle.IsValid()) handle.Release();
        }
    }
}
