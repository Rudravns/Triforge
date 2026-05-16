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

    cir = pycsgame.Circle(pycsgame.Vector2(0, 0), 0.5, sky_blue)


    # Add the shape to the window's draw list
    win.add(cir)
    
    print("Starting render loop...")
    
    def update(dt):
        if win.IsKeyPressed(pycsgame.KEY_ESCAPE):
            win.quit()
        
        if win.IsKeyPressed(pycsgame.KEY_UPARROW):
            cir.move_ip(0, 1*dt)
        if win.IsKeyPressed(pycsgame.KEY_DOWNARROW):
            cir.move_ip(0, -1*dt)
        if win.IsKeyPressed(pycsgame.KEY_LEFTARROW):
            cir.move_ip(-1*dt, 0)
        if win.IsKeyPressed(pycsgame.KEY_RIGHTARROW):
            cir.move_ip(1*dt, 0)




    # Start the loop
    win.run(update=update)
    
if __name__ == "__main__":
    os.system('cls' if os.name == 'nt' else 'clear')
    run()