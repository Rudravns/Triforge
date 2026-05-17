using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace csgame
{
    public class Transform
    {
        public Vector3d<float> Position =
            new Vector3d<float>(0f, 0f, 0f);

        public Vector3d<float> Rotation =
            new Vector3d<float>(0f, 0f, 0f);

        public Vector3d<float> Scale =
            new Vector3d<float>(1f, 1f, 1f);

        public Matrix4x4 GetModelMatrix()
        {
            Matrix4x4 translation =
                Matrix4x4.CreateTranslation(
                    Position.X,
                    Position.Y,
                    Position.Z
                );

            Matrix4x4 rotationX =
                Matrix4x4.CreateRotationX(Rotation.X);

            Matrix4x4 rotationY =
                Matrix4x4.CreateRotationY(Rotation.Y);

            Matrix4x4 rotationZ =
                Matrix4x4.CreateRotationZ(Rotation.Z);

            Matrix4x4 scale =
                Matrix4x4.CreateScale(
                    Scale.X,
                    Scale.Y,
                    Scale.Z
                );

            return scale *
                   rotationX *
                   rotationY *
                   rotationZ *
                   translation;
        }

        public void move_ip(Vector3d<float> p)
        {
            Position += p;
        }

        public void rotate_ip(Vector3d<float> p)
        {
            Rotation += p;
        }

        public void scale_ip(Vector3d<float> p)
        {
            Scale += p;
        }
    }

    public unsafe class Rect3D : Drawable
    {
        private Vector3d<float> size;

        public Rect3D(
            Vector3d<float> pos,
            Vector3d<float> size,
            Vector4d<float> color
        )
        {
            this.size = size;

            Transform.Position = pos;

            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
        }

        protected override void UpdateBuffer(GL gl)
        {
            float w = size.X / 2f;
            float h = size.Y / 2f;
            float d = size.Z / 2f;

            float[] vertices =
            {
                // FRONT
                -w, -h,  d,
                 w, -h,  d,
                 w,  h,  d,

                 w,  h,  d,
                -w,  h,  d,
                -w, -h,  d,

                // BACK
                -w, -h, -d,
                -w,  h, -d,
                 w,  h, -d,

                 w,  h, -d,
                 w, -h, -d,
                -w, -h, -d,

                // LEFT
                -w,  h,  d,
                -w,  h, -d,
                -w, -h, -d,

                -w, -h, -d,
                -w, -h,  d,
                -w,  h,  d,

                // RIGHT
                 w,  h,  d,
                 w, -h, -d,
                 w,  h, -d,

                 w, -h, -d,
                 w,  h,  d,
                 w, -h,  d,

                // TOP
                -w,  h, -d,
                -w,  h,  d,
                 w,  h,  d,

                 w,  h,  d,
                 w,  h, -d,
                -w,  h, -d,

                // BOTTOM
                -w, -h, -d,
                 w, -h,  d,
                -w, -h,  d,

                 w, -h,  d,
                -w, -h, -d,
                 w, -h, -d
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
                36
            );
        }

        public void SetSize(Vector3d<float> newSize)
        {
            size = newSize;
        }

        public void Move(Vector3d<float> amount)
        {
            Transform.Position += amount;
        }

        public void Rotate(Vector3d<float> amount)
        {
            Transform.Rotation += amount;
        }
    }
}