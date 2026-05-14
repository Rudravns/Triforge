import sys
from pathlib import Path

# Add src directory to path so csgame can be imported
sys.path.insert(0, str(Path(__file__).parent.parent))

import csgame

try:
    # Setup colors with explicit Single precision
    sky_blue = csgame.Color(135, 206, 235, 255)
    
    # Vector2d for size
    window_size = csgame.Vector2(800, 600)
    
    # Create the window
    win = csgame.create_window("Python Powered Renderer", window_size)
    
    # Create a rectangle
    shape = csgame.create_rect(csgame.Rect(100.0, 100.0, 200.0, 150.0), sky_blue)
    
    # Add the shape to the window's draw list
    win.AddDrawable(shape)
    
    print("Starting render loop...")

    # Start the loop
    csgame.Run(win)
    
except Exception as e:
    print(f"An error occurred: {e}")