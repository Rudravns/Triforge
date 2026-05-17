# pyright: reportAttributeAccessIssue=false, reportMissingImports=false, reportInconsistentOverload=false
from . import loader
import csgame as cg
import System
from .csmath import * # pyright: ignore[reportAssignmentType]
from pathlib import Path
import os 

class Image:
    def __init__(
        self,
        path:str,
        pos:Vector2 = Vector2(0, 0),
        size: Vector2 = Vector2(0.5, 0.5),
        alpha:float = 1.0
    ) -> None:

        # Get path relative to the CURRENT PYTHON FILE
        base_dir = Path(os.getcwd())
        full_path = (base_dir / path).resolve()

        self.path = str(full_path)
        self.pos = pos
        self.alpha = alpha
        self.size = size

        self.raw = cg.Image(
            self.path,
            pos.raw,
            size.raw,
            alpha
        )
    
    def get_raw(self):
        return self.raw
    
    def move_ip(self, x:float = 0.0, y:float = 0.0):
        self.pos.x += x
        self.pos.y += y
        self.raw.Move_ip(x, y)

class Text:
    def __init__(self, text:str, pos:Vector2 = Vector2(-0.5, -0.5), size:int = 32, 
                 color:Color = Color(255, 255, 255), bold:bool = False, italic:bool = False, underline:bool = False) -> None:
        self.pos = pos
        self.color = color
        self.raw = cg.Text(text, pos.raw, size, color.raw, bold, italic, underline)
    
    def get_raw(self):
        return self.raw
    
    @property
    def text(self): return self.raw._text
    @text.setter
    def text(self, v): self.raw.set_text(str(v))

    @property
    def bold(self): return self.raw._bold
    @bold.setter
    def bold(self, v:bool): self.raw.SetBold(v)

    @property
    def italic(self): return self.raw._italic
    @italic.setter
    def italic(self, v:bool): self.raw.SetItalic(v)

    @property
    def underline(self): return self.raw._underline
    @underline.setter
    def underline(self, v:bool): self.raw.SetUnderline(v)

    @property
    def size(self): return self.raw._fontSize
    @size.setter
    def size(self, v:int): self.raw.set_size(v)

    @property
    def isScreenSpace(self): return self.raw.ScreenSpace 
    @isScreenSpace.setter
    def isScreenSpace(self, v:bool): self.raw.ScreenSpace = v


    

    def move_ip(self, x:float = 0.0, y:float = 0.0):
        self.pos.x += x
        self.pos.y += y
        self.raw.Move_ip(x, y)

