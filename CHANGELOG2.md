# Changelog

Todas las versiones importantes de VictorLib estarán documentadas acá.  
Este proyecto sigue [Semantic Versioning](https://semver.org/lang/es/).

---

## [1.0.10- Final beta] - 2025-05-17

### ✨ Agregado

- Sistema de **dump persistente en JSON** con método `DumpToFile()` y `LoadFromFile()` (alternativa más legible y multiplataforma al `dump` nativo).
- Clase `VictorIndexSnapshot` y `VectorEntry` para serialización de índices.
- Implementación del **patrón de diseño `Double Using`**, demostrando separación de responsabilidades entre índices (insertar con `FLAT`, buscar con `HNSW`).
- **Suite de testing unitaria completa** con `NUnit`.
- Sistema de **benchmark**: compara rendimiento en inserción de los índices `FLAT`, `NSW` y `HNSW` (min, max y promedio).
- Clase `VictorRenderTest` para realizar mediciones de performance.
- **Métodos más semánticos** (por ejemplo, `IndexType.HNSW` en vez de usar códigos numéricos).
- Agregada compatibilidad básica para `Linux` y arquitectura extensible para multiplataforma.

### 🛠 Cambiado

- Refactor de `VictorSDK`:
  - Reemplazo de excepciones genéricas por clase `VictorException` con múltiples overloads.
  - Manejo defensivo en `Insert` para detectar vectores nulos o con dimensiones incorrectas.
- Eliminación de métodos nativos no bindeables.
- Renombrado y limpieza de métodos redundantes.
- Simplificación de `Dispose` e implementación idiomática de `IDisposable`.

### 🐛 Corregido

- Error silencioso por incompatibilidad entre enums y macros nativas (`IndexType.HNSW` mapeaba incorrectamente).
- Falla en `dump` nativo al no insertar elementos antes de serializar.
- Errores en tests intermitentes por inputs inválidos o sin aleatoriedad.

### 🧪 Testing

- Se agregaron más de 20 tests con cobertura completa de flujo (inserción, búsqueda, contains, errores, etc).
- Test del patrón `Double Using`.
- Test para `Dump` y `Load` con verificación de contenido real.
- Test negativos para dimensiones inválidas y vectores vacíos.

### 📦 CI/CD

- Corrección del pipeline de GitHub Actions.
- Soporte para `libvictor.dll` embebido en el paquete.
- Pipeline unificado para build y test en Windows (por ahora).

---

## [1.0.0- Release] - 2025-0x-XX

Primera versión pública del binding C# para VictorBase.

- Soporte para FLAT, NSW y HNSW.
- Métodos nativos básicos: alloc, insert, search, destroy, etc.
- Estructura inicial del SDK y proyecto de pruebas.
