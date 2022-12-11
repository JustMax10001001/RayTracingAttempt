using RayTracing.Objects;

namespace RayTracing;

public class Skybox : IMesh
{
    private readonly Vector3 skyboxTopColor = new(0.3f, 0.70f, 1f);
    private readonly Vector3 skyboxBottomColor = new(0f, 0.0f, 0.3f);
    private readonly Vector3 skyboxTopBottomDelta;
    
    public Matrix3 Transform { get; } = new();
    public Vector3 EmissionColor { get; } = Vector3.Zero;
    public float Emission { get; } = 0f;

    public Skybox()
    {
        skyboxTopBottomDelta = skyboxTopColor - skyboxBottomColor;
    }

    public bool TryBounceRay(Ray ray, out Ray bouncedRay)
    {
        var rayAngles = ray.Direction.Asin();

        var pixelRgb = new Vector3();

        if (MathF.Abs(ray.Direction.Y) < 0.003f)
        {
            pixelRgb.Z += 1;
        }
        else if (MathF.Abs(ray.Direction.X) < 0.003f)
        {
            pixelRgb.Y += 1;
        }
        else if (MathF.Abs(ray.Direction.Z) < 0.003f)
        {
            pixelRgb.X += 1;
        }
        else
        {
            var sin = MathF.Sin((rayAngles.Y + MathF.PI / 2) / 2);
            pixelRgb = skyboxBottomColor + skyboxTopBottomDelta * MathF.Pow(sin, 0.7f);
        }

        bouncedRay = new Ray
        {
            Color = pixelRgb
        };

        return true;
    }
}