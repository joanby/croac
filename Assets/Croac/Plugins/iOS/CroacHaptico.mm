#import <UIKit/UIKit.h>

extern "C" UIView* UnityGetGLView();

// Lo llama Vibrador.cs con [DllImport("__Internal")]: 0 ligero, 1 medio y 2 fuerte (el enum Haptico).
extern "C" void CroacVibrar(int intensidad)
{
    static const UIImpactFeedbackStyle estilos[] = { UIImpactFeedbackStyleLight, UIImpactFeedbackStyleMedium, UIImpactFeedbackStyleHeavy };
    UIImpactFeedbackGenerator* generador;
    if (@available(iOS 17.5, *))
        generador = [UIImpactFeedbackGenerator feedbackGeneratorWithStyle:estilos[intensidad] forView:UnityGetGLView()];
    else
        generador = [[UIImpactFeedbackGenerator alloc] initWithStyle:estilos[intensidad]];
    [generador impactOccurred];
    NSLog(@"CroacVibrar %d", intensidad);
}
