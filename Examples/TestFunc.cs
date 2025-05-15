/*
 * Victor Base SDK - Developed by Iván E. Rodriguez
 * Based on the vector database core created by Emiliano A. Billi.
 * 
 * Copyright (C) 2025 Iván E. Rodriguez
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */


using System;
using NativeMethodsInterface;
using Victor.NativeMethods.Factory;
using VictorBaseDotNET.Src.utils;

namespace TestFunc
{
    internal static class TestBinding
    {
        private static readonly INativeMethods _native = NativeMethodsFactory.Create();
        public static void TestAllocIndex()
        {
            Console.WriteLine("\n Probando alloc_inde\n");

            // Parámetros de prueba:
            IndexType type = IndexType.FLAT;  // Tipo genérico
            DistanceMethod method = DistanceMethod.DOTPROD; // Método de prueba
            ushort dims = 128; // Dimensiones del vectorcito
            IntPtr icontext = IntPtr.Zero; 

            // Llamada al binding:
            IntPtr index = _native.alloc_index(type, method, dims, icontext);

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
                int result = _native.destroy_index(ref index);

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