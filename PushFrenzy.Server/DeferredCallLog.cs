using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushFrenzy.Rules;
using SignalR.Hubs;

namespace PushFrenzy.Server
{
    public class DeferredCallLog : IMessageLog
    {
        private List<Action<dynamic>> calls = new List<Action<dynamic>>();

        private void LogCall(Action<dynamic> action)
        {
            calls.Add(action);
        }

        public void ExecuteCalls(IClientAgent agent, string gameId)
        {
            var clients = ((dynamic)agent)[gameId];
            foreach (var call in calls)
                call(clients);
        }

        public void MovePlayer(Player player, Slot slot)
        {
            LogCall(group => group.movePlayer(player.Name, slot.ToXY()));
        }

        public void StartGame(Game game)    
        {
            var dimensions = new
            {
                width = game.Board.Width,
                height = game.Board.Height
            };

            LogCall(group => group.startGame(dimensions));
        }

        public void AddPlayer(Player player)
        {
            var color = string.Format("rgb({0},{1},{2})", (int)player.Color.R, (int)player.Color.G, (int)player.Color.B);
            
            LogCall(group => group.addPlayer(player.Name, color, player.Position.ToXY()));

        }

        public void AddPiece(Slot slot)
        {
            LogCall(group => group.addPiece(slot.Piece.Owner.Name, slot.ToXY()));
        }

        public void MovePiece(Slot origin, Slot destination)
        {
            LogCall(group => group.movePiece(destination.Piece.Owner.Name, origin.ToXY(), destination.ToXY()));
        }

        public void UpdateSweep(Sweep sweep)
        {
            var bounds = sweep.Bounds;
            LogCall(group => group.updateSweep(bounds.Item1.ToXY(), bounds.Item2.ToXY()));
        }

        public void RemovePiece(Player player, Slot slot)
        {            
            LogCall(group => group.removePiece(player.Name, slot.ToXY()));
        }

        public void UpdateScores(Game game)
        {
            var scores = game.Players.Select(p => new
            {
                name = p.Name,
                value = p.Score
            }).ToArray();

            LogCall(group => group.updateScores(scores));
        }

        public void RemovePlayer(Player player)
        {
            LogCall(group => group.removePlayer(player.Name));
        }
    }

    public static class AnonymousExtensionMethods
    {
        public static dynamic ToXY(this Slot slot)
        {
            return new
            {
                x = slot.X,
                y = slot.Y
            };
        }
    }
}
