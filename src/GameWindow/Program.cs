using System;
using GameWindow.GameScenes;

namespace GameWindow
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
            using (var game = new SceneManager(1200, 800))
                game.Run();
        }
    }
#endif
}
