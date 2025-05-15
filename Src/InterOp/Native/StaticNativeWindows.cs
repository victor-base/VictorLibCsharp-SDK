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


/*
  Project: Victor Native Binding Library
  File: NativeMethods.cs

  --- ENGLISH ---
  Description:
  This file serves as the bridge between the managed .NET code and the native Victor library coded in c (`libvictorTEST4.dll`).
  It declares the signatures of the native functions using Platform Invocation Services (P/Invoke). These functions
  include essential operations such as memory management (allocation and destruction of indices), insertion,
  deletion, and vector search functionalities.

  Functionality:
  - `alloc_index`: Initializes a new index in native memory.
  - `destroy_index`: Frees the allocated memory for an index.
  - `insert`: Inserts a vector into the index.
  - `delete`: Removes a vector by its ID.
  - `search`: Performs a single result search using a query vector.
  - `search_n`: Executes a query to retrieve the top N results.

  Purpose:
  This file ensures seamless interoperability between high-level SDK classes (VictorSDK) and the native Victor library. 
  The goal is to maintain efficient resource management and high-performance vector operations.

  Contact:
  For any quiestions or suggestions, feel free to contact me at: ivanrwcm25@gmail.com

  --- ESPAÑOL ---
  Descripción:
  Este archivo actúa como un puente entre el código administrado de .NET y la biblioteca nativa Victor escrita en C (`libvictorTEST.dll`).
  Declara las firmas de las funciones nativas utilizando Invocación de Plataforma (P/Invoke). Estas funciones
  incluyen operaciones esenciales como la gestión de memoria (asignación y destrucción de índices), inserción,
  eliminación y funcionalidades de búsqueda vectorial en el marco de lo que es una bbdd vectorial.

  Funcionalidad:
  - `alloc_index`: Inicializa un nuevo índice en memoria nativa.
  - `destroy_index`: Libera la memoria asignada a un índice.
  - `insert`: Inserta un vector en el índice.
  - `delete`: Elimina un vector mediante su ID.
  - `search`: Realiza una búsqueda para obtener un único resultado con un vector de consulta.
  - `search_n`: Ejecuta una consulta para recuperar los N mejores resultados.

  Propósito:
  Este archivo asegura una interoperabilidad fluida entre las clases del SDK de alto nivel (VictorSDK) y la biblioteca nativa Victor. 
  El objetivo es garantizar una gestión eficiente de los recursos y operaciones de vectores de alto rendimiento.

  Contacto:
  Para cualquier consulta o sugerencia, contactar: ivanrwcm25@gmail.com
*/

// Here we have a partial class that contains the P/Invoke signatures for the native functions in the Victor library. this approach allows us to make this binding portable across different platforms (Windows and Linux) using conditional compilation directives.
// The P/Invoke signatures are defined using the DllImport attribute, which specifies the name of the native library and the calling convention to be used. The functions are declared as extern, indicating that they are implemented in the native library.
// The functions cover various operations such as allocation and deallocation of indexes, searching for vectors, inserting and deleting vectors, updating the index context, and retrieving statistics. The use of IntPtr allows for passing pointers to the native functions, enabling efficient memory management and interoperability between managed and unmanaged code.
using System;
using System.Runtime.InteropServices;
using VictorBaseDotNET.Src.utils;


namespace NativeWindowsStatic;

internal static class NativeMethodsWindowsStatic
{

  // Destroy_index frees the memory allocated for the index.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int destroy_index(ref IntPtr index);

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr alloc_index(IndexType type, DistanceMethod method, ushort dims, IntPtr icontext);

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr index_name(IntPtr index);

  // Search and Insert Functions
  // Search_n retrieves the top N results for a given vector query.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);

  // Search retrieves a single result for a given vector query.
  // The result is stored in the provided result pointer.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int search(IntPtr index, float[] vector, ushort dims, IntPtr result);

  // The insert function adds a vector to the index with a specified ID and dimensions.
  // the vector is passed as an array of floats, and the function returns an integer indicating success or failure.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int insert(IntPtr index, ulong id, float[] vector, ushort dims);

  // The delete function removes a vector from the index using its ID. It returns an integer indicating success or failure.
  // This function is useful for managing the index and ensuring that it only contains relevant vectors.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int delete(IntPtr index, ulong id);

  // The update_icontext function updates the index context with the provided icontext pointer. It returns an integer indicating success or failure.
  // This function is useful for modifying the behavior of the index or changing its parameters without needing to recreate it.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int update_icontext(IntPtr index, IntPtr icontext);

  // The stats function retrieves statistics about the index and stores them in the provided stats pointer. It returns an integer indicating success or failure.
  // This function is useful for monitoring the performance and efficiency of the index, allowing developers to optimize their vector search operations.


  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int stats(IntPtr index, IntPtr stats);

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int hnsw_index(IntPtr idx, int method, ushort dims, IntPtr context);


  // The load_index_context function loads an index context from a file with the specified filename. It returns a pointer to the loaded index context.
  // This function is useful for retrieving previously saved index context data, allowing for quick access to the parameters and settings used for the index.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int init_asort(ref Asort asort, int n, int method);

  // The as_update function updates the index with the provided inputs. It returns an integer indicating success or failure.
  // This function is useful for modifying the index based on new data or changing its parameters without needing to recreate it from scratch.

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int as_update(ref Asort asort, [In] InternalMatchResult[] inputs, int n);

  // The as_close function closes the index and retrieves the results. It returns an integer indicating success or failure.
  // This function is useful for finalizing the index and ensuring that all data is properly stored and accessible.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int as_close(ref Asort asort, [Out] InternalMatchResult[] outputs, int n);

  // The size function retrieves the size of the index and stores it in the provided sz pointer. It returns an integer indicating success or failure.
  // This function is useful for understanding the memory usage of the index and ensuring that it fits within the available resources.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int size(IntPtr index, out ulong sz);

  // The dump function saves the index to a file with the specified filename. It returns an integer indicating success or failure.
  // This function is useful for persisting the index to disk, allowing for later retrieval and use without needing to recreate it from scratch.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int dump(IntPtr index, string filename);

  // The contains function checks if a vector with the specified ID exists in the index. It returns an integer indicating success or failure.
  // This function is useful for verifying the presence of a vector in the index before performing operations on it.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int contains(IntPtr index, ulong id);

  // The load_index function loads an index from a file with the specified filename. It returns a pointer to the loaded index.
  // This function is useful for retrieving a previously saved index, allowing for quick access to the vector data without needing to recreate it from scratch.
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr load_index(string filename);

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr __LIB_SHORT_VERSION();

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr __LIB_VERSION();
  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern IntPtr victor_strerror(ErrorCode code);

  [DllImport("libvictor.dll", CallingConvention = CallingConvention.Cdecl)]
  public static extern int update_icontext(IntPtr index, IntPtr context, int mode);

}




