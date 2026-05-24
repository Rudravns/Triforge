using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Silk.NET.OpenGL;
using Silk.NET.Maths;
using StbImageSharp;

using GLPixelFormat = Silk.NET.OpenGL.PixelFormat;
using GDPixelFormat = System.Drawing.Imaging.PixelFormat;
using GDRectangle = System.Drawing.Rectangle;

namespace csgame
{
    public unsafe class Image : Drawable
    {
        protected uint _texture;
        protected Vector2d<float> _pos;
        protected Vector2d<float> _size;
        protected string _path;

        public Image(string path, Vector2d<float> pos, Vector2d<float> size, float alpha = 1f)
        {
            _path = path;
            _pos = pos;
            _size = size;
            this.a = alpha;
            Transform.Position = new Vector3d<float>(pos.X, pos.Y, 0f);
        }

        protected Image(Vector2d<float> pos, Vector2d<float> size, float alpha)
        {
            _pos = pos;
            _size = size;
            this.a = alpha;
            Transform.Position = new Vector3d<float>(pos.X, pos.Y, 0f);
        }

        private void LoadTexture(GL gl)
        {
            _texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, _texture);

            if (!File.Exists(_path))
                throw new FileNotFoundException("Image not found at: " + _path);

            using (var stream = File.OpenRead(_path))
            {
                // Disable flip on load so row 0 (top) stays at the start of memory
                StbImage.stbi_set_flip_vertically_on_load(0) ;

                ImageResult image = ImageResult.FromStream(
                    stream,
                    ColorComponents.RedGreenBlueAlpha
                );

                fixed (byte* ptr = image.Data)
                {
                    gl.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        InternalFormat.Rgba,
                        (uint)image.Width,
                        (uint)image.Height,
                        0,
                        GLPixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        ptr
                    );
                    gl.GenerateMipmap(TextureTarget.Texture2D);
                }
            }

