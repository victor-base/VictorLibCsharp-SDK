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
 */
using NativeMethodsInterface;
using System;
using NativeLinuxStatic;
using System.Runtime.InteropServices;
using VictorBaseDotNET.Src.utils;


namespace Victor.NativeMethods.Linux;

internal class NativeMethodsLinux : INativeMethods
{
    public IntPtr alloc_index(IndexType type, DistanceMethod method, ushort dims, IntPtr? icontext) => NativeMethodsLinuxStatic.alloc_index(type, method, dims, icontext ?? IntPtr.Zero);
    public int destroy_index(ref IntPtr index) => NativeMethodsLinuxStatic.destroy_index(ref index);
    public int search(IntPtr index, float[] vector, ushort dims, IntPtr results) => NativeMethodsLinuxStatic.search(index, vector, dims, results);
    public int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n) => NativeMethodsLinuxStatic.search_n(index, vector, dims, results, n);
    public int insert(IntPtr index, ulong id, float[] vector, ushort dims) => NativeMethodsLinuxStatic.insert(index, id, vector, dims);
    public int delete(IntPtr index, ulong id) => NativeMethodsLinuxStatic.delete(index, id);
    public int update_icontext(IntPtr index, IntPtr icontext) => NativeMethodsLinuxStatic.update_icontext(index, icontext);
    public int stats(IntPtr index, IntPtr stats) => NativeMethodsLinuxStatic.stats(index, stats);
    public int init_asort(ref Asort asort, int n, int method) => NativeMethodsLinuxStatic.init_asort(ref asort, n, method);
    public int as_update(ref Asort asort, InternalMatchResult[] inputs, int n) => NativeMethodsLinuxStatic.as_update(ref asort, inputs, n);
    public int as_close(ref Asort asort, InternalMatchResult[] outputs, int n) => NativeMethodsLinuxStatic.as_close(ref asort, outputs, n);
    public int size(IntPtr index, out ulong sz) => NativeMethodsLinuxStatic.size(index, out sz);
    public int dump(IntPtr index, string filename) => NativeMethodsLinuxStatic.dump(index, filename);
    public int update_icontext(IntPtr index, IntPtr context, int mode) => NativeMethodsLinuxStatic.update_icontext(index, context, mode);
    public int contains(IntPtr index, ulong id) => NativeMethodsLinuxStatic.contains(index, id);
    public int hnsw_index(IntPtr idx, int method, ushort dims, IntPtr context) => NativeMethodsLinuxStatic.hnsw_index(idx, method, dims, context);
    public IntPtr load_index(string filename) => NativeMethodsLinuxStatic.load_index(filename);
    public IntPtr __LIB_SHORT_VERSION() => NativeMethodsLinuxStatic.__LIB_SHORT_VERSION();
    public IntPtr __LIB_VERSION() => NativeMethodsLinuxStatic.__LIB_VERSION();
    public IntPtr victor_strerror(ErrorCode code) => NativeMethodsLinuxStatic.victor_strerror(code);
    public IntPtr index_name(IntPtr index) => NativeMethodsLinuxStatic.index_name(index);
    public void normalize_vector([In, Out] float[] vector, ushort dims) => NativeMethodsLinuxStatic.normalize_vector(vector, dims);
    public int get_time_ms_monotonic() => NativeMethodsLinuxStatic.get_time_ms_monotonic();
    public int nsw_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsLinuxStatic.nsw_index_load(index, ioContext);
    public int hnsw_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsLinuxStatic.nsw_index_load(index, ioContext);
    public int flat_index_load(IntPtr index, IntPtr ioContext) => NativeMethodsLinuxStatic.flat_index_load(index, ioContext);
    public int nsw_search(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult result) => NativeMethodsLinuxStatic.nsw_search(index, vector, dims, result);
    public int nsw_search_n(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult[] results, int n) => NativeMethodsLinuxStatic.nsw_search_n(index, vector, dims, results, n);
    public int hnsw_search(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult result) => NativeMethodsLinuxStatic.hnsw_search(index, vector, dims, result);
    public int hnsw_search_n(IntPtr index, float[] vector, ushort dims, [Out] InternalMatchResult[] results, int n) => NativeMethodsLinuxStatic.hnsw_search_n(index, vector, dims, results, n);
}

