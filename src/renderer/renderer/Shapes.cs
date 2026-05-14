using System.Collections.Generic;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace csgame
{
    public abstract unsafe class Drawable
    {
        protected uint vao;
        protected uint vbo;
        protected bool initialized = false;
        protected float r, g, b, a;

        // Abstract methods: Children MUST implement these
        public abstract void Draw(GL gl, uint shader);
        protected abstract void UpdateBuffer(GL gl);

        // Virtual method: Children CAN use this or override it


        public virtual void Initialize(GL gl)
        {
            vao = gl.GenVertexArray();
            vbo = gl.GenBuffer();
            gl.BindVertexArray(vao);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            // Attributes are usually the same for simple 2D shapes
            gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);
            initialized = true;
        }
    }

    public unsafe class Rectangle : Drawable
    {
        private Rect rectData;

        public Rectangle(Rect rect, Vector4d<float> color)
        {
            this.rectData = rect;
            this.r = color.R / 255;
            this.g = color.G / 255f;
            this.b = color.B / 255f;
            this.a = color.A;
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized) Initialize(gl);

            int useTexLoc = gl.GetUniformLocation(shader, "uUseTexture");
            gl.Uniform1(useTexLoc, 0);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            // Reset layout for solid shapes
            gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);
            gl.DisableVertexAttribArray(1);

            int colorLoc = gl.GetUniformLocation(shader, "uColor");
            gl.Uniform4(colorLoc, r, g, b, a);

            gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        protected override void UpdateBuffer(GL gl)
        {
            float x = rectData.X;
            float y = rectData.Y;
            float w = rectData.W;
            float h = rectData.H;

            float[] vertices =
            {
                x, y, x + w, y, x, y + h, // Tri 1
                x + w, y, x + w, y + h, x, y + h // Tri 2
            };

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = vertices)
            {
                gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), v,
                    BufferUsageARB.DynamicDraw);
            }
        }

        public void Move(float dx, float dy) => rectData.Move_ip(dx, dy);
    }


    public unsafe class Triangle : Drawable
    {
        private List<Vector2d<int>> points;

        public Triangle(List<Vector2d<int>> points, Vector4d<float> color)
        {

            this.points = points;
            this.r = color.R / 255f;
            this.g = color.G / 255f;
            this.b = color.B / 255f;
            this.a = color.A;
        }

        protected override void UpdateBuffer(GL gl)
        {
            float[] vertices =
            {
                (float)points[0].X, (float)points[0].Y,
                (float)points[1].X, (float)points[1].Y,
                (float)points[2].X, (float)points[2].Y
            };

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            fixed (float* v = vertices)
            {
                uint size = (uint)(vertices.Length * sizeof(float));

                // Use BufferData instead of BufferSubData to ensure memory is allocated.
                // If you only want to allocate once, you can keep using BufferData 
                // with DynamicDraw; it is very fast for 3 vertices.
                gl.BufferData(BufferTargetARB.ArrayBuffer, size, v, BufferUsageARB.DynamicDraw);
            }
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized) Initialize(gl);

            // 1. Tell the shader NOT to use texture logic
            int useTexLoc = gl.GetUniformLocation(shader, "uUseTexture");
            gl.Uniform1(useTexLoc, 0);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            // 2. FORCE the layout to 2 floats (Stride: 2 * sizeof(float))
            // This fixes the crash caused by the Image class changing the stride to 4.
            gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), (void*)0);
            gl.EnableVertexAttribArray(0);
            gl.DisableVertexAttribArray(1); // Hide UVs

            int colorLoc = gl.GetUniformLocation(shader, "uColor");
            gl.Uniform4(colorLoc, r, g, b, a);

            gl.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }


    }
}