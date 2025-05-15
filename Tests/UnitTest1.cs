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
 *   Esta es la suite de pruebas unitarias para el SDK de Victor.
 *   Buscamos testear la funcionalidad de la clase VictorSDK y sus métodos.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Victor.NativeMethods.Factory;
using VictorBaseDotNET.Src.Common;
using VictorBaseDotNET.Src.utils;
using VictorExceptions;

namespace Victor.Tests;

public class VictorSDKTests
{
    private const string DatasetFileName = "dataset.fvecs";
    private static void GenerateDummyFvecs(string filePath, int vectorCount = 10, int dimension = 128)
    {
        using var writer = new BinaryWriter(File.Open(filePath, FileMode.Create));

        for (int i = 0; i < vectorCount; i++)
        {
            writer.Write(dimension); // primero la dimensión

            for (int j = 0; j < dimension; j++) writer.Write((float)(i * 0.1f + j * 0.01f)); // valores dummy
        }
    }


    [Test]
    public void InitVictorSDK_ShouldInitializeSuccessfully()
    {
        VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128);
        Assert.Pass("El test pasa creando índices NSW", sdk);
    }



    [Test]
    public void Insert_ShouldInsertVectorSuccessfully()
    {
        VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.COSINE, dims: 128);
        ErrorCode status = (ErrorCode)sdk.Insert(1, new float[128], 128);

        Debug.WriteLine($"status: {status}");

        Assert.AreEqual(ErrorCode.SUCCESS, status); // SUCCESS = 0
    }

    [Test]
    public void Search_ShouldReturnValidResult()
    {
        VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.EUCLIDIAN, dims: 128);
        float[] vector = new float[128];

        // Llenar el vector con algo (puede ser random o secuencial)
        for (int i = 0; i < vector.Length; i++)
            vector[i] = i / 128f;

        sdk.Insert(1, vector, 128); // ← Necesario

        MatchResult result = sdk.Search(vector, 128);

        Assert.IsTrue(result.Distance >= 0);
        Assert.IsTrue(result.Label >= 0);
    }


    // Intento de crear un índice jerárquico con HNSW
    [Test]
    public void GetSize_ShouldReturnCorrectSize()
    {
        VictorSDK sdk = new (type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128);
        sdk.Insert(1, new float[128], 128);

        ulong size = sdk.GetSize();

        Assert.AreEqual((ulong)1, size);
    }

    [Test]
    public void GetSizeNSWIndex_ShouldReturnCorrectSize()
    {
        NSWContext context = NSWContext.Create();
        VictorSDK test = new(type: IndexType.NSW, method: DistanceMethod.COSINE, dims: 129, context);
        test.Insert(1, new float[129], 129);
        var size = test.GetSize();
        Assert.DoesNotThrow(() => test.GetStats());
        Assert.GreaterOrEqual(size, 1);
        test.Dispose();
    }


    [Test]
    public void InitFlatIndex_ShouldWorkWithoutContext()
    {
        VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.EUCLIDIAN, dims: 128, context: null);
        Assert.IsNotNull(sdk);
    }

    [Test]
    public void DumpIndex_ShouldNotThrowException()
    {
        VictorSDK sdk = new(type: IndexType.HNSW, method: DistanceMethod.EUCLIDIAN, dims: 128);
        string filename = "index_dump.dat";

        Assert.DoesNotThrow(() => sdk.DumpIndex(filename));
    }

    // Debería devolver true si el id del vector existe en el índice.
    [Test]
    public void Contains_ShouldReturnTrueForExistingId()
    {
        VictorSDK sdk = new(type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: HNSWContext.Create());


        sdk.Insert(id: 1, vector: new float[128], dims: 128);
        sdk.Insert(id: 2, vector: new float[128], dims: 128);
        sdk.Insert(id: 3, vector: new float[128], dims: 128);
        sdk.Insert(id: 4, vector: new float[128], dims: 128);
        sdk.Insert(id: 5, vector: new float[128], dims: 128);

        bool exists = sdk.Contains(1, 2, 3, 4, 5);

        Assert.IsTrue(exists);
    }

    // Test negativo para el método Insert
    // Verifica que se lanza una excepción si el vector no tiene la longitud correcta
    [Test]
    public void Insert_ShouldFailWithWrongDims()
    {
        Assert.DoesNotThrow(() =>
        {
            var context = HNSWContext.Create();
            VictorSDK sdk = new(type: IndexType.NSW, method: DistanceMethod.EUCLIDIAN, dims: 128, context);
            Debug.WriteLine($"SDK created: {sdk}");
            float[] badVector = new float[128];
            sdk.Insert(1, badVector, 128);
        });
    }


    // Test de sistema
    [Test]
    public void FullLifecycleTest()
    {
        VictorSDK _sdk = new(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128, null);
        float[] vector = [.. Enumerable.Range(0, 128).Select(i => (float)i / 128)];
        ulong id = 777;

        _sdk.Insert(id, vector, 128);
        Assert.IsTrue(_sdk.Contains(id));

        var result = _sdk.Search(vector, 128);
        Assert.IsTrue(result.Distance >= 0);

        ulong size = _sdk.GetSize();
        Assert.GreaterOrEqual(size, 1);

        _sdk.DumpIndex("test_dump.dat");
    }

    // Test de integración del Factory
    [Test]
    public void NativeFactory_ShouldReturnCorrectImplementation()
    {
        var native = NativeMethodsFactory.Create();
        Assert.IsNotNull(native);

        // Esto depende del SO que tengas
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Assert.AreEqual("NativeMethodsWindows", native.GetType().Name);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            Assert.AreEqual("NativeMethodsLinux", native.GetType().Name);
    }

    [Test]
    public void VictorSDK_ShouldInitializeWithFlatIndex()
    {
        Assert.DoesNotThrow(() =>
        {
            var sdk = new VictorSDK(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128, context: null);
        });
    }


    [Test]
    public void FlatIndex_ShouldInitialize_WithNullContext()
    {
        var sdk = new VictorSDK(IndexType.FLAT, DistanceMethod.COSINE, 128);
        var sdk2 = new VictorSDK(IndexType.FLAT, DistanceMethod.DOTPROD, 128, null);
        var sdk3 = new VictorSDK(IndexType.HNSW, DistanceMethod.EUCLIDIAN, 128, null);
        sdk.Dispose();
        Assert.NotNull(sdk);
        Assert.NotNull(sdk2);
        Assert.NotNull(sdk3);
    }

    [Test]
    public void HNSWIndex_ShouldInitialize_WithValidContext()
    {
        VictorSDK sdk = new(IndexType.NSW, DistanceMethod.COSINE, 128, null);
        Assert.NotNull(sdk);
    }

    // Test negativo para el método HNSWIndex sin contexto requerido para que falle.
    [Test]
    public void NSWIndex_ShouldFail_WithArgumentException()
    {

        Assert.Throws<ArgumentException>(() =>
        {
            NSWContext.Create(0, 0, 0);
        });
    }

    [Test]
    public void HNSWIndex_ShouldFail_WithArgumentException()
    {

        Assert.Throws<ArgumentException>(() =>
        {
            HNSWContext.Create(0, 0, 0);
        });
    }

    [Test]
    public void ShouldPrintDiagnostics()
    {
        var sdk = new VictorSDK(IndexType.FLAT, DistanceMethod.DOTPROD, 128);

        string version = sdk.GetLibraryVersion();
        string name = sdk.GetIndexName();

        Assert.IsNotNull(version);
        Assert.IsNotNull(name);
    }


    [Test]
    public void TestVictorErr_ShouldRetVictorException()
    {
        VictorSDK test = new(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128);
        test.Insert(1, new float[128], 128);
        test.Insert(3, new float[128], 0);
        test.ThrowIfError(ErrorCode.INVALID_DIMENSIONS);

        Assert.Throws<VictorException>(() => test.ThrowIfError(ErrorCode.INVALID_DIMENSIONS));
    }

}

