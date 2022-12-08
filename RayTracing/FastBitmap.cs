using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RayTracing;

public sealed class FastBitmap : IDisposable
{
    private bool _isDisposed;

    private readonly int _width;
    private readonly int _height;

    private readonly int[] _bits;
    private readonly Bitmap _bitmapImpl;

    private GCHandle _bitsBufferHandle;

    public FastBitmap(int width, int height)
    {
        _width = width;
        _height = height;
        
        _bits = new int[width * height];

        _bitsBufferHandle = GCHandle.Alloc(_bits, GCHandleType.Pinned);
        _bitmapImpl = CreateBitmapFromBits(width, height, _bitsBufferHandle);
    }

    public void SetColor(int x, int y, int r, int g, int b)
    {
#pragma warning disable CS0675
        _bits[x + _width * y] = (int)(0xFF000000 | r << 16 | g << 8 | b);
#pragma warning restore CS0675
    }

    public void SetColor(int addr, int r, int g, int b)
    {
#pragma warning disable CS0675
        _bits[addr] = (int)(0xFF000000 | r << 16 | g << 8 | b);
#pragma warning restore CS0675
    }

    public Image GetImage()
    {
        return _bitmapImpl;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _bitsBufferHandle.Free();

        _isDisposed = true;
    }

    private static Bitmap CreateBitmapFromBits(int width, int height, GCHandle bitsBufferHandle)
    {
        return new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb,
            bitsBufferHandle.AddrOfPinnedObject());
    }
}