using System.Drawing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace RayTracing.Benchmarks;

public class Program
{
    static void Main()
    {
        BenchmarkRunner.Run<FastBitmapBenchmark>();
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