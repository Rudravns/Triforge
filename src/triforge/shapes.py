# pyright: reportAttributeAccessIssue=false, reportMissingImports=false, reportInconsistentOverload=false
from typing import overload, Union
from . import loader, constants
import csgame as cg
import System
from .csmath import *
from System.Collections.Generic import List


from typing import overload, Union

class Rect:
    @overload
    def __init__(self, x: float, y: float, w: float, h: float, color: Color = Color(255, 255, 255), z = 0.0) -> None: ...

    @overload
    def __init__(self, rect: tuple[float, float, float, float], color: Color = Color(255, 255, 255), z = 0.0) -> None: ...

    def __init__(self, rect_or_x: Union[tuple[float, float, float, float], float], *args, color: Color = None, z = 0.0) -> None:  # pyright: ignore[reportArgumentType]
        if isinstance(rect_or_x, (tuple, list)):
            # Overload: Rect((x, y, w, h), color=...)
            x, y, w, h = rect_or_x
            self.color_obj = args[0] if args else (color or Color(255, 255, 255))
        else:
            # Overload: Rect(x, y, w, h, color=...)
            x = rect_or_x
            if len(args) >= 3:
                y, w, h = args[:3]
                # Capture color if it was passed positionally as the 5th argument
                self.color_obj = args[3] if len(args) > 3 else (color or Color(255, 255, 255))
            else:
                raise TypeError("Rect() requires 4 float arguments (x, y, w, h) or a 4-element tuple")

        # Create the raw C# Rect object
        self.z = z
        self.raw = cg.Rect(System.Single(x), System.Single(y), System.Single(w), System.Single(h))


    
    
    def move_ip(self, x:float = 0.0, y:float = 0.0):
        """
        Move the shape by (x,y) amount
        :x: move x by this amount
        :y: move y by this amount
        """
        x = float(x)
        y = float(y)
        self.raw.Move_ip(x,y)

    def collide(self, other:"Rect") -> bool:
        return self.raw.CheckCollision(other.raw)
    

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

    @property
    def color(self): return self.color_obj
    @color.setter
    def color(self, v:Color): self.color_obj = v

    def get_raw(self):
        """
        Helper function to create a C# Rectangle drawable.
        Usage: shape = create_rect(Rect(100, 100, 200, 150), sky_blue)
        """
        # Extract raw C# objects if the arguments are our Python wrappers
        raw_rect = self.raw
        raw_color = self.color_obj.raw if hasattr(self.color_obj, 'raw') else self.color_obj

        return cg.Rectangle(raw_rect, raw_color, System.Single(self.z))
    
    def __repr__(self) -> str:
        return f"Rect(X={self.x}, Y={self.y}, W={self.w}, H={self.h}, Color={self.color})"

class Triangle:
    def __init__(self, points: list[Vector3], color:Color = Color(255, 255, 255)) -> None:
        self.points = points
        self.color = color
        self.raw = None
        self.__create_raw()
        
        
        
    def __create_raw(self):
        """
        creating raw (cs type)
        """
        raw_list = List[cg.Vector3d[constants.CSFLOAT]]()
        for point in self.points:
            raw_list.Add(point.raw)
        self.raw = cg.Triangle(raw_list, self.color.raw)

    
    def move_ip(self, x:float = 0.0, y:float = 0.0):
        """
        Move the shape by (x,y) amount
        :x: move x by this amount
        :y: move y by this amount
        """

        for vec in self.points:
            vec.x += x
            vec.y += y
        self.__create_raw()
    
    def get_raw(self):
        return self.raw
    
class Circle:
    def __init__(self, center:Vector2, radius:float, color:Color = Color(255, 255, 255), segments:constants.CSINT = 32) -> None:
        self.raw = cg.Circle(center.raw, radius, color.raw, segments)
    
    def get_raw(self):
        return self.raw

    def move_ip(self, x:float = 0.0, y:float = 0.0):
        self.raw.Move(x, y)
    
    def rotate_ip(self, delta: Vector3):
        self.raw.Rotate(delta.raw)

    @property
    def center(self): return self.raw.center
    @center.setter
    def center(self, v:Vector2): self.raw.center = v.raw

    @property
    def radius(self): return self.raw.radius
    @radius.setter
    def radius(self, v:float): self.raw.radius = v

    @property
    def position(self): return self.raw.pos
    

pass