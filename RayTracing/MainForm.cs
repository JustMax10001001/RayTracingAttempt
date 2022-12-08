using Timer = System.Threading.Timer;

namespace RayTracing;

public sealed partial class MainForm : Form
{
    private readonly Renderer _renderer;

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
    }

    public void InvokeRender()
    {
        if (Disposing || IsDisposed)
        {
            return;
        }
        
        Invoke(() =>
        {
            if (Disposing || IsDisposed)
            {
                return;
            }

            Invalidate();
        });
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.DrawImageUnscaled(_renderer.NextFrame, 0, 0);
    }
}