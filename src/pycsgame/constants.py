#pyright: reportMissingImports=false
from . import loader
import System 
import csgame as cg
from typing import *  # pyright: ignore[reportWildcardImportFromLibrary]
from enum import Enum

#Value Types

CSINT: TypeAlias = System.Int32
CSFLOAT: TypeAlias = System.Single
CSDOUBLE: TypeAlias = System.Double
CSLIST: TypeAlias = System.Collections.Generic.List[System.Object]
CSIMAGE: TypeAlias = cg.Image

CSNUM: TypeAlias = CSINT | CSDOUBLE | CSFLOAT | float | int


#math
CSVECTOR3D: TypeAlias = cg.Vector3d[System.Single]
CSVECTOR2D: TypeAlias = cg.Vector2d[System.Single]
CSVECTOR4D: TypeAlias = cg.Vector4d[System.Single]

#Shape types
CSRECT: TypeAlias = cg.Rectangle
CSTRIANGLE: TypeAlias = cg.Triangle

#rect faces
class CubeFace(Enum):
    FRONT = 0
    BACK = 1
    LEFT = 2
    RIGHT = 3
    TOP = 4
    BOTTOM = 5

class KeyboardKey(Enum):
#letters
    K_A = 0
    K_B = 1
    K_C = 2
    K_D = 3
    K_E = 4
    K_F = 5
    K_G = 6
    K_H = 7
    K_I = 8
    K_J = 9
    K_K = 10
    K_L = 11
    K_M = 12
    K_N = 13
    K_O = 14
    K_P = 15
    K_Q = 16
    K_R = 17
    K_S = 18
    K_T = 19
    K_U = 20
    K_V = 21
    K_W = 22
    K_X = 23
    K_Y = 24
    K_Z = 25
#numbers
    K_0 = 26
    K_1 = 27
    K_2 = 28
    K_3 = 29
    K_4 = 30
    K_5 = 31
    K_6 = 32
    K_7 = 33
    K_8 = 34
    K_9 = 35
#arrows
    K_UPARROW = 36
    K_DOWNARROW = 37
    K_LEFTARROW = 38
    K_RIGHTARROW = 39
#special
    K_SPACE = 40
    K_ENTER = 41
    K_ESCAPE = 42
    K_SHIFT = 43
    K_CTRL = 44
    K_ALT = 45
    K_BACKSPACE = 47
    K_TAB = 47
    K_CAPSLOCK = 48

#functon key
    K_F1 = 49
    K_F2 = 50
    K_F3 = 51
    K_F4 = 52
    K_F5 = 53
    K_F6 = 54
    K_F7 = 55
    K_F8 = 56
    K_F9 = 57
    K_F10 = 58
    K_F11 = 59
    K_F12 = 60