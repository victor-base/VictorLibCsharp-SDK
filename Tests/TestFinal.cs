// Copyright (C) 2025 Iván E. Rodríguez. Contact: ivanrwcm25@gmail.com
//
// This file is part of the Victor Base binding for .NET.
// The original Victor Base core project was created by Emiliano Billi (email: emiliano.billi@gmail.com).
//
// This program is free software: you can redistribute it and/or modify  
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation, either version 3 of the License,  
// or (at your option) any later version.  
//
// This program is distributed in the hope that it will be useful,  
// but WITHOUT ANY WARRANTY; without even the implied warranty of  
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the  
// GNU General Public License for more details.  
//
// You should have received a copy of the GNU General Public License  
// along with this program. If not, see <https://www.gnu.org/licenses/>.




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