namespace RayTracing;

public struct ColorF
{
    public float Red { get; set; }
    public float Green { get; set; }
    public float Blue { get; set; }

    public ColorF(float r, float g, float b)
    {
        Red = r;
        Green = g;
        Blue = b;
    }

    public static ColorF GetDistantColor(ColorF emissionColor, float emission, Vector3 origin, Vector3 dest)
    {
        var distanceSquared = origin.DistanceToSquared(dest);
        var brightnessCoefficient = emission / distanceSquared;

        return new ColorF(Math.Clamp(emissionColor.Red * brightnessCoefficient, 0.0f, 1.0f),
            Math.Clamp(emissionColor.Green * brightnessCoefficient, 0.0f, 1.0f),
            Math.Clamp(emissionColor.Blue * brightnessCoefficient, 0.0f, 1.0f));
    }
}