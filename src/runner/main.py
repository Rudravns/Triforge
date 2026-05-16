import os
import sys
from pathlib import Path



# Add src directory to path so csgame can be imported
sys.path.insert(0, str(Path(__file__).parent.parent))

import pycsgame


    

def run():
    # Setup colors with explicit Single precision
    sky_blue = pycsgame.Color(135, 206, 235, 255)
    
    # FIX: Use CSINT (System.Int32) for window dimensions
    window_size = pycsgame.Vector2(800, 600, pycsgame.CSINT)
    
    # Create the window
    win = pycsgame.window(window_size, "test", True)
    
    # Create a rectangle
    shape = pycsgame.Rect(-0.5, -0.5, 1.0, 1.0, sky_blue)
    tri = pycsgame.Triangle([pycsgame.Vector3(0, 0, 0), pycsgame.Vector3(1, 0, 0), pycsgame.Vector3(0, 1, 0)])
    img = pycsgame.Image(
        "Images/cs.png",
        pycsgame.Vector2(-0.5, -0.5),
        pycsgame.Vector2(0.5, 1.0)
    )
    txt = pycsgame.Text(str(win.get_fps()))
    # Add the shape to the window's draw list
    win.add(txt)
    win.add(tri)
    
    print("Starting render loop...")
    
    def update(dt):
        txt.text = str(round(win.get_fps()))
        tri.move_ip(0.01*dt)


    # Start the loop
    win.run(update=update)
    
if __name__ == "__main__":
    os.system('cls' if os.name == 'nt' else 'clear')
    run()