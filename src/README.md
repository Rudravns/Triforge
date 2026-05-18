# triforge - Python API

The Python scripting layer for the triforge engine.

This module provides high-level access to the engine's rendering, input, camera, and geometry systems through Python bindings powered by the C# renderer backend.

---

## Features

- Python scripting support
- OpenGL rendering
- Camera controls
- 2D and 3D primitives
- Image rendering
- Text rendering
- Input handling
- Collision detection
- Native C++ accelerated backend systems

---

## Entry Point

Main testing and development entry point:

```text
src/runner/main.py
```

---

## Example

```python
import pytriforge

win = pytriforge.window(
pytriforge.Vector2(800, 600),
"triforge"
)

rect = pytriforge.Rect(
0,
0,
1,
1
)

win.add(rect.get_raw())

win.run()
```

---

## Available Components

### Rendering
- `Rect`
- `Rect3d`
- `Triangle`
- `Circle`
- `Image`
- `Text`

### Math
- `Vector2`
- `Vector3`
- `Color`

### Engine
- `window`
- `Camera`
- `Transform`

### Input
- `KeyboardKey`
- `MouseButton`

---

## Architecture

```text
Python
↓
C# Renderer Layer
↓
C++ Native Backend
```

The Python layer communicates with the renderer through Python.NET bindings, while performance-critical systems are executed in native C++.

---

## Notes

- Python is intended for gameplay scripting and rapid development.
- Heavy engine systems are handled in C# and C++.
- The engine currently targets Windows x64 systems.