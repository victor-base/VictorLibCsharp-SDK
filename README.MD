# Victor Base C# Binding & SDK - INDEV Beta Final 1.0.10

## Descripción / Description

- **Español:**  
  VictorLib es el SDK de interoperabilidad en C# para la librería nativa Victor Base. Permite acceso eficiente a funcionalidades de indexación, búsqueda y gestión de vectores, con una API moderna y segura para .NET.

- **English:**  
  VictorLib is the C# interoperability SDK for the native Victor Base library. It provides efficient access to indexing, search, and vector management features, with a modern and safe API for .NET.

---

## Estructura del proyecto / Project Structure

```
Src/
  Common/
    Mapping.cs
    Structs.cs
  InterOp/
    Factory/
      Factory.cs
    Native/
      INativeMethods.cs
      NativeMethodsLinux.cs
      NativeMethodsWindows.cs
      StaticNativeLinux.cs
      StaticNativeWindows.cs
    VictorExceptions/
      VictorExceptions.cs
    VictorSDK/
      VictorLib.cs
      VictorSDK_Index.cs
      VictorSDKAsort.cs
Tests/
  UnitTest1.cs
Examples/
  (usage examples)
```

- **Español:**  
  - `InterOp` contiene **VictorSDK**, la capa de abstracción sobre las funciones nativas.
  - `Common` incluye estructuras y utilidades reutilizables.
  - `Tests` contiene pruebas unitarias y de integración.
  - `Examples` incluye ejemplos prácticos de uso.

- **English:**  
  - `InterOp` contains **VictorSDK**, the abstraction layer over native functions.
  - `Common` includes reusable structs and utilities.
  - `Tests` contains unit and integration tests.
  - `Examples` provides practical usage examples.

---

## Métodos principales / Main Methods

- **Insert:**  
  - **Español:** Inserta un vector en el índice con un ID único.  
  - **English:** Adds a vector to the index using a unique ID.

- **Search:**  
  - **Español:** Busca el vector más cercano en el índice.  
  - **English:** Searches for the closest vector in the index.

- **Search_n:**  
  - **Español:** Devuelve los N vectores más cercanos.  
  - **English:** Returns the N closest vectors.

- **Delete:**  
  - **Español:** Elimina un vector por su ID.  
  - **English:** Deletes a vector by its ID.

- **GetSize:**  
  - **Español:** Obtiene el número total de vectores en el índice.  
  - **English:** Gets the total number of vectors in the index.

- **DumpIndex:**  
  - **Español:** Guarda el estado actual del índice en un archivo.  
  - **English:** Saves the current index state to a file.

- **LoadIndex:**  
  - **Español:** Carga un índice desde un archivo.  
  - **English:** Loads an index from a file.

- **Dispose:**  
  - **Español:** Libera recursos y evita fugas de memoria.  
  - **English:** Releases resources and prevents memory leaks.

- **GetStats:**  
  - **Español:** Obtiene estadísticas detalladas del índice (operaciones, tiempos, etc).  
  - **English:** Gets detailed index statistics (operations, timings, etc).

- **SetBasePath**
  - **Español:** Te permite setear una ruta custom para guardar tus archivos json de índices.
  - **English:** Allows you to set a custom file path to save your index json files.

- **DumpToAutoPath**
  - **Español:** Guarda el/los índices en un json generado en una carpeta llamada ".victorIndex"(generalmente en la carpeta bin/Debug) de manera automática si no le proporcionás una ruta custom.
  - **English:** Automatically saves the index/indices in a JSON file generated in a folder named ".victorIndex" (normally in bin/Debug) if you don't provide a custom path.

---

## Instalación / Installation

### Español

Para instalar la librería desde NuGet:

```bash
dotnet add package VictorLib --prerelease
```

### English

To install the library from NuGet:

```bash
dotnet add package VictorLib --prerelease
```

---

## Ejemplo de uso básico/Minimal Usage Example

### Español

