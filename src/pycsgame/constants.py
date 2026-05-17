#pyright: reportMissingImports=false
from . import loader
import System 
import csgame as cg
from typing import *  # pyright: ignore[reportWildcardImportFromLibrary]

#Value Types

CSINT: TypeAlias = System.Int32
CSFLOAT: TypeAlias = System.Single
CSDOUBLE: TypeAlias = System.Double
CSLIST: TypeAlias = System.Collections.Generic.List[System.Object]
CSKEY: TypeAlias = System.Int32
CSIMAGE: TypeAlias = cg.Image

CSNUM: TypeAlias = CSINT | CSDOUBLE | CSFLOAT | float | int


#math
CSVECTOR3D: TypeAlias = cg.Vector3d[System.Single]
CSVECTOR2D: TypeAlias = cg.Vector2d[System.Single]
CSVECTOR4D: TypeAlias = cg.Vector4d[System.Single]

#Shape types
CSRECT: TypeAlias = cg.Rectangle
CSTRIANGLE: TypeAlias = cg.Triangle

#letters
KEY_A = 0
KEY_B = 1
KEY_C = 2
KEY_D = 3
KEY_E = 4
KEY_F = 5
KEY_G = 6
KEY_H = 7
KEY_I = 8
KEY_J = 9
KEY_K = 10
KEY_L = 11
KEY_M = 12
KEY_N = 13
KEY_O = 14
KEY_P = 15
KEY_Q = 16
KEY_R = 17
KEY_S = 18
KEY_T = 19
KEY_U = 20
KEY_V = 21
KEY_W = 22
KEY_X = 23
KEY_Y = 24
KEY_Z = 25
#numbers
KEY_0 = 26
KEY_1 = 27
KEY_2 = 28
KEY_3 = 29
KEY_4 = 30
KEY_5 = 31
KEY_6 = 32
KEY_7 = 33
KEY_8 = 34
KEY_9 = 35
#arrows
KEY_UPARROW = 36
KEY_DOWNARROW = 37
KEY_LEFTARROW = 38
KEY_RIGHTARROW = 39
#special
KEY_SPACE = 40
KEY_ENTER = 41
KEY_ESCAPE = 42
KEY_SHIFT = 43
KEY_CTRL = 44
KEY_ALT = 45
KEY_BACKSPACE = 47
KEY_TAB = 47
KEY_CAPSLOCK = 48

#functon key
KEY_F1 = 49
KEY_F2 = 50
KEY_F3 = 51
KEY_F4 = 52
KEY_F5 = 53
KEY_F6 = 54
KEY_F7 = 55
KEY_F8 = 56
KEY_F9 = 57
KEY_F10 = 58
KEY_F11 = 59
KEY_F12 = 60