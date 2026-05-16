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
    Triangle
)

# 4. Import Window and Core Logic from window.py
from .window import (
    create_window,
    add,
    window,
    run
)

from .image import Image, Text


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
    'run',
    "CSINT",
    "CSFLOAT",
    "CSDOUBLE",
    "CSRECT",
    "CSTRIANGLE",
    'Image',
    'Text'


]