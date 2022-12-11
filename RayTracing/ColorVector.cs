using System.Runtime.CompilerServices;

namespace RayTracing;

public static class ColorVector
{
    public static Vector3 GetDistantColor(Vector3 emissionColor, float emission, Vector3 origin, Vector3 dest)
    {
        var distanceSquared = origin.DistanceToSquared(dest);
        var brightnessCoefficient = emission / distanceSquared;

        return new Vector3(Math.Clamp(emissionColor.X * brightnessCoefficient, 0.0f, 1.0f),
            Math.Clamp(emissionColor.Y * brightnessCoefficient, 0.0f, 1.0f),
            Math.Clamp(emissionColor.Z * brightnessCoefficient, 0.0f, 1.0f));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 FromRgb(int rgb)
    {
        return new Vector3((rgb >> 16 & 0xff) / 255f, (rgb >> 8 & 0xff) / 255f, (rgb & 0xff) / 255f);
    }
}