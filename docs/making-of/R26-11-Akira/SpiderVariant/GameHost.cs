using System;
using System.Collections.Generic;

namespace Cheron.Engine
{
    public static class GameHost
    {
        public static GameHost CreateSpiderGame()
        {
            return new GameHost(new SpiderGame());
        }

        // Other methods for creating different games
    }
}