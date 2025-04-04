// Copyright (C) 2025 Iván E. Rodríguez. Contact: ivanrwcm25@gmail.com
//
// This file is part of the Victor Base binding for .NET.
// The original Victor Base core project was created by Emiliano Billi (email: emiliano.billi@gmail.com).
//
// This program is free software: you can redistribute it and/or modify  
// it under the terms of the GNU General Public License as published  
// by the Free Software Foundation, either version 3 of the License,  
// or (at your option) any later version.  
//
// This program is distributed in the hope that it will be useful,  
// but WITHOUT ANY WARRANTY; without even the implied warranty of  
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the  
// GNU General Public License for more details.  
//
// You should have received a copy of the GNU General Public License  
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Runtime.InteropServices;
using VictorNative;
using Src.Common;

namespace Victor
{
    /// <summary>
    /// Clase principal para interactuar con el índice nativo. Proporciona funcionalidad de inserción, búsqueda y eliminación de vectores.
    /// </summary>
    public class VictorSDK : IDisposable
    {
        private IntPtr _index; // Puntero al índice nativo
        private bool _disposedFlag; // Flag para evitar liberar más de una vez la memoria.

        /// <summary>
        /// Inicializa un nuevo índice con los parámetros especificados.
        /// </summary>
        /// <param name="type">Tipo de índice.</param>
        /// <param name="method">Método de búsqueda.</param>
        /// <param name="dims">Número de dimensiones del índice.</param>
        /// <param name="context">Contexto opcional. Se deja como puntero nulo por defecto.</param>
        /// <exception cref="InvalidOperationException">Se lanza si el índice no puede ser inicializado.</exception>
        public VictorSDK(int type, int method, ushort dims, IntPtr context = default)
        {
            
            _index = NativeMethods.alloc_index(type, method, dims, context);

            if (_index == IntPtr.Zero)
            {
                throw new InvalidOperationException($"\nErr to initilice index: type={type}, method={method}, dims={dims}\n");
            }

            Console.WriteLine("\nIndex created succesfully.\n");
        }

        /// <summary>
        /// Inserta un vector en el índice asociado a un ID único.
        /// </summary>
        /// <param name="id">Identificador único del vector.</param>
        /// <param name="vector">Vector a insertar.</param>
        /// <param name="dims">Número de dimensiones del vector.</param>
        /// <returns>Código de estado devuelto por la función nativa.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el índice no está inicializado o si ocurre un error en la inserción.</exception>
        /// <exception cref="ArgumentException">Se lanza si las dimensiones del vector no coinciden con las especificadas.</exception>
        public int Insert(ulong id, float[] vector, ushort dims)
        {
            if (_index == IntPtr.Zero)
                throw new InvalidOperationException("\nIndex not created.\n");

            if (vector.Length != dims)
                throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

            int status = NativeMethods.insert(_index, id, vector, dims);

            if (status != 0)
                throw new InvalidOperationException($"\nErr with vector insert. status code: {status}\n");

            Console.WriteLine($"\nVector with ID {id} inserted succesfully.\n");
            return status;
        }

        /// <summary>
        /// Realiza una búsqueda en el índice y devuelve un único resultado.
        /// </summary>
        /// <param name="vector">Vector de búsqueda.</param>
        /// <param name="dims">Número de dimensiones del vector.</param>
        /// <returns>Resultado de la búsqueda, incluyendo ID y distancia.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el índice no está inicializado o si ocurre un error en la búsqueda.</exception>
        /// <exception cref="ArgumentException">Se lanza si las dimensiones del vector no coinciden con las especificadas.</exception>
        public MatchResult Search(float[] vector, ushort dims)
        {
            if (_index == IntPtr.Zero)
                throw new InvalidOperationException("\nIndex not created.\n");

            if (vector.Length != dims)
                throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

            IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatchResult)));

            try
            {
                int status = NativeMethods.search(_index, vector, dims, resultPtr);
                if (status != 0)
                    throw new InvalidOperationException($"\nErr in search: status code: {status}\n");

                MatchResult result = Marshal.PtrToStructure<MatchResult>(resultPtr);
                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(resultPtr);
            }
        }

        /// <summary>
        /// Realiza una búsqueda en el índice y devuelve múltiples resultados.
        /// </summary>
        /// <param name="vector">Vector de búsqueda.</param>
        /// <param name="dims">Número de dimensiones del vector.</param>
        /// <param name="n">Cantidad de resultados deseados.</param>
        /// <returns>Arreglo de resultados de búsqueda, incluyendo ID y distancia.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el índice no está inicializado o si ocurre un error en la búsqueda.</exception>
        public MatchResult[] Search_n(float[] vector, ushort dims, int n)
        {
            IntPtr resultsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatchResult)) * n);

            try
            {
                int status = NativeMethods.search_n(_index, vector, dims, resultsPtr, n);

                if (status != 0)
                {
                    throw new InvalidOperationException($"\nERR in search. status code: {status}\n");
                }

                MatchResult[] results = new MatchResult[n];
                for (int i = 0; i < n; i++)
                {
                    IntPtr resultOffset = IntPtr.Add(resultsPtr, i * Marshal.SizeOf(typeof(MatchResult)));
                    results[i] = Marshal.PtrToStructure<MatchResult>(resultOffset);
                }

                return results;
            }
            finally
            {
                Marshal.FreeHGlobal(resultsPtr);
            }
        }

        /// <summary>
        /// Elimina un vector del índice utilizando su ID.
        /// </summary>
        /// <param name="id">ID único del vector a eliminar.</param>
        /// <returns>Código de estado devuelto por la función nativa.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si el índice no está inicializado o si ocurre un error en la eliminación.</exception>
        public int Delete(ulong id)
        {
            if (_index == IntPtr.Zero)
                throw new InvalidOperationException("\nIndex not created.\n");

            int status = NativeMethods.delete(_index, id);

            if (status != 0)
                throw new InvalidOperationException($"\nERR: Can't eliminate vector with ID: {id}. status code: {status}\n");

            Console.WriteLine($"\nVector with ID: {id} eliminated succesfully.\n");
            return status;
        }

        /// <summary>
        /// Libera los recursos asociados al índice.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Lógica centralizada para liberar recursos.
        /// </summary>
        /// <param name="disposing">Verdadero si se llama explícitamente desde Dispose; falso si desde el destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedFlag)
            {
                if (_index != IntPtr.Zero)
                {
                    NativeMethods.destroy_index(ref _index);
                    _index = IntPtr.Zero;
                    Console.WriteLine("Elements Destroyed succesfully.");
                }

                _disposedFlag = true;
            }
        }

        /// <summary>
        /// Destructor: fallback en caso de que Dispose no sea invocado.
        /// </summary>
        ~VictorSDK()
        {
            Dispose(false);
            Console.WriteLine("\nSecurity fallback on.");
            Console.WriteLine("Next time don't forget use Dispose() to free memory.\n");
        }
    }
}