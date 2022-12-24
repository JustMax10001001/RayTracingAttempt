using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RayTracing.Objects;

namespace RayTracing;

[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public sealed class Renderer
{
    private const int RaysPerCast = 4;

    private readonly Random _random = new();
    private readonly float _horizontalFovRad;

    private readonly MainForm _mainForm;
    private readonly List<IMesh> _objects = new();

    private FastBitmap _nextFrame = new(1, 1);

    public Image NextFrame;
    public Camera Camera { get; }

    public Renderer(MainForm mainForm, float horizontalFovDegrees = 120)
    {
        _mainForm = mainForm;

        _horizontalFovRad = horizontalFovDegrees / 180f * MathF.PI;
        NextFrame = _nextFrame.GetImage();

        Camera = new Camera
        {
            Pitch = 0.0f,
            Yaw = -0.0f,
            PosX = 0,
            PosZ = 0
        };


        _objects.Add(new SphereObject(emission: 200)
        {
            Transform = new Matrix3
            {
                Tx = 8,
                Tz = 0
            }
        });
        _objects.Add(new SphereObject(emission: 200)
        {
            Transform = new Matrix3
            {
                Tx = -8,
                Tz = 0
            }
        });
        _objects.Add(new SphereObject(emission: 200, color: new Vector3(0, 0.7f, 0.7f))
        {
            Transform = new Matrix3
            {
                Tx = 0,
                Tz = 8
            }
        });
        _objects.Add(new SphereObject(emission: 200, color: new Vector3(0, 0.7f, 0.7f))
        {
            Transform = new Matrix3
            {
                Tx = 0,
                Tz = -8
            }
        });
        _objects.Add(new SphereObject(color: new Vector3(0, 0.7f, 0.7f), emission: 200)
        {
            Transform = new Matrix3
            {
                Ty = -8,
                Tx = 0,
                Tz = 0
            }
        });
        _objects.Add(new SphereObject(color: new Vector3(0, 0.7f, 0.7f), emission: 200)
        {
            Transform = new Matrix3
            {
                Ty = 8,
                Tx = 0,
                Tz = 0
            }
        });

        _objects.Add(new Plane(new Vector3(0, 1.0f, 0.0f))
        {
            Transform = new Matrix3
            {
                Tz = 0,
                Ty = -4
            }
        });

        /*_objects.Add(new SphereObject(radius: 2, emission: 400, color: new Vector3(0.2f, 0.8f, 0.2f))
        {
            Transform = new Matrix3
            {
                Tx = 11.5f,
                Tz = -5.5f
            }
        });
        _objects.Add(new SphereObject(radius: 2, emission: 400, color: new Vector3(0.2f, 0.8f, 0.2f))
        {
            Transform = new Matrix3
            {
                Tx = 11.5f,
                Tz = -5.5f,
                Ty = -7,
            }
        });*/


        _objects.Add(new Skybox());
    }

    public void BeginRendering()
    {
        while (!_mainForm.IsDisposed)
        {
            var now = DateTime.Now;

            RenderNextFrame(_mainForm.Width, _mainForm.Height);

            Debug.WriteLine($"Rendered in {DateTime.Now.Subtract(now).TotalMilliseconds} ms");

            NextFrame = _nextFrame.GetImage();

            _mainForm.InvokeRender();
        }
    }

    private void RenderNextFrame(int width, int height)
    {
        bool hasPreviousFrame = false;

        if (_nextFrame is null || _nextFrame.Width != width || _nextFrame.Height != height)
        {
            _nextFrame?.Dispose();
            _nextFrame = new FastBitmap(width, height);
            hasPreviousFrame = false;
        }

        var cameraTransform = Camera.Transform;
        var screenToWorldTransform = CreateScreenToWorldTransform(_horizontalFovRad, width, height);

        var pixelWidthOnScreen = 1f / width;
        var screenY = 0f;

        for (int pixelY = 0; pixelY < height; pixelY++, screenY += pixelWidthOnScreen)
        {
            var screenX = 0f;

            for (int pixelX = 0; pixelX < width; pixelX++, screenX += pixelWidthOnScreen)
            {
                var color = RenderPixel(new Vector3(screenX, screenY, 0), pixelWidthOnScreen, screenToWorldTransform,
                    cameraTransform);

                if (hasPreviousFrame)
                {
                    const float prevFrameCoefficient = 0.85f;

                    color = prevFrameCoefficient * ColorVector.FromRgb(_nextFrame.GetColor(pixelX, pixelY))
                            + (1 - prevFrameCoefficient) * color;
                }

                _nextFrame.SetColor(pixelX, pixelY, color);
            }
        }
    }

    private Vector3 RenderPixel(Vector3 pixelTopLeftScreenPosition, float pixelWidth, Matrix3 screenToWorldTransform,
        Matrix3 cameraTransform)
    {
        var pixelRgb = new Vector3();

        for (int rayIndex = 0; rayIndex < RaysPerCast; rayIndex++)
        {
            var randomVerticalDelta = pixelWidth * _random.NextSingle();
            var randomHorizontalDelta = pixelWidth * _random.NextSingle();

            var screenPosition = new Vector3
            {
                X = pixelTopLeftScreenPosition.X + randomHorizontalDelta,
                Y = pixelTopLeftScreenPosition.Y + randomVerticalDelta
            };

            var worldRayEnd = screenPosition * screenToWorldTransform * cameraTransform;
            var worldRayOrigin = Vector3.Zero * cameraTransform;

            var worldRayDirection = worldRayEnd - worldRayOrigin;
            worldRayDirection.Normalize();

            var ray = new Ray
            {
                Direction = worldRayDirection,
                Origin = worldRayOrigin,
            };

            for (var objectIndex = 0; objectIndex < _objects.Count; objectIndex++)
            {
                if (!_objects[objectIndex].TryBounceRay(ray, out var newRay))
                {
                    continue;
                }

                if (_objects[objectIndex] is Plane)
                {
                    for (int reflectedObjectIndex = 0; reflectedObjectIndex < _objects.Count; reflectedObjectIndex++)
                    {
                        if (reflectedObjectIndex == objectIndex)
                        {
                            continue;
                        }

                        if (!_objects[reflectedObjectIndex].TryBounceRay(newRay, out var newNewRay))
                        {
                            continue;
                        }

                        newRay = newNewRay;
                        break;
                    }
                    
                    newRay.Color = newRay.Color * 0.9f + new Vector3(1, 1, 1) * 0.1f;
                }

                pixelRgb += newRay.Color;

                break;
            }
        }

        return pixelRgb / RaysPerCast;
    }

    private static Matrix3 CreateScreenToWorldTransform(float horizontalFovRad, float width, float height)
    {
        var halfFov = horizontalFovRad / 2;

        var screenZ = 0.5f * MathF.Atan(halfFov);
        var screenHeightHalf = 0.5f * height / width;

        return new Matrix3
        {
            M22 = -1,
            Tx = -0.5f,
            Ty = screenHeightHalf,
            Tz = screenZ
        };
    }
}