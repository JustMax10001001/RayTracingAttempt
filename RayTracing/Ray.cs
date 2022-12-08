namespace RayTracing;

public struct Ray
{
    public ColorF Color { get; set; } = default;
    
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    public Ray()
    {
    }

    public static Ray CreateBounced(Ray ray, float t, float emission, ColorF emissionColor)
    {
        var newRay = new Ray
        {
            Origin = ray.Origin + t * new Vector3(ray.Direction.X, ray.Direction.Y, ray.Direction.Z)
        };

        newRay.Color = ColorF.GetDistantColor(emissionColor, emission, ray.Origin, newRay.Origin);

        return newRay;
    }
}