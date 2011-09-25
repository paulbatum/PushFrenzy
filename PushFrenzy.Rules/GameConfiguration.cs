using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public class GameConfiguration
    {
        public int NumberOfPlayers { get; private set; }
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }

        private static List<GameConfiguration> configurations = new List<GameConfiguration>
        {
            new GameConfiguration(1, 6, 6),
            new GameConfiguration(2, 8, 8),
            new GameConfiguration(4, 12, 12),
            new GameConfiguration(8, 20, 20)
        };

        private GameConfiguration(int numberOfPlayers, int width, int height)
        {
            NumberOfPlayers = numberOfPlayers;
            BoardWidth = width;
            BoardHeight = height;
        }     

        public static GameConfiguration FromNumberOfPlayers(int playerCount)
        {
            var config = configurations.SingleOrDefault(gc => gc.NumberOfPlayers == playerCount);

            if (config == null)
                throw new ArgumentException(string.Format("No configuration for {0} players found.", playerCount), "playerCount");

            return config;
        }
        
    }
}
