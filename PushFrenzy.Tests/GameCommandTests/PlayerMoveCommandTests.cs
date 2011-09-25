using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushFrenzy.Rules;
using PushFrenzy.Rules.GameCommands;
using PushFrenzy.Server;

namespace PushFrenzy.Tests.GameCommandTests
{
    [TestClass]
    public class PlayerMoveCommandTests
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
        public void MoveCommandTriggersPlayerMove()
        {
            player1.JumpTo(1, 1);
            var command = new PlayerMoveCommand { Player = player1, Direction = Direction.Up };
            command.Execute(game, log);

            Assert.AreEqual(game.Board.SlotAt(1, 0), player1.Position);            
        }

        [TestMethod]
        public void MoveCommandWritesToLog()
        {
            player1.JumpTo(1, 1);
            var command = new PlayerMoveCommand { Player = player1, Direction = Direction.Up };
            command.Execute(game, log);

            Assert.AreEqual(1, log.Messages.Count);
        }

        [TestMethod]
        public void NoLogWrittenForNullMove()
        {
            player1.JumpTo(0, 1);
            var command = new PlayerMoveCommand { Player = player1, Direction = Direction.Left };
            command.Execute(game, log);

            Assert.AreEqual(0, log.Messages.Count);
        }
        
    }
}
