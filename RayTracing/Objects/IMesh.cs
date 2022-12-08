namespace RayTracing.Objects;

public interface IMesh: IGameObject
{
    ColorF EmissionColor { get; }
    float Emission { get; }

    bool TryBounceRay(Ray ray, out Ray bouncedRay);
}