import os
import sys
import clr

# 1. Initialize the loader first to set up DLL paths and references
from . import loader

# --- Native DLL Path Fix (Silk.NET Support) ---
# We ensure the OS can find native binaries (GLFW/SDL) in the renderer folder
dll_dir = loader.dll_dir
if os.path.exists(dll_dir):
    if hasattr(os, 'add_dll_directory'):
        os.add_dll_directory(dll_dir)
    os.environ['PATH'] = dll_dir + os.pathsep + os.environ['PATH']

# 2. Import Constants first so they can be overridden by our wrapper classes
from .constants import *

# 3. Import Math Wrappers from csmath.py
from .csmath import (
    Color, 
    Vector2, 
    Vector3, 
)

from .shapes import (
    Rect,
    Triangle,
    Circle
)

# 4. Import Window and Core Logic from window.py
from .window import (
    create_window,
    add,
    window,
    Camera
)

from .image import Image, Text

from .three_d_shapes import Rect3d

from .objloader import Model

# --- Metadata ---
__version__ = "1.0.0"
__author__ = "Rudransh Kumar"

# This list defines what is public when someone does 'from csgame import *'
__all__ = [
    'Color', 
    'Vector2', 
    'Vector3', 
    'Rect',
    'Triangle',
    'create_window',
    'window',
    'add',
    "CSINT",
    "CSFLOAT",
    "CSDOUBLE",
    "CSIMAGE",
    "CSRECT",
    "CSTRIANGLE",
    'Image',
    'Text',
    'Circle',
    'Rect3d',
    'Camera',
    'KeyboardKey',
    'CubeFace',
    'Model',
    'MouseButton'




]