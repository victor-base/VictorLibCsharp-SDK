# Victor Base C# Binding & SDK - INDEV Beta 1.0.10 (Pendiente de refactor)

## Descripción / Description
- **Español:** VictorLib es el SDK de interoperabilidad en C# para la librería nativa Victor Base, proporcionandote acceso eficiente a sus funcionalidades de indexación y búsqueda, etc.
- **English:** VictorLib is the C# interoperability SDK for the native Victor library, providing efficient access to its indexing and search functionalities.

---

## Estructura del proyecto / Project File Structure

![Estructura actual / Current Structure](Assets/estructura%20de%20archivos.png)

- **Español:**
  - `InterOp` contiene **VictorSDK**, el archivo fuente del SDK que actúa como capa de abstracción de las funciones nativas.
  - `InterOp` también incluye **VictorNative.cs**, primera capa de contacto con las funciones del núcleo de la base de datos.
  - Carpeta `Tests`, dedicada a pruebas unitarias y de usuario.
  - `Common` contiene archivos fuente reutilizables en distintas partes del proyecto.
  - `Examples` incluye ejemplos prácticos de uso del SDK.

- **English:**
  - `InterOp` contains **VictorSDK**, the SDK source file serving as an abstraction layer for native functionalities.
  - `InterOp` also includes **VictorNative.cs**, the first contact layer with the core database functions.
  - `Tests` folder: focused on user and unit tests.
  - `Common` includes reusable source files across different components.
  - `Examples` contains practical examples of SDK usage.

---

## Métodos disponibles / Available Methods

- **Insert:**  
  - **Español:** Inserta un vector en el índice con un ID único.  
  - **English:** Adds a vector to the index using a unique ID.  

- **Search:**  
  - **Español:** Realiza una búsqueda en el índice y devuelve el resultado más cercano.  
  - **English:** Performs a search in the index and returns the closest match.  

- **Search_n:**  
  - **Español:** Realiza una búsqueda y devuelve múltiples resultados cercanos.  
  - **English:** Performs a search and returns multiple closest matches.  

- **Delete:**  
  - **Español:** Elimina un vector del índice mediante su ID.  
  - **English:** Deletes a vector from the index using its ID.  

- **GetSize:**  
  - **Español:** Obtiene el número total de vectores almacenados en el índice.  
  - **English:** Retrieves the total number of vectors stored in the index.  

- **DumpIndex:**  
  - **Español:** Guarda el estado actual del índice en un archivo.  
  - **English:** Saves the current state of the index to a file.  

- **LoadIndex:**  
  - **Español:** Carga un índice desde un archivo previamente guardado.  
  - **English:** Loads an index from a previously saved file.  

- **Dispose:**  
  - **Español:** Libera recursos y evita leaks de memoria.  
  - **English:** Releases resources and prevents memory leaks.  

---

## Instalación / Installation

# Español
Para instalar la librería desde NuGet, utilizá el siguiente comando:

```bash
dotnet add package VictorLib --prerelease
```

### English
To install the library from NuGet, use the following command:
```bash
dotnet add package VictorLib --prerelease
```
### Ejemplos de uso/ Usage Examples

# Español

```c
VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);

// Insertar un vector
sdk.Insert(1, new float[128], 128);

// Buscar un vector
MatchResult result = sdk.Search(new float[128], 128);
Console.WriteLine($"Match ID: {result.MatchId}, Distance: {result.MatchData}");

// Obtener el tamaño del índice
ulong size = sdk.GetSize();
Console.WriteLine($"Index size: {size}");

// Guardar el índice en un archivo
sdk.DumpIndex("index_dump.dat");

// Liberar recursos
sdk.Dispose();
```

# English
```c
VictorSDK sdk = new VictorSDK(type: 1, method: 2, dims: 128);

// Insert a vector
sdk.Insert(1, new float[128], 128);

// Search for a vector
MatchResult result = sdk.Search(new float[128], 128);
Console.WriteLine($"Match ID: {result.MatchId}, Distance: {result.MatchData}");

// Get the size of the index
ulong size = sdk.GetSize();
Console.WriteLine($"Index size: {size}");

// Save the index to a file
sdk.DumpIndex("index_dump.dat");

// Release resources
sdk.Dispose();
```

### Contribuciones / Contributions
# Español
Las contribuciones son bienvenidas, pero los Pull Requests (PR) deben ser aprobados por el autor del proyecto.

## Cómo contribuir:

- Creá tu rama con el siguiente formato:

```bash
git checkout -b feature/nombre-del-issue o nombre del feat
```

# English
Contributions are welcome, but Pull Requests (PR) must be approved by the author of this project before being accepted.

## How to contribute:

- Create a branch following this format:

```bash
git checkout -b feature/nombre-del-issue or feat name
```

### 📜 Créditos y Licencia / Credits & License
# Español

- Este SDK complementa Victor Base (https://github.com/victor-base), un proyecto desarrollado por Emiliano A. Billi.
Este SDK también está licenciado bajo LGPL-3.0, y su autor es Iván E. Rodríguez.
# English

- This SDK complements Victor Base (https://github.com/victor-base), a project developed by Emiliano A. Billi.
This SDK is also licensed under LGPL-3.0, and its author is Iván E. Rodríguez.

### License

- This project was developed by Iván Ezequiel Rodriguez under the LGPL-3.0 license.
Its use is permitted in both open-source and commercial/private projects, as long as the terms of the license are respected.


- For inquiries or direct technical support, contact: ivanrwcm25@gmail.com
More information about the developer: https://ivanrodportolio.netlify.app/
