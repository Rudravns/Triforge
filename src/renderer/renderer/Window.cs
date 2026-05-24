﻿using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Numerics;


namespace csgame
{
    public enum KeyboardKey
    {
        // Letters
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        // Numbers
        Number0, Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8, Number9,

        // Arrows
        Up, Down, Left, Right,

        // Special
        Space, Enter, Escape, Shift, Control, Alt, Backspace, Tab, CapsLock,

        // Function Keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12
    }

    public class MyWindow
    {
        private IWindow window;
        public Camera Camera = new Camera(); // the camera
        private GL gl;
        private IInputContext input;
        private IKeyboard primaryKeyboard; // Store the keyboard here
        private IMouse primaryMouse;
        private Vector2 lastMousePos;
        private bool firstMouseMove = true;
        
        private string Title;
        public static int Width;
        public static int Height;
        public static int initialWidth;
        public static int initialHeight;


        private uint shader2D;
        private uint shader3D;
        private List<Drawable> drawables = new List<Drawable>();

        public (int, int) get_size() => (Width, Height);
        public GL GetGL() => gl;


        private double fps = 0.0;
        private double deltaTime = 0.0;

        private double fpsTimer = 0.0;
        private int fpsFrames = 0;
        private bool vsync_ = true;

        // Fixed the Vector2d to Vector2D<double> for Silk.NET compatibility
        public MyWindow(Vector2d<int> size, string title = "My Silk.NET Window", bool vsync = true)
        {
            Title = title;
            Width = size.X;
            Height = size.Y;
            initialWidth = size.X;
            initialHeight = size.Y;
            vsync_ = vsync;
        }


        public bool IsKeyPressed(KeyboardKey key)
        {
            if (primaryKeyboard != null && KeyMap.TryGetValue(key, out var silkKey))
            {
                return primaryKeyboard.IsKeyPressed(silkKey);
            }

            return false;
        }

        public bool[] isMousedown()
        {
            if (primaryMouse != null)
            {
                return new bool[]
                {
                    primaryMouse.IsButtonPressed(Silk.NET.Input.MouseButton.Left),
                    primaryMouse.IsButtonPressed(Silk.NET.Input.MouseButton.Middle),
                    primaryMouse.IsButtonPressed(Silk.NET.Input.MouseButton.Right)
                };
            }

            return new bool[] { false, false, false };
        }

        public (float x, float y) Mouse_pos()
        {
            if (primaryMouse != null)
            {
                return (primaryMouse.Position.X, primaryMouse.Position.Y);
            }
            return (0f, 0f);
        }

        public void SetMousePos(Vector2d<float> position)
        {
            if (primaryMouse != null)
            {
                primaryMouse.Position = new Vector2(position.X, position.Y);
            }
        }

