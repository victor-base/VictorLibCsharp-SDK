using System;
using VictorNative;

namespace TestFunc
{


    public static class TestBinding
    {
        public static void TestAllocIndex()
        {
            Console.WriteLine("Probando alloc_index...");

            // Parámetros de prueba:
            int type = 0;  // Tipo genérico
            int method = 1; // Método de prueba
            ushort dims = 128; // Dimensiones
            IntPtr icontext = IntPtr.Zero; // Sin contexto adicional

            // Llamada al binding:
            IntPtr index = NativeMethods.alloc_index(type, method, dims, icontext);

            // Verifica el resultado:
            if (index != IntPtr.Zero)
            {
                Console.WriteLine("Índice asignado correctamente.");
            }
            else
            {
                Console.WriteLine("Error al asignar el índice.");
            }
        }
    }
}