```csharp

// Pista: El ctor VictorSDK implementa IDisposable, por lo tanto podemos evitar el metodo Dispose() con using cada vez que inicialicemos el ctor.
VictorSDK sdk = new(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128);

// Insertar un vector
sdk.Insert(id: 1,vector: new float[128],dims: 128);

// Buscar un vector
var result = sdk.Search(new float[128], 128);
Console.WriteLine($"Match ID: {result.Label}, Distance: {result.Distance}");

// Obtener el tamaño del índice
ulong size = sdk.GetSize();
Console.WriteLine($"Index size: {size}");

// Guardar el índice en un archivo
sdk.DumpIndex("index_dump.dat");

// Liberar recursos manualmente.
sdk.Dispose();
```

### English

```csharp
//Hint: VictorSDK implements IDisposable, so you can avoid Dispose() with using to free resources.
VictorSDK sdk = new (type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128);

// Insert a vector
sdk.Insert(1, new float[128], 128);

// Search for a vector
var result = sdk.Search(new float[128], 128);
Console.WriteLine($"Match ID: {result.Label}, Distance: {result.Distance}");

// Get the size of the index
ulong size = sdk.GetSize();
Console.WriteLine($"Index size: {size}");

// Save the index to a file
sdk.DumpIndex("index_dump.dat");

// Release resources manually
sdk.Dispose();
```

---

## Notas importantes / Important Notes

- **Contextos:**  
  - Para índices tipo `HNSW` o `NSW`, debes pasar un contexto válido al constructor.  
  - Para índices tipo `FLAT`, el contexto es ignorado y puede ser `null`.

- **Logging y Diagnóstico:**  
  - El SDK utiliza `Debug.WriteLine` para mensajes de depuración.  
  - No se imprimen mensajes en consola a menos que el usuario lo haga explícitamente.

- **Compatibilidad:**  
  - Compatible con Windows y Linux (x64).
  - Requiere la biblioteca nativa Victor Base instalada y accesible en el sistema.

- **DevX:**
  - Más abajo te cuento cómo recomiendo usar los métodos en un ejemplo más detallado aprovechando mejor la potencia de la lib. Consultar: "Buenas prácticas / Good coding".
  - Below you will find a more detailed example showing how to make the most of the library's features. See: "Good coding" for best practices.

---

## Notas sobre tipos de índices y métodos de búsqueda / Notes on Index Types and Search Methods

### Tipos de índice / Index Types

- **FLAT:**  
  - No requiere contexto, pero puedes pasar `null` para el parámetro `context` en el constructor.
  - Uso recomendado para índices pequeños o cuando no necesitas estructuras jerárquicas.

- **HNSW / NSW:**  
  - Permiten y suelen requerir un contexto (`HNSWContext` o `NSWContext`) para configurar parámetros avanzados.
  - Si pasas `null`, se usarán valores por defecto.
  - Uso recomendado para grandes volúmenes de datos y búsquedas rápidas.

**Nota:**  

- Todos los tipos de índice aceptan `null` como contexto en el constructor.  
- El contexto solo es obligatorio para `HNSW` y `NSW` si quieres personalizar parámetros.

### Métricas de distancia / Distance Metrics

- **cosine:**  
  - Mide la similitud angular entre dos vectores.
  - Útil para comparar orientación más que magnitud (por ejemplo, embeddings de texto).

- **euclidian:**  
  - Calcula la distancia euclidiana estándar.
  - Útil para datos donde la magnitud importa.

- **dotproduct:**  
  - Calcula el producto punto entre dos vectores.
  - Útil para tareas de ranking y algunas aplicaciones de machine learning.

### Métodos de búsqueda / Search Methods

- **Search:**  
  - Devuelve el vector más cercano al vector de consulta.
  - Uso general para obtener el mejor match.

- **Search_n:**  
  - Devuelve los N vectores más cercanos.
  - Útil para recomendaciones, clustering, o análisis de vecinos.

- **Contains:**  
  - Verifica si uno o más IDs existen en el índice.

