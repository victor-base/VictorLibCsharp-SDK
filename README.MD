# Victor Base C# Binding & SDK - INDEV Beta 1.0.10 (Pendiente de refactor)

## Descripción / Description
- **Español:** VictorNative es el binding de interoperabilidad en C# para la librería nativa Victor Base, proporcionando acceso eficiente a sus funcionalidades de indexación y búsqueda.
- **English:** VictorNative is the C# interoperability binding for the native Victor library, providing efficient access to its indexing and search functionalities.

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

### Español
Para instalar la librería desde NuGet, utilizá el siguiente comando:

```bash
dotnet add package VictorLib --prerelease
