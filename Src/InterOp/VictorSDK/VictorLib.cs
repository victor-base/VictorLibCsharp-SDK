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
using System.Collections.Generic;
#nullable enable

namespace Victor;

public partial class VictorSDK : IDisposable
{
    private readonly INativeMethods _native;
    private IntPtr _index;
    private readonly List<VectorEntry> _insertedVectors = new();
    private bool _disposedFlag;


    // Constructor privado para inicializar desde un índice cargado
    private VictorSDK(IntPtr index)
    {
        _native = NativeMethodsFactory.Create();
        _index = index;
        Debug.WriteLine("\nIndex loaded successfully.\n");
    }

    /// <summary>
    /// ENG: Creates a new vector index. ESP: Crea un nuevo índice de vectores.
    /// </summary>
    /// <param name="type">ENG: Index type (FLAT, HNSW, NSW). ESP: Tipo de índice (FLAT, HNSW, NSW).</param>
    /// <param name="method">ENG: Distance metric. ESP: Métrica de distancia.</param>
    /// <param name="dims">ENG: Number of dimensions. ESP: Número de dimensiones.</param>
    /// <param name="context">ENG: Optional context for advanced configuration. ESP: Contexto opcional para configuración avanzada.</param>
    public VictorSDK(IndexType type, DistanceMethod method, ushort dims, object? context = null)
    {
        _native = NativeMethodsFactory.Create();

        IntPtr ctxPtr = IntPtr.Zero;

        _index = _native.alloc_index(type, method, dims, ctxPtr);
        Debug.WriteLine($"Index created: {_index}");

    }

#nullable disable

    public IReadOnlyList<VectorEntry> GetInsertedVectors() => _insertedVectors;

}



public partial class VictorSDK
{
    /// <summary>
    /// ENG: Updates the index context (parameters) at runtime. ESP: Actualiza el contexto (parámetros) del índice en tiempo de ejecución.
    /// </summary>
    /// <param name="context">ENG: HNSW context. ESP: Contexto HNSW.</param>
    /// <param name="mode">ENG: Update mode. ESP: Modo de actualización.</param>
    public void UpdateContext(HNSWContext context, int mode)
    {
        IntPtr ptr = HNSWContext.ToPointer(context);
        int status = _native.update_icontext(_index, ptr, mode);
        Marshal.FreeHGlobal(ptr);
        ThrowIfError((ErrorCode)status);
    }



    /// <summary>
    /// ENG: Gets the name of the current index. ESP: Obtiene el nombre del índice actual.
    /// </summary>
    /// <returns>ENG: Index name. ESP: Nombre del índice.</returns>
    public string GetIndexName()
    {
        IntPtr ptr = _native.index_name(_index);
        return Marshal.PtrToStringAnsi(ptr);
    }

    /// <summary>
    /// ENG: Gets the full version of the Victor library. ESP: Obtiene la versión completa de la biblioteca Victor.
    /// </summary>
    /// <returns>ENG: Library version. ESP: Versión de la biblioteca.</returns>
    public string GetLibraryVersion()
    {
        IntPtr ptr = _native.__LIB_VERSION();
        return Marshal.PtrToStringAnsi(ptr);
    }


    public string GetShortVersion()
    {
        IntPtr ptr = _native.__LIB_SHORT_VERSION();
        if (ptr == IntPtr.Zero) throw new VictorException("\nInvalid Reference.\n", ErrorCode.INVALID_INIT);

        return Marshal.PtrToStringAnsi(ptr);
    }


    public void PrintDiagnostics()
    {
        Console.WriteLine($"Victor Library v{GetLibraryVersion()}");
        Console.WriteLine($"Index name: {GetIndexName()}");
        Console.WriteLine($"Index size: {GetSize()}");
    }
    public void PrintDiagnosticsShort()
    {
        Console.WriteLine($"Victor Library v{GetShortVersion()}");
        Console.WriteLine($"Index name: {GetIndexName()}");
        Console.WriteLine($"Index size: {GetSize()}");
    }

    public void ThrowIfError(ErrorCode code, int? additionalCode = null)
    {
        if (code == ErrorCode.SUCCESS) return; // No hay error, salir del método.

        IntPtr ptr = _native.victor_strerror(code);
        string msg = Marshal.PtrToStringAnsi(ptr);

        // Si se proporciona un código adicional, inclúyelo en el mensaje de error.
        if (additionalCode.HasValue)
        {
            throw new VictorException($"Victor error {code} (Additional Code: {additionalCode}): {msg}", code);
        }
        else
        {
            throw new VictorException($"Victor error {code}: {msg}", code);
        }
    }

    /// <summary>
    /// ENG: Inserts a vector into the index. ESP: Inserta un vector en el índice.
    /// </summary>
    /// <param name="id">ENG: Unique vector ID. ESP: ID único del vector.</param>
    /// <param name="vector">ENG: Vector data. ESP: Datos del vector.</param>
    /// <param name="dims">ENG: Number of dimensions. ESP: Número de dimensiones.</param>
    /// <returns>ENG: Status code. ESP: Código de estado.</returns>
    public int Insert(ulong id, float[] vector, ushort dims)
    {
        if (_index == IntPtr.Zero) throw new VictorException(ErrorCode.INVALID_INIT);

        if (vector == null || vector.Length == 0) throw new VictorException("Vector is null or empty.", ErrorCode.INVALID_VECTOR);

        if (vector.Length != dims) throw new VictorException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n", ErrorCode.INVALID_DIMENSIONS);

        int status = _native.insert(_index, id, vector, dims);

        if (status != 0) throw new VictorException($"Insert failed with code {status}", (ErrorCode)status);

        Debug.WriteLine($"\nVector with ID {id} inserted succesfully.\n");
        return status;
    }


    /// <summary>
    /// ENG: Searches for the closest vector in the index. ESP: Busca el vector más cercano en el índice.
    /// </summary>
    /// <param name="vector">ENG: Query vector. ESP: Vector de consulta.</param>
    /// <param name="dims">ENG: Number of dimensions. ESP: Número de dimensiones.</param>
    /// <returns>ENG: Match result. ESP: Resultado de la búsqueda.</returns>
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


    /// <summary>
    /// ENG: Searches for the N closest vectors in the index. ESP: Busca los N vectores más cercanos en el índice.
    /// </summary>
    /// <param name="vector">ENG: Query vector. ESP: Vector de consulta.</param>
    /// <param name="dims">ENG: Number of dimensions. ESP: Número de dimensiones.</param>
    /// <param name="n">ENG: Number of results. ESP: Número de resultados.</param>
    /// <returns>ENG: Array of match results. ESP: Arreglo de resultados de búsqueda.</returns>
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
            Debug.WriteLine("Recursos liberados");
        }
    }




    /// <summary>
    /// ENG: Gets aggregated statistics of the index. ESP: Obtiene estadísticas agregadas del índice.
    /// </summary>
    /// <returns>ENG: Index statistics. ESP: Estadísticas del índice.</returns>
    public IndexStatsResult GetStats()
    {
        if (_index == IntPtr.Zero) throw new InvalidOperationException("\nIndex not created.\n");

        IntPtr statsPtr = Marshal.AllocHGlobal(Marshal.SizeOf<IndexStatsResult>());

        try
        {
            int status = _native.stats(_index, statsPtr);
            if (status != 0) throw new VictorException("\nError retrieving index statistics.\n");

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
