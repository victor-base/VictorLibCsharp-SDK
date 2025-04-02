using System;
using System.Runtime.InteropServices;
using VictorNative;
using Src.Common;

namespace VictorSDK
{
    /// <summary>
    /// Clase principal para interactuar con el índice nativo. Proporciona funcionalidad de inserción, búsqueda y eliminación de vectores.
    /// </summary>
    public class Victor : IDisposable
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
        public Victor(int type, int method, ushort dims, IntPtr context = default)
        {
            Console.WriteLine("\nInicializando índice...\n");
            _index = NativeMethods.alloc_index(type, method, dims, context);

            if (_index == IntPtr.Zero)
            {
                throw new InvalidOperationException($"\nError al asignar el índice: tipo={type}, método={method}, dims={dims}\n");
            }

            Console.WriteLine("\nÍndice inicializado correctamente.\n");
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
                throw new InvalidOperationException("\nEl índice no está inicializado.\n");

            if (vector.Length != dims)
                throw new ArgumentException($"\nEl tamaño del vector ({vector.Length}) no coincide con las dimensiones especificadas ({dims}).\n");

            int status = NativeMethods.insert(_index, id, vector, dims);

            if (status != 0)
                throw new InvalidOperationException($"\nError al insertar el vector: código de estado {status}\n");

            Console.WriteLine($"\nVector con ID {id} insertado correctamente.\n");
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
                throw new InvalidOperationException("\nÍndice no inicializado.\n");

            if (vector.Length != dims)
                throw new ArgumentException($"\nEl tamaño del vector ({vector.Length}) no coincide con las dimensiones dadas ({dims}).\n");

            IntPtr resultPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatchResult)));

            try
            {
                int status = NativeMethods.search(_index, vector, dims, resultPtr);
                if (status != 0)
                    throw new InvalidOperationException($"\nError en la búsqueda: código de estado {status}\n");

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
                    throw new InvalidOperationException($"\nError en la búsqueda: código de estado {status}\n");
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
                throw new InvalidOperationException("\nÍndice no inicializado.\n");

            int status = NativeMethods.delete(_index, id);

            if (status != 0)
                throw new InvalidOperationException($"\nError al eliminar el vector con ID {id}: código de estado {status}\n");

            Console.WriteLine($"\nVector con ID {id} eliminado correctamente.\n");
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
                }

                _disposedFlag = true;
            }
        }

        /// <summary>
        /// Destructor: fallback en caso de que Dispose no sea invocado.
        /// </summary>
        ~Victor()
        {
            Dispose(false);
            Console.WriteLine("\nSecurity fallback on.");
            Console.WriteLine("Next time don't forget use Dispose() to free memory.\n");
        }
    }
}