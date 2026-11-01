using System;

namespace Cheron
{
    class Program
    {
        static void Main(string[] args)
        {
            using var gameHost = new GameHost(new SpiderGame());
            gameHost.Run();
        }
    }
}