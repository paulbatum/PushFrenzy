using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushFrenzy.Rules
{
    public class Board
    {
        private Random random;
        private Slot[,] slotGrid;
        private List<Slot> slots;

        public List<Slot> StartingPositions { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            random = new Random();

            slotGrid = new Slot[width, height];
            slots = new List<Slot>(width * height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var slot = new Slot(i, j);
                    slotGrid[i, j] = slot;
                    slots.Add(slot);
                }
            }

            StartingPositions = new List<Slot>()
            {
                slotGrid[0,0], slotGrid[width-1, height -1], slotGrid[0, height-1], slotGrid[width-1, 0], 
                slotGrid[0, height / 2], slotGrid[width / 2, 0], slotGrid[width-1, height / 2], slotGrid[width / 2, height-1]
            };
        }

        public Slot SlotAt(int x, int y)
        {
            if (x >= Width || y >= Height)
                return null;

            return slotGrid[x, y];
        }

        public IEnumerable<Slot> Slots
        {
            get { return slots; }
        }

        public IEnumerable<Slot> Row(int y)
        {
            for (int x = 0; x < Width; x++)
                yield return slotGrid[x, y];
        }

        public IEnumerable<Slot> Column(int x)
        {
            for (int y = 0; y < Height; y++)
                yield return slotGrid[x, y];
        }      

        public Slot GetRandomEmptySlot()
        {
            var emptySlots = slots.Where(s => s.Empty).ToList();
            if (emptySlots.Count == 0)
                return null;

            return emptySlots[random.Next(0, emptySlots.Count)];
        }

        public Slot AddNewPieceAtRandomEmptySlot(Player owner)
        {
            var slot = GetRandomEmptySlot();
            if (slot != null)
                AddNewPieceToSlot(slot, owner);
            return slot;
        }

        public void AddNewPieceToSlot(Slot slot, Player owner)
        {
            var newPiece = new Piece { Owner = owner };
            slot.Piece = newPiece;
        }

        public void MovePieceToSlot(Slot origin, Slot destination, IMessageLog log)
        {
            if (origin.Piece == null)
                throw new InvalidOperationException("Cannot move piece because the origin slot is empty");

            destination.Piece = origin.Piece;
            origin.Piece = null;

            if(log != null)
                log.MovePiece(origin, destination);
        }

        public Slot GetSlotForDirection(Slot origin, Direction direction)
        {
            int xDelta = 0;
            int yDelta = 0;

            switch (direction)
            {
                case Direction.Up:
                    yDelta = -1;
                    break;
                case Direction.Down:
                    yDelta = +1;
                    break;
                case Direction.Left:
                    xDelta = -1;
                    break;
                case Direction.Right:
                    xDelta = +1;
                    break;
            }

            int newX = Math.Min(Width - 1, Math.Max(0, origin.X + xDelta));
            int newY = Math.Min(Height - 1, Math.Max(0, origin.Y + yDelta));

            return SlotAt(newX, newY);
        }

        public void RemovePlayerAndPieces(Player player)
        {
            foreach (var slot in Slots)
            {
                if (slot.Piece != null && slot.Piece.Owner == player)
                {
                    slot.Piece = null;
                }

                if (slot.Player == player)
                {
                    slot.Player = null;
                }
            }
        }
    }

    public class Slot
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        private Piece piece;
        private Player player;

        public Piece Piece
        {
            get
            {
                return piece;
            }
            set
            {
                if (Empty == false && value != null)
                    throw new InvalidOperationException(string.Format("Slot at {0},{1} is not empty", X, Y));

                piece = value;
            }
        }

        public Player Player
        {
            get
            {
                return player;
            }
            set
            {
                if (Empty == false && value != null)
                    throw new InvalidOperationException(string.Format("Slot at {0},{1} is not empty", X, Y));

                player = value;
            }
        }

        public Slot(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Empty
        {
            get { return this.Piece == null && this.Player == null; }
        }

    }



    public class Piece
    {
        public Player Owner { get; set; }
    }
}

