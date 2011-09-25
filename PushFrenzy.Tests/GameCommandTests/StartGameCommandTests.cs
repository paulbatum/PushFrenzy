using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushFrenzy.Rules;
using PushFrenzy.Rules.GameCommands;
using PushFrenzy.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PushFrenzy.Tests.GameCommandTests
{
    [TestClass]
    public class StartGameCommandTests
    {
        [TestMethod]
        public void PlayerStartingPositionsAreLogged()
        {
            var player1 = new Player("Player 1");
            var player2 = new Player("Player 2");
            var game = Game.CreateNewGame(10, 10, player1, player2);
            var log = new JsonMessageLog();

            var command = new StartGameCommand();
            command.Execute(game, log);

            log.AssertMessageTypes(GameMessageType.NewGame, GameMessageType.PlayerAdded, GameMessageType.PlayerAdded);
        }

        [TestMethod]
        public void TwoPlayersStartInOppositeCorners()
        {
            var player1 = new Player("Player 1");
            var player2 = new Player("Player 2");
            var game = Game.CreateNewGame(10, 10, player1, player2);
            var log = new JsonMessageLog();

            var command = new StartGameCommand();
            command.Execute(game, log);

            var pos1 = player1.Position;
            var pos2 = player2.Position;

            Assert.AreEqual(0, pos1.X);
            Assert.AreEqual(0, pos1.Y);

            Assert.AreEqual(9, pos2.X);
            Assert.AreEqual(9, pos2.Y);
        }


    }
}
