# pyright: reportMissingImports=false

from . import loader
from . import csmath, constants
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

class Camera:
    def __init__(self) -> None:
        self.internal = cg.Camera()


    def get_raw(self):
        return self.internal
    
    def move_ip(self, delta: csmath.Vector3):
        self.internal.Move(delta.raw)

    def rotate_ip(self, delta: csmath.Vector3):
        self.internal.Rotate(delta.raw)

    def MoveBackwards(self, amount: float):
        self.internal.MoveBackward(System.Single(amount))

    def MoveForwards(self, amount: float):
        self.internal.MoveForward(System.Single(amount))

    def MoveLeft(self, amount: float):
        self.internal.MoveLeft(System.Single(amount))

    def MoveRight(self, amount: float):
        self.internal.MoveRight(System.Single(amount))

    def MoveUp(self, amount: float):
        self.internal.MoveUp(System.Single(amount))

    def MoveDown(self, amount: float):
        self.internal.MoveDown(System.Single(amount))   

    @property
    def position(self):
        return self.internal.Position

    @position.setter
    def position(self, v:csmath.Vector3):
        self.internal.Position = v.raw

    @property
    def rotation(self):
        return self.internal.Rotation

    @rotation.setter
    def rotation(self, v:csmath.Vector3):
        self.internal.Rotation = v.raw



class window:
    def __init__(self, size: csmath.Vector2, title: str ="pycsgame window", vsync:bool = True):
        self.internal = create_window(size, title, vsync)
        self.down = []
        [self.down.append(False) for _ in constants.KeyboardKey]
        self.mouse = [False, False, False]
        self.update_cam = False

    def run(self, update=lambda dt: None, load=lambda window: print("Window loaded successfully!")):
        load_action = Action[cg.MyWindow](load)
        update_action = Action[System.Double](update)
        self.internal.Run(load_action, update_action)

    def update_camera(self, sens = 0.002, lock_mouse = True, hide_mouse = True):
        if self.update_cam:
            self.internal.update_camera(System.Single(sens), lock_mouse, hide_mouse)
        else:
            # Camera update is disabled, ensure mouse is completely free
            self.LockAndHideMouse(False, False)

    def add(self, obj):
        self.internal.AddDrawable(obj.get_raw())

    def IsKeyPressed(self, key:constants.KeyboardKey) -> bool:
        cs_key = cg.KeyboardKey(key.value)
        return self.internal.IsKeyPressed(cs_key)

    def isKeyClicked(self, key:constants.KeyboardKey) -> bool:
        if self.IsKeyPressed(key):
            if not self.down[key.value]:
                self.down[key.value] = True
                return True
        else:
            self.down[key.value] = False
        return False

    def isMousedown(self, button: constants.MouseButton) -> bool:
        return self.internal.isMousedown()[button.value]

    def isMouseClicked(self, button:constants.MouseButton) -> bool:
        if self.isMousedown(button):
            if not self.mouse[button.value]:
                self.mouse[button.value] = True
                return True
        else:
            self.mouse[button.value] = False
        return False

    def LockAndHideMouse(self, lock:bool, hide:bool):
        self.internal.LockAndHideMouse(lock, hide)

    def center_mouse(self):
        self.internal.center_mouse()

    def set_camera(self, cam: Camera):
        self.internal.Camera = cam.get_raw()

    def get_fps(self):
        return self.internal.get_fps()

    def dt(self):
        return self.internal.get_fps()/1000

    def quit(self):
        self.internal.Close()

    @property
    def mouse_pos(self):
        return self.internal.Mouse_pos()

    @mouse_pos.setter
    def mouse_pos(self, v: csmath.Vector2):
        self.internal.SetMousePos(v.raw)

    @property
    def camera_mode(self):
        return self.update_cam

    @camera_mode.setter
    def camera_mode(self, v: bool):
        # Check if we are specifically switching from locked (True) to unlocked (False)
        if self.update_cam and not v:
            self.center_mouse()
            self.LockAndHideMouse(False, False)
            
        self.update_cam = v
