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
 *  Este es el SDK de Victor. Estos son todos los métodos que, como usuario, deberías poder usar. 
 *  La clase esta fragmentada en partes para que sea más fácil de leer. (aún hay que mejorar la legibilidad)
 *
 */


using System;
using System.Runtime.InteropServices;
using VictorBaseDotNET.Src.Common;
using VictorBaseDotNET.Src.utils;
using NativeMethodsInterface;
using Victor.NativeMethods.Factory;
using System.Diagnostics;
using VictorExceptions;

namespace Victor;

public partial class VictorSDK : IDisposable
{
    private readonly INativeMethods _native;
    private IntPtr _index;
    private bool _disposedFlag;

    public VictorSDK(IndexType type, DistanceMethod method, ushort dims, HNSWContext? context = null)
    {
        _native = NativeMethodsFactory.Create() ?? throw new InvalidOperationException("Native methods could not be initialized.");

        IntPtr ctxPtr = IntPtr.Zero;
        if (type == IndexType.HNSW)
        {
            if (context == null) throw new InvalidOperationException("HNSW index requires a valid context.");

            ctxPtr = HNSWContext.ToPointer(context.Value);
        }

        _index = _native.alloc_index(type, method, dims, ctxPtr);
        Debug.WriteLine($"Index created: {_index}");
       
        if (_index == IntPtr.Zero) throw new InvalidOperationException("Error al inicializar el índice.");
    }


}


public partial class VictorSDK
{

    public void UpdateContext(HNSWContext context, int mode)
    {
        IntPtr ptr = HNSWContext.ToPointer(context);
        int status = _native.update_icontext(_index, ptr, mode);
        Marshal.FreeHGlobal(ptr);
        ThrowIfError(status);
    }

    public void InitHNSW(HNSWContext config, int method, ushort dims)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<HNSWContext>());
        Marshal.StructureToPtr(config, ptr, false);

        int code = _native.hnsw_index(_index, method, dims, ptr);
        Marshal.FreeHGlobal(ptr);

        ThrowIfError(code);
    }



    internal string GetIndexName()
    {
        IntPtr ptr = _native.index_name(_index);
        return Marshal.PtrToStringAnsi(ptr);
    }

    internal string GetLibraryVersion()
    {
        IntPtr ptr = _native.__LIB_VERSION();
        return Marshal.PtrToStringAnsi(ptr);
    }

    internal string GetShortVersion()
    {
        IntPtr ptr = _native.__LIB_SHORT_VERSION();
        return Marshal.PtrToStringAnsi(ptr);
    }


    public void PrintDiagnostics()
    {
        Console.WriteLine($"Victor Library v{GetLibraryVersion()}");
        Console.WriteLine($"Index name: {GetIndexName()}");
        Console.WriteLine($"Index size: {GetSize()}");
    }

    public void ThrowIfError(int code)
    {
        if (code == 0) return;

        IntPtr ptr = _native.victor_strerror((ErrorCode)code);
        string msg = Marshal.PtrToStringAnsi(ptr);

        throw new VictorException((ErrorCode)code, $"Victor error {code}: {msg}");
    }


    public int Insert(ulong id, float[] vector, ushort dims)
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

        int status = _native.insert(_index, id, vector, dims);

        if (status != 0) throw new InvalidOperationException($"\nErr with vector insert. status code: {status}\n");

        Debug.WriteLine($"\nVector with ID {id} inserted succesfully.\n");
        return status;
    }



    public MatchResult Search(float[] vector, ushort dims)
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

        IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InternalMatchResult>());

        try
        {
            int status = _native.search(_index, vector, dims, resultPtr);
            if (status != 0) throw new InvalidOperationException($"\nErr in search: status code: {status}\n");

            InternalMatchResult internalResult = Marshal.PtrToStructure<InternalMatchResult>(resultPtr);
            MatchResult result = StructMapper.Map(internalResult);
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

        if (vector.Length != dims) throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions ({dims}).\n");

        IntPtr resultsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InternalMatchResult>() * n);

        try
        {
            int status = _native.search_n(_index, vector, dims, resultsPtr, n);
            if (status != 0) throw new InvalidOperationException($"\nERR in search_n. status code: {status}\n");

            return StructMapper.MapArray(resultsPtr, n);
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
            int status = _native.stats(_index, statsPtr);
            if (status != 0) throw new InvalidOperationException("\nError retrieving index statistics.\n");

            IndexStatsResult stats = Marshal.PtrToStructure<IndexStatsResult>(statsPtr);
            Debug.WriteLine($"Index statistics: {stats}");
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
                Debug.WriteLine("Elements Destroyed succesfully.");
                _native.destroy_index(ref _index);
                _index = IntPtr.Zero;
            }

            _disposedFlag = true;
        }
    }

    // Destructor. Probably not needed, but kept for safety.
    // It will be called if Dispose() is not called explicitly. which is less probable.
    ~VictorSDK()
    {
        Dispose(false);
        Debug.WriteLine("\nSecurity fallback on.");
        Debug.WriteLine("Next time don't forget use Dispose() to free memory.\n");
    }
}
