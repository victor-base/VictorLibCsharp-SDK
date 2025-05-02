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
using System.Runtime.InteropServices;
using VictorNative;
using VictorBaseDotNET.Src.Common;

namespace Victor;

public partial class VictorSDK : IDisposable
{
    private IntPtr _index;
    private bool _disposedFlag;


    public VictorSDK(int type, int method, ushort dims, IntPtr context = default)
    {
        _index = NativeMethods.alloc_index(type, method, dims, context);

        if (_index == IntPtr.Zero) throw new InvalidOperationException($"\nErr to initilice index: type={type}, method={method}, dims={dims}\n");
        

        System.Diagnostics.Debug.WriteLine("\nIndex created succesfully.\n");
    }
}




public partial class VictorSDK : IDisposable
{

    public int Insert(ulong id, float[] vector, ushort dims)
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

        int status = NativeMethods.insert(_index, id, vector, dims);

        if (status != 0) throw new InvalidOperationException($"\nErr with vector insert. status code: {status}\n");

        System.Diagnostics.Debug.WriteLine($"\nVector with ID {id} inserted succesfully.\n");
        return status;
    }



    public MatchResult Search(float[] vector, ushort dims)
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

        IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InternalMatchResult>());

        try
        {
            int status = NativeMethods.search(_index, vector, dims, resultPtr);
            if (status != 0) throw new InvalidOperationException($"\nErr in search: status code: {status}\n");

            InternalMatchResult internalResult = Marshal.PtrToStructure<InternalMatchResult>(resultPtr);
            MatchResult result = StructMapper.Map<MatchResult, InternalMatchResult>(internalResult);

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(resultPtr);
        }
    }

    public MatchResult[] Search_n(float[] vector, ushort dims, int n)
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

        IntPtr resultsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InternalMatchResult>() * n);

        try
        {
            int status = NativeMethods.search_n(_index, vector, dims, resultsPtr, n);
            if (status != 0) throw new InvalidOperationException($"\nERR in search. status code: {status}\n");

            MatchResult[] results = StructMapper.MapArray<MatchResult, InternalMatchResult>(resultsPtr, n);

            return results;
        }
        finally
        {
            Marshal.FreeHGlobal(resultsPtr);
        }
    }




    public IndexStatsResult GetStats()
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        IntPtr statsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IndexStatsResult>());

        try
        {
            int status = NativeMethods.stats(_index, statsPtr);
            if (status != 0) throw new InvalidOperationException("\nError retrieving index statistics.\n");

            IndexStatsResult stats = Marshal.PtrToStructure<IndexStatsResult>(statsPtr);
            return stats;
        }
        finally
        {
            Marshal.FreeHGlobal(statsPtr);
        }
    }


    /// <summary>
    /// Releases resources used by the instance and suppresses finalization.
    /// </summary>
    /// <remarks>
    /// Ensures managed and unmanaged resources are properly released to avoid memory leaks.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedFlag)
        {
            if (_index != IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine("Elements Destroyed succesfully.");
                NativeMethods.destroy_index(ref _index);
                _index = IntPtr.Zero;
            }

            _disposedFlag = true;
        }
    }

    ~VictorSDK()
    {
        Dispose(false);
        System.Diagnostics.Debug.WriteLine("\nSecurity fallback on.");
        System.Diagnostics.Debug.WriteLine("Next time don't forget use Dispose() to free memory.\n");
    }
}
