using System;

namespace SpaceshipGame
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // execute GeonBit with Game1 instance.
            GeonBit.GeonBitMain.Instance.Run(new SpaceshipGame());
        }
    }
}
