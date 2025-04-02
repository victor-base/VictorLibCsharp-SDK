using System;
using System.IO;
using System.Runtime.InteropServices;

namespace VictorNative
{
    public static class NativeMethods
    {

        [DllImport("libvictorTEST.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr alloc_index(int type, int method, ushort dims, IntPtr icontext);
    
    
    }

}