        private static readonly Dictionary<KeyboardKey, Key> KeyMap = new Dictionary<KeyboardKey, Key>
        {
            { KeyboardKey.A, Key.A }, { KeyboardKey.B, Key.B }, { KeyboardKey.C, Key.C },
            { KeyboardKey.D, Key.D }, { KeyboardKey.E, Key.E }, { KeyboardKey.F, Key.F },
            { KeyboardKey.G, Key.G }, { KeyboardKey.H, Key.H }, { KeyboardKey.I, Key.I },
            { KeyboardKey.J, Key.J }, { KeyboardKey.K, Key.K }, { KeyboardKey.L, Key.L },
            { KeyboardKey.M, Key.M }, { KeyboardKey.N, Key.N }, { KeyboardKey.O, Key.O },
            { KeyboardKey.P, Key.P }, { KeyboardKey.Q, Key.Q }, { KeyboardKey.R, Key.R },
            { KeyboardKey.S, Key.S }, { KeyboardKey.T, Key.T }, { KeyboardKey.U, Key.U },
            { KeyboardKey.V, Key.V }, { KeyboardKey.W, Key.W }, { KeyboardKey.X, Key.X },
            { KeyboardKey.Y, Key.Y }, { KeyboardKey.Z, Key.Z },

            { KeyboardKey.Number0, Key.Number0 }, { KeyboardKey.Number1, Key.Number1 },
            { KeyboardKey.Number2, Key.Number2 }, { KeyboardKey.Number3, Key.Number3 },
            { KeyboardKey.Number4, Key.Number4 }, { KeyboardKey.Number5, Key.Number5 },
            { KeyboardKey.Number6, Key.Number6 }, { KeyboardKey.Number7, Key.Number7 },
            { KeyboardKey.Number8, Key.Number8 }, { KeyboardKey.Number9, Key.Number9 },

            { KeyboardKey.Up, Key.Up }, { KeyboardKey.Down, Key.Down },
            { KeyboardKey.Left, Key.Left }, { KeyboardKey.Right, Key.Right },
            { KeyboardKey.Space, Key.Space }, { KeyboardKey.Enter, Key.Enter },
            { KeyboardKey.Escape, Key.Escape }, { KeyboardKey.Shift, Key.ShiftLeft },
            { KeyboardKey.Control, Key.ControlLeft }, { KeyboardKey.Alt, Key.AltLeft },
            { KeyboardKey.Backspace, Key.Backspace }, { KeyboardKey.Tab, Key.Tab },
            { KeyboardKey.CapsLock, Key.CapsLock },

            { KeyboardKey.F1, Key.F1 }, { KeyboardKey.F2, Key.F2 }, { KeyboardKey.F3, Key.F3 },
            { KeyboardKey.F4, Key.F4 }, { KeyboardKey.F5, Key.F5 }, { KeyboardKey.F6, Key.F6 },
            { KeyboardKey.F7, Key.F7 }, { KeyboardKey.F8, Key.F8 }, { KeyboardKey.F9, Key.F9 },
            { KeyboardKey.F10, Key.F10 }, { KeyboardKey.F11, Key.F11 }, { KeyboardKey.F12, Key.F12 }
        };


        public void AddDrawable(Drawable drawable) => drawables.Add(drawable);

        public void RemoveDrawable(Drawable drawable) => drawables.Remove(drawable);


        public void Run(Action<MyWindow> onLoad, Action<double> onUpdate = null)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(Width, Height);
            options.Title = Title;
            options.VSync = vsync_;
            window = Window.Create(options);

            window.Load += () =>
            {
                gl = GL.GetApi(window);
                input = window.CreateInput();
                primaryKeyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;
                primaryMouse = input.Mice.Count > 0 ? input.Mice[0] : null;

                window.Resize += size =>
                {
                    Width = size.X;
                    Height = size.Y;

                    gl.Viewport(
                        0,
                        0,
                        (uint)Width,
                        (uint)Height
                    );
                };
                gl.Viewport(0, 0, (uint)Width, (uint)Height);
                gl.Enable(EnableCap.Blend);
                gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                gl.Enable(EnableCap.DepthTest);
                gl.DepthFunc(DepthFunction.Less);
                gl.Enable(EnableCap.CullFace);
                gl.CullFace(TriangleFace.Back);
                gl.FrontFace(FrontFaceDirection.Ccw);
                shader2D = Create2DShader();
                shader3D = Create3DShader();
                onLoad?.Invoke(this);
            };

            window.Update += (dt) =>
            {
                deltaTime = dt;

                fpsFrames++;
                fpsTimer += dt;

                if (fpsTimer >= 1.0)
                {
                    fps = fpsFrames / fpsTimer;

                    fpsFrames = 0;
                    fpsTimer = 0.0;
                }

                onUpdate?.Invoke((float)dt);
            };

            window.Render += OnRender;

            window.Run();
        }

