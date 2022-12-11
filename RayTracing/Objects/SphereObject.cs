namespace RayTracing.Objects;

public class SphereObject : IMesh
{
    private readonly float _sphereRadius;
    public Vector3 EmissionColor { get; set; }
    public float Emission { get; set; }

    public Matrix3 Transform { get; set; } = new();

    public SphereObject(float radius = 4, Vector3? color = null, float emission = 0f)
    {
        _sphereRadius = radius;
        EmissionColor = color ?? new Vector3(0.6f, 0.2f, 0.2f);
        Emission = emission;
    }

    public bool TryBounceRay(Ray ray, out Ray bouncedRay)
    {
        var deltaX = Transform.Tx - ray.Origin.X;
        var deltaY = Transform.Ty - ray.Origin.Y;
        var deltaZ = Transform.Tz - ray.Origin.Z;
        
        var rayDirX = ray.Direction.X;
        var rayDirY = ray.Direction.Y;
        var rayDirZ = ray.Direction.Z;
        
        var b = -2 * (rayDirX * deltaX + rayDirY * deltaY + rayDirZ * deltaZ);

        var a = rayDirX * rayDirX + rayDirY * rayDirY + rayDirZ * rayDirZ;
        var c = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ - _sphereRadius * _sphereRadius;
        var discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            bouncedRay = default;
            return false;
        }

        var sqrtD = MathF.Sqrt(discriminant);

        var t1 = (sqrtD - b) / 2 * a;
        var t2 = (-sqrtD - b) / 2 * a;

        if (t1 < 0 && t2 < 0)
        {
            bouncedRay = default;
            return false;
        }

        bouncedRay = Ray.CreateBounced(ray, Math.Min(t1, t2), Emission, EmissionColor);
        return true;
    }
}