- **Delete:**  
  - Elimina un vector por su ID.

### Ejemplo de inicialización con y sin contexto

```csharp
// FLAT index 
var flatSdk = new VictorSDK(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128, context: null);

// HNSW index 
var hnswContext = HNSWContext.Create(efConstruct: 200, efSearch: 100, m0: 32);
VictorSDK hnswSdk = new (type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: hnswContext);

// HNSW index 
VictorSDK hnswDefaultSdk = new (type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: null);

```

---

## Notes on Index Types and Search Methods

### Index Types

- **FLAT:**  
  - Does not require a context, but you can pass `null` for the `context` parameter in the constructor.
  - Recommended for small indices or when you do not need hierarchical structures.

- **HNSW / NSW:**  
  - Allow and usually require a context (`HNSWContext` or `NSWContext`) to configure advanced parameters.
  - If you pass `null`, default values will be used.
  - Recommended for large datasets and fast searches.

### Distance Metrics

- **cosine:**  
  - Measures the angular similarity between two vectors.
  - Useful for comparing orientation rather than magnitude (e.g., text embeddings).

- **euclidian:**  
  - Computes the standard Euclidean distance.
  - Useful when magnitude matters.

- **dotproduct:**  
  - Computes the dot product between two vectors.
  - Useful for ranking tasks and some machine learning applications.

**Note:**  

- All index types accept `null` as the context in the constructor.  
- The context is only required for `HNSW` and `NSW` if you want to customize parameters.

### Search Methods

- **Search:**  
  - Returns the closest vector to the query vector.
  - General use for obtaining the best match.

- **Search_n:**  
  - Returns the N closest vectors.
  - Useful for recommendations, clustering, or neighbor analysis.

- **Contains:**  
  - Checks if one or more IDs exist in the index.

- **Delete:**  
  - Deletes a vector by its ID.

### Example: Initialization With and Without Context

```csharp
// FLAT index (context can be null)
var flatSdk = new VictorSDK(type: IndexType.FLAT, method: DistanceMethod.DOTPROD, dims: 128, context: null);

// HNSW index (with custom context)
var hnswContext = HNSWContext.Create(efConstruct: 200, efSearch: 100, m0: 32);
VictorSDK hnswSdk = new (type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: hnswContext);

// HNSW index 
VictorSDK hnswDefaultSdk = new (type: IndexType.HNSW, method: DistanceMethod.COSINE, dims: 128, context: null);
```

---

## Buenas prácticas / Good coding

- Un pequeño archivo de docs adicional donde te muestro cómo recomiendo usar las funciones de persistencia, y un patrón de diseño que recomiendo mientras uses la librería.

[📖 Recomendado: Patrón Double Using + Persistencia JSON (Español)](./RECOMENDEDESP.MD)

- A little file where i show you how to use the Json persistence methods & a design pattern

[📖 Recommended: Double Using + JSON Persistence Pattern (English)](./RECOMENDEDENG.md)

---

## Contribuciones / Contributions

### Español

Las contribuciones son bienvenidas. Por favor, abrí un issue o un pull request siguiendo este formato:

```bash
git checkout -b feature/nombre-del-issue
```

### English

Contributions are welcome. Please open an issue or pull request using this format:

```bash
git checkout -b feature/issue-name
```

---

## Créditos y Licencia / Credits & License

- **Español:**  
  Este SDK complementa Victor Base <https://github.com/victor-base>, un proyecto de Emiliano A. Billi.  
  SDK desarrollado por Iván E. Rodríguez bajo licencia LGPL-3.0.

- **English:**  
  This SDK complements Victor Base <https://github.com/victor-base>, a project by Emiliano A. Billi.  
  SDK developed by Iván E. Rodríguez under LGPL-3.0 license.

---

## Soporte / Support

- Para consultas técnicas: <ivanrwcm25@gmail.com>  
- Más información sobre el desarrollador: <https://ivanrodportolio.netlify.app/>
