namespace RayTracing;

public interface IGameObject
{
    Matrix3 Transform { get; }
    ColorF EmissionColor { get; }
    float Emission { get; }

    bool TryBounceRay(Ray ray, out Ray bouncedRay);
}