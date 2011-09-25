using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public class Player
    {
        public Game Game { get; internal set; }
        public string Name { get; internal set; }        
        public Slot Position { get; private set; }
        public int Score { get; set; }
        public Color Color { get; set; }

        public Player(string name)
        {
            Name = name;
        }

        private void MoveTo(Slot destination, Direction direction, IMessageLog log)
        {
            if (Position == destination)
                return;

            if (destination.Empty)
            {
                UpdateStateForValidMove(destination, log);
                return;
            }
            else
            {
                // Is the move blocked by another player?
                if (destination.Player != null)
                    return;

                // Is the move blocked by a piece we don't control?
                if (destination.Piece.Owner != this)
                    return;

                var pushSlot = Game.Board.GetSlotForDirection(destination, direction);
                
                // Is the push blocked by something behind it?
                if (pushSlot.Empty == false)
                    return;

                Game.Board.MovePieceToSlot(destination, pushSlot, log);
                UpdateStateForValidMove(destination, log);
            }

            
        }

        private void UpdateStateForValidMove(Slot destination, IMessageLog log = null)
        {
            if (Position != null)
                Position.Player = null;
            
            Position = destination;                
            destination.Player = this;

            if(log != null)
                log.MovePlayer(this, destination);
        }

        public void JumpTo(Slot destination)
        {
            if (destination.Empty == false)
                throw new ArgumentException(string.Format("Cannot jump to the location {0},{1} - it is not empty", destination.X, destination.Y));

            UpdateStateForValidMove(destination);
        }

        public void JumpTo(int x, int y)
        {
            var slot = Game.Board.SlotAt(x, y);
            JumpTo(slot);
        }

        public void Move(Direction direction, IMessageLog log = null)
        {
            var slot = Game.Board.GetSlotForDirection(this.Position, direction);
            MoveTo(slot, direction, log);
        }

        
    }

    public enum Direction
    {
        Up, Down, Left, Right
    }
}
