namespace RayTracing.Objects;

public class Plane: IMesh
{
    public Matrix3 Transform { get; }
    public Vector3 EmissionColor { get; }
    public float Emission { get; }
    
    public Vector3 NormalVector { get; set; }
    public Vector3 Origin { get; set; }

    public Plane(Vector3 normalVector = default, Vector3 origin = default)
    {
        NormalVector = normalVector == default ? new Vector3(0, 1f, 0) : normalVector;
        Origin = origin;
    }
    
    public bool TryBounceRay(Ray ray, out Ray bouncedRay)
    {
        throw new NotImplementedException();
    }
}