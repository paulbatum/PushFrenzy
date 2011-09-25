using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules.GameCommands
{
    public class TimerTickCommand : GameCommand
    {
        public override void Execute(Game game, IMessageLog log)
        {
            game.Tick(log);
        }
    }
}
