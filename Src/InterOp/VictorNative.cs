//////////////////////////////////////////////////////////////////////////////////////////
// English Documentation
//////////////////////////////////////////////////////////////////////////////////////////

// This file serves as the first point of contact with the low-level exposed functions 
// from the core database engine. It acts as a "wrapper" to create interoperability 
// between native C/C++ code and the .NET ecosystem (C#).
//
// First, preprocessor directives are used to identify the operating system 
// the code is being compiled on. Based on the platform (Windows, Linux, etc.), 
// the corresponding native libraries are loaded via the DllImport directive.
// This allows calling the core database functions, written in C, and exposing them 
// for use within the C# environment.
//
// Furthermore, the DllImport library is used to declare the native functions, 
// making them accessible as static methods within the NativeMethods class.
//
// This file provides basic interoperability for operations such as:

// - Index initialization
// - Database searching
// - Insertion and deletion of data
// - Memory and resource management
//
// The native functions referenced via DllImport are as follows:

// Example of a native function:
// [DllImport("victor_native.dll", CallingConvention = CallingConvention.Cdecl)]
// public static extern int InsertData(int id, string data);
//
// These functions must match exactly with the signatures defined in the C/C++ library.
//
// Note: Any changes made to the native functions in the core database (C/C++) must also be reflected here, 
// ensuring consistency between the C# and C/C++ implementations.

// --- Contact Information ---
// If you have any questions or suggestions, feel free to contact me at:
// Email: ivanrwcm25@gmail.com

//////////////////////////////////////////////////////////////////////////////////////////
// Documentación en Español
//////////////////////////////////////////////////////////////////////////////////////////

// NativeMethods.cs
//
// Este archivo se encarga de ser el primer punto de contacto con las funciones expuestas de bajo nivel en el núcleo de la base de datos (core).
// Funciona como un "wrapper" para crear interoperabilidad entre el código nativo (C/C++) y el ecosistema .NET (C#).
//
// En primer lugar, se utilizan directivas de preprocesamiento para identificar el sistema operativo en el que se está compilando el código.
// Dependiendo de la plataforma (Windows, Linux, etc.), se cargan las librerías nativas correspondientes mediante la directiva DllImport.
// Esto permite llamar a las funciones del núcleo de la base de datos que están escritas en C y exponerlas para su uso en el entorno C#.
//
// Además, la librería DllImport se utiliza para declarar las funciones nativas y hacerlas accesibles como métodos estáticos dentro de la clase NativeMethods.
//
// Este archivo proporciona la interoperabilidad básica para operaciones como:

// - Inicialización de índices
// - Búsqueda en la base de datos
// - Inserción y eliminación de datos
// - Gestión de la memoria y recursos
//
// A continuación, se detallan las funciones nativas a las que se hace referencia a través de DllImport:

// Ejemplo de función nativa:
// [DllImport("victor_native.dll", CallingConvention = CallingConvention.Cdecl)]
// public static extern int InsertData(int id, string data);
// 
// Estas funciones deben coincidir exactamente con las firmas definidas en la librería C/C++.
//
// Nota: Es importante que cualquier cambio en las funciones nativas en el núcleo de la base de datos (C/C++) se refleje también aquí, 
// manteniendo la coherencia entre ambas implementaciones (C# y C/C++).

// --- Información de contacto ---
// Por cualquier duda, sugerencia o consulta, contactar a:
// Email: ivanrwcm25@gmail.com

using System;
using System.Runtime.InteropServices;

static class NativeMethods
{
    private const string LIB_NAME =

#if WINDOWS
        "Victor.dll";
#elif LINUX
        "libvictor.so";
#elif OSX
        "libvictor.dylib";
#else
    throw new PlatformNotSupportedException("Unsupported OS -- SO no soportado.");

#endif

// LLamada a funciones del core. --  Call Low level functions.

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr alloc_index(int type, int method, ushort dims, IntPtr icontext);

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int destroy_index(ref IntPtr index); // ref es necesario porque en C es un doble puntero

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int insert(IntPtr index, ulong id, float[] vector, ushort dims);

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);
}

