namespace RayTracing;

public struct Matrix3
{
    public float Tx;
    public float Ty;
    public float Tz;

    public float M11;
    public float M12;
    public float M13;

    public float M21;
    public float M22;
    public float M23;

    public float M31;
    public float M32;
    public float M33;

    public Matrix3()
    {
        M11 = 1;
        M22 = 1;
        M33 = 1;
    }

    public static Matrix3 operator *(Matrix3 a, Matrix3 b)
    {
        return a.Multiply(b);
    }

    public Matrix3 Multiply(Matrix3 b)
    {
        return new Matrix3
        {
            M11 = M11 * b.M11 + M12 * b.M21 + M13 * b.M31,
            M12 = M11 * b.M12 + M12 * b.M22 + M13 * b.M32,
            M13 = M11 * b.M13 + M12 * b.M23 + M13 * b.M33,

            M21 = M21 * b.M11 + M22 * b.M21 + M23 * b.M31,
            M22 = M21 * b.M12 + M22 * b.M22 + M23 * b.M32,
            M23 = M21 * b.M13 + M22 * b.M23 + M23 * b.M33,

            M31 = M31 * b.M11 + M32 * b.M21 + M33 * b.M31,
            M32 = M31 * b.M12 + M32 * b.M22 + M33 * b.M32,
            M33 = M31 * b.M13 + M32 * b.M23 + M33 * b.M33,

            Tx = b.Tx + Tx * b.M11 + Ty * b.M21 + Tz * b.M31,
            Ty = b.Ty + Tx * b.M12 + Ty * b.M22 + Tz * b.M32,
            Tz = b.Tz + Tx * b.M13 + Ty * b.M23 + Tz * b.M33
        };
    }

    public static Matrix3 CreateTranspose(float tx, float ty, float tz)
    {
        return new Matrix3
        {
            Tx = tx,
            Ty = ty,
            Tz = tz,
        };
    }

    public static Matrix3 CreateScale(float s)
    {
        return CreateScale(s, s, s);
    }

    public static Matrix3 CreateScale(float sx, float sy, float sz)
    {
        return new Matrix3
        {
            M11 = sx,
            M22 = sy,
            M33 = sz
        };
    }

    public static Matrix3 CreateRotation(float rotX, float rotY, float rotZ)
    {
        var sinRotX = MathF.Sin(rotX);
        var cosRotX = MathF.Cos(rotX);
        var sinRotY = MathF.Sin(rotY);
        var cosRotY = MathF.Cos(rotY);
        var sinRotZ = MathF.Sin(rotZ);
        var cosRotZ = MathF.Cos(rotZ);

        return new Matrix3
        {
            M11 = cosRotX * cosRotY,
            M12 = cosRotX * sinRotY * sinRotZ - sinRotX * cosRotZ,
            M13 = cosRotX * sinRotY * cosRotZ + sinRotX * sinRotZ,

            M21 = sinRotX * cosRotY,
            M22 = sinRotX * sinRotY * sinRotZ + cosRotX * cosRotZ,
            M23 = sinRotX * sinRotY * cosRotZ - cosRotX * sinRotZ,

            M31 = -sinRotY,
            M32 = cosRotY * sinRotZ,
            M33 = cosRotY * cosRotZ
        };
    }

    public Vector3 GetTranslateVector()
    {
        return new Vector3(Tx, Ty, Tz);
    }
}