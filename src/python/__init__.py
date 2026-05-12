#from .main import *

__version__ = "1.0.0"
__author__ = "Rudransh Kumar"
__all__ = []


#===========================
#loader setup
#===========================
import os
import sys
import clr # This comes from the 'pythonnet' package

# --- Your Path Setup ---
script_dir = os.path.dirname(os.path.abspath(__file__))
dll_dir = os.path.abspath(
    os.path.join(script_dir, "..", "renderer", "renderer", "bin", "Debug")
)
dll_path = os.path.join(dll_dir, "Renderer.dll")

# --- Essential Step ---
# Add the DLL directory to sys.path so dependencies (Silk.NET, etc.) are found
if dll_dir not in sys.path:
    sys.path.append(dll_dir)

# Load your specific library
clr.AddReference(dll_path)

# --- Import your C# Classes ---
# Note: Use the 'RootNamespace' from your .csproj (which was "Renderer")
# or the namespace you manually wrapped your classes in.
from Renderer import MyWindow, Vector2d, Rectangle, Rect, Vector4d # pyright: ignore[reportMissingImports]

# --- Run the Engine ---
def run_game():
    # 1. Setup Data
    # Note: Python handles the Generic <int> or <float> automatically
    win_size = Vector2d[int](1000, 800)
    game = MyWindow(win_size, "Running from Python!")

    color = Vector4d[float](255.0, 0.0, 0.0, 1.0)
    rect_data = Rect(100.0, 100.0, 100.0, 100.0)
    my_rect = Rectangle(rect_data, color)

    # 2. Define your callbacks
    def on_load(win):
        win.AddDrawable(my_rect)

    def on_update(dt):
        # You can even do logic here in Python!
        pass

    # 3. Run it
    game.Run(on_load, on_update)

if __name__ == "__main__":
    run_game()