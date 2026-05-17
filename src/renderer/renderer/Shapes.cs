using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace csgame
{

    public abstract unsafe class Drawable
    {
        protected uint vao;
        protected uint vbo;

        protected bool initialized = false;

        protected float r, g, b, a;

        public Transform Transform = new Transform();

        public abstract void Draw(GL gl, uint shader);

        protected abstract void UpdateBuffer(GL gl);

        public virtual void Initialize(GL gl)
        {
            vao = gl.GenVertexArray();
            vbo = gl.GenBuffer();

            gl.BindVertexArray(vao);
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

            // NOW USING VEC3
            gl.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                3 * sizeof(float),
                (void*)0
            );

            gl.EnableVertexAttribArray(0);

            initialized = true;
        }

        protected void ApplyModelMatrix(GL gl, uint shader)
        {
            Matrix4x4 model = Transform.GetModelMatrix();

            int modelLoc =
                gl.GetUniformLocation(shader, "model");

            gl.UniformMatrix4(
                modelLoc,
                1,
                false,
                (float*)&model
            );
        }
    }

    public unsafe class Rectangle : Drawable
    {
        private Rect rectData;

        public Rectangle(Rect rect, Vector4d<float> color)
        {
            rectData = rect;

            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
                Initialize(gl);

            int useTexLoc =
                gl.GetUniformLocation(shader, "uUseTexture");

            gl.Uniform1(useTexLoc, 0);

            ApplyModelMatrix(gl, shader);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int colorLoc =
                gl.GetUniformLocation(shader, "uColor");

            gl.Uniform4(colorLoc, r, g, b, a);

            gl.DrawArrays(
                PrimitiveType.Triangles,
                0,
                6
            );
        }

        protected override void UpdateBuffer(GL gl)
        {
            float x = rectData.X;
            float y = rectData.Y;
            float w = rectData.W;
            float h = rectData.H;

            float z = 0f;

            // 2 TRIANGLES = RECTANGLE
            float[] vertices =
            {
                // Triangle 1
                x,     y,     z,
                x+w,   y,     z,
                x,     y+h,   z,

                // Triangle 2
                x+w,   y,     z,
                x+w,   y+h,   z,
                x,     y+h,   z
            };

            gl.BindBuffer(
                BufferTargetARB.ArrayBuffer,
                vbo
            );

            fixed (float* v = vertices)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (uint)(vertices.Length * sizeof(float)),
                    v,
                    BufferUsageARB.DynamicDraw
                );
            }
        }

        public void Move(float dx, float dy)
        {
            rectData.Move_ip(dx, dy);
        }
    }

    public unsafe class Triangle : Drawable
    {
        private List<Vector3d<float>> points;

        public Triangle(
            List<Vector3d<float>> points,
            Vector4d<float> color)
        {
            this.points = points;

            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
        }

        protected override void UpdateBuffer(GL gl)
        {
            float[] vertices =
            {
                points[0].X, points[0].Y, points[0].Z,
                points[1].X, points[1].Y, points[1].Z,
                points[2].X, points[2].Y, points[2].Z
            };

            gl.BindBuffer(
                BufferTargetARB.ArrayBuffer,
                vbo
            );

            fixed (float* v = vertices)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (uint)(vertices.Length * sizeof(float)),
                    v,
                    BufferUsageARB.DynamicDraw
                );
            }
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
                Initialize(gl);

            int useTexLoc =
                gl.GetUniformLocation(shader, "uUseTexture");

            gl.Uniform1(useTexLoc, 0);

            ApplyModelMatrix(gl, shader);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int colorLoc =
                gl.GetUniformLocation(shader, "uColor");

            gl.Uniform4(colorLoc, r, g, b, a);

            gl.DrawArrays(
                PrimitiveType.Triangles,
                0,
                3
            );
        }
    }


    public unsafe class Circle : Drawable
    {
        private Vector2d<float> center;
        private float radius;
        private int segments;

        public Circle(Vector2d<float> center, float radius, Vector4d<float> color, int segments = 36)
        {
            this.center = center;
            this.radius = radius;
            this.segments = Math.Max(3, segments); // Needs at least 3 segments to form a shape

            // Normalize colors (Matches your Rectangle setup)
            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized) Initialize(gl);

            int useTexLoc = gl.GetUniformLocation(shader, "uUseTexture");
            gl.Uniform1(useTexLoc, 0);

            ApplyModelMatrix(gl, shader);
            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int colorLoc = gl.GetUniformLocation(shader, "uColor");
            gl.Uniform4(colorLoc, r, g, b, a);

            // +2 vertices: 1 for the center vertex, 1 to close the loop at the start/end
            gl.DrawArrays(PrimitiveType.TriangleFan, 0, (uint)(segments + 2));
        }

        protected override void UpdateBuffer(GL gl)
        {
            // Vertex layout: Center point + peripheral points surrounding it
            List<float> vertices = new List<float>
            {
                center.X, center.Y, 0f // Center point
            };

            for (int i = 0; i <= segments; i++)
            {
                // Calculate current angle around the circle
                float angle = i * ((float)Math.PI * 2f) / segments;

                float x = center.X + (radius * (float)Math.Cos(angle));
                float y = center.Y + (radius * (float)Math.Sin(angle));
                float z = 0f;

                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(z);
            }

            float[] verticesArray = vertices.ToArray();

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (float* v = verticesArray)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (uint)(verticesArray.Length * sizeof(float)),
                    v,
                    BufferUsageARB.DynamicDraw
                );
            }
        }

        public void Move(float dx, float dy)
        {
            center.X += dx;
            center.Y += dy;
        }
    }
}




