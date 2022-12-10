﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RayTracing.Objects;

namespace RayTracing;

[SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
public sealed class Renderer
{
    private const int RaysPerCast = 4;

    private float _horizontalFovRad;
    private readonly float _verticalFovRad;

    private readonly MainForm _mainForm;
    private FastBitmap _nextFrame = new(1, 1);
    private readonly List<IMesh> _objects = new();

    public Image NextFrame;
    public Camera Camera { get; }
    
    public Renderer(MainForm mainForm, float horizontalFovDegrees = 60)
    {
        _mainForm = mainForm;

        _verticalFovRad = horizontalFovDegrees / 180f * MathF.PI;
        NextFrame = _nextFrame.GetImage();

        Camera = new Camera
        {
            Pitch = 0.0f,
            Yaw = -0.0f,
            PosZ = -12,
            PosY = -4
        };

        _objects.Add(new SphereObject(emission: 200)
        {
            Transform = new Matrix3
            {
                Tz = 16
            }
        });

        _objects.Add(new SphereObject(radius: 2, emission: 400, color: new ColorF(0.2f, 0.8f, 0.2f))
        {
            Transform = new Matrix3
            {
                Tz = 11.5f,
                Ty = -5.5f
            }
        });
        _objects.Add(new SphereObject(radius: 2, emission: 400, color: new ColorF(0.2f, 0.8f, 0.2f))
        {
            Transform = new Matrix3
            {
                Tz = 11.5f,
                Ty = -5.5f,
                Tx = -7,
            }
        });
    }

    public void BeginRendering()
    {
        var random = new Random();

        while (!_mainForm.IsDisposed)
        {
            var now = DateTime.Now;

            RenderNextFrame(random, _mainForm.Width, _mainForm.Height);
            
            Debug.WriteLine($"Rendered in {DateTime.Now.Subtract(now).TotalMilliseconds} ms");
        }
    }

    private void RenderNextFrame(Random random, int width, int height)
    {
        _horizontalFovRad = _verticalFovRad / height * width;
        float fovStep = _verticalFovRad / height;
        float fovHalfStep = fovStep / 2;

        if (_nextFrame is null || _nextFrame.Width != width || _nextFrame.Height != height)
        {
            _nextFrame?.Dispose();
            _nextFrame = new FastBitmap(width, height);
        }

        var cameraTransform = Camera.Transform;

        float angleVertical = _verticalFovRad / 2;

        for (int y = 0; y < height; y++)
        {
            float angleHorizontal = -_horizontalFovRad / 2;
            for (int x = 0; x < width; x++)
            {
                float r = 0, g = 0, b = 0;

                for (int rayIndex = 0; rayIndex < RaysPerCast; rayIndex++)
                {
                    var randomAngleVertical = fovStep * random.NextSingle() - fovHalfStep;
                    var randomAngleHorizontal = fovStep * random.NextSingle() - fovHalfStep;

                    var rayAngleHorizontal = angleHorizontal + fovHalfStep + randomAngleHorizontal;
                    var rayAngleVertical = angleVertical + fovHalfStep + randomAngleVertical;

                    var screenDirection = new Vector3
                    {
                        X = MathF.Sin(rayAngleHorizontal) * MathF.Cos(rayAngleVertical),
                        Y = MathF.Sin(rayAngleVertical),
                        Z = MathF.Cos(rayAngleHorizontal) * MathF.Cos(rayAngleVertical),
                    };

                    var worldRayEnd = screenDirection * cameraTransform;
                    var worldRayOrigin = new Vector3() * cameraTransform;

                    var worldRayDirection = worldRayEnd - worldRayOrigin;
                    worldRayDirection.Normalize();

                    var ray = new Ray
                    {
                        Direction = worldRayDirection,
                        Origin = worldRayOrigin,
                    };

                    bool anyRayHit = false;

                    for (var objectIndex = 0; objectIndex < _objects.Count; objectIndex++)
                    {
                        if (!_objects[objectIndex].TryBounceRay(ray, out var newRay))
                        {
                            continue;
                        }

                        r += newRay.Color.Red;
                        g += newRay.Color.Green;
                        b += newRay.Color.Blue;

                        anyRayHit = true;

                        break;
                    }

                    if (anyRayHit)
                    {
                        continue;
                    }

                    if (MathF.Abs(ray.Direction.Y) < 0.003f)
                    {
                        b += 1;
                    }
                    else if (MathF.Abs(ray.Direction.X % (MathF.PI / 18)) < 0.003f)
                    {
                        g += 1;
                    }
                    else if (MathF.Abs(ray.Direction.Z) < 0.003f)
                    {
                        r += 1;
                    }
                    else
                    {
                        g += 0.7f;
                        b += 0.85f;
                    }
                }

                _nextFrame.SetColor(x, y, (int)(255 * r / RaysPerCast), (int)(255 * g / RaysPerCast),
                    (int)(255 * b / RaysPerCast));

                angleHorizontal += fovStep;
            }

            angleVertical -= fovStep;
        }


        NextFrame = _nextFrame.GetImage();

        _mainForm.InvokeRender();
    }
}