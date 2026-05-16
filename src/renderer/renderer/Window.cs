using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Numerics;


namespace csgame
{
    public enum KeyboardKey
    {
        // Letters
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,

        // Numbers
        Number0,
        Number1,
        Number2,
        Number3,
        Number4,
        Number5,
        Number6,
        Number7,
        Number8,
        Number9,

        // Arrows
        Up,
        Down,
        Left,
        Right,

        // Special
        Space,
        Enter,
        Escape,
        Shift,
        Control,
        Alt,
        Backspace,
        Tab,
        CapsLock,

        // Function Keys
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12
    }

/* python:
 *
 * class name (parent) :
 *  def __init__(var:type = 35 ):
 *  .__super__()
 *      pass
 *
 *
 */
    public class MyWindow
    {
        private IWindow window;
        private GL gl;
        private IInputContext input;
        private IKeyboard primaryKeyboard; // Store the keyboard here

        private string Title;
        private int Width;
        private int Height;


        private uint shader;
        private List<Drawable> drawables = new List<Drawable>();

        public (int, int) get_size() => (Width, Height);
        public GL GetGL() => gl;

        // Fixed the Vector2d to Vector2D<double> for Silk.NET compatibility
        public MyWindow(Vector2d<int> size, string title = "My Silk.NET Window")
        {
            Title = title;
            Width = size.X;
            Height = size.Y;

        }


        public bool IsKeyPressed(KeyboardKey key)
        {
            if (primaryKeyboard != null && KeyMap.TryGetValue(key, out var silkKey))
            {
                return primaryKeyboard.IsKeyPressed(silkKey);
            }

            return false;
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


        // Inside MyWindow class
        public void Run(Action<MyWindow> onLoad, Action<double> onUpdate = null)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(Width, Height);
            options.Title = Title;

            window = Window.Create(options);

            window.Load += () =>
            {
                gl = GL.GetApi(window);
                input = window.CreateInput();
                primaryKeyboard = input.Keyboards.Count > 0 ? input.Keyboards[0] : null;

                gl.Viewport(0, 0, (uint)Width, (uint)Height);
                gl.Enable(EnableCap.Blend);
                gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                gl.Enable(EnableCap.DepthTest); // This enables the depth test
                gl.DepthFunc(DepthFunction.Less);
                shader = CreateShader();
                onLoad?.Invoke(this);
            };

            // Add this to handle logic (movement, physics) separate from rendering
            window.Update += (deltaTime) => { onUpdate?.Invoke(deltaTime); };

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

            gl.UseProgram(shader);
            // update the viewport
            window.Resize += (size) =>
            {
                Width = size.X;
                Height = size.Y;

                gl.Viewport(0, 0, (uint)Width, (uint)Height);
            };

            Matrix4x4 view =
                Matrix4x4.CreateLookAt(
                    new Vector3(0, 0, 3),
                    Vector3.Zero,
                    Vector3.UnitY
                );

            float aspect = Width / (float)Height;

            float orthoSize = 1f;

            Matrix4x4 projection =
                Matrix4x4.CreateOrthographicOffCenter(
                    -aspect * orthoSize,
                     aspect * orthoSize,
                    -orthoSize,
                     orthoSize,
                     0.1f,
                     100f
                );

            int viewLoc =
                gl.GetUniformLocation(shader, "view");

            int projLoc =
                gl.GetUniformLocation(shader, "projection");

            gl.UniformMatrix4(
                viewLoc,
                1,
                false,
                (float*)&view
            );

            gl.UniformMatrix4(
                projLoc,
                1,
                false,
                (float*)&projection
            );

            foreach (var drawable in drawables)
            {
                drawable.Draw(gl, shader);
            }
        }


        private uint CreateShader()
        {
            // Updated Vertex Shader to support UVs
            string vertex = @"#version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aTexCoord;

            out vec2 TexCoord;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            void main()
            {
                gl_Position = projection * view * model * vec4(aPos, 1.0);
                TexCoord = aTexCoord;
            }";

            // Updated Fragment Shader to support Texture vs Color
            string fragment = @"#version 330 core
                out vec4 FragColor;
                in vec2 TexCoord;

                uniform vec4 uColor;
                uniform sampler2D uTexture;
                uniform bool uUseTexture; // Toggle: true for images, false for shapes

                void main() {
                    if (uUseTexture) {
                        // Multiply texture by uColor so we can still use alpha/tinting
                        FragColor = texture(uTexture, TexCoord) * uColor;
                    } else {
                        FragColor = uColor;
                    }
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
    }

    public class test
    {
        public string write()
        {
            return "try 3";
        }

        public int add(int a, int b)
        {
            return a + b;
        }
    }
}


