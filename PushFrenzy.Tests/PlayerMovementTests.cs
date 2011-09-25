using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushFrenzy.Rules;
using PushFrenzy.Rules.GameCommands;

namespace PushFrenzy.Tests
{
    [TestClass]
    public class PlayerMovementTests
    {
        private Game game;
        private Player player1;
        private Player player2;

        [TestInitialize]
        public void SetUp()
        {
            player1 = new Player("Player 1");
            player2 = new Player("Player 2");
            game = Game.CreateNewGame(10, 10, player1, player2);
        }

        [TestMethod]
        public void MoveRightIncrementsX()
        {
            player1.JumpTo(0, 0);
            player1.Move(Direction.Right);
            Assert.AreEqual(game.Board.SlotAt(1, 0), player1.Position);
        }

        [TestMethod]
        public void MoveLeftDecrementsX()
        {
            player1.JumpTo(1, 0);
            player1.Move(Direction.Left);
            Assert.AreEqual(game.Board.SlotAt(0, 0), player1.Position);
        }

        [TestMethod]
        public void MoveDownIncrementsY()
        {
            player1.JumpTo(0, 0);
            player1.Move(Direction.Down);
            Assert.AreEqual(game.Board.SlotAt(0, 1), player1.Position);
        }

        [TestMethod]
        public void MoveUpDecrementsY()
        {
            player1.JumpTo(0, 1);
            player1.Move(Direction.Up);
            Assert.AreEqual(game.Board.SlotAt(0, 0), player1.Position);
        }

        [TestMethod]
        public void MovingUpFromTopRowDoesNothing()
        {
            player1.JumpTo(2, 0);
            player1.Move(Direction.Up);
            Assert.AreEqual(game.Board.SlotAt(2, 0), player1.Position);
        }

        [TestMethod]
        public void MovingDownFromBottomRowDoesNothing()
        {
            player1.JumpTo(2, 9);
            player1.Move(Direction.Down);
            Assert.AreEqual(game.Board.SlotAt(2, 9), player1.Position);
        }

        [TestMethod]
        public void MovingLeftFromLeftColumnDoesNothing()
        {
            player1.JumpTo(0, 2);
            player1.Move(Direction.Left);
            Assert.AreEqual(game.Board.SlotAt(0, 2), player1.Position);
        }

        [TestMethod]
        public void MovingRightFromRightColumnDoesNothing()
        {
            player1.JumpTo(9, 2);
            player1.Move(Direction.Right);
            Assert.AreEqual(game.Board.SlotAt(9, 2), player1.Position);
        }

        [TestMethod]
        public void PlayerOccupiesSlot()
        {
            player1.JumpTo(0, 0);
            var slot = game.Board.SlotAt(0, 0);
            Assert.IsFalse(slot.Empty);
            Assert.AreEqual(player1, slot.Player);
        }

        [TestMethod]
        public void PlayerLeavesSlotWhenMoving()
        {
            player1.JumpTo(0, 0);
            player1.JumpTo(0, 1);
            Assert.IsNull(game.Board.SlotAt(0, 0).Player);            
        }

        [TestMethod]
        public void PlayerIsStillInSlotAfterIllegalMove()
        {
            player1.JumpTo(9, 2);
            player1.Move(Direction.Right);
            Assert.AreEqual(player1, game.Board.SlotAt(9, 2).Player);
        }

    }
}
