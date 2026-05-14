 using System;
namespace csgame
{
    public class Vector2d<T>
    {
        public T X { get; set; }
        public T Y { get; set; }

        // 1. Generic Constructor (The <T> constructor)
        public Vector2d(T x, T y)
        {
            X = x;
            Y = y;
        }

        // 2. Copy Constructor
        public Vector2d(Vector2d<T> other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        // 3. Vector + Vector Addition
        public static Vector2d<T> operator +(Vector2d<T> a, Vector2d<T> b)
        {
            return new Vector2d<T>((dynamic)a.X + (dynamic)b.X, (dynamic)a.Y + (dynamic)b.Y);
        }

        // 4. Vector + Scalar (Number) Addition
        public static Vector2d<T> operator +(Vector2d<T> a, T scalar)
        {
            return new Vector2d<T>((dynamic)a.X + (dynamic)scalar, (dynamic)a.Y + (dynamic)scalar);
        }

        // 5. Vector * Scalar Multiplication
        public static Vector2d<T> operator *(Vector2d<T> a, T scalar)
        {
            return new Vector2d<T>((dynamic)a.X * (dynamic)scalar, (dynamic)a.Y * (dynamic)scalar);
        }

        // Your ChangeBy Method using dynamic to support any numeric T
        public void ChangeBy(T dx, T dy)
        {
            X = (dynamic)X + dx;
            Y = (dynamic)Y + dy;
        }

        // Dot product
        public T Dot(Vector2d<T> other)
        {
            return (dynamic)this.X * other.X + (dynamic)this.Y * other.Y;
        }

        // Overriding ToString for easy debugging
        public override string ToString() => $"({X}, {Y})";
    }



    public class Vector3d<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
        public T Z { get; set; }

        // 1. Generic Constructor (The <T> constructor)
        public Vector3d(T x, T y, T z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        // 2. Copy Constructor
        public Vector3d(Vector3d<T> other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }

        // 3. Vector + Vector Addition
        public static Vector3d<T> operator +(Vector3d<T> a, Vector3d<T> b)
        {
            return new Vector3d<T>((dynamic)a.X + (dynamic)b.X , (dynamic)a.Y + (dynamic)b.Y, (dynamic)a.Z + (dynamic)b.Z);
        }

        // 4. Vector + Scalar (Number) Addition
        public static Vector3d<T> operator + (Vector3d<T> a, T scalar)
        {
            return new Vector3d<T>((dynamic)a.X + (dynamic)scalar, (dynamic)a.Y + (dynamic)scalar, (dynamic)a.Z + (dynamic)scalar);
        }

        // 5. Vector * Scalar Multiplication
        public static Vector3d<T> operator *(Vector3d<T> a, T scalar)
        {
            return new Vector3d<T>((dynamic)a.X * (dynamic)scalar, (dynamic)a.Y * (dynamic)scalar, (dynamic)a.Z * (dynamic)scalar);
        }

        // Your ChangeBy Method using dynamic to support any numeric T
        public void ChangeBy(T dx, T dy, T dz)
        {
            X = (dynamic)X + dx;
            Y = (dynamic)Y + dy;
            Z = (dynamic)Y + dz;
        }

        //dot product 
        public T Dot(Vector3d<T> other)
        {
            return (dynamic)this.X * other.X + (dynamic)this.Y * other.Y + (dynamic)this.Z * other.Z;
        }

        // Overriding ToString for easy debugging
        public override string ToString() => $"({X}, {Y}, {Z})";
    }

    public class Vector4d<T>
    {
        public T R { get; set; }
        public T G { get; set; }
        public T B { get; set; }
        public T A { get; set; }

        // 1. Generic Constructor
        public Vector4d(T r, T g, T b, T a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        // 2. Copy Constructor
        public Vector4d(Vector4d<T> other)
        {
            this.R = other.R;
            this.G = other.G;
            this.B = other.B;
            this.A = other.A;
        }

        // 3. Vector + Vector Addition
        public static Vector4d<T> operator +(Vector4d<T> a, Vector4d<T> b)
        {
            return new Vector4d<T>(
                (dynamic)a.R + (dynamic)b.R,
                (dynamic)a.G + (dynamic)b.G,
                (dynamic)a.B + (dynamic)b.B,
                (dynamic)a.A + (dynamic)b.A
            );
        }

        // 4. Vector + Scalar Addition
        public static Vector4d<T> operator +(Vector4d<T> a, T scalar)
        {
            return new Vector4d<T>(
                (dynamic)a.R + (dynamic)scalar,
                (dynamic)a.G + (dynamic)scalar,
                (dynamic)a.B + (dynamic)scalar,
                (dynamic)a.A + (dynamic)scalar
            );
        }

        // 5. Vector * Scalar Multiplication
        public static Vector4d<T> operator *(Vector4d<T> a, T scalar)
        {
            return new Vector4d<T>(
                (dynamic)a.R * (dynamic)scalar,
                (dynamic)a.G * (dynamic)scalar,
                (dynamic)a.B * (dynamic)scalar,
                (dynamic)a.A * (dynamic)scalar
            );
        }

        // ChangeBy Method
        public void ChangeBy(T dr, T dg, T db, T da)
        {
            R = (dynamic)R + dr;
            G = (dynamic)G + dg;
            B = (dynamic)B + db;
            A = (dynamic)A + da;
        }

        public override string ToString() => $"({R}, {G}, {B}, {A})";
    }


    public class Rect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }

        // 1. Generic Constructor
        public Rect(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;

        }

        public void Move_ip(float x, float y)
        {
            X += x;
            Y += y;
        }
        

        public override string ToString() => $"({X}, {Y}, {W}, {H})";
    }
}