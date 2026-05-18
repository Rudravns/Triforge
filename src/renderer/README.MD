# triforge - C# Renderer

The rendering and engine layer of triforge, built using C# and Silk.NET.

This layer handles rendering, input, window management, camera systems, drawable objects, and communication with the native C++ backend.

---

## Features

- OpenGL rendering
- Window creation and management
- Input handling
- Camera systems
- 2D and 3D rendering
- Texture rendering
- Text rendering
- Native C++ backend integration
- Python interoperability

---

## Core Systems

### Rendering
- `Drawable`
- `Rectangle`
- `Triangle`
- `Circle`
- `Image`
- `Text`
- `Rect3d`

### Engine
- `MyWindow`
- `Camera`
- `Transform`

### Input
- `KeyboardKey`
- `MouseButton`

### Native Integration
- `CppIntegration`
- backend DLL loading
- native collision systems

---

## Technologies

- .NET
- Silk.NET
- OpenGL
- System.Numerics
- Python.NET

---

## Project Structure

```text
renderer/
├── Renderer/
│ ├── Window.cs
│ ├── Shapes.cs
│ ├── Camera.cs
│ ├── Transform.cs
│ ├── cpp_integration.cs
│ └── ...
```

---

## Native Backend

The renderer communicates with the native C++ backend using `DllImport`.

Example:

```csharp
[DllImport("backend.dll")]
public static extern bool CheckRectCollision(...);
```

This allows performance-critical systems to run natively in C++.

---

## Rendering Pipeline

```text
Drawable
↓
OpenGL Buffers
↓
Shaders
↓
GPU Rendering
```

---

## Notes

- The renderer currently targets OpenGL 3.3 Core.
- The project is primarily developed on Windows x64.
- Native DLLs must match the renderer architecture.
