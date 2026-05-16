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

#math
CSVECTOR3D: TypeAlias = cg.Vector3d[System.Single]
CSVECTOR2D: TypeAlias = cg.Vector2d[System.Single]
CSVECTOR4D: TypeAlias = cg.Vector4d[System.Single]

#Shape types
CSRECT: TypeAlias = cg.Rectangle
CSTRIANGLE: TypeAlias = cg.Triangle

