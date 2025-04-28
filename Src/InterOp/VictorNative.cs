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
  This file serves as the bridge between the managed .NET code and the native Victor library coded in c (`libvictorTEST.dll`).
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

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VictorNative
{
  internal static class NativeMethods
  {

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr alloc_index(int type, int method, ushort dims, IntPtr icontext);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int destroy_index(ref IntPtr index);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int search(IntPtr index, float[] vector, ushort dims, IntPtr result);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int insert(IntPtr index, ulong id, float[] vector, ushort dims);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int delete(IntPtr index, ulong id);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int update_icontext(IntPtr index, IntPtr icontext);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int stats(IntPtr index, IntPtr stats);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int size(IntPtr index, out ulong sz);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int dump(IntPtr index, string filename);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int contains(IntPtr index, ulong id);

    [DllImport("libvictorTEST4.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr load_index(string filename);
  }


}

