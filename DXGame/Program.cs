using ShooterEngine;
using System;

namespace DXGame
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new ShooterEngineGame())
                game.Run();
        }
    }
#endif
}
