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
    cam = pycsgame.Camera()
    win.set_camera(cam)
    

    # Create a rectangle
    shape = pycsgame.Rect(5, -0.5, 1.0, 1.0, sky_blue)
    tri = pycsgame.Triangle([pycsgame.Vector3(0, 5, 0), pycsgame.Vector3(1, 5, 0), pycsgame.Vector3(0, 6, 0)])
    img = pycsgame.Image(
        "Images/cs.png",
        pycsgame.Vector2(-2.5, -2.5),
        pycsgame.Vector2(1.0, 1.0)
    )
    text = pycsgame.Text("Hello World!", pycsgame.Vector2(-0.9, -0.9), 32, sky_blue)
    fps = pycsgame.Text(f"FPS: 0", pycsgame.Vector2(-0.9, 0.9), 32, sky_blue)
    txt2 = pycsgame.Text("Hello World!", pycsgame.Vector2(-0.9, -0.9), 32, sky_blue)
    txt2.isScreenSpace = True
    cir = pycsgame.Circle(pycsgame.Vector2(3, 0), 0.5, sky_blue)
    rect3d = pycsgame.Rect3d(pycsgame.Vector3(0, 0, 0), pycsgame.Vector3(1, 1, 1), sky_blue)
    fps.isScreenSpace = True
    # Add the shape to the window's draw list
    win.add(rect3d)
    win.add(text)
    win.add(shape)
    win.add(tri)
    win.add(img)
    win.add(cir)
    win.add(fps)
    win.add(txt2)
    
    rect3d.assign_texture("Images/cs.png", pycsgame.CubeFace.FRONT)
    rect3d.assign_texture("Images/cs.png", pycsgame.CubeFace.BACK)
    rect3d.assign_texture("Images/cs.png", pycsgame.CubeFace.LEFT)
    

    print("Starting render loop...")
    mouse = False
    def update(dt):
        nonlocal mouse

        speed = 2 * dt
        fps.text = f"FPS: {round(win.get_fps())}"

        if win.IsKeyPressed(pycsgame.KeyboardKey.K_ESCAPE):
            win.quit()

        
        
        if mouse: win.update_camera(0.002)

        down = win.isKeyClicked(pycsgame.KeyboardKey.K_P)
        if down:
            mouse = not mouse 


        

        if win.IsKeyPressed(pycsgame.KeyboardKey.K_W):
            cam.MoveForwards(speed)

        if win.IsKeyPressed(pycsgame.KeyboardKey.K_S):
            cam.MoveBackwards(speed)


        if win.IsKeyPressed(pycsgame.KeyboardKey.K_A):
            cam.MoveLeft(speed)


        if win.IsKeyPressed(pycsgame.KeyboardKey.K_D):
            cam.MoveRight(speed)

        if win.IsKeyPressed(pycsgame.KeyboardKey.K_SPACE):
            cam.MoveUp(speed)
        
        if win.IsKeyPressed(pycsgame.KeyboardKey.K_SHIFT):
            cam.MoveDown(speed)
        
        is_lmb = win.isMousedown(pycsgame.MouseButton.LEFT)
        is_lmb_clicked = win.isMouseClicked(pycsgame.MouseButton.LEFT)
        text.text = f"pos: {cam.position}, rot: {cam.rotation}, p_down: {down}, LMB: {is_lmb}, Click: {is_lmb_clicked}"
        txt2.text = f"mouse_pos: {win.mouse_pos}"




    # Start the loop
    win.run(update=update)
    
if __name__ == "__main__":
    os.system('cls' if os.name == 'nt' else 'clear')
    run()