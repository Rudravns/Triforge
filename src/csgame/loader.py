#==========================
#loader setup
#===========================
import os
import sys
import clr # This comes from the 'pythonnet' package


# --- Your Path Setup ---
script_dir = os.path.dirname(os.path.abspath(__file__))
dll_dir = os.path.abspath(
    os.path.join(script_dir, "..", "renderer", "renderer", "bin", "Debug")
)
dll_path = os.path.join(dll_dir, "Renderer.dll")

# --- Essential Step ---
# Add the DLL directory to sys.path so dependencies (Silk.NET, etc.) are found
if dll_dir not in sys.path:
    sys.path.append(dll_dir)

# Load your specific library
clr.AddReference("Renderer")

# --- Fix for Silk.NET Native Dependencies ---
# Silk.NET needs to find native DLLs (GLFW, SDL, etc.) which are usually 
# in the same folder as your Renderer.dll. 
# We need to tell the OS to look in that folder.
if os.path.exists(dll_dir):
    # For Python 3.8+, this is the preferred way to load native DLLs
    if hasattr(os, 'add_dll_directory'):
        os.add_dll_directory(dll_dir)
    # Also add to PATH for older versions or specific sub-dependencies
    os.environ['PATH'] = dll_dir + os.pathsep + os.environ['PATH']