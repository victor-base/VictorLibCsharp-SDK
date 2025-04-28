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
using VictorNative;

namespace Victor;

public partial class VictorSDK
{

	// Constructor privado para inicializar desde un índice cargado
	private VictorSDK(IntPtr index)
	{
		_index = index;
		System.Diagnostics.Debug.WriteLine("\nIndex loaded successfully.\n");
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
		if (_index == IntPtr.Zero)
			throw new InvalidOperationException("\nIndex not created.\n");

		int status = NativeMethods.update_icontext(_index, icontext);
		if (status != 0)
			throw new InvalidOperationException("\nError updating index context.\n");

		System.Diagnostics.Debug.WriteLine("\nIndex context updated successfully.\n");
	}




	/// <summary>
	/// ENG: Dumps the current index to a file.
	/// </summary>
	/// <param name="filename">ENG: The file path where the index will be dumped. ESP: La ruta donde el archivo se va a descartar.</param>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created or if there is an error dumping the index.</exception>
	/// <remarks>
	/// ESP: Guarda el índice actual en un archivo.
	/// </remarks>
	public void DumpIndex(string filename)
	{
		if (_index == IntPtr.Zero)
			throw new InvalidOperationException("\nIndex not created.\n");

		int status = NativeMethods.dump(_index, filename);
		if (status != 0)
			throw new InvalidOperationException($"\nError dumping index to file: {filename}\n");

		System.Diagnostics.Debug.WriteLine($"\nIndex dumped successfully to {filename}.\n");
	}




	/// <summary>
	/// ENG: Checks if the index contains the specified ID.
	/// </summary>
	/// <param name="id">The ID to check for in the index.</param>
	/// <returns>True if the ID exists in the index, otherwise false.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the index is not created.</exception>
	/// <remarks>
	/// ESP: Verifica si el índice contiene el ID especificado.
	/// </remarks>
	public bool Contains(ulong id)
	{
		if (_index == IntPtr.Zero)
			throw new InvalidOperationException("\nIndex not created.\n");

		int result = NativeMethods.contains(_index, id);
		return result == 1;
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
		if (_index == IntPtr.Zero)
			throw new InvalidOperationException("\nIndex not created.\n");

		int status = NativeMethods.size(_index, out ulong size);
		if (status != 0)
			throw new InvalidOperationException("\nError retrieving index size.\n");

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
	public static VictorSDK LoadIndex(string filename)
	{
		IntPtr index = NativeMethods.load_index(filename);
		if (index == IntPtr.Zero)
			throw new InvalidOperationException($"\nError loading index from file: {filename}\n");

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
		if (_index == IntPtr.Zero)
			throw new InvalidOperationException("\nIndex not created.\n");

		int status = NativeMethods.delete(_index, id);

		if (status != 0)
			throw new InvalidOperationException($"\nERR: Can't eliminate vector with ID: {id}. status code: {status}\n");

		System.Diagnostics.Debug.WriteLine($"\nVector with ID: {id} eliminated succesfully.\n");
		return status;
	}

}