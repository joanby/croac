using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Croac.EditorTools
{
    /// <summary>
    /// Compilación por línea de comandos. Se lanza con <c>-executeMethod Croac.EditorTools.CroacBuild.Build -quit</c> y:
    /// <list type="bullet">
    /// <item><c>-croacTarget android|ios|ios-sim|mac</c>: destino.</item>
    /// <item><c>-croacOut &lt;carpeta&gt;</c>: dónde deja el APK/AAB, el proyecto de Xcode o la .app.</item>
    /// <item><c>-croacDev</c>: build de desarrollo.</item>
    /// <item><c>-croacAab</c>: Android App Bundle en vez de APK.</item>
    /// <item><c>-croacInforme</c>: escribe <c>packed-assets.csv</c> (asset,tipo,formato,bytes) a partir del BuildReport.</item>
    /// </list>
    /// La firma de Android sale de las variables de entorno <c>CROAC_KEYSTORE</c>, <c>CROAC_KEYSTORE_PASS</c>,
    /// <c>CROAC_KEY_ALIAS</c> y <c>CROAC_KEY_PASS</c>; sin ellas, un APK se firma como diga el proyecto (con su keystore,
    /// cuyas contraseñas no se guardan, o con la clave de depuración) y un AAB falla. Al acabar, el keystore del proyecto vuelve a ser el que era.
    /// Si la build falla, lanza una excepción y Unity sale con código 1.
    /// </summary>
    public static class CroacBuild
    {
        public static void Build()
        {
            var args = Environment.GetCommandLineArgs();
            string destino = Valor(args, "-croacTarget");
            string salida = Path.GetFullPath(Valor(args, "-croacOut"));
            bool aab = args.Contains("-croacAab");
            Directory.CreateDirectory(salida);

            var opciones = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                options = args.Contains("-croacDev") ? BuildOptions.Development : BuildOptions.None,
            };
            var sdkIos = PlayerSettings.iOS.sdkVersion;
            var firma = (PlayerSettings.Android.useCustomKeystore, PlayerSettings.Android.keystoreName, PlayerSettings.Android.keyaliasName);
            switch (destino)
            {
                case "android":
                    opciones.target = BuildTarget.Android;
                    opciones.locationPathName = Path.Combine(salida, aab ? "Croac.aab" : "Croac.apk");
                    EditorUserBuildSettings.buildAppBundle = aab;
                    Firmar(aab);
                    break;
                case "ios":
                case "ios-sim":
                    opciones.target = BuildTarget.iOS;
                    opciones.locationPathName = Path.Combine(salida, "Xcode");
                    PlayerSettings.iOS.sdkVersion = destino == "ios-sim" ? iOSSdkVersion.SimulatorSDK : iOSSdkVersion.DeviceSDK;
                    break;
                case "mac":
                    opciones.target = BuildTarget.StandaloneOSX;
                    opciones.locationPathName = Path.Combine(salida, "Croac.app");
                    break;
                default:
                    throw new ArgumentException($"-croacTarget desconocido: '{destino}' (android, ios, ios-sim o mac)");
            }
            opciones.targetGroup = BuildPipeline.GetBuildTargetGroup(opciones.target);

            BuildReport informe;
            try
            {
                informe = BuildPipeline.BuildPlayer(opciones);
            }
            finally
            {
                PlayerSettings.iOS.sdkVersion = sdkIos;
                (PlayerSettings.Android.useCustomKeystore, PlayerSettings.Android.keystoreName, PlayerSettings.Android.keyaliasName) = firma;
            }

            var resumen = informe.summary;
            Debug.Log($"CroacBuild: {resumen.result} {destino} → {opciones.locationPathName} ({resumen.totalSize} bytes, {resumen.totalTime})");
            if (args.Contains("-croacInforme")) EscribirInforme(informe, opciones.target, Path.Combine(salida, "packed-assets.csv"));
            if (resumen.result != BuildResult.Succeeded) throw new Exception($"CroacBuild: la build terminó en {resumen.result}");
        }

        static string Valor(string[] args, string nombre)
        {
            int i = Array.IndexOf(args, nombre);
            if (i < 0 || i + 1 >= args.Length) throw new ArgumentException($"Falta {nombre} <valor>");
            return args[i + 1];
        }

        /// <summary>Firma con el keystore de las variables de entorno. Un AAB sin keystore es un error: no se firma con la clave de depuración.</summary>
        static void Firmar(bool obligatoria)
        {
            string keystore = Environment.GetEnvironmentVariable("CROAC_KEYSTORE");
            if (string.IsNullOrEmpty(keystore))
            {
                if (obligatoria) throw new ArgumentException("-croacAab necesita CROAC_KEYSTORE (y CROAC_KEYSTORE_PASS, CROAC_KEY_ALIAS, CROAC_KEY_PASS)");
                return;
            }
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystore;
            PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable("CROAC_KEYSTORE_PASS");
            PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable("CROAC_KEY_ALIAS");
            PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable("CROAC_KEY_PASS");
        }

        /// <summary>Una fila por asset empaquetado, de mayor a menor: asset,tipo,formato,bytes.</summary>
        static void EscribirInforme(BuildReport informe, BuildTarget target, string ruta)
        {
            string plataforma = target switch
            {
                BuildTarget.Android => "Android",
                BuildTarget.iOS => "iPhone",
                _ => "Standalone",
            };
            var filas = informe.packedAssets
                .SelectMany(p => p.contents)
                .OrderByDescending(c => c.packedSize)
                .Select(c => $"{Csv(c.sourceAssetPath)},{c.type.Name},{Formato(c.sourceAssetPath, plataforma)},{c.packedSize}");
            File.WriteAllLines(ruta, new[] { "asset,tipo,formato,bytes" }.Concat(filas));
            Debug.Log($"CroacBuild: informe en {ruta}");
        }

        static string Formato(string asset, string plataforma)
        {
            if (AssetImporter.GetAtPath(asset) is not TextureImporter importador) return "";
            var ajustes = importador.GetPlatformTextureSettings(plataforma);
            return (ajustes.overridden ? ajustes.format : importador.GetAutomaticFormat(plataforma)).ToString();
        }

        static string Csv(string texto) => texto.Contains(',') ? $"\"{texto}\"" : texto;
    }
}
