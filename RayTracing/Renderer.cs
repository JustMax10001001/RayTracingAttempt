using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RayTracing;

[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public sealed class Renderer
{
    private const int RaysPerCast = 4;

    private float _horizontalFovRad;
    private readonly float _verticalFovRad;

    private readonly MainForm _mainForm;
    private FastBitmap _nextFrame;
    private readonly int _width;
    private readonly int _height;
    private readonly List<IGameObject> _objects = new();

    public float Pitch, Yaw;

    public Image NextFrame;

    public Renderer(MainForm mainForm, float horizontalFovDegrees = 60)
    {
        _mainForm = mainForm;

        _width = _mainForm.Width;
        _height = _mainForm.Height;

        _verticalFovRad = horizontalFovDegrees / 180f * MathF.PI;

        _nextFrame = new FastBitmap(_width, _height);
        NextFrame = _nextFrame.GetImage();

        _objects.Add(new SphereObject(emission: 200)
        {
            Transform =
            {
                Tz = 16
            }
        });

        _objects.Add(new SphereObject(radius: 2, emission: 400, color: new ColorF(0.2f, 0.8f, 0.2f))
        {
            Transform =
            {
                Tz = 11.5f,
                Ty = -5
            }
        });
    }

    public void BeginRendering()
    {
        int frame = 0;
        var random = new Random();

        while (!_mainForm.IsDisposed)
        {
            int width = _mainForm.Width;
            int height = _mainForm.Height;
            _horizontalFovRad = _verticalFovRad / height * width;
            float fovStep = _verticalFovRad / height;
            float fovHalfStep = fovStep / 2;

            _nextFrame.Dispose();
            _nextFrame = new FastBitmap(width, height);
            
            var now = DateTime.Now;
            float angleVertical = _verticalFovRad / 2;

            for (int y = 0; y < height; y++)
            {
                float angleHorizontal = -_horizontalFovRad / 2;
                for (int x = 0; x < width; x++)
                {
                    float r = 0, g = 0, b = 0;

                    for (int j = 0; j < RaysPerCast; j++)
                    {
                        var randomAngle = fovStep * (float)random.NextDouble() - fovHalfStep;

                        var rayAngleHorizontal = Yaw + angleHorizontal + fovHalfStep + randomAngle;
                        var rayAngleVertical = Pitch + angleVertical + fovHalfStep + randomAngle;

                        var ray = new Ray
                        {
                            Kx = MathF.Sin(rayAngleHorizontal) * MathF.Cos(rayAngleVertical),
                            Ky = MathF.Sin(rayAngleVertical),
                            Kz = MathF.Cos(rayAngleHorizontal) * MathF.Cos(rayAngleVertical),
                        };

                        ray.Origin = new Vector3(0, -8, -8);

                        for (var i = 0; i < _objects.Count; i++)
                        {
                            if (!_objects[i].TryBounceRay(ray, out var newRay))
                            {
                                continue;
                            }

                            r += newRay.Color.Red;
                            g += newRay.Color.Green;
                            b += newRay.Color.Blue;

                            break;
                        }
                    }

                    _nextFrame.SetColor(x, y, (int)(255 * r / RaysPerCast), (int)(255 * g / RaysPerCast),
                        (int)(255 * b / RaysPerCast));

                    angleHorizontal += fovStep;
                }

                angleVertical -= fovStep;
            }

            Debug.WriteLine($"Rendered in {DateTime.Now.Subtract(now).TotalMilliseconds} ms");

            NextFrame = (Image)_nextFrame.GetImage();

            _mainForm.InvokeRender();
            ++frame;
        }
    }
}