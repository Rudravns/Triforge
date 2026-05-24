import os
import sys
from pathlib import Path

# Add src directory to path so csgame can be imported
sys.path.insert(0, str(Path(__file__).parent.parent)    )

import triforge


    

def run():
    # Setup colors with explicit Single precision
    sky_blue = triforge.Color(135, 206, 235, 255)
    red = triforge.Color(255, 0, 0, 255)
    
    # FIX: Use CSINT (System.Int32) for window dimensions
    window_size = triforge.Vector2(800, 600, triforge.CSINT)
    
    # Create the window
    win = triforge.window(window_size, "test", True)
    cam = triforge.Camera()
    win.set_camera(cam)
    

    # Create a rectangle
    shape = triforge.Rect(5, -0.5, 1.0, 1.0, sky_blue, z=4.0)
    shape2 = triforge.Rect(6, -0.5, 1.0, 1.0, red)
    tri = triforge.Triangle([triforge.Vector3(0, 5, 0), triforge.Vector3(1, 5, 0), triforge.Vector3(0, 6, 0)])
    img = triforge.Image(
        "Images/cs.png",
        triforge.Vector2(-2.5, -2.5),
        triforge.Vector2(1.0, 1.0)
    )
    text = triforge.Text("Hello World!", triforge.Vector2(-0.9, -0.9), 14, sky_blue)
    fps = triforge.Text(f"FPS: 0", triforge.Vector2(10, 10), 14, sky_blue)
    txt2 = triforge.Text("Hello World!", triforge.Vector2(10, 550), 14, sky_blue)
    txt2.isScreenSpace = True
    cir = triforge.Circle(triforge.Vector2(3, 0), 0.5, sky_blue)
    rect3d = triforge.Rect3d(triforge.Vector3(0, 0, 5), triforge.Vector3(1, 1, 1), sky_blue)

    donut = triforge.Model("Images/Blender/donut.obj", sky_blue)
    mmonkey = triforge.Model("Images/Blender/monkey.obj", sky_blue)
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
    win.add(shape2)
    #win.add(donut)
    win.add(mmonkey)

    rect3d.assign_texture("Images/cs.png", triforge.CubeFace.FRONT)
    rect3d.assign_texture("Images/cs.png", triforge.CubeFace.BACK)
    rect3d.assign_texture("Images/cs.png", triforge.CubeFace.LEFT)
    

    print("Starting render loop...")
    def update(dt):
        speed = 2 * dt
        fps.text = f"FPS: {round(win.get_fps())}"

        if win.IsKeyPressed(triforge.KeyboardKey.K_ESCAPE):
            win.quit()

        rect3d.rotate_ip(triforge.Vector3(0, dt, 0))
        cir.rotate_ip(triforge.Vector3(0, dt, 0))
        
        win.update_camera(0.002)

        down = win.isKeyClicked(triforge.KeyboardKey.K_P)
        if down:
            win.camera_mode = not win.camera_mode


        if win.IsKeyPressed(triforge.KeyboardKey.K_RIGHTARROW):
            shape2.move_ip(speed)
        if win.IsKeyPressed(triforge.KeyboardKey.K_LEFTARROW):
            shape2.move_ip(-speed)
        

        if win.IsKeyPressed(triforge.KeyboardKey.K_W):
            cam.MoveForwards(speed)

        if win.IsKeyPressed(triforge.KeyboardKey.K_S):
            cam.MoveBackwards(speed)


        if win.IsKeyPressed(triforge.KeyboardKey.K_A):
            cam.MoveLeft(speed)


        if win.IsKeyPressed(triforge.KeyboardKey.K_D):
            cam.MoveRight(speed)

        if win.IsKeyPressed(triforge.KeyboardKey.K_SPACE):
            cam.MoveUp(speed)
        
        if win.IsKeyPressed(triforge.KeyboardKey.K_SHIFT):
            cam.MoveDown(speed)
        
        is_lmb = win.isMousedown(triforge.MouseButton.LEFT)
        is_lmb_clicked = win.isMouseClicked(triforge.MouseButton.LEFT)
        text.text = f"pos: {cam.position}, rot: {cam.rotation}, p_down: {down}, LMB: {is_lmb}, Click: {is_lmb_clicked}"
        txt2.text = f"mouse_pos: {win.mouse_pos}"




    # Start the loop
    win.run(update=update)
    
if __name__ == "__main__":
    os.system('cls' if os.name == 'nt' else 'clear')
    run()