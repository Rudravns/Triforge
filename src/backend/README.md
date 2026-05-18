# triforge - C++ Backend

The native backend of triforge, written in modern C++ for high-performance systems such as collision detection, physics, mathematics, and future low-level engine components.

This backend is compiled as a shared library (`DLL`) and is used by the C# renderer layer through native interop.

---

## Features

- Native C++ performance
- Collision detection systems
- Mathematical utilities
- Future physics and ECS support
- Shared library integration with C#
- Built using CMake

---

## Project Structure

```text
backend/
├── include/
│ └── backend.h
├── src/
│ └── backend.cpp
├── CMakeLists.txt
```

---

## Building

### Requirements

- CMake 3.12+
- Visual Studio 2022 or newer
- C++20 compatible compiler

---

### Build with CMake

```bash
cmake -B out/build
cmake --build out/build
```

---

## Output

Compiled binaries are typically generated in:

```text
out/build/x64-debug/backend/
```

Example outputs:

```text
backend.dll
backend.lib
```

---

## Integration

The backend is loaded by the C# renderer using `DllImport`.

Example:

```csharp
[DllImport("backend.dll")]
public static extern bool CheckRectCollision(...);
```

---

## Current Systems

- Rectangle collision detection
- Native interop bridge

---

## Planned Features

- AABB collision system
- Raycasting
- Physics simulation
- SIMD optimizations
- Spatial partitioning
- ECS architecture
- 3D collision systems

---

## Notes

- The backend must be compiled for the same architecture as the renderer (`x64` recommended).
- `extern "C"` is used to prevent C++ name mangling.
- Debug builds are recommended during development.