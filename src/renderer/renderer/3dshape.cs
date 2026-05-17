using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;
using StbImageSharp;

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

    public enum CubeFace
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }
    public unsafe class Rect3D : Drawable
    {
        private Vector3d<float> size;
        private Dictionary<CubeFace, uint> textures = new Dictionary<CubeFace, uint>();

        private Dictionary<CubeFace, string> pendingTextures =new Dictionary<CubeFace, string>();
        private GL currentGL;


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
            currentGL = gl;

            float w = size.X / 2f;
            float h = size.Y / 2f;
            float d = size.Z / 2f;

            float[] vertices =
            {
            // FRONT
            -w,-h, d, 0f,0f,
             w,-h, d, 1f,0f,
             w, h, d, 1f,1f,

             w, h, d, 1f,1f,
            -w, h, d, 0f,1f,
            -w,-h, d, 0f,0f,

            // BACK
            -w,-h,-d, 0f,0f,
            -w, h,-d, 0f,1f,
             w, h,-d, 1f,1f,

             w, h,-d, 1f,1f,
             w,-h,-d, 1f,0f,
            -w,-h,-d, 0f,0f,

            // LEFT
            -w, h, d, 1f,1f,
            -w, h,-d, 0f,1f,
            -w,-h,-d, 0f,0f,

            -w,-h,-d, 0f,0f,
            -w,-h, d, 1f,0f,
            -w, h, d, 1f,1f,

            // RIGHT
             w, h, d, 0f,1f,
             w,-h,-d, 1f,0f,
             w, h,-d, 1f,1f,

             w,-h,-d, 1f,0f,
             w, h, d, 0f,1f,
             w,-h, d, 0f,0f,

            // TOP
            -w, h,-d, 0f,1f,
            -w, h, d, 0f,0f,
             w, h, d, 1f,0f,

             w, h, d, 1f,0f,
             w, h,-d, 1f,1f,
            -w, h,-d, 0f,1f,

            // BOTTOM
            -w,-h,-d, 0f,1f,
             w,-h, d, 1f,0f,
            -w,-h, d, 0f,0f,

             w,-h, d, 1f,0f,
            -w,-h,-d, 0f,1f,
             w,-h,-d, 1f,1f
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
                Initialize(gl, true);

            currentGL = gl;

            foreach (var kv in pendingTextures)
            {
                if (!textures.ContainsKey(kv.Key))
                {
                    textures[kv.Key] =
                        LoadTexture(kv.Value);
                }
            }
            ApplyModelMatrix(gl, shader);

            UpdateBuffer(gl);

            gl.BindVertexArray(vao);

            int useTexLoc =
                gl.GetUniformLocation(shader, "uUseTexture");

            int colorLoc =
                gl.GetUniformLocation(shader, "uColor");

            gl.Uniform4(colorLoc, r, g, b, a);

            CubeFace[] faces =
            {
                CubeFace.Front,
                CubeFace.Back,
                CubeFace.Left,
                CubeFace.Right,
                CubeFace.Top,
                CubeFace.Bottom
            };

            for (int i = 0; i < 6; i++)
            {
                if (textures.ContainsKey(faces[i]))
                {
                    gl.Uniform1(useTexLoc, 1);

                    gl.ActiveTexture(TextureUnit.Texture0);

                    gl.BindTexture(
                        TextureTarget.Texture2D,
                        textures[faces[i]]
                    );
                }
                else
                {
                    gl.Uniform1(useTexLoc, 0);
                }

                gl.DrawArrays(
                    PrimitiveType.Triangles,
                    i * 6,
                    6
                );
            }
        }

        private unsafe uint LoadTexture(
            string path
        )
        {
            uint tex =
                currentGL.GenTexture();

            currentGL.BindTexture(
                TextureTarget.Texture2D,
                tex
            );

            ImageResult img;

            using (var stream = File.OpenRead(path))
            {
                img = ImageResult.FromStream(
                    stream,
                    ColorComponents.RedGreenBlueAlpha
                );
            }

            fixed (byte* d = img.Data)
            {
                currentGL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    InternalFormat.Rgba,
                    (uint)img.Width,
                    (uint)img.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    d
                );
            }

            currentGL.GenerateMipmap(
                TextureTarget.Texture2D
            );

            return tex;
        }


        public void assign_tex(
            string path,
            int face
        )
        {
            CubeFace cubeFace =
                (CubeFace)face;

            pendingTextures[cubeFace] = path;
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