        private unsafe void OnRender(double deltaTime)
        {
            gl.ClearColor(0.1f, 0.1f, 0.1f, 1f);

            gl.Clear(
                (uint)ClearBufferMask.ColorBufferBit |
                (uint)ClearBufferMask.DepthBufferBit
            );

            Matrix4x4 view = Camera.GetViewMatrix();

            float aspect = Width / (float)Height;

            Matrix4x4 projection =
                Matrix4x4.CreatePerspectiveFieldOfView(
                    (float)Math.PI / 4f,
                    aspect,
                    0.1f,
                    100f
                );

            Matrix4x4 uiProjection =
                Matrix4x4.CreateOrthographicOffCenter(
                    0,
                    initialWidth,
                    initialHeight,
                    0,
                    -1,
                    1
                );

            var worldDrawables = new List<Drawable>();
            var uiDrawables = new List<Drawable>();

            foreach (var drawable in drawables)
            {
                if (drawable.ScreenSpace)
                    uiDrawables.Add(drawable);
                else
                    worldDrawables.Add(drawable);
            }

            // Draw lists cleanly and prevent address context capturing errors
            void DrawList(List<Drawable> listToDraw, Matrix4x4 localView, Matrix4x4 localProjection)
            {
                foreach (var drawable in listToDraw)
                {
                    bool isModel = drawable.GetType().Name == "Model";
                    uint shaderToUse = isModel ? shader3D : shader2D;

                    gl.UseProgram(shaderToUse);

                    int viewLoc = gl.GetUniformLocation(shaderToUse, "view");
                    int projLoc = gl.GetUniformLocation(shaderToUse, "projection");
                    int resLoc = gl.GetUniformLocation(shaderToUse, "uResolution");

                    gl.UniformMatrix4(viewLoc, 1, false, (float*)&localView);
                    gl.UniformMatrix4(projLoc, 1, false, (float*)&localProjection);

                    if (resLoc != -1)
                    {
                        gl.Uniform2(resLoc, (float)Width, (float)Height);
                    }

                    if (shaderToUse == shader3D)
                    {
                        gl.Uniform3(
                            gl.GetUniformLocation(shaderToUse, "lightPos"),
                            Camera.Position.X,
                            Camera.Position.Y,
                            Camera.Position.Z
                        );
                    }

                    drawable.Draw(gl, shaderToUse);
                }
            }

            // Pass 1: Render 3D World (Depth Testing ON)
            gl.Enable(EnableCap.DepthTest);
            DrawList(worldDrawables, view, projection);

            // Pass 2: Render UI / Screenspace (Depth Testing OFF so it is always on top)
            gl.Disable(EnableCap.DepthTest);
            DrawList(
                uiDrawables,
                Matrix4x4.Identity,
                uiProjection
            );

            // Restore state
            gl.Enable(EnableCap.DepthTest);
        }

        private uint Create3DShader()
        {
            string vertex = @"#version 330 core

            layout(location = 0) in vec3 aPos;
            layout(location = 1) in vec2 aTexCoord;
            layout(location = 2) in vec3 aNormal;

            out vec2 TexCoord;
            out vec3 Normal;
            out vec3 FragPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform int uScreenSpace;
            uniform vec2 uResolution;

            void main()
            {
                FragPos = vec3(model * vec4(aPos, 1.0));

                Normal =
                    mat3(transpose(inverse(model)))
                    * aNormal;

                if (uScreenSpace == 1)
                {
                    vec4 pos = model * vec4(aPos, 1.0);
                    
                    float baseAspect = 800.0 / 600.0;
                    float currentAspect = uResolution.x / max(uResolution.y, 1.0);
                    pos.x *= (baseAspect / currentAspect);
                    
                    gl_Position = pos;
                    
                    // Correct flipped Y coordinates for Screen Space (UI coordinates have Y-down)
                    TexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
                }
                else
                {
                    gl_Position =
                        projection *
                        view *
                        vec4(FragPos, 1.0);
                        
                    TexCoord = aTexCoord;
                }
            }";

            string fragment = @"#version 330 core

            out vec4 FragColor;

            in vec2 TexCoord;
            in vec3 Normal;
            in vec3 FragPos;

            uniform vec4 uColor;
            uniform sampler2D uTexture;
            uniform bool uUseTexture;

            uniform vec3 lightPos;

            void main()
            {
                vec4 baseColor =
                    uUseTexture
                    ? texture(uTexture, TexCoord) * uColor
                    : uColor;

                vec3 norm =
                    normalize(Normal);

                vec3 lightDir =
                    normalize(lightPos - FragPos);

                float diffuse =
                    max(dot(norm, lightDir), 0.0);

                float ambient = 0.2;

                float lighting =
                    ambient + diffuse;

                FragColor =
                    vec4(
                        baseColor.rgb * lighting,
                        baseColor.a
                    );
            }";

            uint vs = gl.CreateShader(ShaderType.VertexShader);
            gl.ShaderSource(vs, vertex);
            gl.CompileShader(vs);

            uint fs = gl.CreateShader(ShaderType.FragmentShader);
            gl.ShaderSource(fs, fragment);
            gl.CompileShader(fs);

            uint program = gl.CreateProgram();
            gl.AttachShader(program, vs);
            gl.AttachShader(program, fs);
            gl.LinkProgram(program);

            gl.DeleteShader(vs);
            gl.DeleteShader(fs);

            return program;
        }

