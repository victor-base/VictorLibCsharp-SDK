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
    [SetUp]
    public static float[] RandomVector()
    {
        Random rand = new();
        return Enumerable.Range(0, 128).Select(_ => (float)rand.NextDouble()).ToArray();
    }


    // Con custom Path
    [Test]
    public void DoubleUsing_FlatDumpToHNSWLoad_CustomPath_ShouldWork()
    {

        ushort dims = 128;
        string path;

        // Paso 1: Crear índice FLAT, insertar y persistir en una ruta custom
        using (var flat = new VictorSDK(IndexType.FLAT, DistanceMethod.COSINE, dims))
        {
            for (ulong i = 1; i <= 50; i++)
            {
                float[] vector = RandomVector();
                flat.Insert(i, vector, dims);
            }

            // Seteamos la ruta custom
            VictorPersistence.SetBasePath(@"D:\Users\pc\Desktop\Indices");

            // Dump a esa ruta
            path = VictorPersistence.DumpToPath_snapshot(flat);
            Debug.WriteLine($"Índice FLAT dumpeado a: {path}");

            Assert.That(File.Exists(path), Is.True, "El archivo JSON no fue creado.");
        }

        // Paso 2: Cargar como HNSW para búsquedas rápidas
        var hnswContext = HNSWContext.Create(efConstruct: 200, efSearch: 100, m0: 32);
        using (var hnsw = VictorPersistence.LoadFromFile_snapshot(hnswContext, path, overrideType: IndexType.HNSW))
        {
            float[] query = Enumerable.Repeat(0.3f, dims).ToArray();
            var result = hnsw.Search(query, dims);

            Debug.WriteLine($"Resultado: ID = {result.Label}, Distancia = {result.Distance}");
            Assert.IsTrue(result.Label > 0);
            Assert.IsTrue(File.Exists(path), $"El archivo no fue creado: {path}");

        }

    }




    [Test]
    public void DoubleUsing_FlatDumpToHNSWLoad_SearchShouldWork()
    {
        ushort dims = 128;
        string path;

        // Paso 1: Crear índice FLAT, insertar y persistir
        using (var flat = new VictorSDK(IndexType.FLAT, DistanceMethod.COSINE, dims))
        {
            for (ulong i = 1; i <= 50; i++)
            {
                float[] vector = RandomVector();
                flat.Insert(i, vector, dims);
            }

            // Dump a archivo automático
            path = VictorPersistence.DumpToPath_snapshot(flat);
            Debug.WriteLine($"Índice FLAT dumpeado a: {path}");
        }

        // Paso 2: Cargar como HNSW para búsquedas rápidas
        var hnswContext = HNSWContext.Create(efConstruct: 200, efSearch: 100, m0: 32);
        using (var hnsw = VictorPersistence.LoadFromFile_snapshot(hnswContext, path, overrideType: IndexType.HNSW, overrideMethod: DistanceMethod.DOTPROD))
        {
            float[] query = Enumerable.Repeat(0.3f, dims).ToArray();
            var result = hnsw.Search(query, dims);

            Debug.WriteLine($"Resultado: ID = {result.Label}, Distancia = {result.Distance}");
            Assert.IsTrue(result.Label > 0);
        }

        // Limpieza opcional
        File.Delete(path);
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
    public void InitFlatIndex_ShouldWorkWithoutContext()
    {
        VictorSDK sdk = new(type: IndexType.HNSW, method: DistanceMethod.EUCLIDIAN, dims: 128, context: HNSWContext.Create());
        Assert.IsNotNull(sdk);
    }

    [Test]
    public void DoubleUsing_DumpFlat_ThenLoadHNSW()
    {
        ushort dims = 128;
        string finalPath;

        // PRIMER USING: índice FLAT → inserción rápida y persistencia
        using (VictorSDK flat = new(IndexType.FLAT, DistanceMethod.COSINE, dims))
        {
            for (ulong i = 1; i <= 100; i++)
            {
                float[] vector = Enumerable.Repeat((float)i / 100, dims).ToArray();
                flat.Insert(i, vector, dims);
            }
            VictorPersistence.SetBasePath(@"D:\Users\pc\Desktop\Indices");
            // Persistencia automática en carpeta ./.victor/
            finalPath = VictorPersistence.DumpToPath_snapshot(flat);

            Console.WriteLine($"Índice FLAT dumpeado a: {flat}");
        }

        // SEGUNDO USING: índice HNSW → carga del dump y búsqueda eficiente
        // Paso 2: cargar el snapshot y reinsertar en índice HNSW 
        var snapshot = VictorPersistence.ReadSnapshot(finalPath);
        Debug.WriteLine($"Snapshot leído: {snapshot}");

        using (VictorSDK hnsw = new(IndexType.HNSW, snapshot.Method, snapshot.Dimensions, HNSWContext.Create()))
        {
            foreach (var entry in snapshot.Vectors) hnsw.Insert(entry.Id, entry.Vector, snapshot.Dimensions);

            float[] query = Enumerable.Repeat(0.33f, dims).ToArray();
            var result = hnsw.Search(query, dims);

            Console.WriteLine($"Resultado: ID = {result.Label}, Distancia = {result.Distance}");
            Assert.DoesNotThrow(() => hnsw.Search(query, dims));
        }
    }

    // Double using pattern: primero FLAT, luego HNSW[Test]
    public void DoubleUsing_DumpFromFlatAndSearchWithHNSW_ShouldSucceed()
    {
        ushort dims = 128;
        string path;

        // PRIMER USING: índice FLAT que inserta y dumpea
        using (VictorSDK flat = new(IndexType.FLAT, DistanceMethod.COSINE, dims))
        {
            for (ulong i = 1; i <= 50; i++)
            {
                float[] vector = Enumerable.Repeat((float)i / 100, dims).ToArray();
                flat.Insert(i, vector, dims);
            }

            path = VictorPersistence.DumpToPath_snapshot(flat); // ← devuelve la ruta
            Assert.IsTrue(File.Exists(path), "El archivo JSON no fue creado correctamente.");
        }

        // SEGUNDO USING: índice HNSW que carga desde archivo y busca
        using (VictorSDK hnsw = VictorPersistence.LoadFromFile_snapshot(HNSWContext.Create(), path, overrideType: IndexType.HNSW, overrideMethod: DistanceMethod.COSINE))
        {
            float[] query = Enumerable.Repeat(0.3f, dims).ToArray();
            var result = hnsw.Search(query, dims);

            Assert.IsTrue(result.Label > 0, "No se encontró un resultado válido.");
            Assert.IsTrue(result.Distance >= 0);
        }

        Debug.WriteLine("El segundo índice HNSW se creó y buscó correctamente.");
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
        Assert.Throws<VictorException>(() =>
        {
            var context = HNSWContext.Create();
            VictorSDK sdk = new(type: IndexType.NSW, method: DistanceMethod.EUCLIDIAN, dims: 128, context);
            Debug.WriteLine($"SDK created: {sdk}");
            float[] TestVector = new float[129];
            sdk.Insert(1, TestVector, 128);
        });
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



}

