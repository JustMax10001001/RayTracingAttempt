namespace RayTracing.Objects;

public class Camera : IGameObject
{
    private Matrix3 _transform = new();
    private float _pitch;
    private float _yaw;
    private float _posX;
    private float _posY;
    private float _posZ;

    public Matrix3 Transform => _transform;

    public float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = value;

            UpdateTransform();
        }
    }

    public float Yaw
    {
        get => _yaw;
        set
        {
            _yaw = value;

            UpdateTransform();
        }
    }

    public float PosX
    {
        get => _posX;
        set
        {
            _posX = value;

            var t = _transform;

            t.Tx = _posX;

            _transform = t;
        }
    }

    public float PosY
    {
        get => _posY;
        set
        {
            _posY = value;

            var t = _transform;

            t.Ty = _posY;

            _transform = t;
        }
    }

    public float PosZ
    {
        get => _posZ;
        set
        {
            _posZ = value;

            var t = _transform;

            t.Tz = _posZ;

            _transform = t;
        }
    }

    private void UpdateTransform()
    {
        var newTransform = Matrix3.CreateRotation(0, 0, _pitch) * Matrix3.CreateRotation(0, _yaw, 0);

        newTransform.Tx = _transform.Tx;
        newTransform.Ty = _transform.Ty;
        newTransform.Tz = _transform.Tz;

        _transform = newTransform;
    }
}