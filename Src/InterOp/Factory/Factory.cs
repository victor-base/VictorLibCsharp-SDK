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
 * En este archivo tenemos la clase responsable de crear instancias de INativeMethods dependiendo del sistema operativo usando un patrón Factory clásico.
 * Permitiendo la interoperabilidad entre el SDK de .NET y la biblioteca nativa de forma portable y elegnte.
 *
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NativeMethodsInterface;
using Victor.NativeMethods.Linux;
using Victor.NativeMethods.Windows;

namespace Victor.NativeMethods.Factory;
// Esta clase es responsable de crear instancias de INativeMethods dependiendo del sistema operativo.
internal static class NativeMethodsFactory
{
    public static INativeMethods Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Debug.WriteLine("Creando NativeMethodsWindows");
            return new NativeMethodsWindows();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {

            Debug.WriteLine("Creando NativeMethodsLinux");
            return new NativeMethodsLinux();
        }

        else throw new PlatformNotSupportedException("Your OS is not supported yet :(");
    }
}
