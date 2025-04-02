using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VictorNative
{
    public static class NativeMethods
    {

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr alloc_index(int type, int method, ushort dims, IntPtr icontext);

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int destroy_index(ref IntPtr index);

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int search_n(IntPtr index, float[] vector, ushort dims, IntPtr results, int n);

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int search(IntPtr index, float[] vector, ushort dims, IntPtr result);

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int insert(IntPtr index, ulong id, float[] vector, ushort dims);

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int delete(IntPtr index, ulong id);

    }


}

