# Changelog

All notable changes to this project will be documented in this file.  
This project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.0.12 -FinalBeta] - 2025-05-19

### Added

- New parameter in `LoadFromFile` to override the distance method (`overrideMethod`)
- Custom base path support via `SetBasePath(string)` for saving dumped index snapshots to a preferred folder
- Improved `ReadSnapshot(path)` validation and exception handling
- More flexible `DumpToPath_snapshot()` method using inferred SDK metadata (no redundant params)
- New integration test: `DoubleUsing_DumpFlat_ThenLoadHNSW()` demonstrating FLAT-to-HNSW conversion via dump/load

### Changed

- Cleaned up `DumpToFile_snapshot` API: now receives only the `VictorSDK` instance and infers metadata
- Updated XML docs with clear dev-facing comments in English and Spanish
- Enhanced developer experience (DX) with more idiomatic C# usage

### Fixed

- Path errors and misleading exceptions when passing folders instead of file paths
- Stack overflow caused by recursive method call (bug in older `DumpToPath_snapshot`)

### Notes

- This is still a FinalBeta, but nearing RC.
- Great stability across Linux and Windows.
