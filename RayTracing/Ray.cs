namespace RayTracing;

public struct Ray
{
    public ColorF Color { get; set; } = default;
    
    public Vector3 Origin { get; set; }
    public float Kx { get; set; }
    public float Ky { get; set; }
    public float Kz { get; set; }

    public Ray()
    {
    }

    public static Ray CreateBounced(Ray ray, float t, float emission, ColorF emissionColor)
    {
        var newRay = new Ray
        {
            Origin = ray.Origin + t * new Vector3(ray.Kx, ray.Ky, ray.Kz)
        };

        newRay.Color = ColorF.GetDistantColor(emissionColor, emission, ray.Origin, newRay.Origin);

        return newRay;
    }
}