using System;
using VictorNative;

namespace TestFunc
{
    public static class TestBinding
    {
        public static void TestAllocIndex()
        {
            Console.WriteLine("\n Probando alloc_index... \n");

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
                Console.WriteLine("\nÍndice asignado correctamente. \n");
                
                // Liberar el índice usando el método destroy_index
                Console.WriteLine("\nProcediendo a liberar el índice... \n");
                TestDestroyIndex(ref index); // Llamada al método para liberar el índice
            }
            else
            {
                Console.WriteLine("\nError al asignar el índice.\n");
            }
        }

        public static void TestDestroyIndex(ref IntPtr index)
        {
            Console.WriteLine("\nProbando destroy_index... \n");

            // Verificar que el índice existe antes de liberarlo
            if (index != IntPtr.Zero)
            {
                int result = NativeMethods.destroy_index(ref index);

                if (result == 0)
                {
                    Console.WriteLine("\n Índice liberado correctamente.\n");
                }
                else
                {
                    Console.WriteLine($"Error al liberar el índice: Código {result}.");
                }
            }
            else
            {
                Console.WriteLine("\n El índice ya estaba liberado o no se asignó.\n");
            }
        }
    }
}