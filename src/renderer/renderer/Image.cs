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

            if (!File.Exists(_path))
                throw new FileNotFoundException("Image not found at: " + _path);

            using (var stream = File.OpenRead(_path))
            {
                StbImage.stbi_set_flip_vertically_on_load(1);

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
        }
        protected override void UpdateBuffer(GL gl)
        {
            float x = _pos.X;
            float y = _pos.Y;
            float w = _size.X;
            float h = _size.Y;
            float z = 0f;
            float[] vertices =
            {
                x,     y,     z, 0f, 1f,
                x+w,   y,     z, 1f, 1f,
                x,     y+h,   z, 0f, 0f,

                x+w,   y,     z, 1f, 1f,
                x+w,   y+h,   z, 1f, 0f,
                x,     y+h,   z, 0f, 0f
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
            vao = gl.GenVertexArray();
            vbo = gl.GenBuffer();

            gl.BindVertexArray(vao);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            // Position
            gl.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                (void*)0
            );
            gl.EnableVertexAttribArray(0);

            // UV
            gl.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                (void*)(3 * sizeof(float))
            );
            gl.EnableVertexAttribArray(1);

            initialized = true;
        }

        public void Move_ip(float x, float y)
        {
            _pos.ChangeBy(x, y);
        }
    }

    public unsafe class Text : Image
    {
        public string _text { get; private set; }
        public int _fontSize { get; private set; }

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

                _size = new Vector2d<float>(
                    width / 800f,
                    height / 600f
                );

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
        }
    }
}