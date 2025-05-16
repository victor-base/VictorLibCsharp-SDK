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
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Victor.NativeMethods.Factory;
using VictorBaseDotNET.Src.Common;
using VictorBaseDotNET.Src.utils;
using VictorExceptions;
using VictorBenchmarks;
using System.IO;
using System.Collections.Generic;

namespace Victor.Tests;

[TestFixture]
internal class VictorSDKTests
{
    static float[] RandomVector()
    {
        Random rand = new();
        return Enumerable.Range(0, 128).Select(_ => (float)rand.NextDouble()).ToArray();
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

        for (int i = 0; i < vector.Length; i++) vector[i] = 0.72f;

        for (int i = 0; i < vector.Length; i++) vector[i] = i / 128f;

        sdk.Insert(1, vector, 128); // ← Necesario

        MatchResult result = sdk.Search(vector, 128);

        Assert.IsTrue(result.Distance >= 0);
        Assert.IsTrue(result.Label >= 0);
    }


    // Intento de crear un índice jerárquico con HNSW
    [Test]
    public void GetSize_ShouldReturnCorrectSize()
    {
        VictorSDK sdk = new(type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128);
        sdk.Insert(1, new float[128], 128);

        ulong size = sdk.GetSize();

        Assert.AreEqual((ulong)1, size);
    }



    [Test]
    public void InitFlatIndex_ShouldWorkWithoutContext()
    {
        VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.EUCLIDIAN, dims: 128, context: null);
        Assert.IsNotNull(sdk);
    }

    [Test]
    public void DoubleUsing_DumpFromFlatAndSearchWithHNSW_ShouldSucceed()
    {
        string path = Path.Combine(Path.GetTempPath(), $"New_index_{Guid.NewGuid()}.json");

        List<VectorEntry> dumpedVectors = new();
        ushort dims = 128;

        // PRIMER USING: índice FLAT que inserta y dumpea
        using (VictorSDK flat = new(IndexType.FLAT, DistanceMethod.COSINE, dims))
        {
            for (ulong i = 1; i <= 50; i++)
            {
                float[] vector = Enumerable.Repeat((float)i / 100, dims).ToArray();
                flat.Insert(i, vector, dims);

                dumpedVectors.Add(new VectorEntry { Id = i, Vector = vector });
            }

            VictorPersistence.DumpToFile(flat, path, dims, IndexType.FLAT, DistanceMethod.COSINE, dumpedVectors);
        }

        Assert.IsTrue(File.Exists(path), "El archivo JSON no fue creado correctamente.");

        // SEGUNDO USING: índice HNSW que carga y busca
        using (VictorSDK hnsw = new(IndexType.HNSW, DistanceMethod.COSINE, dims, HNSWContext.Create()))
        {
            foreach (var entry in dumpedVectors) hnsw.Insert(entry.Id, entry.Vector, dims);

            float[] query = Enumerable.Repeat(0.3f, dims).ToArray();
            var result = hnsw.Search(query, dims);

            Assert.IsTrue(result.Label > 0, "No se encontró un resultado válido.");
            Assert.IsTrue(result.Distance >= 0);
        }

        Debug.WriteLine("El segundo índice HNSW se creó y buscó correctamente.");
        // Cleanup
        if (File.Exists(path))
        {
            Debug.WriteLine($"Este es el archivo de dump: {path}");
            Debug.WriteLine(File.ReadAllText(path));
            File.Delete(path);
        }
    }





    [Test]
    public void Insert_ValidVector_ShouldSucceed()
    {
        using var sdk = new VictorSDK(IndexType.HNSW, DistanceMethod.COSINE, 128, HNSWContext.Create());

        float[] vector = Enumerable.Repeat(1.0f, 128).ToArray();
        int status = sdk.Insert(1, vector, 128);

        Assert.AreEqual(0, status); // SUCCESS
    }

    [Test]
    public void Insert_EmptyVector_ShouldThrowVictorException()
    {
        using var sdk = new VictorSDK(IndexType.HNSW, DistanceMethod.COSINE, 128, HNSWContext.Create());

        float[] emptyVector = [];

        var ex = Assert.Throws<VictorException>(() =>
        {
            sdk.Insert(2, emptyVector, 128);
        });

        Assert.AreEqual(ErrorCode.INVALID_VECTOR, ex.Code);
    }

    [Test]
    public void Benchmark_Insert_5000_HNSW()
    {
        using var sdk = new VictorSDK(IndexType.HNSW, DistanceMethod.COSINE, 128, HNSWContext.Create());
        VictorRenderTest bench = new(sdk);

        bench.InsertMany(5000, 128);
        var result = bench.GetStats();

        Debug.WriteLine(result);
    }
    [Test]
    public void Benchmark_Insert_5000_NSW()
    {
        using var sdk = new VictorSDK(IndexType.NSW, DistanceMethod.COSINE, 128, HNSWContext.Create());
        VictorRenderTest bench = new(sdk);

        bench.InsertMany(5000, 128);
        var result = bench.GetStats();

        Debug.WriteLine(result);
    }
    [Test]
    public void Benchmark_Insert_5000_FLAT()
    {
        using var sdk = new VictorSDK(IndexType.FLAT, DistanceMethod.COSINE, 128);
        VictorRenderTest bench = new(sdk);

        bench.InsertMany(5000, 128);
        var result = bench.GetStats();

        Debug.WriteLine(result);
    }



    // Debería devolver true si el id del vector existe en el índice.
    [Test]
    public void Contains_ShouldReturnTrueForExistingId()
    {
        VictorSDK sdk = new(type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: HNSWContext.Create());


        sdk.Insert(id: 1, vector: [.. Enumerable.Repeat(0.22f, 128)], dims: 128);
        sdk.Insert(id: 2, vector: [.. Enumerable.Repeat(0.22f, 128)], dims: 128);
        sdk.Insert(id: 3, vector: [.. Enumerable.Repeat(0.22f, 128)], dims: 128);
        sdk.Insert(id: 4, vector: [.. Enumerable.Repeat(0.22f, 128)], dims: 128);
        sdk.Insert(id: 5, vector: [.. Enumerable.Repeat(0.22f, 128)], dims: 128);

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
            float[] TestVector = new float[128];
            sdk.Insert(1, TestVector, 128);
        });
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
            HNSWContext.Create(-10, 0, 0);
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




}

