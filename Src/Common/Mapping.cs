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

using System.Runtime.InteropServices;
using System;

namespace VictorBaseDotNET.Src.Common;


public interface IMappableFrom<T>
{
    void MapFrom(T internalStruct);
}

public struct MatchResult
{
    public int MatchId;
    public float MatchData;

    internal MatchResult(InternalMatchResult internalStruct) // Constructor interno
    {
        MatchId = internalStruct.Id;
        MatchData = internalStruct.Distance;
    }
}

public struct NSWContextResult
{
    public int EfSearch;
    public int EfConstruct;
    public int Odegree;

    internal NSWContextResult(NSWContext internalStruct)
    {
        EfSearch = internalStruct.EfSearch;
        EfConstruct = internalStruct.EfConstruct;
        Odegree = internalStruct.Odegree;
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
    public ulong InsertCount;
    public ulong DeleteCount;
    public ulong SearchCount;
    public ulong SearchNCount;
    public double TotalTime;
    public double LastOperationTime;
    public double MinOperationTime;
    public double MaxOperationTime;

    internal IndexStatsResult(InternalIndexStatsResult stats)
    {
        InsertCount = stats.InsertCount;
        DeleteCount = stats.DeleteCount;
        SearchCount = stats.SearchCount;
        SearchNCount = stats.SearchNCount;
        TotalTime = stats.TotalTime;
        LastOperationTime = stats.LastOperationTime;
        MinOperationTime = stats.MinOperationTime;
        MaxOperationTime = stats.MaxOperationTime;
    }

}


// Clase de mapeo para estructuras internas a estructuras públicas universales
internal static class StructMapper
{
    public static T Map<T, U>(U internalStruct) where T : struct
    {
        // Usa Activator.CreateInstance para invocar el constructor interno
        return (T)Activator.CreateInstance(typeof(T), internalStruct);
    }

    public static T[] MapArray<T, U>(IntPtr arrayPtr, int count) where T : struct where U : struct
    {
        T[] results = new T[count];
        for (int i = 0; i < count; i++)
        {
            IntPtr elementPtr = IntPtr.Add(arrayPtr, i * Marshal.SizeOf<U>());
            U internalStruct = Marshal.PtrToStructure<U>(elementPtr);
            results[i] = Map<T, U>(internalStruct);
        }
        return results;
    }
}