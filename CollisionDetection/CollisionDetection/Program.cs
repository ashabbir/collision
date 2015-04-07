using System;

namespace CollisionDetection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CollisionDetection game = new CollisionDetection())
                game.Run();
        }
    }
}

