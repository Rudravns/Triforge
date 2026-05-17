using System;
using System.Numerics;

namespace csgame
{
    public class Camera
    {
        public Vector3d<float> Position =
            new Vector3d<float>(0f, 0f, 3f);

        public Vector3d<float> Rotation =
            new Vector3d<float>(0f, 0f, 0f);

        public float MoveSpeed = 2f;
        public float RotationSpeed = 2f;

        public Matrix4x4 GetViewMatrix()
        {
            Vector3 position = new Vector3(
                Position.X,
                Position.Y,
                Position.Z
            );

            Matrix4x4 rotation =
                Matrix4x4.CreateRotationX(Rotation.X) *
                Matrix4x4.CreateRotationY(Rotation.Y) *
                Matrix4x4.CreateRotationZ(Rotation.Z);

            Vector3 forward =
                Vector3.Transform(-Vector3.UnitZ, rotation);

            Vector3 up =
                Vector3.Transform(Vector3.UnitY, rotation);

            return Matrix4x4.CreateLookAt(
                position,
                position + forward,
                up
            );
        }

        public void Move(Vector3d<float> amount)
        {
            Position += amount;
        }

        public void Rotate(Vector3d<float> amount)
        {
            Rotation += amount;
        }

        public Vector3d<float> MoveForward(
            float amount,
            bool update = true
        )
        {
            var new_vec = new Vector3d<float>(
                -(float)Math.Sin(Rotation.Y) * amount,
                0f,
                -(float)Math.Cos(Rotation.Y) * amount
            );

            if (update)
            {
                Position += new_vec;
            }

            return new_vec;
        }

        public Vector3d<float> MoveBackward(
            float amount,
            bool update = true
        )
        {
            var new_vec =
                -MoveForward(amount, false);

            if (update)
            {
                Position += new_vec;
            }

            return new_vec;
        }

        public Vector3d<float> MoveRight(
            float amount,
            bool update = true
        )
        {
            var forward =
                MoveForward(1f, false);

            var up =
                new Vector3d<float>(
                    0f,
                    1f,
                    0f
                );

            var new_vec =
                forward.Cross(up)
                        .normalize() * amount;

            if (update)
            {
                Position += new_vec;
            }

            return new_vec;
        }

        public Vector3d<float> MoveLeft(
            float amount,
            bool update = true
        )
        {
            var new_vec =
                -MoveRight(amount, false);

            if (update)
            {
                Position += new_vec;
            }

            return new_vec;
        }


        public void MoveUp(float amount)
        {
            Position.Y += amount;
        }

        public void MoveDown(float amount)
        {
            Position.Y -= amount;
        }
    }
}