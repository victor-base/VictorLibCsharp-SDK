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
 *
 * En este archivo tenemos una interfaz que define los métodos nativos de la biblioteca Victor, comunicados con la parte de instancia de ellos. 
 * La idea es que funciona como un puente entre el SDK de .NET y la biblioteca nativa, permitiendo la interoperabilidad entre ambos.
 *
 *
 */

using System;
using System.Runtime.InteropServices;
using VictorBaseDotNET.Src.utils;

namespace NativeMethodsInterface;

internal interface INativeMethods
{
	IntPtr alloc_index(IndexType type, DistanceMethod method, ushort dims, IntPtr? icontext);
	IntPtr index_name(IntPtr index);
	IntPtr victor_strerror(ErrorCode code);
	IntPtr load_index(string filename);
	IntPtr __LIB_VERSION();
	IntPtr __LIB_SHORT_VERSION();
	int destroy_index(ref IntPtr index);
	int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);
	int search(IntPtr index, float[] vector, ushort dims, IntPtr results);
	int insert(IntPtr index, ulong id, float[] vector, ushort dims);
	int delete(IntPtr index, ulong id);
	int update_icontext(IntPtr index, IntPtr icontext);
	int stats(IntPtr index, IntPtr stats);
	int init_asort(ref Asort asort, int n, int method);
	int as_update(ref Asort asort, [In] InternalMatchResult[] inputs, int n);
	int as_close(ref Asort asort, [Out] InternalMatchResult[] outputs, int n);
	int size(IntPtr index, out ulong sz);
	int dump(IntPtr index, string filename);
	int contains(IntPtr index, ulong id);
	int update_icontext(IntPtr index, IntPtr context, int mode);
	
}
