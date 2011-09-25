using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public abstract class GameCommand
    {
        public abstract void Execute(Game game, IMessageLog log);        
    } 
}
