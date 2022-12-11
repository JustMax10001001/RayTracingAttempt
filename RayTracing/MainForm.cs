using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Devices;
using Timer = System.Threading.Timer;

namespace RayTracing;

public sealed partial class MainForm : Form
{
    private readonly Renderer _renderer;
    private readonly Timer _timer;

    public MainForm()
    {
        InitializeComponent();

        _renderer = new Renderer(this);

        DoubleBuffered = true;

        new Thread(_renderer.BeginRendering)
        {
            IsBackground = true,
            Name = "Render"
        }.Start();

        _timer = new Timer(CheckKeysPressed, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(20));
    }

    private void CheckKeysPressed(object? state)
    {
        if (ActiveForm != this)
        {
            return;
        }

        if (GetKeyState(Keys.Q) is KeyStates.Down)
        {
            _renderer.Camera.Yaw += 0.025f;
        }
        else if (GetKeyState(Keys.E) is KeyStates.Down)
        {
            _renderer.Camera.Yaw -= 0.025f;
        }
        else if (GetKeyState(Keys.W) is KeyStates.Down)
        {
            _renderer.Camera.Pitch += 0.01f;
        }
        else if (GetKeyState(Keys.S) is KeyStates.Down)
        {
            _renderer.Camera.Pitch -= 0.01f;
        }
    }

    public void InvokeRender()
    {
        if (Disposing || IsDisposed)
        {
            return;
        }

        try
        {
            Invoke(() =>
            {
                if (Disposing || IsDisposed)
                {
                    return;
                }

                Invalidate();
            });
        }
        catch (ObjectDisposedException)
        {
        }
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.DrawImageUnscaled(_renderer.NextFrame, 0, 0);
    }

    [Flags]
    private enum KeyStates
    {
        None = 0,
        Down = 1,
        Toggled = 2
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern short GetAsyncKeyState(int keyCode);

    private static KeyStates GetKeyState(Keys key)
    {
        var state = KeyStates.None;

        var retVal = GetAsyncKeyState((int)key);

        //If the high-order bit is 1, the key is down
        //otherwise, it is up.
        if ((retVal & 0x8000) == 0x8000)
            state |= KeyStates.Down;

        //If the low-order bit is 1, the key is toggled.
        if ((retVal & 1) == 1)
            state |= KeyStates.Toggled;

        return state;
    }
}