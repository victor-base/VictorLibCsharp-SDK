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

public partial class VictorSDK : IDisposable
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
	// public VictorSDK LoadIndex(string filename)
	// {
	// 	IntPtr index = _native.load_index(filename);

	// 	if (index == IntPtr.Zero) throw new InvalidOperationException($"\nError loading index from file: {filename}\n");

	// 	Debug.WriteLine($"\nÍndice cargado correctamente desde el archivo {filename}.\n");

	// 	return new VictorSDK(index);
	// }




	/// <summary>
	/// ENG: Deletes a vector with the specified ID from the index. ESP: Elimina un vector con el ID especificado del índice.
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

#nullable enable

public static class VictorPersistence
{

	private static string? _customBasePath;


	/// <summary>
	/// Permite configurar una ruta base para guardar archivos.
	/// </summary>
	public static void SetBasePath(string path) => _customBasePath = path;


	// overload del metodo para simplificar laburo al dev
	public static string DumpToAutoPath(VictorSDK sdk, ushort dims, IndexType type, DistanceMethod method) => DumpToAutoPath(sdk, dims, type, method, sdk.GetInsertedVectors());

	/// <summary>
	/// ENG: Dumps generated index with a auto file path. ESP: Dump del índice a archivo con ruta generada automáticamente.
	/// </summary>
	/// <param name="sdk">Instancia de <see cref="VictorSDK"/>.</param>
	/// <param name="dims">Dimensiones del índice.</param>
	/// <param name="type">Tipo de índice.</param>
	/// <param name="method">Método de distancia.</param>
	/// <param name="vectors">Vectores a serializar.</param>
	/// <returns>Ruta del archivo generado.</returns>
	/// <exception cref="VictorException">Lanza una excepción si no se puede serializar el índice.</exception>
	public static string DumpToAutoPath(VictorSDK sdk, ushort dims, IndexType type, DistanceMethod method, IEnumerable<VectorEntry> vectors)
	{
		string folder = _customBasePath ?? Path.Combine(Directory.GetCurrentDirectory(), ".victorIndex");

		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

		string fileName = $"victor_index_{DateTime.Now:yyyyMMdd_HHmmss}_.json";
		string fullPath = Path.Combine(folder, fileName);

		DumpToFile(sdk, fullPath, dims, type, method, vectors);
		EnsureGitIgnore(folder);

		Console.WriteLine($"\nIndex dumped successfully to file: {fullPath}.\n");
		Debug.WriteLine($"\nIndex dumped successfully to file: {fullPath}.\n");

		return fullPath;
	}

	private static void EnsureGitIgnore(string folder)
	{
		string gitignore = Path.Combine(folder, ".gitignore");

		if (!File.Exists(gitignore)) File.WriteAllText(gitignore, "*\n!.gitignore\n");

		else
		{
			var lines = File.ReadAllLines(gitignore).ToList();
			bool modified = false;

			if (!lines.Contains("*")) { lines.Add("*"); modified = true; }
			if (!lines.Contains("!.gitignore")) { lines.Add("!.gitignore"); modified = true; }

			if (modified) File.WriteAllLines(gitignore, lines);
		}

		File.WriteAllText(gitignore, "*\n!.gitignore\n");
	}



	/// <summary> ENG: Dumps the current index to a JSON file at the specified path. ESP: Persiste el índice seleccionado en un archivo Json. </summary>
	/// <param name="sdk">The instance of <see cref="VictorSDK"/> associated with the operation.</param>
	/// <param name="path">The file path where the JSON output will be written.</param>
	/// <param name="dims">The number of dimensions for the vectors.</param>
	/// <param name="type">The type of index to be used.</param>
	/// <param name="method">The distance method to be applied.</param>
	/// <param name="vectors">A collection of <see cref="VectorEntry"/> objects to be serialized.</param>
	///<returns>A <see cref="VictorSDK"/>This function does not have any return.</returns>
	public static void DumpToFile(VictorSDK sdk, string path, ushort dims, IndexType type, DistanceMethod method, IEnumerable<VectorEntry> vectors)
	{
		VictorIndexSnapshot snapshot = new()
		{
			Dimensions = dims,
			IndexType = type,
			Method = method,
			Vectors = vectors.ToList()
		};

		string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(path, json);
		Console.WriteLine($"\nIndex dumped successfully to file: {path}.\n");
		Debug.WriteLine($"\nIndex dumped successfully to file: {path}.\n");
	}

	/// <summary>
	/// Loads a <see cref="VictorSDK"/> ENG: instance from a JSON file at the specified path.  ESP: Carga una instancia de <see cref="VictorSDK"/> desde un archivo JSON en la ruta especificada.
	/// </summary>
	/// <param name="path">The file path to the JSON file containing the index snapshot.</param>
	/// <returns>A <see cref="VictorSDK"/> instance initialized with the data from the file.</returns>
	///<exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error deleting the vector.</exception>
	public static VictorSDK LoadFromFile(string path)
	{
		string json = File.ReadAllText(path);
		var snapshot = JsonSerializer.Deserialize<VictorIndexSnapshot>(json) ?? throw new VictorException("No se pudo deserializar el índice.", ErrorCode.FILEIO_ERROR);



		object? context = snapshot.IndexType switch
		{
			IndexType.HNSW => HNSWContext.Create(),
			IndexType.NSW => NSWContext.Create(),
			_ => null
		};

		VictorSDK sdk = new(snapshot.IndexType, snapshot.Method, snapshot.Dimensions, context);

		foreach (VectorEntry? entry in snapshot.Vectors) sdk.Insert(entry.Id, entry.Vector, snapshot.Dimensions);

		return sdk;
	}
	public static VictorIndexSnapshot ReadSnapshot(string path)
	{
		string json = File.ReadAllText(path);
		return JsonSerializer.Deserialize<VictorIndexSnapshot>(json) ?? throw new VictorException("No se pudo leer el snapshot", ErrorCode.FILEIO_ERROR);
	}


}
