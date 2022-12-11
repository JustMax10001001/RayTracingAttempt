using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RayTracing;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[DebuggerDisplay("({X}, {Y}, {Z})")]
public struct Vector3
{
    public float X;
    public float Y;
    public float Z;

    public static readonly Vector3 Zero = new();

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
    public static Vector3 operator /(Vector3 v, float k)
    {
        return v.Divide(k);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator +(Vector3 x, Vector3 y)
    {
        return x.Add(y);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 operator -(Vector3 x, Vector3 y)
    {
        return x.Subtract(y);
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
    public Vector3 Divide(float k)
    {
        return new Vector3
        {
            X = X / k,
            Y = Y / k,
            Z = Z / k,
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
    public Vector3 Subtract(Vector3 other)
    {
        return new Vector3
        {
            X = X - other.X,
            Y = Y - other.Y,
            Z = Z - other.Z
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float DistanceToSquared(Vector3 other)
    {
        return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Magnitude()
    {
        return MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float MagnitudeSquared()
    {
        return X * X + Y * Y + Z * Z;
    }

    public Vector3 Asin()
    {
        return new Vector3(MathF.Asin(X), MathF.Asin(Y), MathF.Asin(Z));
    }

    public void Normalize()
    {
        var magnitude = this.Magnitude();

        X /= magnitude;
        Y /= magnitude;
        Z /= magnitude;
    }
}