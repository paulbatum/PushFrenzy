using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public class Game
    {
        public static readonly int ExpectedTickIntervalMilliseconds = 100;


        public IList<Player> Players { get; private set; }
        public Board Board { get; private set; }

        private int ticks = 0;
        private Sweep sweep;

        private static readonly List<Color> PlayerColors = CreateColors();

        public Game(int x, int y)
        {
            Players = new List<Player>();
            Board = new Board(x, y);
            sweep = new Sweep(Board, new PiecesInARowClearingRule(this, 4));
        }

        private static List<Color> CreateColors()
        {
            return new List<Color>
            {
                Color.Red,
                Color.Blue,
                Color.Green,
                Color.Purple,
                Color.Orange,
                Color.Brown,
                Color.Gray,
                Color.Aqua,
            };
        }

        public static Game CreateNewGame(int x, int y, params Player[] players)
        {
            var game = new Game(x, y);

            foreach (var player in players)
                game.AddPlayer(player);

            return game;
        }        

        public void AddPlayer(Player player)
        {
            if (Players.Count >= Math.Min(Board.StartingPositions.Count, PlayerColors.Count))
                throw new InvalidOperationException("Too many players.");

            if (player.Game != null)
                throw new InvalidOperationException(string.Format("Player '{0}' is already associated with a game", player.Name));

            while (Players.Any(p => p.Name == player.Name))
                player.Name = player.Name + "1";
                
            player.Game = this;
            player.Color = PlayerColors[Players.Count];
            Players.Add(player);            
        }

        public void RemovePlayer(Player player, IMessageLog log)
        {
            Board.RemovePlayerAndPieces(player);
            log.RemovePlayer(player);
            Players.Remove(player);            
        }


        internal void Tick(IMessageLog log)
        {
            ticks++;

            sweep.Tick(ticks, log);

            if (ticks % 30 == 0)
            {
                foreach (var player in Players)
                {
                    var slot = Board.AddNewPieceAtRandomEmptySlot(player);

                    if (slot != null)
                        log.AddPiece(slot);
                }
            }


        }


        
    }

    public class Sweep
    {
        private Board board;
        private int counter;
        private int updateInterval;

        private IClearingRule clearingRule;

        public Sweep(Board board, IClearingRule clearingRule)
        {
            this.board = board;
            this.counter = 0;
            this.clearingRule = clearingRule;

            // Scale the update interval based on the size of the board
            updateInterval = Math.Max(1, (int)(10 / Math.Round(((decimal)(board.Width + board.Height)) / 10m)));
        }

        public void Tick(int ticks, IMessageLog log)
        {
            if (ticks % updateInterval == 0)
            {
                counter++;
                if (counter >= board.Width + board.Height)
                    counter = 0;

                clearingRule.Apply(this, log);
                log.UpdateSweep(this);

            }


        }

        public Tuple<Slot, Slot> Bounds
        {
            get
            {
                if (counter < board.Width)
                {
                    return Tuple.Create(board.SlotAt(counter, 0), board.SlotAt(counter, board.Height - 1));
                }
                else
                {
                    var offset = counter - board.Width;
                    return Tuple.Create(board.SlotAt(0, offset), board.SlotAt(board.Width - 1, offset));
                }
            }
        }
    }

    public interface IClearingRule
    {
        void Apply(Sweep sweep, IMessageLog log);
    }

    public class PiecesInARowClearingRule : IClearingRule
    {
        private Game game;
        public int Target { get; private set; }

        public PiecesInARowClearingRule(Game game, int target)
        {
            this.game = game;
            Target = target;
        }

        public void Apply(Sweep sweep, IMessageLog log)
        {
            List<Slot> slotsToClear = new List<Slot>();

            Tuple<Slot, Slot> bounds = sweep.Bounds;

            IEnumerable<Slot> searchSet = null; 
            if (bounds.Item1.X == bounds.Item2.X)
                searchSet = game.Board.Column(bounds.Item1.X);
            else
                searchSet = game.Board.Row(bounds.Item1.Y);

            List<Slot> currentRun = new List<Slot>();

            foreach (var slot in searchSet)
            {
                if (slot.Piece == null)
                {
                    if (currentRun.Count >= Target)
                        slotsToClear.AddRange(currentRun);

                    currentRun.Clear();
                }
                else
                {
                    if (currentRun.Count > 0 && currentRun.First().Piece.Owner != slot.Piece.Owner)
                    {
                        if (currentRun.Count >= Target)
                            slotsToClear.AddRange(currentRun);

                        currentRun.Clear();
                    }
                   
                    currentRun.Add(slot);
                }
            }

            if (currentRun.Count >= Target)
                slotsToClear.AddRange(currentRun);

            foreach (var slot in slotsToClear)
            {
                log.RemovePiece(slot);
                slot.Piece.Owner.Score++;
                slot.Piece = null;                
            }

            if(slotsToClear.Count > 0)
                log.UpdateScores(game);
                
        }
    }
}
