namespace RayTracing;

public struct Ray
{
    public Vector3 Color { get; set; } = default;
    
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    public Ray()
    {
    }

    public static Ray CreateBounced(Ray ray, float t, float emission, Vector3 emissionVectorColor)
    {
        var newRay = new Ray
        {
            Origin = ray.Origin + t * new Vector3(ray.Direction.X, ray.Direction.Y, ray.Direction.Z)
        };

        newRay.Color = ColorVector.GetDistantColor(emissionVectorColor, emission, ray.Origin, newRay.Origin);

        return newRay;
    }
}