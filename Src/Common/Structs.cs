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
    FLAT = 0x00,
    NSW  = 0x02,
    HNSW = 0x03
}


public enum DistanceMethod
{
    EUCLIDIAN = 0x00,
    DOTPROD = 0x01,
    COSINE = 0x02
}

[StructLayout(LayoutKind.Sequential)]
public struct NSWContext
{
    public int EFSEARCH;
    public int EF_CONSTRUCT;
    public int ODEGREE;

    public static NSWContext Create(int efConstruct = 240, int efSearch = 240, int odegree = 32)
    {
        if (efConstruct <= 0) throw new ArgumentException("efConstruct debe ser mayor que 0.", nameof(efConstruct));
        if (efSearch <= 0) throw new ArgumentException("efSearch debe ser mayor que 0.", nameof(efSearch));
        if (odegree <= 0) throw new ArgumentException("odegree debe ser mayor que 0.", nameof(odegree));

        return new NSWContext
        {
            EF_CONSTRUCT = efConstruct,
            EFSEARCH = efSearch,
            ODEGREE = odegree
        };
    }

    public static IntPtr ToPointer(NSWContext context)
    {
        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<NSWContext>());
        Marshal.StructureToPtr(context, ptr, false);
        return ptr;
    }
}




[StructLayout(LayoutKind.Sequential)]
public struct HNSWContext
{
    public int EFSEARCH;
    public int EF_CONSTRUCT;
    public int M0;

