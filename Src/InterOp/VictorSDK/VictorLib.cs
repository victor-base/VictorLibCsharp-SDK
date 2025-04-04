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

    public class VictorSDK : IDisposable
    {
        private IntPtr _index; 
        private  bool _disposedFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="VictorSDK"/> class with the specified parameters.
        /// </summary>
        /// <remarks>
        /// This constructor creates an internal index structure by invoking the native allocation method.
        /// The parameters specify the type of index, the search method, and the dimensionality of the data.
        /// If the allocation fails (e.g., due to invalid parameters or insufficient resources), an exception
        /// is thrown to ensure proper error handling.
        /// </remarks>
        /// <param name="type">
        /// The type of index to create. This determines the underlying algorithm or data structure
        /// used for storing and querying vectors.
        /// </param>
        /// <param name="method">
        /// The search method to be used in the index. This affects how similarity or distance
        /// metrics are computed during queries.
        /// </param>
        /// <param name="dims">
        /// The number of dimensions for the vectors in the index. All inserted vectors must conform
        /// to this dimensionality; otherwise, insertion or search operations will fail.
        /// </param>
        /// <param name="context">
        /// Optional context for advanced configurations. By default, this is set to <c>IntPtr.Zero</c>.
        /// Use this parameter to provide additional settings or shared resources for the index.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the index fails to initialize due to invalid parameters or an error in the
        /// underlying native allocation function.
        /// </exception>
        /// <example>
        /// <code>
        /// try
        /// {
        ///     var sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        ///     Console.WriteLine("Index initialized successfully.");
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"Failed to initialize index: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public VictorSDK(int type, int method, ushort dims, IntPtr context = default)
        {
            
            _index = NativeMethods.alloc_index(type, method, dims, context);

            if (_index == IntPtr.Zero)
            {
                throw new InvalidOperationException($"\nErr to initilice index: type={type}, method={method}, dims={dims}\n");
            }

            System.Diagnostics.Debug.WriteLine("\nIndex created succesfully.\n");
        }

        /// <summary>
        /// Inserts a vector into the index associated with a unique identifier.
        /// </summary>
        /// <remarks>
        /// This method allows you to add data points (vectors) to the index, which can be later queried.
        /// The input vector must conform to the index's predefined dimensional constraints.
        /// If the vector dimensions do not match or the index is uninitialized, exceptions will be thrown.
        /// This operation relies on native resources to handle the insertion process and expects proper
        /// error handling for memory safety.
        /// </remarks>
        /// <param name="id">
        /// A unique identifier (ID) for the vector being inserted. The ID must be globally distinct within
        /// the index to avoid collisions or overwrites.
        /// </param>
        /// <param name="vector">
        /// The input vector to insert into the index. The size of this array must match the number of
        /// dimensions (`dims`) specified during the initialization of the index.
        /// </param>
        /// <param name="dims">
        /// The number of dimensions of the input vector. This value must match the index's configuration.
        /// Passing inconsistent dimensions will result in an <see cref="ArgumentException"/>.
        /// </param>
        /// <returns>
        /// An integer status code returned by the native insertion method. A successful insertion returns 0.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the index is not initialized or if an error occurs during the insertion process.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the size of the input vector does not match the specified number of dimensions (`dims`).
        /// This ensures data consistency within the index.
        /// </exception>
        public int Insert(ulong id, float[] vector, ushort dims)
        {
            if (_index == IntPtr.Zero)
                throw new InvalidOperationException("\nIndex not created.\n");

            if (vector.Length != dims)
                throw new ArgumentException($"\nVector size ({vector.Length}) doesn't match with dimensions :({dims}).\n");

            int status = NativeMethods.insert(_index, id, vector, dims);

            if (status != 0)
                throw new InvalidOperationException($"\nErr with vector insert. status code: {status}\n");

            System.Diagnostics.Debug.WriteLine($"\nVector with ID {id} inserted succesfully.\n");
            return status;
        }

        /// <summary>
        /// Performs a search in the index using the input vector and returns a single result.
        /// </summary>
        /// <remarks>
        /// This method queries the internal index with the provided vector and retrieves the
        /// closest match based on distance metrics such as cosine similarity or Euclidean distance,
        /// depending on the index configuration.
        /// The search relies on native resources for result computation, and the output is marshaled
        /// back into managed code. Proper handling of memory allocation is performed to ensure
        /// reliability.
        /// </remarks>
        /// <param name="vector">
        /// The query vector used for the search operation. Ensure the size of the vector
        /// matches the number of dimensions (`dims`) specified during the initialization of the index.
        /// </param>
        /// <param name="dims">
        /// The number of dimensions of the input vector. This must correspond to the
        /// dimensions defined at index creation. Passing an inconsistent value will
        /// trigger an <see cref="ArgumentException"/>.
        /// </param>
        /// <returns>
        /// A <see cref="MatchResult"/> object containing the ID and distance of the closest match
        /// from the index.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the index is not initialized or an error occurs during the search process.
        /// This typically points to an issue with the underlying native implementation.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the size of the input vector does not match the specified number of dimensions (`dims`).
        /// Ensure that both the vector and dimensions are consistent.
        /// </exception>
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
        /// Performs a search on the index and retrieves multiple results based on the given input vector.
        /// </summary>
        /// <remarks>
        /// This method searches through the internal index using the provided vector as a query.
        /// The method allocates native memory to temporarily store the search results, processes
        /// those results into an array of managed objects (`MatchResult[]`), and returns them to the caller.
        /// Ensure that the index is properly initialized before calling this method.
        /// </remarks>
        /// <param name="vector">
        /// The input search vector, representing the query to find similar vectors in the index.
        /// The size of the vector must match the number of dimensions (`dims`) specified during
        /// index creation; otherwise, an <see cref="ArgumentException"/> will be thrown.
        /// </param>
        /// <param name="dims">
        /// The number of dimensions of the input vector. This value must match the dimensions
        /// used when initializing the index. Passing an incorrect value will result in an
        /// <see cref="ArgumentException"/> being thrown.
        /// </param>
        /// <param name="n">
        /// The number of results to retrieve. This determines how many top matches (based on
        /// similarity or distance) will be returned by the search. Ensure this value is greater than 0.
        /// </param>
        /// <returns>
        /// An array of <see cref="MatchResult"/> objects, each containing the ID and distance
        /// of a match from the index. The array length will be equal to the requested number of results (`n`).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the index is uninitialized or an error occurs during the search process.
        /// This indicates a failure in the underlying native search implementation.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the size of the input vector does not match the number of specified dimensions (`dims`).
        /// Ensure that both parameters are consistent with the index configuration.
        /// </exception>
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
        /// Deletes a vector from the index using its unique identifier.
        /// </summary>
        /// <remarks>
        /// This method removes a vector from the index by its unique ID. The operation relies on
        /// the native implementation to locate and delete the corresponding vector efficiently.
        /// It is the caller's responsibility to ensure the index is properly initialized before
        /// invoking this method. If the vector does not exist or an error occurs during deletion,
        /// an exception will be thrown to notify the caller.
        /// </remarks>
        /// <param name="id">
        /// The unique identifier of the vector to be deleted from the index. The ID must correspond
        /// to a previously inserted vector; otherwise, the method may throw an exception indicating
        /// the deletion failed.
        /// </param>
        /// <returns>
        /// An integer status code returned by the native deletion function. A status of 0 indicates
        /// successful deletion, while any non-zero value signals an error.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the index has not been initialized or if the deletion operation fails due to
        /// an underlying issue in the native function.
        /// </exception>
        /// <example>
        /// <code>
        /// try
        /// {
        ///     int result = sdk.Delete(12345UL);
        ///     if (result == 0)
        ///     {
        ///         Console.WriteLine("Vector deleted successfully.");
        ///     }
        /// }
        /// catch (InvalidOperationException ex)
        /// {
        ///     Console.WriteLine($"Failed to delete vector: {ex.Message}");
        /// }
        /// </code>
        /// </example>
        public int Delete(ulong id)
        {
            if (_index == IntPtr.Zero)
                throw new InvalidOperationException("\nIndex not created.\n");

            int status = NativeMethods.delete(_index, id);

            if (status != 0)
                throw new InvalidOperationException($"\nERR: Can't eliminate vector with ID: {id}. status code: {status}\n");

            System.Diagnostics.Debug.WriteLine($"\nVector with ID: {id} eliminated succesfully.\n");
            return status;
        }

        /// <summary>
        /// Releases resources used by the instance and suppresses finalization.
        /// </summary>
        /// <remarks>
        /// Ensures managed and unmanaged resources are properly released to avoid memory leaks.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by this instance, both managed and unmanaged, based on the disposing flag.
        /// </summary>
        /// <remarks>
        /// This method is a critical part of the resource cleanup process for the class. It ensures that
        /// unmanaged resources, such as those allocated through the native library, are properly released
        /// to avoid memory leaks. If the `disposing` parameter is <c>true</c>, it also enables the cleanup
        /// of managed resources.
        /// 
        /// The method uses a flag (`_disposedFlag`) to ensure resources are only released once, preventing
        /// multiple disposal operations on the same instance. The method also resets the index pointer
        /// (<c>_index</c>) to <c>IntPtr.Zero</c> after the native resources are destroyed, signaling that
        /// the instance no longer holds a reference to the index.
        /// </remarks>
        /// <param name="disposing">
        /// A boolean indicating whether the method is called from the <see cref="Dispose"/> method
        /// (<c>true</c>) or from the finalizer (<c>false</c>). When <c>true</c>, both managed
        /// and unmanaged resources are released. When <c>false</c>, only unmanaged resources are cleaned up.
        /// </param>
        /// <example>
        /// <code>
        /// // Proper usage of Dispose to clean up resources
        /// var sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        /// try
        /// {
        ///     // Perform operations with the instance
        /// }
        /// finally
        /// {
        ///     // Ensure resources are released
        ///     sdk.Dispose();
        /// }
        /// </code>
        /// </example>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the cleanup process encounters issues, such as the inability to destroy
        /// the native resources correctly.
        /// </exception>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedFlag)
            {
                if (_index != IntPtr.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("Elements Destroyed succesfully.");
                    NativeMethods.destroy_index(ref _index);
                    _index = IntPtr.Zero;
                }

                _disposedFlag = true;
            }
        }

        /// <summary>
        /// Finalizer for the <see cref="VictorSDK"/> class to ensure resources are released as a fallback.
        /// </summary>
        /// <remarks>
        /// This destructor acts as a safety mechanism to release unmanaged resources in case
        /// the <see cref="Dispose"/> method is not explicitly called by the user. It invokes
        /// the <see cref="Dispose(bool)"/> method with a <c>false</c> parameter to clean up
        /// unmanaged resources only, ensuring no memory leaks occur.
        /// 
        /// A debug message is logged to remind developers to explicitly call <see cref="Dispose"/>
        /// for proper resource management, as relying solely on the finalizer may lead to
        /// delayed cleanup due to garbage collection timing.
        /// </remarks>
        /// <example>
        /// <code>
        /// var sdk = new VictorSDK(type: 1, method: 2, dims: 128);
        /// // If Dispose() is not called, the finalizer ensures resource cleanup:
        /// ~VictorSDK();
        /// </code>
        /// </example>
        ~VictorSDK()
        {
            Dispose(false);
            System.Diagnostics.Debug.WriteLine("\nSecurity fallback on.");
            System.Diagnostics.Debug.WriteLine("Next time don't forget use Dispose() to free memory.\n");
        }
    }
}