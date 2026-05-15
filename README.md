# FPS Project

This project is a Python-based game development environment utilizing the `pycsgame` library. It provides a simplified interface for window management, rendering primitives, and handling game loops, leveraging C#-style type precision for performance and compatibility.

## Project Structure

```text
FPS/
├── src/
│   ├── pycsgame/         # The core game engine wrapper
│   └── runner/
│       └── main.py       # Main entry point for the application
└── README.md
```

## Requirements

- Python 3.8+
- `pycsgame` dependencies (ensure the `src` directory is in your `PYTHONPATH`)

## How to Run

To start the application, navigate to the project root and execute the runner script:

```bash
python src/runner/main.py
```

## Basic Usage

The current implementation in `main.py` demonstrates how to:
1.  **Initialize a Window**: Create a window with specific dimensions using `Vector2` and explicit `CSINT` (System.Int32) types.
2.  **Define Colors**: Use RGBA values via `pycsgame.Color`.
3.  **Draw Shapes**: Create rectangles using `pycsgame.Rect` and add them to the window's draw list.
4.  **Manage the Loop**: Start the rendering engine using `pycsgame.run()`.

## Example Snippet

```python
window_size = pycsgame.Vector2(800, 600, pycsgame.CSINT)
win = pycsgame.create_window(window_size, "Game Window")
```