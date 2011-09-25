using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public interface IMessageLog
    {
        void MovePlayer(Player player, Slot slot);
        void StartGame(Game game);
        void AddPlayer(Player player);
        void AddPiece(Slot slot);
        void MovePiece(Slot origin, Slot destination);
        void UpdateSweep(Sweep sweep);
        void RemovePiece(Slot slot);
        void UpdateScores(Game game);

        void RemovePlayer(Player player);
    }
}
