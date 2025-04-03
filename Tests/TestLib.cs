using System;
using System.Runtime.InteropServices;
using System.IO;


namespace LibTest
{
public class TestLoadLibrary
{
    public static void LibLoader()
    {
        try
        {
            string libraryName = @"D:\Users\pc\Desktop\VictorCsharpBinding-SDK\libvictorTEST.dll";  // Ruta completa de la DLL hardcodeada (solo para testear)

            Console.WriteLine($"\nIntentando cargar la biblioteca: {libraryName}\n");

            // Verificar si la dll existe
            if (!File.Exists(libraryName))
            {
                Console.WriteLine($"\nError: El archivo DLL no existe en la ruta especificada: {libraryName}\n");
                return;
            }

            IntPtr libraryHandle = LoadLibrary(libraryName);

            if (libraryHandle == IntPtr.Zero)
            {
                Console.WriteLine("\nNo se pudo cargar la biblioteca.\n");
            }
            else
            {
                Console.WriteLine("\nBiblioteca cargada correctamente.\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n Error al cargar la librería: {ex.Message}\n");
        }
    }

    // Declaración de la función LoadLibrary de kernel32.dll. No se que carajo hace.
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr LoadLibrary(string lpFileName);
}
}