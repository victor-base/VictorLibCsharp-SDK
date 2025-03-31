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

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr alloc_index(int type, int method, ushort dims, IntPtr icontext);

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int destroy_index(ref IntPtr index); // ref es necesario porque en C es un doble puntero

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int insert(IntPtr index, ulong id, float[] vector, ushort dims);

    [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);
}
