# Changelog

Todas las versiones importantes de VictorLib estarán documentadas acá.  
Este proyecto sigue [Semantic Versioning](https://semver.org/lang/es/).

---

## [1.0.11-FinalBeta] - 2025-05-19

### Añadido

- Nuevo parámetro en `LoadFromFile` para sobrescribir el método de distancia (`overrideMethod`)
- Soporte para ruta base custom vía `SetBasePath(string)` para guardar snapshots en la carpeta deseada
- Validación más robusta en `ReadSnapshot(path)` con mejores mensajes de error
- Método `DumpToPath_snapshot()` simplificado: usa los metadatos del SDK y evita parámetros repetidos
- Test de integración nuevo: `DoubleUsing_DumpFlat_ThenLoadHNSW()` que demuestra conversión de FLAT a HNSW

### Cambiado

- Limpieza en la API `DumpToFile_snapshot`: ahora toma solo el `VictorSDK` y deduce lo demás
- Mejoras en la documentación XML en inglés y español
- Mejor experiencia de desarrollo (DX) con estilo más idiomático de C#

### Corregido

- Errores de ruta y excepciones confusas al pasar carpetas en lugar de archivos
- Stack overflow causado por llamada recursiva en `DumpToPath_snapshot` (bug corregido)

### Notas

- Aún en estado FinalBeta, pero muy cerca del Release Candidate.
- Alta estabilidad confirmada en Linux y Windows.
