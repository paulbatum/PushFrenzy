using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushFrenzy.Rules;
using PushFrenzy.Server;

namespace PushFrenzy.Tests
{
    [TestClass]
    public class PiecePushingTests
    {
        private Game game;
        private Player player1;
        private Player player2;
        private JsonMessageLog log;

        [TestInitialize]
        public void SetUp()
        {
            player1 = new Player("Player 1");
            player2 = new Player("Player 2");
            game = Game.CreateNewGame(10, 10, player1, player2);
            log = new JsonMessageLog();
        }

        [TestMethod]
        public void PlayerMovingIntoSlotWithOwnedPiecePushesPiece()
        {
            player1.JumpTo(0, 0);
            var slot1 = game.Board.SlotAt(1, 0);
            var slot2 = game.Board.SlotAt(2, 0);
            
            game.Board.AddNewPieceToSlot(slot1, player1);
            var piece = slot1.Piece;
            player1.Move(Direction.Right, log);
            
            Assert.AreEqual(slot1, player1.Position);
            Assert.AreEqual(piece, slot2.Piece);

            log.AssertMessageTypes(GameMessageType.PieceMoved, GameMessageType.PlayerMoved);
        }

        [TestMethod]
        public void PlayerCannotMovingIntoSlotWithPieceHeDoesNotOwn()
        {
            var slot0 = game.Board.SlotAt(0, 0);
            player1.JumpTo(slot0);
            
            var slot1 = game.Board.SlotAt(1, 0);
            var slot2 = game.Board.SlotAt(2, 0);

            game.Board.AddNewPieceToSlot(slot1, player2);
            var piece = slot1.Piece;
            player1.Move(Direction.Right, log);

            Assert.AreEqual(slot0, player1.Position);
            Assert.AreEqual(piece, slot1.Piece);
            Assert.IsTrue(slot2.Empty);

            Assert.AreEqual(0, log.Messages.Count);
        }

        [TestMethod]
        public void PlayerCannotPushTwoPieces()
        {
            var slot0 = game.Board.SlotAt(0, 0);
            player1.JumpTo(slot0);

            var slot1 = game.Board.SlotAt(1, 0);
            var slot2 = game.Board.SlotAt(2, 0);

            game.Board.AddNewPieceToSlot(slot1, player1);
            game.Board.AddNewPieceToSlot(slot2, player1);
            player1.Move(Direction.Right, log);

            Assert.AreEqual(slot0, player1.Position);
            Assert.IsFalse(slot1.Empty);
            Assert.IsFalse(slot2.Empty);

            Assert.AreEqual(0, log.Messages.Count);
        }

        [TestMethod]
        public void PlayerCannotPushPieceOffTheBoard()
        {
            var slot0 = game.Board.SlotAt(0, 0);            
            var slot1 = game.Board.SlotAt(1, 0);

            game.Board.AddNewPieceToSlot(slot0, player1);
            var piece = slot0.Piece;
            player1.JumpTo(slot1);                       
            player1.Move(Direction.Left, log);

            Assert.IsFalse(slot0.Empty);
            Assert.AreEqual(piece, slot0.Piece);

            Assert.AreEqual(0, log.Messages.Count);
        }
    }
}
