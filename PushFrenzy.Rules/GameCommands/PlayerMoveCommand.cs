using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules.GameCommands
{
    public class PlayerMoveCommand : GameCommand
    {        
        public Player Player { get; set; }
        public Direction Direction { get; set; }

        public override void Execute(Game game, IMessageLog log)
        {
            this.Player.Move(Direction, log);
        }
    }


    
}
