// using System;
// using System.Runtime.InteropServices;
// using VictorNative;
// using Src.Common;


// namespace VictorSDK
// {
//     public class VictorIndex : IDisposable 
//     {
//         internal IntPtr _index;
//         private bool _disposedFlag = false;
//         /// <summary>
//         /// Crea un índice en la base de datos.
//         /// </summary>
//         /// <param name="type">Tipo de índice.</param>
//         /// <param name="method">Método de búsqueda.</param>
//         /// <param name="dims">Número de dimensiones.</param>
//         /// <param name="context">Contexto opcional.</param>
//         public VictorIndex(int type, int method, ushort dims, IntPtr context = default) // Constructor de instancia
//         {
//             _index = NativeMethods.alloc_index(type, method, dims, context);
//             if (_index == IntPtr.Zero)// esto es equivalente a if *index == NULL en c/cpp
//             {
//                 throw new InvalidOperationException("Index cannot be asigned.");
//             }
//         }

//         /// <summary>
//         /// Inserta un vector en el índice.
//         /// </summary>
//         public void InsertVector(ulong id, float[] vector)
//         {
//             if (_index == IntPtr.Zero) throw new ObjectDisposedException(nameof(VictorIndex));

//             int result = NativeMethods.insert(_index, id, vector, (ushort)vector.Length);
//             if (result != 0)
//             {
//                 throw new InvalidOperationException($"ERR: Cant insert data. CODE:{result}");
//             }
//         }

//         /// <summary>
//         /// Realiza una búsqueda en el índice.
//         /// </summary>
//         public void Search(float[] vector, int n, out IntPtr results)
//         {
//             if (_index == IntPtr.Zero)
//                 throw new ObjectDisposedException(nameof(VictorIndex));

//             // Asignar memoria para el puntero de resultados
//             results = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatchResult)) * n);

//             // Llamar a la función nativa search_n para obtener los resultados
//             int result = NativeMethods.search_n(_index, vector, (ushort)vector.Length, results, n);

//             // Verificar si hubo algún error
//             if (result != 0 || results == IntPtr.Zero)
//             {
//                 Marshal.FreeHGlobal(results);  // Liberar memoria si hubo un error
//                 throw new InvalidOperationException($"Error en la búsqueda: Código de error: {result}");
//             }

//             // Si la búsqueda fue exitosa, el puntero 'results' contiene los resultados
//             // No es necesario hacer nada adicional aquí, ya que los resultados se llenaron en la memoria asignada.
//         }


//         /// <summary>
//         /// Libera el índice ya que los objetos nativos de C/C++ NO SON MANEJADOS POR EL GARBAGE COLLECTOR DE C#/.NET. Entonces se invoca a un destructor en Dispose.
//         /// Si el usuario NO LLAMA EL DISPOSE, Se invoca a un destructor automáticamente para proteger el sistema en caso de que el memory management del bajo nivel falle.
//         /// </summary>
//         /// 
//         // Manager central de liberación de memoria. Es virtual para que, en caso de escala, pueda ser exportado a otros archivos y escalado internamente gracias al polimorfismo.
//         protected virtual void Dispose(bool disposing)
//         {
//             if (!_disposedFlag)
//             {
//                 if (_index != IntPtr.Zero)
//                 {
//                     NativeMethods.destroy_index(ref _index);
//                     _index = IntPtr.Zero;
//                 }
//                 _disposedFlag = true;
//             }
//         }

//         // Método público para liberar recursos manualmente
//         public void Dispose()
//         {
//             Dispose(true);
//             GC.SuppressFinalize(this); // Evita que el garbage llame al destructor automáticamente.
//         }

//         // Destructor (fallback de seguridad)
//         ~VictorIndex()
//         {
//             Dispose(false);
//         }
//     }
// }
