# triforge

A hybrid game engine and rendering framework built with C++, C#, and Python.

triforge combines native performance, modern rendering, and high-level scripting into a modular multi-language architecture designed for experimentation, engine development, and game creation.

---

## Architecture

```text
Python
↓
C# Renderer Layer
↓
C++ Native Backend
```

### Layers

- **C++ Backend**
- Native performance systems
- Collision detection
- Physics and math systems
- Future ECS/runtime systems

- **C# Renderer**
- OpenGL rendering
- Windowing and input
- Scene management
- GPU resource handling

- **Python API**
- Gameplay scripting
- Rapid prototyping
- Testing and development

---

## Features

- OpenGL rendering
- 2D and 3D primitives
- Camera systems
- Texture rendering
- Text rendering
- Input handling
- Native C++ acceleration
- Python scripting support
- Shared library architecture

---

## Project Structure

```text
triforge/
├── src/
│ ├── backend/ # Native C++ backend
│ ├── renderer/ # C# rendering engine
│ ├── pytriforge/ # Python bindings
│ └── runner/ # Test scripts and examples
│
├── assets/
├── examples/
└── README.md
```

---

## Technologies

- **C++20**
- **C# / .NET**
- **Python**
- **OpenGL**
- **Silk.NET**
- **Python.NET**
- **CMake**

---

## Goals

triforge is designed to explore:
- engine architecture
- rendering systems
- native interop
- scripting systems
- graphics programming
- physics systems
- custom tooling

---

## Current State

The engine currently supports:
- window creation
- rendering pipelines
- textures
- camera movement
- input systems
- collision detection
- Python scripting integration

---

## Future Plans

- physics engine
- ECS architecture
- model loading
- shaders/material system
- lighting
- audio
- networking
- editor tooling
- Vulkan backend
- multithreading

---

## Development

### Backend (C++)

Uses CMake and builds as a shared library (`DLL`).

### Renderer (C#)

Built with .NET and Silk.NET.

### Python API

Powered through Python.NET bindings.

---

## License

Currently in active development.


