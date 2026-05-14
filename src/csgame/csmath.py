from . import loader
import csgame as cg
import System

def create_rect(rect_obj, color_obj):
    """
    Helper function to create a C# Rectangle drawable.
    Usage: shape = create_rect(Rect(100, 100, 200, 150), sky_blue)
    """
    # Extract raw C# objects if the arguments are our Python wrappers
    raw_rect = rect_obj.raw if hasattr(rect_obj, 'raw') else rect_obj
    raw_color = color_obj.raw if hasattr(color_obj, 'raw') else color_obj
    
    return cg.Rectangle(raw_rect, raw_color)


class Color:
    def __init__(self, red, green, blue, alpha=255.0):
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
    def __init__(self, x, y, dtype=System.Single):
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
    def __init__(self, x, y, z, dtype=System.Single):
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

class Rect:
    def __init__(self, x, y, w, h):
        # C# Rect class uses floats
        self.raw = cg.Rect(System.Single(x), System.Single(y), System.Single(w), System.Single(h))
    
    @property
    def x(self): return self.raw.X
    @x.setter
    def x(self, v): self.raw.X = System.Single(v)

    @property
    def y(self): return self.raw.Y
    @y.setter
    def y(self, v): self.raw.Y = System.Single(v)

    @property
    def w(self): return self.raw.W
    @w.setter
    def w(self, v): self.raw.W = System.Single(v)

    @property
    def h(self): return self.raw.H
    @h.setter
    def h(self, v): self.raw.H = System.Single(v)

    def __repr__(self):
        return f"Rect(X={self.x}, Y={self.y}, W={self.w}, H={self.h})"