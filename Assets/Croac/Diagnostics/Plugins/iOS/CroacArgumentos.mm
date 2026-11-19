#import <Foundation/Foundation.h>
#include <string.h>

// Los argumentos del proceso, uno por línea, para LineaDeComandos.cs. IL2CPP libera la cadena devuelta con free.
extern "C" const char* CroacArgumentos()
{
    NSString* todos = [[[NSProcessInfo processInfo] arguments] componentsJoinedByString:@"\n"];
    return strdup([todos UTF8String]);
}
