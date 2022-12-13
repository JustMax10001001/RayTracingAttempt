namespace RayTracing.Objects;

public class Plane: IMesh
{
    public Matrix3 Transform { get; set; }
    public Vector3 EmissionColor { get; }
    public float Emission { get; }

    public float Reflectivity { get; } = 1.0f;
    
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
        
        if (t <= 0)
        {
            bouncedRay = default;
            return false;
        }

        bouncedRay = new Ray
        {
            Origin = ray.Origin + t * ray.Direction,
            Direction = newRayDirection,
        };
        return true;
    }
}