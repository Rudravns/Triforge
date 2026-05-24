# pyright: reportAttributeAccessIssue=false, reportMissingImports=false
from . import loader
from .constants import *
import csgame as cg
import System


class Color:
    def __init__(self, red:float, green:float, blue:float, alpha:float=1.0):
        self.raw = cg.Vector4d[System.Single](
            System.Single(red), 
            System.Single(green), 
            System.Single(blue), 
            System.Single(alpha)
        )
    
    @property
    def red(self): return self.raw.R
    @red.setter
    def red(self, v): self.raw.R = System.Single(v)

    @property
    def green(self): return self.raw.G
    @green.setter
    def green(self, v): self.raw.G = System.Single(v)

    @property
    def blue(self): return self.raw.B
    @blue.setter
    def blue(self, v): self.raw.B = System.Single(v)

    @property
    def alpha(self): return self.raw.A
    @alpha.setter
    def alpha(self, v): self.raw.A = System.Single(v)

    def __repr__(self):
        return f"Color(R={self.red}, G={self.green}, B={self.blue}, A={self.alpha})"

class Vector2:
    def __init__(self, x:CSNUM = 0, y:CSNUM = 0, dtype=CSFLOAT):
        # We use System.Single (float) by default as it's most common for rendering
        self.raw = cg.Vector2d[dtype](dtype(x), dtype(y))
    
    @property
    def x(self): return self.raw.X
    @x.setter
    def x(self, v): self.raw.X = type(self.raw.X)(v)

    @property
    def y(self): return self.raw.Y
    @y.setter
    def y(self, v): self.raw.Y = type(self.raw.Y)(v)

    def __repr__(self):
        return f"Vector2(X={self.x}, Y={self.y})"

class Vector3:
    def __init__(self, x:CSNUM = 0, y:CSNUM = 0, z:CSNUM = 0, dtype=CSFLOAT):
        self.raw = cg.Vector3d[dtype](dtype(x), dtype(y), dtype(z))
    
    @property
    def x(self): return self.raw.X
    @x.setter
    def x(self, v): self.raw.X = type(self.raw.X)(v)

    @property
    def y(self): return self.raw.Y
    @y.setter
    def y(self, v): self.raw.Y = type(self.raw.Y)(v)

    @property
    def z(self): return self.raw.Z
    @z.setter
    def z(self, v): self.raw.Z = type(self.raw.Z)(v)

    def __repr__(self):
        return f"Vector3(X={self.x}, Y={self.y}, Z={self.z})"
