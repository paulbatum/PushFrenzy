using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushFrenzy.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PushFrenzy.Tests
{
    public static class JsonAsserts
    {
        public static void AssertMessageTypes(this JsonMessageLog log, params GameMessageType[] messageTypes)
        {            
            Assert.AreEqual(messageTypes.Length, log.Messages.Count);

            for (int i = 0; i < log.Messages.Count; i++)
                Assert.AreEqual(messageTypes[i].ToString(), ((dynamic)log.Messages[i]).type.Value);
        }

        public static void AssertAllOfType(this JsonMessageLog log, GameMessageType messageType)
        {
            foreach (dynamic msg in log.Messages)
            {
                Assert.IsTrue(msg.type == messageType.ToString());
            }
        }
    }
}
