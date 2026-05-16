using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using StbImageSharp;

// Aliases to resolve naming conflicts between libraries
using GLPixelFormat = Silk.NET.OpenGL.PixelFormat;
using GDPixelFormat = System.Drawing.Imaging.PixelFormat;
using GDRectangle = System.Drawing.Rectangle;

namespace csgame
{
    
    public unsafe class Image : Drawable
    {
        protected uint _texture;
        protected Vector2d<float> _pos; // Using your custom Vector2d
        protected Vector2d<float> _size;
        protected string _path;
        public Transform Transform = new Transform();

        public Image(string path, Vector2d<float> pos, Vector2d<float> size, float alpha = 1f)
        {
            _path = path;
            _pos = pos;
            _size = size;
            this.a = alpha;
        }

        // Constructor for Text to use internally
        protected Image(Vector2d<float> pos, Vector2d<float> size, float alpha)
        {
            _pos = pos;
            _size = size;
            this.a = alpha;
        }

        private void LoadTexture(GL gl)
        {
            _texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, _texture);

            string relativePath = Path.Combine("..", "..", _path);
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Image not found at: " + fullPath);

            using (var stream = File.OpenRead(fullPath))
            {
                StbImage.stbi_set_flip_vertically_on_load(1);
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                fixed (byte* ptr = image.Data)
                {
                    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width,
                        (uint)image.Height, 0, GLPixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
            }

            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
            {
                Initialize(gl);
                LoadTexture(gl);
            }

            UpdateBuffer(gl);

            gl.Uniform1(gl.GetUniformLocation(shader, "uUseTexture"), 1);
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _texture);
            gl.Uniform1(gl.GetUniformLocation(shader, "uTexture"), 0);
            gl.Uniform4(gl.GetUniformLocation(shader, "uColor"), 1f, 1f, 1f, a);

            gl.BindVertexArray(vao);
            gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        protected override void UpdateBuffer(GL gl)
        {
            float x = _pos.X;
            float y = _pos.Y;
            float w = _size.X;
            float h = _size.Y;
            float z = 1f;
            float[] vertices =
            {
                x, y, z, 0f, 0f,
                x + w, y, z, 1f, 0f,
                x, y + h, z, 0f, 1f,
                x + w, y, z, 1f, 0f,
                x + w, y + h, z, 1f, 1f,
                x, y + h, z, 0f, 1f
            };

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), v,
                    BufferUsageARB.DynamicDraw);
            }
        }

        public override void Initialize(GL gl)
        {
            base.Initialize(gl);
            gl.BindVertexArray(vao);
            gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);
            gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                (void*)(2 * sizeof(float)));
            gl.EnableVertexAttribArray(1);
        }
    }

    public unsafe class Text : Image
    {
        private string _text;
        private int _fontSize;
        private Vector4d<float> _vecColor; // Store your custom vector color
        private FontStyle _style;

        public Text(string text, Vector2d<float> pos, int size, Vector4d<float> color, bool bold = false)
            : base(pos, new Vector2d<float>(0, 0), 1.0f)
        {
            _text = text;
            _fontSize = size;
            _vecColor = color; // (R, G, B, A)
            _style = bold ? FontStyle.Bold : FontStyle.Regular;
        }

        private void CreateTextTexture(GL gl)
        {
            using (Font font = new Font("Arial", _fontSize, _style))
            using (Bitmap tempBmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(tempBmp))
            {
                // 1. Measure string
                SizeF size = g.MeasureString(_text, font);
                int width = (int)Math.Ceiling(size.Width);
                int height = (int)Math.Ceiling(size.Height);
                _size = new Vector2d<float>(width, height);

                // 2. Convert your Vector4d to System.Drawing.Color
                // Note: Vector4d is 0.0-1.0, Color.FromArgb is 0-255
                Color drawingColor = Color.FromArgb(
                    (int)Math.Min(255, _vecColor.A * 255),
                    (int)Math.Min(255, _vecColor.R * 255),
                    (int)Math.Min(255, _vecColor.G * 255),
                    (int)Math.Min(255, _vecColor.B * 255)
                );

                using (Bitmap bitmap = new Bitmap(width, height, GDPixelFormat.Format32bppArgb))
                {
                    using (Graphics gfx = Graphics.FromImage(bitmap))
                    {
                        gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        gfx.Clear(Color.Transparent);

                        using (Brush brush = new SolidBrush(drawingColor))
                        {
                            gfx.DrawString(_text, font, brush, 0, 0);
                        }
                    }

                    // 3. Upload to OpenGL
                    _texture = gl.GenTexture();
                    gl.BindTexture(TextureTarget.Texture2D, _texture);

                    BitmapData data = bitmap.LockBits(new GDRectangle(0, 0, width, height),
                        ImageLockMode.ReadOnly, GDPixelFormat.Format32bppArgb);

                    gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0,
                        GLPixelFormat.Bgra, PixelType.UnsignedByte, (void*)data.Scan0);

                    bitmap.UnlockBits(data);

                    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
                    gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
                }
            }
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
            {
                Initialize(gl);
                CreateTextTexture(gl);
            }

            base.Draw(gl, shader);
        }
    }
}