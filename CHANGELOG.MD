# Changelog

All notable changes to this project will be documented in this file.  
This project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.0.10- Final beta] - 2025-05-17

### ✨ Added

- **JSON-based managed dump/load system** via `DumpToFile()` and `LoadFromFile()` (a more readable and cross-platform alternative to the native `dump`).
- `VictorIndexSnapshot` and `VectorEntry` models for index serialization.
- **`Double Using` design pattern**: decouples write (`FLAT`) and search (`HNSW`) logic in separate scoped indices.
- Complete **unit test suite** using `NUnit`.
- **Benchmark system** comparing performance of FLAT, NSW, and HNSW index insertions (min, max, avg).
- `VictorRenderTest` class to collect timing metrics.
- Fully **semantic enum usage** (e.g., `IndexType.HNSW` instead of magic numbers).
- Basic Linux compatibility and an extensible architecture for future platform support.

### 🛠 Changed

- Major `VictorSDK` refactor:
  - Replaced generic exceptions with `VictorException` (supports overloads).
  - Defensive validation on `Insert` (null or empty vectors, invalid dimensions).
- Removed unbindable native methods.
- Cleaned up method names and removed legacy files.
- Improved `Dispose` pattern and implemented idiomatic `IDisposable`.

### 🐛 Fixed

- Silent error due to mismatch between enum and native macro values (`IndexType.HNSW`).
- Crash in `dump` if index contained no vectors.
- Flaky test failures caused by static or uninitialized inputs.

### 🧪 Testing

- Over 20 NUnit tests covering major SDK functionality (insertion, search, contains, error handling).
- Test for `Double Using` pattern.
- Test for managed `Dump` and `Load` with content verification.
- Negative tests for invalid dimensions and empty vectors.

### 📦 CI/CD

- GitHub Actions pipeline now stable and passing.
- `libvictor.dll` is embedded correctly into the NuGet package.
- CI workflow supports Windows build and test (Linux/macOS coming soon).

---

## [1.0.0- Release Candidate] - 2025-0x-XX

Initial public beta release of the C# binding for VictorBase.

- Support for FLAT, NSW, and HNSW index types.
- Native functions: alloc, insert, search, destroy, etc.
- Base SDK structure with test harness.
