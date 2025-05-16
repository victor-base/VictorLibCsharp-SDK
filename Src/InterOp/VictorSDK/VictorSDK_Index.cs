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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using VictorBaseDotNET.Src.utils;
using VictorExceptions;
using VictorSnapshots;

namespace Victor;

public partial class VictorSDK
{

	/// <summary>
	/// Updates the index context with the provided pointer.
	/// </summary>
	/// <param name="icontext">Pointer to the new index context.</param>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error updating the context.</exception>
	/// <remarks>
	/// Actualiza el contexto del índice con el puntero proporcionado.
	/// </remarks>
	public void UpdateIndexContext(IntPtr icontext)
	{
		if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

		int status = _native.update_icontext(_index, icontext);

		if (status != 0) throw new InvalidOperationException("\nError updating index context.\n");

		Debug.WriteLine("\nIndex context updated successfully.\n");
	}




	/// <summary>
	/// ENG: Dumps the current index to a file.
	/// </summary>
	/// <param name="filename">ENG: The file path where the index will be dumped.  ESP: La ruta donde el archivo se va a guardar.</param>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error dumping the index.</exception>
	/// <remarks>
	/// ESP: Guarda el índice actual en un archivo.
	/// </remarks>
	public void DumpIndex(string filename)
	{
		if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

		int status = _native.dump(_index, filename);
		if (status != 0) throw new VictorException($"\nError dumping index to file: {filename}\n", ErrorCode.INVALID_FILE);

		Debug.WriteLine($"\nIndex dumped successfully to file: {filename}.\n");
	}



	/// <summary>
	/// ENG: Checks if the index contains the specified ID(s). Returns true if all IDs exist in the index, otherwise false.
	/// </summary>
	/// <param name="ids">One or more IDs to check for in the index.</param>
	/// <returns>True if all IDs exist in the index, otherwise false.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or is invalid.</exception>
	/// <remarks>
	/// ESP: Verifica si el índice contiene los ID(s) especificados. Devuelve True si todos los ID están en el índice, de lo contrario, False.
	/// </remarks>
	public bool Contains(params ulong[] ids)
	{
		if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");
		if (ids == null || ids.Length == 0) throw new ArgumentException("At least one ID must be provided.", nameof(ids));

		foreach (var id in ids)
		{
			int result = _native.contains(_index, id);
			if (result < 0) throw new InvalidOperationException($"\nError checking if ID {id} exists in the index.\n");
			if (result == 0) return false; // ID not found
		}

		return true;
	}



	/// <summary>
	/// ENG: Retrieves the size of the index.
	/// </summary>
	/// <returns>The size of the index as an unsigned long.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error retrieving the size.</exception>
	/// <remarks>
	/// ESP: Obtiene el tamaño del índice.
	/// </remarks>
	public ulong GetSize()
	{
		if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

		int status = _native.size(_index, out ulong size);
		if (status != 0) throw new InvalidOperationException("\nError retrieving index size.\n");

		Debug.Write($"Tamaño del Índice: {size}");
		return size;
	}



	/// <summary>
	/// ENG: Loads an index from a file.
	/// </summary>
	/// <param name="filename">The file path from which to load the index.</param>
	/// <returns>An instance of <see cref="VictorSDK"/> initialized with the loaded index.</returns>
	/// <exception cref="InvalidOperationException">Thrown if there is an error loading the index from the file.</exception>
	/// <remarks>
	/// ESP: Carga un índice desde un archivo.
	/// </remarks>
	public VictorSDK LoadIndex(string filename)
	{
		IntPtr index = _native.load_index(filename);

		if (index == IntPtr.Zero) throw new InvalidOperationException($"\nError loading index from file: {filename}\n");

		Debug.WriteLine($"\nÍndice cargado correctamente desde el archivo {filename}.\n");

		return new VictorSDK(index);
	}




	/// <summary>
	/// ENG: Deletes a vector with the specified ID from the index.
	/// </summary>
	/// <param name="id">The ID of the vector to delete.</param>
	/// <returns>The status code of the delete operation.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error deleting the vector.</exception>
	/// <remarks>
	/// ESP: Elimina un vector con el ID especificado del índice.
	/// </remarks>
	public int Delete(ulong id)
	{
		if (_index == IntPtr.Zero) throw new VictorException("\nIndex not created.\n");

		int status = _native.delete(_index, id);

		if (status != 0) throw new VictorException($"\nERR: Can't eliminate vector with ID: {id}. status code: {status}\n");

		Debug.WriteLine($"\nVector with ID: {id} eliminated succesfully.\n");

		return status;
	}


}

internal static class VictorPersistence
{
    public static void DumpToFile(VictorSDK sdk, string path, ushort dims, IndexType type, DistanceMethod method, IEnumerable<VectorEntry> vectors)
    {
        var snapshot = new VictorIndexSnapshot
        {
            Dimensions = dims,
            IndexType = type,
            Method = method,
            Vectors = vectors.ToList()
        };

        string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static VictorSDK LoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        var snapshot = JsonSerializer.Deserialize<VictorIndexSnapshot>(json)  ?? throw new InvalidOperationException("No se pudo deserializar el índice.");
#nullable enable
        object? context = snapshot.IndexType switch
        {
            IndexType.HNSW => HNSWContext.Create(),
            IndexType.NSW => NSWContext.Create(),
            _ => null
        };

        var sdk = new VictorSDK(snapshot.IndexType, snapshot.Method, snapshot.Dimensions, context);

        foreach (var entry in snapshot.Vectors)
            sdk.Insert(entry.Id, entry.Vector, snapshot.Dimensions);

        return sdk;
    }
}
