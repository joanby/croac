using System;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Croac
{
    /// <summary>
    /// <see cref="IAds"/> con un anuncio recompensado de LevelPlay. En el editor, con la plataforma activa en Android o
    /// iOS, LevelPlay enseña anuncios simulados con cualquier clave; en el móvil hacen falta la clave de la app y el
    /// identificador del bloque de anuncios, que da su panel (con cuenta).
    /// </summary>
    public class AnunciosLevelPlay : IAds
    {
        readonly LevelPlayRewardedAd anuncio;

        public event Action Mostrado;
        public event Action Recompensa;
        public event Action Cerrado;

        public bool Listo => anuncio.IsAdReady();

        public AnunciosLevelPlay(string claveApp, string bloque)
        {
            anuncio = new LevelPlayRewardedAd(bloque);
            anuncio.OnAdLoadFailed += error => Debug.LogWarning($"AnunciosLevelPlay: no ha cargado: {error.ErrorMessage}");
            anuncio.OnAdDisplayed += _ => Mostrado?.Invoke();
            anuncio.OnAdDisplayFailed += AlNoMostrarse;
            anuncio.OnAdRewarded += (_, _) => Recompensa?.Invoke();
            anuncio.OnAdClosed += AlCerrarse;
            LevelPlay.OnInitSuccess += AlIniciar;
            LevelPlay.OnInitFailed += AlFallarInicio;
            LevelPlay.Init(claveApp);
        }

        public void Mostrar() => anuncio.ShowAd();

        /// <summary>Los eventos de LevelPlay son estáticos: sin recarga de dominio, se quitan para no oír a la sesión siguiente.</summary>
        void AlIniciar(LevelPlayConfiguration _)
        {
            LevelPlay.OnInitSuccess -= AlIniciar;
            LevelPlay.OnInitFailed -= AlFallarInicio;
            anuncio.LoadAd();
        }

        void AlFallarInicio(LevelPlayInitError error)
        {
            LevelPlay.OnInitSuccess -= AlIniciar;
            LevelPlay.OnInitFailed -= AlFallarInicio;
            Debug.LogWarning($"AnunciosLevelPlay: LevelPlay no se ha iniciado: {error.ErrorMessage}");
        }

        void AlNoMostrarse(LevelPlayAdInfo _, LevelPlayAdError error)
        {
            Debug.LogWarning($"AnunciosLevelPlay: no se ha mostrado: {error.ErrorMessage}");
            Cerrado?.Invoke();
            anuncio.LoadAd();
        }

        /// <summary>Al cerrarse, se carga el siguiente: así el botón «Revivir» de la próxima partida ya lo tiene.</summary>
        void AlCerrarse(LevelPlayAdInfo _)
        {
            Cerrado?.Invoke();
            anuncio.LoadAd();
        }
    }
}