    public static HNSWContext Create(int efConstruct = 240, int efSearch = 240, int m0 = 32)
    {
        if (efConstruct <= 0) throw new ArgumentException("efConstruct debe ser mayor que 0.", nameof(efConstruct));
        if (efSearch <= 0) throw new ArgumentException("efSearch debe ser mayor que 0.", nameof(efSearch));
        if (m0 <= 0) throw new ArgumentException("m0 debe ser mayor que 0.", nameof(m0));

        return new HNSWContext
        {
            EF_CONSTRUCT = efConstruct,
            EFSEARCH = efSearch,
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
    internal TimeStat(TimeStat internalStat)
    {
        Count = internalStat.Count;
        Total = internalStat.Total;
        Last = internalStat.Last;
        Min = internalStat.Min;
        Max = internalStat.Max;
    }
}



[StructLayout(LayoutKind.Sequential)]
internal struct InternalIndexStatsResult
{
    public TimeStat Insert;
    public TimeStat Delete; // O Remove, dependiendo del entorno
    public TimeStat Dump;
    public TimeStat Search;
    public TimeStat SearchN;
    internal InternalIndexStatsResult(InternalIndexStatsResult internalStats)
    {
        Insert = new TimeStat(internalStats.Insert);
        Delete = new TimeStat(internalStats.Delete);
        Dump = new TimeStat(internalStats.Dump);
        Search = new TimeStat(internalStats.Search);
        SearchN = new TimeStat(internalStats.SearchN);
    }
}



// The Asort struct is defined to represent the internal state of the asort algorithm, which is used for sorting and managing the results of vector searches. It contains a single field, Heap, which is a pointer to the heap memory used by the asort algorithm.

[StructLayout(LayoutKind.Sequential)]
public struct Asort
{
    public IntPtr Heap;

    public Asort(IntPtr heap) => Heap = heap;
}


/// <summary>
/// ENG: Represents error codes that can be returned by the system. ESP: Representa los códigos de error que pueden ser devueltos por el sistema.
/// </summary>
/// <remarks>
/// ENG: This enumeration provides a set of predefined error codes to indicate 
/// various error conditions that may occure during execution. 
/// ESP: Esta enumeración proporciona un conjunto de códigos de error predefinidos para indicar
/// diversas condiciones de error que pueden ocurrir durante la ejecución.
/// </remarks>
///
/// <para>English:</para>
/// <list type="bullet">
/// <item><description><c>SUCCESS</c>: Operation completed successfully.</description></item>
/// <item><description><c>INVALID_INIT</c>: Initialization failed or invalid.</description></item>
/// <item><description><c>INVALID_INDEX</c>: Index provided is invalid.</description></item>
/// <item><description><c>INVALID_VECTOR</c>: Vector provided is invalid.</description></item>
/// <item><description><c>INVALID_RESULT</c>: Result is invalid or unexpected.</description></item>
/// <item><description><c>INVALID_DIMENSIONS</c>: Dimensions provided are invalid.</description></item>
/// <item><description><c>INVALID_ARGUMENT</c>: Argument provided is invalid.</description></item>
/// <item><description><c>INVALID_ID</c>: Identifier provided is invalid.</description></item>
/// <item><description><c>INVALID_REF</c>: Reference provided is invalid.</description></item>
/// <item><description><c>INVALID_METHOD</c>: Method called is invalid or unsupported.</description></item>
/// <item><description><c>DUPLICATED_ENTRY</c>: Entry already exists and cannot be duplicated.</description></item>
/// <item><description><c>NOT_FOUND_ID</c>: Identifier not found in the system.</description></item>
/// <item><description><c>INDEX_EMPTY</c>: Index is empty or uninitialized.</description></item>
/// <item><description><c>THREAD_ERROR</c>: Error occurred in thread execution.</description></item>
/// <item><description><c>SYSTEM_ERROR</c>: General system error occurred.</description></item>
/// <item><description><c>FILEIO_ERROR</c>: File input/output error occurred.</description></item>
/// <item><description><c>NOT_IMPLEMENTED</c>: Feature or method is not implemented.</description></item>
/// <item><description><c>INVALID_FILE</c>: File provided is invalid or corrupted.</description></item>
/// </list>
///
/// <para>Español:</para>
/// <list type="bullet">
/// <item><description><c>SUCCESS</c>: Operación completada con éxito.</description></item>
/// <item><description><c>INVALID_INIT</c>: Inicialización fallida o inválida.</description></item>
/// <item><description><c>INVALID_INDEX</c>: El índice proporcionado es inválido.</description></item>
/// <item><description><c>INVALID_VECTOR</c>: El vector proporcionado es inválido.</description></item>
/// <item><description><c>INVALID_RESULT</c>: El resultado es inválido o inesperado.</description></item>
/// <item><description><c>INVALID_DIMENSIONS</c>: Las dimensiones proporcionadas son inválidas.</description></item>
/// <item><description><c>INVALID_ARGUMENT</c>: El argumento proporcionado es inválido.</description></item>
/// <item><description><c>INVALID_ID</c>: El identificador proporcionado es inválido.</description></item>
/// <item><description><c>INVALID_REF</c>: La referencia proporcionada es inválida.</description></item>
/// <item><description><c>INVALID_METHOD</c>: El método llamado es inválido o no soportado.</description></item>
/// <item><description><c>DUPLICATED_ENTRY</c>: La entrada ya existe y no puede duplicarse.</description></item>
/// <item><description><c>NOT_FOUND_ID</c>: Identificador no encontrado en el sistema.</description></item>
/// <item><description><c>INDEX_EMPTY</c>: El índice está vacío o no inicializado.</description></item>
/// <item><description><c>THREAD_ERROR</c>: Error ocurrido en la ejecución del hilo.</description></item>
/// <item><description><c>SYSTEM_ERROR</c>: Error general del sistema.</description></item>
/// <item><description><c>FILEIO_ERROR</c>: Error de entrada/salida de archivo.</description></item>
/// <item><description><c>NOT_IMPLEMENTED</c>: La característica o método no está implementado.</description></item>
/// <item><description><c>INVALID_FILE</c>: El archivo proporcionado es inválido o está corrupto.</description></item>
/// </list>
public enum ErrorCode
{
    SUCCESS = 0,
    INVALID_INIT = 1,
    INVALID_INDEX = 2,
    INVALID_VECTOR = 3,
    INVALID_RESULT = 4,
    INVALID_DIMENSIONS = 5,
    INVALID_ARGUMENT = 6,
    INVALID_ID = 7,
    INVALID_REF = 8,
    INVALID_METHOD = 9,
    DUPLICATED_ENTRY = 10,
    NOT_FOUND_ID = 11,
    INDEX_EMPTY = 12,
    THREAD_ERROR = 13,
    SYSTEM_ERROR = 14,
    FILEIO_ERROR = 15,
    NOT_IMPLEMENTED = 16,
    INVALID_FILE = 17,
}

