using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Croac.EditorTools
{
    /// <summary>
    /// Sube el objetivo mínimo de todos los pods al de «Croac» (Player Settings › Target minimum iOS Version).
    /// Los pods de LevelPlay y de Unity Ads declaran iOS 12 y 13, y Xcode 27.2 no compila por debajo de iOS 15.
    /// Añade un <c>post_install</c> al Podfile que genera el Mobile Dependency Resolver: con el orden 45, después de
    /// generarlo (40) y antes de <c>pod install</c> (50), como indica su README.
    /// </summary>
    public static class PodsMinimoIos
    {
        [PostProcessBuild(45)]
        static void SubirMinimo(BuildTarget destino, string carpeta)
        {
            if (destino != BuildTarget.iOS) return;
            string minimo = PlayerSettings.iOS.targetOSVersionString;
            File.AppendAllText(Path.Combine(carpeta, "Podfile"),
                "\npost_install do |installer|\n" +
                "  installer.pods_project.targets.each do |target|\n" +
                "    target.build_configurations.each do |config|\n" +
                $"      config.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '{minimo}'\n" +
                "    end\n" +
                "  end\n" +
                "end\n");
        }
    }
}
