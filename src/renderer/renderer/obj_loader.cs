using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.IO;


namespace csgame
{
    public class Mesh
    {
        public float[] Vertices;
        public uint[] Indices;

        public bool HasNormals;
        public bool HasUVs;

        public Mesh(
            float[] vertices,
            uint[] indices,
            bool hasNormals,
            bool hasUVs
        )
        {
            Vertices = vertices;
            Indices = indices;

            HasNormals = hasNormals;
            HasUVs = hasUVs;
        }
    }

    public unsafe class Model : Drawable
    {
        private Mesh mesh;
        private uint ebo;

        public Model(
            Mesh mesh,
            Vector4d<float> color
        )
        {
            this.mesh = mesh;

            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A;
        }

        public override void Initialize(
            GL gl,
            bool textured = true,
            bool hasNormals = false
        )
        {
            base.Initialize(gl, true, mesh.HasNormals);

            ebo = gl.GenBuffer();

            gl.BindVertexArray(vao);

            gl.BindBuffer(
                BufferTargetARB.ElementArrayBuffer,
                ebo
            );

            fixed (uint* inds = mesh.Indices)
            {
                gl.BufferData(
                    BufferTargetARB.ElementArrayBuffer,
                    (uint)(mesh.Indices.Length * sizeof(uint)),
                    inds,
                    BufferUsageARB.StaticDraw
                );
            }

            initialized = true;
        }

        protected override void UpdateBuffer(GL gl)
        {
            gl.BindBuffer(
                BufferTargetARB.ArrayBuffer,
                vbo
            );

            fixed (float* verts = mesh.Vertices)
            {
                gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (uint)(mesh.Vertices.Length * sizeof(float)),
                    verts,
                    BufferUsageARB.StaticDraw
                );
            }
        }

        public override void Draw(
            GL gl,
            uint shader
        )
        {
            if (!initialized)
                Initialize(gl, true);

            ApplyModelMatrix(gl, shader);

            UpdateBuffer(gl);

            gl.Uniform1(
                gl.GetUniformLocation(shader, "uUseTexture"),
                0
            );

            int colorLoc = gl.GetUniformLocation(shader, "uColor");

            gl.Uniform4(
                colorLoc,
                r,
                g,
                b,
                a
            );

            gl.BindVertexArray(vao);

            gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)mesh.Indices.Length,
                DrawElementsType.UnsignedInt,
                null
            );
        }
    }

    public static class ObjLoader
    {
        public static Mesh Load(string path)
        {
            List<Vector3> positions =
                new List<Vector3>();

            List<Vector2> uvs =
                new List<Vector2>();

            List<Vector3> normals =
                new List<Vector3>();

            List<float> vertices =
                new List<float>();

            List<uint> indices =
                new List<uint>();

            Dictionary<string, uint> vertexMap =
                new Dictionary<string, uint>();

            uint currentIndex = 0;

            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("v "))
                {
                    string[] p =
                        line.Split(
                            new char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries);

                    positions.Add(
                        new Vector3(
                            float.Parse(p[1], CultureInfo.InvariantCulture),
                            float.Parse(p[2], CultureInfo.InvariantCulture),
                            float.Parse(p[3], CultureInfo.InvariantCulture)
                        ));
                }
                else if (line.StartsWith("vt "))
                {
                    string[] p =
                        line.Split(
                            new char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries);

                    uvs.Add(
                        new Vector2(
                            float.Parse(p[1], CultureInfo.InvariantCulture),
                            float.Parse(p[2], CultureInfo.InvariantCulture)
                        ));
                }
                else if (line.StartsWith("vn "))
                {
                    string[] p =
                        line.Split(
                            new char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries);

                    normals.Add(
                        new Vector3(
                            float.Parse(p[1], CultureInfo.InvariantCulture),
                            float.Parse(p[2], CultureInfo.InvariantCulture),
                            float.Parse(p[3], CultureInfo.InvariantCulture)
                        ));
                }
                else if (line.StartsWith("f "))
                {
                    string[] p =
                        line.Split(
                            new char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries);

                    List<uint> faceIndices =
                        new List<uint>();

                    for (int i = 1; i < p.Length; i++)
                    {
                        string key = p[i];

                        if (!vertexMap.ContainsKey(key))
                        {
                            string[] parts =
                                key.Split('/');

                            int posIndex =
                                int.Parse(parts[0]) - 1;

                            int uvIndex =
                                (parts.Length > 1 &&
                                 parts[1] != "")
                                ? int.Parse(parts[1]) - 1
                                : -1;

                            int normalIndex =
                                (parts.Length > 2)
                                ? int.Parse(parts[2]) - 1
                                : -1;

                            Vector3 pos =
                                positions[posIndex];

                            Vector2 uv =
                                uvIndex >= 0
                                ? uvs[uvIndex]
                                : Vector2.Zero;

                            Vector3 normal =
                                normalIndex >= 0
                                ? normals[normalIndex]
                                : Vector3.UnitY;

                            vertices.Add(pos.X);
                            vertices.Add(pos.Y);
                            vertices.Add(pos.Z);

                            vertices.Add(uv.X);
                            vertices.Add(uv.Y);

                            vertices.Add(normal.X);
                            vertices.Add(normal.Y);
                            vertices.Add(normal.Z);

                            vertexMap[key] =
                                currentIndex++;
                        }

                        faceIndices.Add(
                            vertexMap[key]);
                    }

                    if (faceIndices.Count == 3)
                    {
                        indices.Add(faceIndices[0]);
                        indices.Add(faceIndices[1]);
                        indices.Add(faceIndices[2]);
                    }
                    else if (faceIndices.Count == 4)
                    {
                        indices.Add(faceIndices[0]);
                        indices.Add(faceIndices[1]);
                        indices.Add(faceIndices[2]);

                        indices.Add(faceIndices[0]);
                        indices.Add(faceIndices[2]);
                        indices.Add(faceIndices[3]);
                    }
                }
            }

            return new Mesh(
                vertices.ToArray(),
                indices.ToArray(),
                normals.Count > 0,
                uvs.Count > 0
            );
        }
    }
}