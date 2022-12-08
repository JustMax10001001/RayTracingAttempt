using System.Runtime.CompilerServices;

namespace RayTracing;

public struct Vector3
{
    public float X;
    public float Y;
    public float Z;

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(Vector3 x, Matrix3 m)
    {
        return x.Multiply(m);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(float k, Vector3 v)
    {
        return v.Multiply(k);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator *(Vector3 v, float k)
    {
        return v.Multiply(k);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 x, Vector3 y)
    {
        return x.Add(y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Multiply(Matrix3 b)
    {
        return new Vector3
        {
            X = b.Tx + X * b.M11 + Y * b.M21 + Z * b.M31,
            Y = b.Ty + X * b.M12 + Y * b.M22 + Z * b.M32,
            Z = b.Tz + X * b.M13 + Y * b.M23 + Z * b.M33,
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Multiply(float k)
    {
        return new Vector3
        {
            X = k * X,
            Y = k * Y,
            Z = k * Z,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Add(Vector3 other)
    {
        return new Vector3
        {
            X = X + other.X,
            Y = Y + other.Y,
            Z = Z + other.Z
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float DistanceToSquared(Vector3 other)
    {
        return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z);
    }
}