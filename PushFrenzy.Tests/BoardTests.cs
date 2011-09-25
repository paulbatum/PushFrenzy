using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushFrenzy.Rules;

namespace PushFrenzy.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void OneSlotForOneByOneBoard()
        {
            var board = new Board(1, 1);
            Assert.AreEqual(1, board.Slots.Count());
        }

        [TestMethod]
        public void ThirtySlotsForFiveBySixBoard()
        {
            var board = new Board(5, 6);
            Assert.AreEqual(30, board.Slots.Count());
        }

        [TestMethod]
        public void FullBoardReturnsNullForRandomEmptySlot()
        {
            var board = new Board(3, 3);
            foreach (var slot in board.Slots)
                slot.Piece = new Piece();

            Assert.IsNull(board.GetRandomEmptySlot());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotOverwritePieceInSlot()
        {
            var board = new Board(3, 3);
            var slot = board.SlotAt(0, 0);
            slot.Piece = new Piece();
            slot.Piece = new Piece();
        }
    }
}
