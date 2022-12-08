namespace RayTracing;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
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
            Transform =
            {
                Tz = 8
            }
        };

        var ray = new Ray
        {
            Kx = 0,
            Ky = 0,
            Kz = 1,
            Origin = new Vector3
            {
                Y = 0,
                Z = 16
            }
        };

        var isBounced = sphere.TryBounceRay(ray, out var newRay);
    }
}