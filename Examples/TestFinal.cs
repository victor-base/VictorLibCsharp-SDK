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
using Victor;
using VictorBaseDotNET.Src.Common;
using VictorBaseDotNET.Src.utils;

namespace TestFuncionalFinal;

internal static class TestFinal
{

    public static void RunTest()
    {
		using var victor = new VictorSDK(IndexType.HNSW, DistanceMethod.EUCLIDIAN, 128, null);
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
		Console.WriteLine($"\nResultado de búsqueda: ID = {result.Label}, Distancia = {result.Distance}\n");

		// Eliminar el vector
		victor.Delete(id1);
		Console.WriteLine($"\nVector con ID {id1} eliminado.\n");


		try
		{
			result = victor.Search(vector1, 128);
			Console.WriteLine($"\nResultado de búsqueda tras eliminar: ID = {result.Label}, Distancia = {result.Distance}\n");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"\nError en la búsqueda tras eliminar: {ex.Message}\n");
		}

	}
}
