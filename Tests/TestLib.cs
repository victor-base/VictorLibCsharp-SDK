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
            string libraryName = @"D:\Users\pc\Desktop\VictorCsharpBinding-SDK\Lib\libvictor.dll";  // Ruta completa de la DLL

            Console.WriteLine($"Intentando cargar la biblioteca: {libraryName}");

            // Verificar si la biblioteca existe
            if (!File.Exists(libraryName))
            {
                Console.WriteLine($"Error: El archivo DLL no existe en la ruta especificada: {libraryName}");
                return;
            }

            IntPtr libraryHandle = LoadLibrary(libraryName);

            if (libraryHandle == IntPtr.Zero)
            {
                Console.WriteLine("No se pudo cargar la biblioteca.");
            }
            else
            {
                Console.WriteLine("Biblioteca cargada correctamente.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar la librería: {ex.Message}");
        }
    }

    // Declaración de la función LoadLibrary de kernel32.dll
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr LoadLibrary(string lpFileName);
}
}