using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PushFrenzy.Rules;

namespace PushFrenzy.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]        
        public void PlayerNameUniquenessIsEnforced()
        {
            var paul = new Player("Paul");
            var anotherPaul = new Player("Paul");

            var game = Game.CreateNewGame(5, 5, paul, anotherPaul);

            Assert.AreEqual("Paul1", anotherPaul.Name);
        }
    }
}

