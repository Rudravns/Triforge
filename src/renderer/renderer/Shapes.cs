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
        public bool ScreenSpace = false;
        protected bool initialized = false;

        protected float r, g, b, a;

        public Transform Transform = new Transform();

        public abstract void Draw(GL gl, uint shader);

        protected abstract void UpdateBuffer(GL gl);

        public virtual void Initialize(
     GL gl,
     bool textured = false,
     bool hasNormals = false
 )
        {
            vao = gl.GenVertexArray();
            vbo = gl.GenBuffer();

            gl.BindVertexArray(vao);
            gl.BindBuffer(
                BufferTargetARB.ArrayBuffer,
                vbo
            );

            if (hasNormals)
            {
                // XYZ UV NORMAL
                gl.VertexAttribPointer(
                    0,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    8 * sizeof(float),
                    (void*)0
                );

                gl.EnableVertexAttribArray(0);

                gl.VertexAttribPointer(
                    1,
                    2,
                    VertexAttribPointerType.Float,
                    false,
                    8 * sizeof(float),
                    (void*)(3 * sizeof(float))
                );

                gl.EnableVertexAttribArray(1);

                gl.VertexAttribPointer(
                    2,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    8 * sizeof(float),
                    (void*)(5 * sizeof(float))
                );

                gl.EnableVertexAttribArray(2);
            }
            else if (textured)
            {
                // XYZ UV
                gl.VertexAttribPointer(
                    0,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    5 * sizeof(float),
                    (void*)0
                );

                gl.EnableVertexAttribArray(0);

                gl.VertexAttribPointer(
                    1,
                    2,
                    VertexAttribPointerType.Float,
                    false,
                    5 * sizeof(float),
                    (void*)(3 * sizeof(float))
                );

                gl.EnableVertexAttribArray(1);
            }
            else
            {
                // XYZ
                gl.VertexAttribPointer(
                    0,
                    3,
                    VertexAttribPointerType.Float,
                    false,
                    3 * sizeof(float),
                    (void*)0
                );

                gl.EnableVertexAttribArray(0);
            }

            initialized = true;
        }

        protected void ApplyModelMatrix(
            GL gl,
            uint shader
        )
        {
            Matrix4x4 model =
                Transform.GetModelMatrix();

            int modelLoc =
                gl.GetUniformLocation(
                    shader,
                    "model"
                );

            gl.UniformMatrix4(
                modelLoc,
                1,
                false,
                (float*)&model
            );

            int screenSpaceLoc =
                gl.GetUniformLocation(
                    shader,
                    "uScreenSpace"
                );

            gl.Uniform1(
                screenSpaceLoc,
                ScreenSpace ? 1 : 0
            );
        }
    }

    public unsafe class Rectangle : Drawable
    {
        private Rect rectData;
        public float z;

        public Rectangle(Rect rect, Vector4d<float> color, float z)
        {
            rectData = rect;
            this.z = z;
            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;

            Transform.Position = new Vector3d<float>(
                rect.X + rect.W / 2f,
                rect.Y + rect.H / 2f,
                z
            );
        }

        public override void Draw(GL gl, uint shader)
        {
            if (!initialized)
                Initialize(gl);
            gl.Disable(EnableCap.CullFace);
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
            gl.Enable(EnableCap.CullFace);
        }

        protected override void UpdateBuffer(GL gl)
        {
            float w = rectData.W;
            float h = rectData.H;

            float hw = w / 2f;
            float hh = h / 2f;

            float[] vertices =
            {
                // Triangle 1
                -hw, -hh, this.z,
                 hw, -hh, this.z,
                -hw,  hh, this.z,

                // Triangle 2
                 hw, -hh, this.z,
                 hw,  hh, this.z,
                -hw,  hh, this.z
            };

            gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

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

            float cx =
                (points[0].X +
                 points[1].X +
                 points[2].X) / 3f;

            float cy =
                (points[0].Y +
                 points[1].Y +
                 points[2].Y) / 3f;

            float cz =
                (points[0].Z +
                 points[1].Z +
                 points[2].Z) / 3f;

            Transform.Position =
                new Vector3d<float>(
                    cx,
                    cy,
                    cz
                );
        }

        protected override void UpdateBuffer(GL gl)
        {
            float cx = Transform.Position.X;
            float cy = Transform.Position.Y;
            float cz = Transform.Position.Z;

            float[] vertices =
            {
            points[0].X - cx,
            points[0].Y - cy,
            points[0].Z - cz,

            points[1].X - cx,
            points[1].Y - cy,
            points[1].Z - cz,

            points[2].X - cx,
            points[2].Y - cy,
            points[2].Z - cz
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

            gl.Disable(EnableCap.CullFace);

            int useTexLoc =
                gl.GetUniformLocation(
                    shader,
                    "uUseTexture"
                );

            gl.Uniform1(useTexLoc, 0);

            ApplyModelMatrix(gl, shader);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int colorLoc =
                gl.GetUniformLocation(
                    shader,
                    "uColor"
                );

            gl.Uniform4(
                colorLoc,
                r, g, b, a
            );

            gl.DrawArrays(
                PrimitiveType.Triangles,
                0,
                3
            );

            gl.Enable(EnableCap.CullFace);
        }

        public void Move(
            Vector3d<float> amount)
        {
            Transform.Position += amount;
        }

        public void Rotate(
            Vector3d<float> amount)
        {
            Transform.Rotation += amount;
        }
    }


    public unsafe class Circle : Drawable
    {
        private Vector2d<float> center;
        private float radius;
        private int segments;
        public float z;

        public Circle(Vector2d<float> center, float radius, Vector4d<float> color, int segments = 36, float z = 0f)
        {
            this.center = center;
            this.radius = radius;
            this.segments = Math.Max(3, segments); // Needs at least 3 segments to form a shape

            // Normalize colors (Matches your Rectangle setup)
            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
            this.z = z;

            Transform.Position = new Vector3d<float>(center.X, center.Y, z);
        }

        public float[] pos()
        {
            return new float[]{Transform.Position.X, Transform.Position.Y, Transform.Position.Z};
        }
    

    public override void Draw(GL gl, uint shader)
        {
            if (!initialized) Initialize(gl);
            gl.Disable(EnableCap.CullFace);
            int useTexLoc = gl.GetUniformLocation(shader, "uUseTexture");
            gl.Uniform1(useTexLoc, 0);

            ApplyModelMatrix(gl, shader);
            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int colorLoc = gl.GetUniformLocation(shader, "uColor");
            gl.Uniform4(colorLoc, r, g, b, a);

            // +2 vertices: 1 for the center vertex, 1 to close the loop at the start/end
            gl.DrawArrays(PrimitiveType.TriangleFan, 0, (uint)(segments + 2));
            gl.Enable(EnableCap.CullFace);
        }

        protected override void UpdateBuffer(GL gl)
        {
            // Vertex layout: Center point + peripheral points surrounding it
            List<float> vertices = new List<float>
            {
                0f, 0f, 0f
            };

            for (int i = 0; i <= segments; i++)
            {
                float angle =
                    i * ((float)Math.PI * 2f) / segments;

                float x =
                    radius * (float)Math.Cos(angle);

                float y =
                    radius * (float)Math.Sin(angle);

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

        public void Rotate(Vector3d<float> amount)
        {
            Transform.Rotation += amount;
        }
    }
}




