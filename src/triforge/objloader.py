# pyright: reportAttributeAccessIssue=false, reportMissingImports=false
from . import loader
import csgame as cg
from .constants import *
from .csmath import *



class Model:
    def __init__(self, file_path, color = Color(255, 255, 255)):
        self.color = color.raw
        self.mesh = mesh = cg.ObjLoader.Load(file_path)
        self.model = cg.Model(self.mesh, self.color)

    def get_raw(self):
        return self.model