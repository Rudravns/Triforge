# pyright: reportAttributeAccessIssue=false, reportMissingImports=false
from . import loader
import csgame as cg
from .constants import *
from .csmath import *

class Rect3d:
    def __init__(self, pos: Vector3, size: Vector3, color: Color = Color(255, 255, 255)) -> None:
        self.pos = pos
        self.size = size
        self.color = color
        self.raw = cg.Rect3D(pos.raw, size.raw, color.raw)

    def get_raw(self):
        return self.raw

    def move_ip(self, delta: Vector3):
        self.pos.x += delta.x
        self.pos.y += delta.y
        self.pos.z += delta.z
        self.raw.Move(delta.raw)

    def rotate_ip(self, delta: Vector3):
        self.raw.Rotate(delta.raw)
    
    def assign_texture(self, texture_path:str, face:CubeFace):
        self.raw.assign_tex(texture_path, face.value)

    @property
    def x(self): return self.pos.x
    @x.setter
    def x(self, v): 
        self.pos.x = v
        self.raw.Position = self.pos.raw

    @property
    def y(self): return self.pos.y
    @y.setter
    def y(self, v): 
        self.pos.y = v
        self.raw.Position = self.pos.raw

    @property
    def z(self): return self.pos.z
    @z.setter
    def z(self, v): 
        self.pos.z = v
        self.raw.Position = self.pos.raw
