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

def create_window(size: csmath.Vector2, title: str = "pycsgame Window", vsync:bool = True):
    """
    Wraps the MyWindow C# class. 
    Matches constructor: MyWindow(Vector2d<int> size, string title)
    """
    # FIX: Safety net! Automatically construct a Vector2d<int> for C# 
    # regardless of whether the Python vector was created as float or int.
    cs_size = cg.Vector2d[System.Int32](int(size.x), int(size.y))
    return cg.MyWindow(cs_size, title, vsync) 
  



class window:
    def __init__(self, size: csmath.Vector2, title: str ="pycsgame window", vsync:bool = True):
        self.internal = create_window(size, title, vsync)


    def run(self, update=lambda dt: None, load=lambda window: print("Window loaded successfully!")):
        run(self.internal, update, load)

    def add(self, obj):
        self.internal.AddDrawable(obj.get_raw())

    def get_fps(self):
        return self.internal.get_fps()

    def dt(self):
        return self.internal.get_fps()/1000


def run(win, update, load):
    load_action = Action[cg.MyWindow](load)
    update_action = Action[System.Double](update)
    win.Run(load_action, update_action)