        private uint Create2DShader()
        {
            string vertex = @"#version 330 core

            layout(location = 0) in vec3 aPos;
            layout(location = 1) in vec2 aTexCoord;

            out vec2 TexCoord;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform int uScreenSpace;
            uniform vec2 uResolution;

            void main()
            {
                if (uScreenSpace == 1)
                {
                    gl_Position = 
                        projection * 
                        model * 
                        vec4(aPos, 1.0);
                    TexCoord = aTexCoord;
                }
                else
                {
                    gl_Position =
                        projection *
                        view *
                        model *
                        vec4(aPos, 1.0);
                        
                    TexCoord = aTexCoord;
                }
            }";

            string fragment = @"#version 330 core

            out vec4 FragColor;

            in vec2 TexCoord;

            uniform vec4 uColor;
            uniform sampler2D uTexture;
            uniform bool uUseTexture;

            void main()
            {
                if (uUseTexture)
                {
                    FragColor =
                        texture(uTexture, TexCoord)
                        * uColor;
                }
                else
                {
                    FragColor = uColor;
                }
            }";

            uint vs =
                gl.CreateShader(
                    ShaderType.VertexShader
                );

            gl.ShaderSource(vs, vertex);
            gl.CompileShader(vs);

            uint fs =
                gl.CreateShader(
                    ShaderType.FragmentShader
                );

            gl.ShaderSource(fs, fragment);
            gl.CompileShader(fs);

            uint program =
                gl.CreateProgram();

            gl.AttachShader(program, vs);
            gl.AttachShader(program, fs);
            gl.LinkProgram(program);

            gl.DeleteShader(vs);
            gl.DeleteShader(fs);

            return program;
        }

        public void SetVSync(bool enabled)
        {
            window.VSync = enabled;
            vsync_ = enabled;
        }
        public double get_fps()
        {
            return fps;
        }

        public double get_dt()
        {
            return deltaTime;
        }

        public void Close() { window?.Close(); }

        public void LockAndHideMouse(bool lockMouse, bool hideMouse)
        {
            if (primaryMouse == null) return;

            if (lockMouse && hideMouse)
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Disabled;
            }
            else if (lockMouse && !hideMouse)
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Disabled;
            }
            else if (!lockMouse && hideMouse)
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Hidden;
            }
            else
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Normal;
            }
        }

        public void center_mouse()
        {
            if (primaryMouse != null)
            {
                float centerX = Width / 2f;
                float centerY = Height / 2f;

                primaryMouse.Position = new Vector2(centerX, centerY);
                lastMousePos = new Vector2(centerX, centerY);
            }
        }

        public void update_camera(
            float sensitivity,
            bool lock_mouse = true,
            bool hide_mouse = true
        )
        {
            if (primaryMouse == null)
                return;
            LockAndHideMouse(lock_mouse, hide_mouse);

            Vector2 mousePos = primaryMouse.Position;

            if (firstMouseMove)
            {
                lastMousePos = mousePos;
                firstMouseMove = false;
            }

            float deltaX = mousePos.X - lastMousePos.X;
            float deltaY = mousePos.Y - lastMousePos.Y;

            lastMousePos = mousePos;

            deltaX *= sensitivity;
            deltaY *= sensitivity;

            Camera.Rotation.Y -= deltaX;
            Camera.Rotation.X -= deltaY;

            float limit = (float)Math.PI / 2f - 0.01f;

            if (Camera.Rotation.X > limit)
                Camera.Rotation.X = limit;

            if (Camera.Rotation.X < -limit)
                Camera.Rotation.X = -limit;
        }

    }

    public class test
    {
        public string write()
        {
            return "try 4";
        }

        public int add(int a, int b)
        {
            return a + b;
        }
    }
}