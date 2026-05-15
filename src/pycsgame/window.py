# pyright: reportMissingImports=false

from . import loader
from . import csmath
import csgame as cg
import System
from System import Action
import os

# --- Fix for Silk.NET Native Dependencies ---
# We use the path from your loader.py to find native DLLs
dll_dir = loader.dll_dir 
if os.path.exists(dll_dir):
    if hasattr(os, 'add_dll_directory'):
        os.add_dll_directory(dll_dir)
    os.environ['PATH'] = dll_dir + os.pathsep + os.environ['PATH']


def add(a: int, b: int) -> int:
    """Returns the sum of two values using the C# backend."""
    _cs_tester = cg.test()
    return _cs_tester.add(a, b)

def create_window(size: csmath.Vector2, title: str|None = None):
    """
    Wraps the MyWindow C# class. 
    Matches constructor: MyWindow(Vector2d<int> size, string title)
    """
    # FIX: Safety net! Automatically construct a Vector2d<int> for C# 
    # regardless of whether the Python vector was created as float or int.
    cs_size = cg.Vector2d[System.Int32](int(size.x), int(size.y))
    
    return cg.MyWindow(cs_size, title) if title else cg.MyWindow(cs_size)


# --- Callbacks for the Window Loop ---

def on_load(window):
    """
    This is called when the window starts up.
    FIXED: Removed 'window.Title' access which caused the crash.
    """
    print("Window loaded successfully!")

def on_update(delta_time):
    """
    This is called every frame.
    """
    pass

def run(win):
    load_action = Action[cg.MyWindow](on_load)
    update_action = Action[System.Double](on_update)
    win.Run(load_action, update_action)