            gl.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int)GLEnum.ClampToEdge
            );

            gl.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int)GLEnum.ClampToEdge
            );
            gl.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)GLEnum.LinearMipmapLinear
            );

            gl.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)GLEnum.Linear
            );
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
            {
                Initialize(gl);
                LoadTexture(gl);
            }
            gl.Disable(EnableCap.CullFace);
            UpdateBuffer(gl);

            ApplyModelMatrix(gl, shader);

            gl.Uniform1(gl.GetUniformLocation(shader, "uUseTexture"), 1);

            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _texture);

            gl.Uniform1(gl.GetUniformLocation(shader, "uTexture"), 0);

            gl.Uniform4(
                gl.GetUniformLocation(shader, "uColor"),
                1f, 1f, 1f, a
            );

            gl.BindVertexArray(vao);

            gl.DrawArrays(
                PrimitiveType.Triangles,
                0,
                6
            );
            gl.Enable(EnableCap.CullFace);
        }

        protected override void UpdateBuffer(GL gl)
        {
            float x_min, y_min, x_max, y_max;
            float z = 0f;

            if (ScreenSpace)
            {
                // Placement is handled by Transform.Position (set in constructor or Move_ip).
                // We draw relative to the origin, where (0,0) is the Top-Left of the image/text.
                x_min = 0;
                y_min = 0; 
                x_max = _size.X;
                y_max = _size.Y;
            }
            else
            {
                // In 3D world space, scale down pixel sizes to appropriate metric sizes.
                float scaleFactor = 0.01f;
                float w = _size.X * scaleFactor;
                float h = _size.Y * scaleFactor;
                x_min = -w / 2.0f;
                x_max = w / 2.0f;
                y_min = -h / 2.0f; 
                y_max = h / 2.0f;
            }
            
            // Bitmaps are top-down (row 0 is top). 
            // In UI (Y-down), y_min is top. In World (Y-up), y_max is top.
            float v_top_val = 0f;
            float v_bottom_val = 1f;

            float[] vertices =
            {
                // We define quads such that they are CCW in their respective coordinate systems
                x_min, y_max, z, 0f, (ScreenSpace ? v_bottom_val : v_top_val),    // Top-Left semantic
                x_min, y_min, z, 0f, (ScreenSpace ? v_top_val : v_bottom_val),    // Bottom-Left semantic
                x_max, y_min, z, 1f, (ScreenSpace ? v_top_val : v_bottom_val),    // Bottom-Right semantic

                x_min, y_max, z, 0f, (ScreenSpace ? v_bottom_val : v_top_val),    // Top-Left semantic
                x_max, y_min, z, 1f, (ScreenSpace ? v_top_val : v_bottom_val),    // Bottom-Right semantic
                x_max, y_max, z, 1f, (ScreenSpace ? v_bottom_val : v_top_val)     // Top-Right semantic
            };

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), v,
                    BufferUsageARB.DynamicDraw);
            }
        }

        public override void Initialize(
            GL gl,
            bool textured = true,
            bool hasNormals = false
        )
        {
            base.Initialize(gl, true, false);
        }

        public void Move_ip(float x, float y)
        {
            _pos.ChangeBy(x, y);
            Transform.Position = new Vector3d<float>(_pos.X, _pos.Y, 0f);
        }
    }

    public unsafe class Text : Image
    {
        public string _text { get; private set; }
        public int _fontSize { get; private set; }
        private int _baseFontSize;

        private Vector4d<float> _vecColor;
        private FontStyle _style;

        public bool _bold;
        public bool _italic;
        public bool _underline;

        private bool _dirty = true;

        public Text(
            string text,
            Vector2d<float> pos,
            int size,
            Vector4d<float> color,
            bool bold = false,
            bool italic = false,
            bool underline = false
        ) : base(pos, new Vector2d<float>(0, 0), 1.0f)
        {
            _text = text;
            _fontSize = size;
            _baseFontSize = size;
            _vecColor = color;

            _bold = bold;
            _italic = italic;
            _underline = underline;

            FontStyle style = FontStyle.Regular;

            if (bold)
                style |= FontStyle.Bold;

            if (italic)
                style |= FontStyle.Italic;

            if (underline)
                style |= FontStyle.Underline;

            _style = style;
        }

        public void set_text(string text)
        {
            if (_text != text)
            {
                _text = text;
                _dirty = true;
            }
        }

        public void set_size(int size)
        {
            if (_fontSize != size)
            {
                _fontSize = size;
                _dirty = true;
            }
        }

        public void SetBold(bool bold)
        {
            _bold = bold;

            if (bold)
                _style |= FontStyle.Bold;
            else
                _style &= ~FontStyle.Bold;

            _dirty = true;
        }

        public void SetItalic(bool italic)
        {
            _italic = italic;

            if (italic)
                _style |= FontStyle.Italic;
            else
                _style &= ~FontStyle.Italic;

            _dirty = true;
        }

        public void SetUnderline(bool underline)
        {
            _underline = underline;

            if (underline)
                _style |= FontStyle.Underline;
            else
                _style &= ~FontStyle.Underline;

            _dirty = true;
        }

        private void CreateTextTexture(GL gl)
        {
            if (_texture != 0)
            {
                gl.DeleteTexture(_texture);
            }

            using (Font font = new Font("Arial", _fontSize, _style))
            using (Bitmap tempBmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(tempBmp))
            {
                SizeF size = g.MeasureString(_text, font);

                int width = Math.Max(1, (int)Math.Ceiling(size.Width));
                int height = Math.Max(1, (int)Math.Ceiling(size.Height));
                
                float wr = (float)MyWindow.Width / MyWindow.initialWidth;
                float hr = (float)MyWindow.Height / MyWindow.initialHeight;
                float ratio = Math.Max(0.001f, Math.Min(wr, hr));

                // Scale the logical size back down so it fits the fixed UI projection
                _size = new Vector2d<float>(width / ratio, height / ratio);

                Color drawingColor = Color.FromArgb(
                    (int)Math.Min(255, _vecColor.A * 255),
                    (int)Math.Min(255, _vecColor.R * 255),
                    (int)Math.Min(255, _vecColor.G * 255),
                    (int)Math.Min(255, _vecColor.B * 255)
                );

                using (Bitmap bitmap = new Bitmap(
                    width,
                    height,
                    GDPixelFormat.Format32bppArgb
                ))
                {
                    // No manual flip needed as UVs are now adjusted to top-down memory
                    using (Graphics gfx = Graphics.FromImage(bitmap))
                    {
                        gfx.TextRenderingHint =
                            TextRenderingHint.AntiAliasGridFit;

                        gfx.Clear(Color.Transparent);

                        using (Brush brush = new SolidBrush(drawingColor))
                        {
                            gfx.DrawString(
                                _text,
                                font,
                                brush,
                                0,
                                0
                            );
                        }
                    }

                    _texture = gl.GenTexture();

                    gl.BindTexture(
                        TextureTarget.Texture2D,
                        _texture
                    );

                    BitmapData data = bitmap.LockBits(
                        new GDRectangle(0, 0, width, height),
                        ImageLockMode.ReadOnly,
                        GDPixelFormat.Format32bppArgb
                    );

                    gl.TexImage2D(
                        TextureTarget.Texture2D,
                        0,
                        InternalFormat.Rgba,
                        (uint)width,
                        (uint)height,
                        0,
                        GLPixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        (void*)data.Scan0
                    );

                    bitmap.UnlockBits(data);

                    gl.GenerateMipmap(TextureTarget.Texture2D);

                    gl.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int)GLEnum.LinearMipmapLinear
                    );

                    gl.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int)GLEnum.Linear
                    );

                    gl.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapS,
                        (int)GLEnum.ClampToEdge
                    );

                    gl.TexParameter(
                        TextureTarget.Texture2D,
                        TextureParameterName.TextureWrapT,
                        (int)GLEnum.ClampToEdge
                    );
                }
            }
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
            {
                Initialize(gl);
                initialized = true;
                _dirty = true;
            }

            // Apply the user's font scaling formula
            float wr = (float)MyWindow.Width / MyWindow.initialWidth;
            float hr = (float)MyWindow.Height / MyWindow.initialHeight;
            int targetFontSize = (int)Math.Max(1, Math.Min(wr * _baseFontSize, hr * _baseFontSize));

            if (targetFontSize != _fontSize)
            {
                _fontSize = targetFontSize;
                _dirty = true;
            }

            gl.Disable(EnableCap.CullFace);
            gl.DepthMask(false);
            if (_dirty)
            {
                CreateTextTexture(gl);
                _dirty = false;
            }

            UpdateBuffer(gl);

            ApplyModelMatrix(gl, shader);

            gl.Uniform1(
                gl.GetUniformLocation(shader, "uUseTexture"),
                1
            );

            gl.ActiveTexture(TextureUnit.Texture0);

            gl.BindTexture(
                TextureTarget.Texture2D,
                _texture
            );

            gl.Uniform1(
                gl.GetUniformLocation(shader, "uTexture"),
                0
            );

            gl.Uniform4(
                gl.GetUniformLocation(shader, "uColor"),
                1f,
                1f,
                1f,
                1f
            );

            gl.BindVertexArray(vao);

            gl.DrawArrays(
                PrimitiveType.Triangles,
                0,
                6
            );
            gl.DepthMask(true);
            gl.Enable(EnableCap.CullFace);

        }
    }
}