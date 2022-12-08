using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace RayTracing.Benchmarks;

public class Program
{
    static void Main()
    {
        BenchmarkRunner.Run<SinCosBenchmark>();
    }
}

[SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 10)]
[MemoryDiagnoser]
public class SinCosBenchmark
{
    private readonly Random _random = new Random(6999_42420);
    
    [Params(100, 10_000)]
    public int iterations;

    private float[] angles;
    private (float, float)[] results;

    [GlobalSetup]
    public void Setup()
    {
        angles = new float[iterations];
        results = new (float, float)[iterations];
        for (int i = 0; i < iterations; i++)
        {
            angles[i] = _random.NextSingle() * MathF.PI * 2;
        }
    }

    [Benchmark()]
    public void ExplicitCosSin()
    {
        for (int i = 0; i < iterations; i++)
        {
            results[i] = (MathF.Cos(angles[i]), MathF.Sin(angles[i]));
        }
    }
    
    [Benchmark()]
    public void ExplicitSinCos()
    {
        for (int i = 0; i < iterations; i++)
        {
            results[i] = (MathF.Sin(angles[i]), MathF.Cos(angles[i]));
        }
    }
    
    [Benchmark()]
    public void ImplicitSinByCos()
    {
        for (int i = 0; i < iterations; i++)
        {
            var cos = MathF.Cos(angles[i]);
            results[i] = (MathF.Sqrt(1 - cos * cos), cos);
        }
    }
}

[SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 10)]
[MemoryDiagnoser]
public class FastBitmapBenchmark
{
    [Params(new[] { 800, 600 }, new[] { 1280, 720 }, new[] { 1920, 1080 })]
    public int[] dimensions;

    private FastBitmap _bitmap;

    [GlobalSetup]
    public void Setup()
    {
        _bitmap = new FastBitmap(dimensions[0], dimensions[1]);
    }

    [Benchmark]
    public void SetPixels_Mod()
    {
        int width = dimensions[0];
        int height = dimensions[1];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var i = y * width + x;
                _bitmap.SetColor(x, y, i % 256, (i + 85) % 256, (i + 170) % 256);
            }
        }
    }

    [Benchmark]
    public void SetPixels_LogicAnd()
    {
        int width = dimensions[0];
        int height = dimensions[1];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var i = y * width + x;
                _bitmap.SetColor(x, y, i & 0xFF, (i + 85) & 0xFF, (i + 170) & 0xFF);
            }
        }
    }

    [Benchmark]
    public void SetPixels_DirectAddr_LogicAnd()
    {
        int width = dimensions[0];
        int height = dimensions[1];
        int maxAddr = width * height;

        for (int i = 0; i < maxAddr; i++)
        {
            _bitmap.SetColor(i, i & 0xFF, (i + 85) & 0xFF, (i + 170) & 0xFF);
        }
    }
}