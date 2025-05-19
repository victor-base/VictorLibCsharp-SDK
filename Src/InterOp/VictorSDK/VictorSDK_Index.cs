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
using Victor.NativeMethods.Factory;
using VictorBaseDotNET.Src.utils;
using VictorExceptions;
using VictorSnapshots;

namespace Victor;

public partial class VictorSDK : IDisposable
{

	public VictorSDK(IntPtr nativeIndex)
	{
		_native = NativeMethodsFactory.Create();
		_index = nativeIndex;
		_insertedVectors = new(); // Como no tenés los vectores, lo dejás vacío
		_disposedFlag = false;


		_indexType = 0x00;
		_method = DistanceMethod.COSINE;
		_dims = 0;

		Debug.WriteLine($"Índice cargado desde puntero nativo: {_index}");
	}


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
	public void DumpIndex_binary(string filename)
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
		if (_index == IntPtr.Zero) throw new VictorException("\nIndex not created.\n", ErrorCode.INVALID_INDEX);
		if (ids == null || ids.Length == 0) throw new VictorException("At least one ID must be provided.", ErrorCode.INVALID_ARGUMENT);

		foreach (var id in ids)
		{
			int result = _native.contains(_index, id);
			if (result < 0) throw new VictorException($"\nError checking if ID {id} exists in the index.\n", ErrorCode.INVALID_ID);
			if (result == 0) return false; // ID directamente no existe
		}
		Debug.WriteLine($"\nAll IDs exist in the index.\n");
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
	public VictorSDK LoadIndex_binary(string filename)
	{
		IntPtr index = _native.load_index(filename);

		if (index == IntPtr.Zero) throw new InvalidOperationException($"\nError loading index from file: {filename}\n");

		Debug.WriteLine($"\nÍndice cargado correctamente desde el archivo {filename}.\n");

		return new VictorSDK(index);
	}




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
	/// ENG: Sets a custom base directory where index dump files will be stored.
	/// ESP: Configura una carpeta base personalizada donde se guardarán los archivos de índice.
	/// </summary>
	public static void SetBasePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
			throw new VictorException("Path cannot be null or empty.", ErrorCode.FILEIO_ERROR);

