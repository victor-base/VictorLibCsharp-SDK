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

namespace VictorBaseDotNET.Src.Common;


[StructLayout(LayoutKind.Sequential)]

internal struct InternalMatchResult
{
    public int Id;
    public float Distance;
}


[StructLayout(LayoutKind.Sequential)]
internal struct NSWContext
{
    public int EfSearch;
    public int EfConstruct;
    public int Odegree;
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
