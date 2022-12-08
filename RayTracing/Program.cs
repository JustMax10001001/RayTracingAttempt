using RayTracing.Objects;

namespace RayTracing;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var ray = new Vector3(0, 0.7f, 0.7f);
        var aBitUpTransform = Matrix3.CreateRotation(0.2f, 0, 0f);
        var newRay = ray * aBitUpTransform;
        
        //TestRayBounce();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.Run(new MainForm());
    }

    private static void TestRayBounce()
    {
        var sphere = new SphereObject
        {
            Transform = new Matrix3
            {
                Tz = 8
            }
        };

        var ray = new Ray
        {
            Direction = new Vector3
            {
                X = 0,
                Y = 0,
                Z = 1,
            },
            Origin = new Vector3
            {
                Y = 0,
                Z = 16
            }
        };

        var isBounced = sphere.TryBounceRay(ray, out var newRay);
    }
}