## 📚 Patrón de diseño recomendado para esta lib: Double Using + Persistencia JSON

### ¿Qué es el patrón `Double Using`?

El patrón `Double Using` es una forma organizada y eficiente de trabajar con índices vectoriales cuando querés:

- Cargar e insertar datos rápidamente usando un índice **FLAT**.
- Persistirlos automáticamente como archivo `.json`.
- Cargar esos datos en un índice **jerárquico** (como HNSW o NSW) para realizar búsquedas mucho más rápidas.
- Nos valemos de este patrón debido a que VictorSDK implementa IDisposable para el manejo de recursos, de lo contrario, simplemente tenes que llamar a sdk.Dispose(); al final de cada indexación, búsqueda,etc.


### 🎯 ¿Por qué JSON y no archivos binarios?

A diferencia de los métodos nativos que generan dumps binarios opacos, la persistencia en JSON:

- ✅ Es **legible** y **portable**.
- ✅ Permite **ver el contenido real** (vectores, IDs, tipo de índice, etc.).
- ✅ Es **más mantenible** para debug, versionado y tests.
- ✅ No te ata a cómo se serializa el índice en el core en C.

- Esto te da trazabilidad total: sabés exactamente qué guardaste, cuándo, cómo, y podés restaurarlo sin depender de un formato binario nativo.

---

### 🧪 Ejemplo completo del patrón Double Using

```csharp
[Test]
public void DoubleUsing_DumpFlatAndLoadHNSW_ShouldWork()
{

    // Paso 1: Crear índice FLAT, insertar y persistir
    using (var flat = new VictorSDK(IndexType.FLAT, DistanceMethod.COSINE,dims: 128))
    {
        for (ulong i = 1; i <= 50; i++)
        {
            float[] vector = Enumerable.Repeat((float)i / 100, dims).ToArray();
            flat.Insert(i, vector, dims);
        }

        // Persistencia automática a archivo JSON (en ./.victor/)
      string path = VictorPersistence.DumpToAutoPath(flat, dims, IndexType.FLAT, DistanceMethod.COSINE);
    }

    // Paso 2: Cargar en índice jerárquico (HNSW) para búsquedas rápidas
    using (var hnsw = VictorPersistence.LoadFromFile(path))
    {
        float[] query = Enumerable.Repeat(0.3f, dims).ToArray();
        var result = hnsw.Search(query, dims);

        Assert.IsTrue(result.Label > 0);
        Console.WriteLine($"Match ID: {result.Label}, Distance: {result.Distance}");
    }

    File.Delete(path); // Limpieza opcional
}
```

## 📂 Manejo de archivos y persistencia automática

# Cuando usás DumpToAutoPath, el sistema

- Crea una carpeta .victor/ en la raíz del proyecto si no existe.

- Guarda el archivo con nombre único: "victor_index_YYYYMMDD_HHMMSS_GUID.json"

- Crea automáticamente un .gitignore dentro de .victorindex/ que ignora todo menos el propio .gitignore:

## 🧠 ¿Cuándo usar este patrón?

- Cuando querés preprocesar datos rápidamente con FLAT, pero necesitás performance óptima de búsqueda con HNSW.

- Cuando estás haciendo pruebas y querés guardar snapshots en JSON para comparar.

- Cuando desarrollás una app donde necesitás guardar y recuperar índices persistentes entre ejecuciones.

# Esto te asegura que:

- No vas a subir dumps por accidente al repo.

- Podés mantener una colección local de snapshots para pruebas, debugging, etc.

## ⚙️ Persistencia manual (si querés más control)

- Si querés controlar completamente dónde se guarda el archivo:

``` csharp
string customPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "my_index.json");
VictorPersistence.DumpToFile(flat, customPath, dims, IndexType.FLAT, DistanceMethod.COSINE, flat.GetInsertedVectors());
```

- O configurar una ruta base:

```csharp
VictorPersistence.SetBasePath("C:\\MiProyecto\\VectorDumps");
```

- Después podés seguir usando DumpToAutoPath(...) y los archivos se guardarán ahí.

## ¿Querés ver todos los vectores guardados?

- Solo abrí el archivo .json, vas a ver una estructura como esta:

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