		if (Path.HasExtension(path))
			throw new VictorException("SetBasePath expects a FOLDER, not a FILE path.", ErrorCode.FILEIO_ERROR);

		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		_customBasePath = path;
	}

	/// <summary>
	/// EXPERIMENTAL FEATURE
	/// ENG: Resets the custom base directory to null.
	/// ESP: Restablece la carpeta base personalizada a null.
	/// </summary>
	public static void ReSetBasePath() => _customBasePath = null;

	// overload del metodo para simplificar laburo al dev

	/// <summary>
	/// ENG: Dumps generated index with a auto file path if we dont provide a custom one. ESP: Dump del índice a archivo con ruta generada automáticamente si no le proporcionamos una ruta custom.
	/// </summary>
	/// <param name="sdk">Instancia de <see cref="VictorSDK"/>.</param>
	/// <returns>Ruta del archivo generado.</returns>
	/// <exception cref="VictorException">Lanza una excepción si no se puede serializar el índice.</exception>
	public static string DumpToPath_snapshot(VictorSDK sdk) => DumpToPath_snapshot(sdk, sdk.GetInsertedVectors());

	/// <summary>
	/// ENG: Dumps generated index with a auto file path if we dont provide a custom one. ESP: Dump del índice a archivo con ruta generada automáticamente si no le proporcionamos una ruta custom.
	/// </summary>
	/// <param name="sdk">Instancia de <see cref="VictorSDK"/>.</param>
	/// <param name="vectors">Vectores a serializar.</param>
	/// <returns>Ruta del archivo generado.</returns>
	/// <exception cref="VictorException">Lanza una excepción si no se puede serializar el índice.</exception>
	public static string DumpToPath_snapshot(VictorSDK sdk, IEnumerable<VectorEntry> vectors)
	{
		string folder = _customBasePath ?? Path.Combine(Directory.GetCurrentDirectory(), ".victorIndex");

		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

		string fileName = $"victor_index_{DateTime.Now:yyyyMMdd_HHmmss}_.json";
		string fullPath = Path.Combine(folder, fileName);

		DumpToFile_snapshot(sdk, fullPath, vectors);
		EnsureGitIgnore(folder);

		Debug.WriteLine($"\nIndex dumped successfully to file: {fullPath}.\n");

		return fullPath;
	}


	private static void EnsureGitIgnore(string folder)
	{
		string gitignore = Path.Combine(folder, ".gitignore");

		if (File.Exists(gitignore))
		{

			var lines = File.ReadAllLines(gitignore).ToList();
			bool modified = false;
			if (!lines.Contains("*")) lines.Add("*"); modified = true;
			if (!lines.Contains("!.gitignore")) lines.Add("!.gitignore"); modified = true;

			if (modified) File.WriteAllLines(gitignore, lines);

			return;
		}
		File.WriteAllText(gitignore, "*\n!.gitignore\n");
	}



	/// <summary> ENG: Dumps the current index to a JSON file at the specified path. ESP: Persiste el índice seleccionado en un archivo Json. </summary>
	/// <param name="sdk">The instance of <see cref="VictorSDK"/> associated with the operation.</param>
	/// <param name="path">The file path where the JSON output will be written.</param>
	/// <param name="vectors">A collection of <see cref="VectorEntry"/> objects to be serialized.</param>
	///<returns>A <see cref="VictorSDK"/>This function does not have any return.</returns>
	private static void DumpToFile_snapshot(VictorSDK sdk, string path, IEnumerable<VectorEntry> vectors)
	{
		try
		{

			VictorIndexSnapshot snapshot = new()
			{
				Dimensions = sdk._dims,
				IndexType = sdk._indexType,
				Method = sdk._method,
				Vectors = sdk.GetInsertedVectors().ToList()
			};

			string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(path, json);
			Debug.WriteLine($"\nIndex dumped successfully to file: {path}.\n");


		}
		catch
		{

			throw new VictorException($"\nError dumping index to file: {path}\n", ErrorCode.FILEIO_ERROR);
		}

	}

	/// <summary>
	/// Loads a <see cref="VictorSDK"/> ENG: instance from a JSON file at the specified path.  ESP: Carga una instancia de <see cref="VictorSDK"/> desde un archivo JSON en la ruta especificada.
	/// </summary>
	/// <param name="customContext"></param>
	/// <param name="path">The file path to the JSON file containing the index snapshot.</param>
	/// <param name="overrideType"></param>
	/// <param name="overrideMethod"></param>
	/// <returns>A <see cref="VictorSDK"/> instance initialized with the data from the file.</returns>
	///<exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error deleting the vector.</exception>
	public static VictorSDK LoadFromFile_snapshot(object? customContext, string path, IndexType? overrideType = null, DistanceMethod? overrideMethod = null) // ahora puedo mutar el tipo de índice al cargar el snapshot
	{

		string json = File.ReadAllText(path);

		if (string.IsNullOrEmpty(json)) throw new VictorException("El archivo está vacío o no se pudo leer.", ErrorCode.FILEIO_ERROR);
		if (!json.Contains("IndexType")) throw new VictorException("El archivo no contiene el tipo de índice.", ErrorCode.FILEIO_ERROR);

		var snapshot = JsonSerializer.Deserialize<VictorIndexSnapshot>(json) ?? throw new VictorException("No se pudo deserializar el índice.", ErrorCode.FILEIO_ERROR);

		IndexType finalIndexType = overrideType ?? snapshot.IndexType;
		DistanceMethod finalMethod = overrideMethod ?? snapshot.Method;

		VictorSDK sdk = new(finalIndexType, finalMethod, snapshot.Dimensions, customContext);

		foreach (var entry in snapshot.Vectors) sdk.Insert(entry.Id, entry.Vector, snapshot.Dimensions);

		return sdk;
	}


	/// <summary>
	/// ENG: Reads a snapshot from a JSON file at the specified path. ESP: Lee un snapshot desde un archivo JSON en la ruta especificada.	
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	/// <exception cref="VictorException">Expected a FILE PATH ...</exception>
	/// <exception cref="VictorException">The file is empty ...</exception>
	/// <exception cref="VictorException">Failed to deserialize ...</exception>
	public static VictorIndexSnapshot ReadSnapshot(string path)
	{
		if (!File.Exists(path)) throw new VictorException("Expected a FILE PATH, but got a DIRECTORY OR A INVALID PATH.\nTip: prefix with @ in string literals (e.g. @\"C:\\path\\to\\file.json\")", ErrorCode.FILEIO_ERROR);

		string json = File.ReadAllText(path);

		if (string.IsNullOrWhiteSpace(json)) throw new VictorException("The file is empty or could not be read.", ErrorCode.FILEIO_ERROR);

		return JsonSerializer.Deserialize<VictorIndexSnapshot>(json) ?? throw new VictorException("Failed to deserialize the snapshot JSON.", ErrorCode.FILEIO_ERROR);
	}
}


