namespace RayTracing.Objects;

public class Plane : IMesh
{
    private static readonly float ReflectivityCone = MathF.PI / 8;
    private static readonly Random Random = new();

    public Matrix3 Transform { get; set; }
    public Vector3 EmissionColor { get; }
    public float Emission { get; }

    public float Reflectivity { get; } = 0.9f;

    public Vector3 NormalVector { get; set; }

    public Plane(Vector3 normalVector = default)
    {
        NormalVector = normalVector == default ? new Vector3(0, 0, 1f) : normalVector;
    }

    public bool TryBounceRay(Ray ray, out Ray bouncedRay)
    {
        var normalVectorWorld = NormalVector * Transform - Transform.GetTranslateVector();

        var cosRayNormal = ray.Direction.DotProduct(normalVectorWorld);


        var rayProjectionOnNormal = normalVectorWorld * cosRayNormal;
        var newRayDirection = ray.Direction - 2 * rayProjectionOnNormal;

        var origin = Transform.GetTranslateVector();

        // D
        var freePlaneParameter = -normalVectorWorld.DotProduct(origin);
        var rayNormalDotProduct = cosRayNormal;
        var t = -(freePlaneParameter + ray.Origin.DotProduct(normalVectorWorld)) / rayNormalDotProduct;

        if (t <= 0 || t > 200)
        {
            bouncedRay = default;
            return false;
        }

        var maxReflectionDeviation = ReflectivityCone * (1 - Reflectivity);

        var reflectivityRotation = Matrix3.CreateRotation((Random.NextSingle() / 2 - 1) * maxReflectionDeviation, 0,
            (Random.NextSingle() / 2 - 1) * maxReflectionDeviation);

        var newOrigin = ray.Origin + t * ray.Direction;
        bouncedRay = new Ray
        {
            Origin = newOrigin,
            Direction = newRayDirection * reflectivityRotation,
        };
        return true;
    }
}