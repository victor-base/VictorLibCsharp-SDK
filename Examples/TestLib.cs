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
using System.IO;


namespace LibTest
{
internal class TestLoadLibrary
{
    public static void LibLoader()
    {
        try
        {
            string libraryName = @"D:\Users\pc\Desktop\VictorCsharpBinding-SDK\libvictorTEST.dll";  // Ruta completa de la DLL hardcodeada (solo para testear)

            Console.WriteLine($"\nIntentando cargar la biblioteca: {libraryName}\n");

            // Verificar si la dll existe
            if (!File.Exists(libraryName))
            {
                Console.WriteLine($"\nError: El archivo DLL no existe en la ruta especificada: {libraryName}\n");
                return;
            }

            IntPtr libraryHandle = LoadLibrary(libraryName);

            if (libraryHandle == IntPtr.Zero)
            {
                Console.WriteLine("\nNo se pudo cargar la biblioteca.\n");
            }
            else
            {
                Console.WriteLine("\nBiblioteca cargada correctamente.\n");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n Error al cargar la librería: {ex.Message}\n");
        }
    }

    // Declaración de la función LoadLibrary de kernel32.dll. No se que carajo hace.
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr LoadLibrary(string lpFileName);
}
}