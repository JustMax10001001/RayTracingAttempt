namespace RayTracing.Objects;

public interface IMesh: IGameObject
{
    Vector3 EmissionColor { get; }
    float Emission { get; }

    bool TryBounceRay(Ray ray, out Ray bouncedRay);
}