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
 * En este archivo se encuentran las estructuras internas de la biblioteca nativa que son utilizadas por el SDK de .NET.
 * Implementan la interfaz INativeMethods, que define los métodos nativos de la biblioteca Victor, comunicados con la parte de instancia de ellos.
 * Esta dependencia es la que será inyectada en el SDK de .NET.
 *
 */

using NativeMethodsInterface;
using System;
using NativeWindowsStatic;
using System.Runtime.InteropServices;
using VictorBaseDotNET.Src.utils;


namespace Victor.NativeMethods.Windows;

internal class NativeMethodsWindows : INativeMethods
{
    public IntPtr alloc_index(IndexType type, DistanceMethod method, ushort dims, IntPtr? icontext) => NativeMethodsWindowsStatic.alloc_index(type, method, dims, icontext ?? IntPtr.Zero);
    public int destroy_index(ref IntPtr index) => NativeMethodsWindowsStatic.destroy_index(ref index);
    public int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n) => NativeMethodsWindowsStatic.search_n(index, vector, dims, results, n);
    public int search(IntPtr index, float[] vector, ushort dims, IntPtr results) => NativeMethodsWindowsStatic.search(index, vector, dims, results);
    public int insert(IntPtr index, ulong id, float[] vector, ushort dims) => NativeMethodsWindowsStatic.insert(index, id, vector, dims);
    public int delete(IntPtr index, ulong id) => NativeMethodsWindowsStatic.delete(index, id);
    public int update_icontext(IntPtr index, IntPtr icontext) => NativeMethodsWindowsStatic.update_icontext(index, icontext);
    public int stats(IntPtr index, IntPtr stats) => NativeMethodsWindowsStatic.stats(index, stats);
    public int init_asort(ref Asort asort, int n, int method) => NativeMethodsWindowsStatic.init_asort(ref asort, n, method);
    public int as_update(ref Asort asort, InternalMatchResult[] inputs, int n) => NativeMethodsWindowsStatic.as_update(ref asort, inputs, n);
    public int as_close(ref Asort asort, InternalMatchResult[] outputs, int n) => NativeMethodsWindowsStatic.as_close(ref asort, outputs, n);
    public int size(IntPtr index, out ulong sz) => NativeMethodsWindowsStatic.size(index, out sz);
    public int dump(IntPtr index, string filename) => NativeMethodsWindowsStatic.dump(index, filename);
    public int contains(IntPtr index, ulong id) => NativeMethodsWindowsStatic.contains(index, id);
    public int update_icontext(IntPtr index, IntPtr context, int mode) => NativeMethodsWindowsStatic.update_icontext(index, context, mode);
    public int hnsw_index(IntPtr idx, int method, ushort dims, IntPtr context) => NativeMethodsWindowsStatic.hnsw_index(idx, method, dims, context);
    public void normalize_vector([In, Out] float[] vector, ushort dims) => NativeMethodsWindowsStatic.normalize_vector(vector, dims);
    public int get_time_ms_monotonic() => NativeMethodsWindowsStatic.get_time_ms_monotonic();
    public int nsw_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsWindowsStatic.nsw_index_load(index, ioContext);
    public int hnsw_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsWindowsStatic.nsw_index_load(index, ioContext);
    public int flat_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsWindowsStatic.flat_index_load(index, ioContext);
    public int nsw_search(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult result) => NativeMethodsWindowsStatic.nsw_search(index, vector, dims, result);
    public int nsw_search_n(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult[] results, int n) => NativeMethodsWindowsStatic.nsw_search_n(index, vector, dims, results, n);
    public int hnsw_search(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult result) => NativeMethodsWindowsStatic.hnsw_search(index, vector, dims, result);
    public int hnsw_search_n(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult[] results, int n) => NativeMethodsWindowsStatic.hnsw_search_n(index, vector, dims, results, n);
    public IntPtr load_index(string filename) => NativeMethodsWindowsStatic.load_index(filename);
    public IntPtr __LIB_SHORT_VERSION() => NativeMethodsWindowsStatic.__LIB_SHORT_VERSION();
    public IntPtr __LIB_VERSION() => NativeMethodsWindowsStatic.__LIB_VERSION();
    public IntPtr victor_strerror(ErrorCode code) => NativeMethodsWindowsStatic.victor_strerror(code);
    public IntPtr index_name(IntPtr index) => NativeMethodsWindowsStatic.index_name(index);

}