# Victor Base C# Binding & SDK - INDEV Beta 1.0.5

## Descripción / Description
- **Español:** VictorNative es el binding de interoperabilidad en C# para la librería nativa Victor Base, proporcionando acceso eficiente a sus funcionalidades de indexación y búsqueda.
- **English:** VictorNative is the C# interoperability binding for the native Victor library, providing efficient access to its indexing and search functionalities.


---

## Estructura del proyecto / Project File Structure

![Estructura actual / Current Structure](Assets/estructura%20de%20archivos.png)

- **Español:**
  - `InterOp` contiene **VictorSDK**, el archivo fuente del SDK que actúa como capa de abstracción de las funciones nativas.
  - `InterOp` también incluye **VictorNative.cs**, primera capa de contacto con las funciones del núcleo de la base de datos.
  - Carpeta `Test`, dedicada a pruebas unitarias y de usuario.
  - `Common` contiene archivos fuente reutilizables en distintas partes del proyecto.

- **English:**
  - `InterOp` contains **VictorSDK**, the SDK source file serving as an abstraction layer for native functionalities.
  - `InterOp` also includes **VictorNative.cs**, the first contact layer with the core database functions.
  - `Test` folder: focused on user and unit tests.
  - `Common` includes reusable source files across different components.

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

- **Dispose:**  
  - **Español:** Libera recursos y evita leaks de memoria.  
  - **English:** Releases resources and prevents memory leaks.  

---

## Instalación / Installation

### Español
Para instalar la librería desde NuGet, utilizá el siguiente comando:

dotnet add package VictorLib --prerelease

### English 
To install the library from NuGet, use the following command:

dotnet add package VictorLib --prerelease

## Contribuciones / Contributions
Español
Las contribuciones son bienvenidas, pero los Pull Requests (PR) deben ser aprobados por el autor del proyecto.
Cómo contribuir:

- Creá tu rama con el siguiente formato: git checkout -b feature/nombre-del-issue
- Escribí tus cambios y commitealos con mensajes claros.
- Envía tu Pull Request a la rama master.
- Será revisada y recibirás feedback en ambos casos.

### English
Contributions are welcome, but Pull Requests (PR) must be approved by the author of this proyect before being accepted.
How to contribute:

- Create a branch following this format: git checkout -b feature/issue-name
- Make your changes and commit them with clear messages.
- Submit a Pull Request (PR) to the master repository branch.
- Wait for review and feedback before final approval.

---
### 📜 Créditos y Licencia / Credits & License
# Español

- Este SDK complementa Victor Base (licenciado bajo GPL-3.0) https://github.com/victor-base, un proyecto desarrollado por Emiliano A. Billi.

- Este SDK también está licenciado bajo GPL-3.0, y su autor es Iván E. Rodríguez.

# English

- This SDK complements Victor Base (licensed under GPL-3.0) https://github.com/victor-base, a project developed by Emiliano A. Billi.

- This SDK is also licensed under GPL-3.0, and its author is Iván E. Rodríguez.

### licencia

- Este proyecto fue desarrollado por Iván Ezequiel Rodriguez, bajo la licencia LGPL-3.0.

- Se permite su uso tanto en proyectos de código abierto como en proyectos comerciales y privados, siempre que se respeten los términos de la licencia. Para consultas o soporte técnico directo, consultar: ivanrwcm25@gmail.com o mas info del desarrollador en: https://ivanrodportolio.netlify.app/

