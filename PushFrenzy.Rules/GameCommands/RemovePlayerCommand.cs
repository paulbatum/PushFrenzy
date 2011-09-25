using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules.GameCommands
{
    public class RemovePlayerCommand : GameCommand
    {
        private Player player;

        public RemovePlayerCommand(Player player)
        {
            this.player = player;
        }

        public override void Execute(Game game, IMessageLog log)
        {
            game.RemovePlayer(player, log);
        }
    }
}
