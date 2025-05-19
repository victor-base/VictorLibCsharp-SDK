## 📚 Recommended Design Pattern for this Library: Double Using + JSON Persistence

### What is the `Double Using` pattern?

The `Double Using` pattern is an organized and efficient way to work with vector indices when you want to:

- Quickly load and insert data using a **FLAT** index.
- Automatically persist them as a `.json` file.
- Load that data into a **hierarchical** index (like HNSW or NSW) for much faster searches.

### 🎯 Why JSON and not binary files?

Unlike native methods that generate opaque binary dumps, JSON persistence:

- ✅ Is **readable** and **portable**.
- ✅ Lets you **see the actual content** (vectors, IDs, index type, etc.).
- ✅ Is **more maintainable** for debugging, versioning, and tests.
- ✅ Doesn't tie you to how the core C index is serialized.

- This gives you total traceability: you know exactly what you saved, when, how, and you can restore it without depending on a native binary format.

---

### 🧪 Full Example of the Double Using Pattern

```csharp
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
```

## 📂 File Handling and Automatic Persistence

# When you use DumpToAutoPath, the system

- Creates a `.victor/` folder at the project root if it doesn't exist.
- Saves the file with a unique name: `victor_index_YYYYMMDD_HHMMSS_GUID.json`
- Automatically creates a `.gitignore` inside `.victor/` that ignores everything except the `.gitignore` itself.

## 🧠 When should you use this pattern?

- When you want to preprocess data quickly with FLAT, but need optimal search performance with HNSW.
- When you're testing and want to save JSON snapshots for comparison.
- When developing an app where you need to save and restore persistent indices between runs.

# This ensures

- You won't accidentally commit dumps to your repo.
- You can keep a local collection of snapshots for testing, debugging, etc.

## ⚙️ Manual Persistence (if you want more control)

- If you want to fully control where the file is saved:

```csharp
string customPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my_index.json");
VictorPersistence.DumpToFile(flat, customPath, dims, IndexType.FLAT, DistanceMethod.COSINE, flat.GetInsertedVectors());
```

- Or set a base path (Recomended):

```csharp
var CustomPath = VictorPersistence.SetBasePath("C:\\MyProject\\VectorDumps");
```

- After that, you can keep using DumpToAutoPath(CustomPath ...) and files will be saved there.

## Want to see all saved vectors?

- Just open the `.json` file, you'll see a structure like this:

```json
{
  "Dimensions": 128,
  "IndexType": "FLAT",
  "Method": "COSINE",
  "Vectors": [
    {
      "Id": 1,
      "Vector": [0.01, 0.01, 0.01, ...]
    },
    {
      "Id": 2,
      "Vector": [0.02, 0.02, 0.02, ...]
    }
  ]
}
```
