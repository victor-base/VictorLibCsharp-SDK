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
 *   Este archivo toma las structuras internas de la biblioteca nativa y las convierte en estructuras públicas
 *   para su uso en el SDK de .NET. Esto permite que las estructuras internas sean accesibles y utilizables sin que puedan tomarse desde su código nativo.
 */

using VictorBaseDotNET.Src.utils;
using System.Runtime.InteropServices;
using System;

namespace VictorBaseDotNET.Src.Common;


public struct PublicAsort
{
    public IntPtr Heap; // Puntero a la estructura interna de asort

    internal PublicAsort(InternalAsort internalStruct) => Heap = internalStruct.Heap;

}


public struct MatchResult
{
    public int Label;
    public float Distance;

    public MatchResult(InternalMatchResult internalStruct)
    {
        Label = internalStruct.Label;
        Distance = internalStruct.Distance;
    }
}

public struct NSWContextResult
{
    public int EFSEARCH;
    public int EF_CONSTRUCT;
    public int ODEGREE;

    internal NSWContextResult(NSWContext internalStruct)
    {
        EFSEARCH = internalStruct.EFSEARCH;
        EF_CONSTRUCT = internalStruct.EF_CONSTRUCT;
        ODEGREE = internalStruct.ODEGREE;
    }
}

public struct TimeStatResult
{
    public ulong Count;
    public double Total;
    public double Last;
    public double Min;
    public double Max;

    internal TimeStatResult(TimeStat internalStruct)
    {
        Count = internalStruct.Count;
        Total = internalStruct.Total;
        Last = internalStruct.Last;
        Min = internalStruct.Min;
        Max = internalStruct.Max;
    }
}

public struct IndexStatsResult
{
    internal TimeStat Insert;
    internal TimeStat Delete; // O Remove, dependiendo del entorno
    internal TimeStat Dump;
    internal TimeStat Search;
    internal TimeStat SearchN;

    internal IndexStatsResult(InternalIndexStatsResult stats)
    {
        Insert = stats.Insert;
        Delete = stats.Delete;
        Search = stats.Search;
        Dump = stats.Dump;
        SearchN = stats.SearchN;
    }

    public override string ToString() =>

     $"Insert: {Insert.Count}, Delete: {Delete.Count}, Dump: {Dump.Count}, Search: {Search.Count}, SearchN: {SearchN.Count}";
      
}
internal static class StructMapper
{
    // Mapea de InternalAsort a PublicAsort
    internal static PublicAsort MapToPublic(InternalAsort internalAsort)
    {
        return new PublicAsort(internalAsort);
    }

    // Mapea de PublicAsort a InternalAsort
    internal static InternalAsort MapToInternal(PublicAsort asort)
    {
        return new InternalAsort { Heap = asort.Heap };
    }

    // Mapea de InternalAsort a Asort (estructura interop)
    internal static Asort MapToInterop(InternalAsort internalAsort)
    {
        return new Asort { Heap = internalAsort.Heap };
    }

    // Mapea de Asort (estructura interop) a InternalAsort
    internal static InternalAsort MapFromInterop(Asort asort)
    {
        return new InternalAsort { Heap = asort.Heap };
    }

    // Mapea de InternalMatchResult a MatchResult
    internal static MatchResult Map(InternalMatchResult internalResult)
    {
        return new MatchResult(internalResult);
    }

    // Mapea un arreglo de InternalMatchResult a MatchResult[]
    public static MatchResult[] MapArray(IntPtr resultsPtr, int count)
    {
        var results = new MatchResult[count];
        for (int i = 0; i < count; i++)
        {
            var internalResult = Marshal.PtrToStructure<InternalMatchResult>(
                IntPtr.Add(resultsPtr, i * Marshal.SizeOf<InternalMatchResult>())
            );
            results[i] = Map(internalResult);
        }
        return results;
    }
    // Mapea de InternalIndexStatsResult a IndexStatsResult
    internal static IndexStatsResult MapStats(InternalIndexStatsResult internalStats)
    {
        return new IndexStatsResult(internalStats);
    }
    // Mapea de IndexStatsResult a InternalIndexStatsResult
    internal static InternalIndexStatsResult MapToInternal(IndexStatsResult stats)
    {
        return new InternalIndexStatsResult
        {
            Insert = stats.Insert,
            Delete = stats.Delete,
            Dump = stats.Dump,
            Search = stats.Search,
            SearchN = stats.SearchN
        };
    }

}

