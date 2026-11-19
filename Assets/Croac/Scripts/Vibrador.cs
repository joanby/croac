using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#elif UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Croac
{
    /// <summary>
    /// Escucha el <see cref="CanalHaptico"/> y hace vibrar el móvil. En Android, con <c>VibrationEffect</c> por JNI
    /// (<c>AndroidJavaObject</c>); en iOS, con <c>UIImpactFeedbackGenerator</c> (el plugin <c>CroacHaptico.mm</c>). En el
    /// editor solo lo escribe en la consola.
    /// </summary>
    public class Vibrador : MonoBehaviour
    {
        [SerializeField] CanalHaptico canal;

        void OnEnable() => canal.Pedido += Vibrar;

        void OnDisable() => canal.Pedido -= Vibrar;

#if UNITY_ANDROID && !UNITY_EDITOR
        // VibrationEffect.EFFECT_TICK, EFFECT_CLICK y EFFECT_HEAVY_CLICK (API 29 o más).
        static readonly int[] Predefinidos = { 2, 0, 5 };
        // Por debajo de API 29 no hay efectos predefinidos: una vibración corta, en milisegundos.
        static readonly long[] Duraciones = { 10, 20, 40 };

        AndroidJavaObject vibrador;
        AndroidJavaObject[] efectos;

        /// <summary>Pide el vibrador al sistema y prepara los tres efectos una sola vez.</summary>
        void Awake()
        {
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            int api = version.GetStatic<int>("SDK_INT");
            var actividad = AndroidApplication.currentActivity;
            vibrador = api >= 31
                ? actividad.Call<AndroidJavaObject>("getSystemService", "vibrator_manager").Call<AndroidJavaObject>("getDefaultVibrator")
                : actividad.Call<AndroidJavaObject>("getSystemService", "vibrator");
            using var efecto = new AndroidJavaClass("android.os.VibrationEffect");
            efectos = new AndroidJavaObject[Predefinidos.Length];
            for (int i = 0; i < efectos.Length; i++)
                efectos[i] = api >= 29
                    ? efecto.CallStatic<AndroidJavaObject>("createPredefined", Predefinidos[i])
                    : efecto.CallStatic<AndroidJavaObject>("createOneShot", Duraciones[i], -1);
        }

        void Vibrar(Haptico haptico) => vibrador.Call("vibrate", efectos[(int)haptico]);
#elif UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void CroacVibrar(int intensidad);

        void Vibrar(Haptico haptico) => CroacVibrar((int)haptico);
#else
        void Vibrar(Haptico haptico) => Debug.Log($"Vibrador: {haptico} (en el editor no vibra nada)");
#endif
    }
}
