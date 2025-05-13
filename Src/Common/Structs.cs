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
 * Estas estructuras son utilizadas para representar los resultados de búsqueda, estadísticas y otros datos relevantes para el funcionamiento del SDK.
 * Deberían ser inaccesibles en este archivo (por eso internal), por lo que las expongo parcialmente en el archivo mapping.cs.
 *
 *
 */

using System;
using System.Runtime.InteropServices;

namespace VictorBaseDotNET.Src.utils;

public enum IndexType
{
    FLAT = 0,
    HNSW = 1
}

public enum DistanceMethod
{
    L2 = 0,
    DOTPROD = 1,
    COSINE = 2
}

[StructLayout(LayoutKind.Sequential)]
public struct NSWContext
{
    public int EFSEARCH;
    public int EF_CONSTRUCT;
    public int ODEGREE;

    public static IntPtr ToPointer(int efSearch = 240, int efConstruct = 240, int odegree = 32)
    {
        var context = new NSWContext
        {
            EFSEARCH = efSearch,
            EF_CONSTRUCT = efConstruct,
            ODEGREE = odegree
        };

        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<NSWContext>());
        Marshal.StructureToPtr(context, ptr, false);
        return ptr;
    }
}



[StructLayout(LayoutKind.Sequential)]
public struct HNSWContext
{
    public int ef_construct;
    public int ef_search;
    public int M0;

    public static HNSWContext Create(int efConstruct = 240, int efSearch = 240, int m0 = 32)
    {
        return new HNSWContext
        {
            ef_construct = efConstruct,
            ef_search = efSearch,
            M0 = m0
        };
    }

    public static IntPtr ToPointer(HNSWContext context)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<HNSWContext>());
        Marshal.StructureToPtr(context, ptr, false);
        return ptr;
    }
}



[StructLayout(LayoutKind.Sequential)]
internal struct InternalAsort
{
    public IntPtr Heap;
}


[StructLayout(LayoutKind.Sequential)]
public struct InternalMatchResult
{
    public int Label;
    public float Distance;
}


[StructLayout(LayoutKind.Sequential)]
internal struct TimeStat
{
    public ulong Count;
    public double Total;
    public double Last;
    public double Min;
    public double Max;
}

[StructLayout(LayoutKind.Sequential)]
public struct InternalIndexStatsResult
{
    public ulong InsertCount;
    public ulong DeleteCount;
    public ulong SearchCount;
    public ulong SearchNCount;
    public double TotalTime;
    public double LastOperationTime;
    public double MinOperationTime;
    public double MaxOperationTime;

}

// The Asort struct is defined to represent the internal state of the asort algorithm, which is used for sorting and managing the results of vector searches. It contains a single field, Heap, which is a pointer to the heap memory used by the asort algorithm.

[StructLayout(LayoutKind.Sequential)]
public struct Asort
{
    public IntPtr Heap;

    public Asort(IntPtr heap) => Heap = heap;
}

public enum ErrorCode
{
    SUCCESS,
    INVALID_INIT,
    INVALID_INDEX,
    INVALID_VECTOR,
    INVALID_RESULT,
    INVALID_DIMENSIONS,
    INVALID_ARGUMENT,
    INVALID_ID,
    INVALID_REF,
    INVALID_METHOD,
    DUPLICATED_ENTRY,
    NOT_FOUND_ID,
    INDEX_EMPTY,
    THREAD_ERROR,
    SYSTEM_ERROR,
    FILEIO_ERROR,
    NOT_IMPLEMENTED,
    INVALID_FILE,
}

