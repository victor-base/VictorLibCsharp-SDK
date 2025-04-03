using System;
using VictorNative;
using VictorSDK;
using Src.Common;

namespace TestFuncionalFinal
{
    public static class TestFinal
    {
        
        public static void RunTest()
        {
            using (var victor = new Victor(0, 1, 128, IntPtr.Zero))
            {
                // Insertar vectores en el índice
                ulong id1 = 1;
                float[] vector1 = new float[128];
                for (int i = 0; i < vector1.Length; i++)
                {
                    vector1[i] = (float)i / vector1.Length;
                }
                victor.Insert(id1, vector1, 128);
                Console.WriteLine($"\nVector con ID {id1} insertado.\n");

                // Buscar el vector
                MatchResult result = victor.Search(vector1, 128);
                Console.WriteLine($"\nResultado de búsqueda: ID = {result.Id}, Distancia = {result.Distance}\n");

                // Eliminar el vector
                victor.Delete(id1);
                Console.WriteLine($"\nVector con ID {id1} eliminado.\n");

                
                try
                {
                    result = victor.Search(vector1, 128);
                    Console.WriteLine($"\nResultado de búsqueda tras eliminar: ID = {result.Id}, Distancia = {result.Distance}\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError en la búsqueda tras eliminar: {ex.Message}\n");
                }
            }

        }
    }
}