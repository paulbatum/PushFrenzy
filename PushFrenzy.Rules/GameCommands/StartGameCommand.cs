using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules.GameCommands
{
    public class StartGameCommand : GameCommand
    {
        public override void Execute(Game game, IMessageLog log)
        {
            log.StartGame(game);
 
            for (int i = 0; i < game.Players.Count; i++)
            {
                var player = game.Players[i];                
                player.JumpTo(game.Board.StartingPositions[i]);
                log.AddPlayer(player);
            }
        }
    }
}
