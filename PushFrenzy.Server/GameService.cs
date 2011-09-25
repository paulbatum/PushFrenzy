using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushFrenzy.Rules;
using Microsoft.ServiceModel.WebSockets;

namespace PushFrenzy.Server
{
    public class GameService : WebSocketService
    {
        private static GameServer server = new GameServer();

        private GameConnection connection;

        public override void OnOpen()
        {
            string nickname = QueryParameters["nickname"];
            int gameSize = int.Parse(QueryParameters["gamesize"]);
            connection = server.JoinGame(this, nickname, gameSize);            
        }

        public override void OnMessage(string message)
        {
            connection.ProcessCommand(message);
        }

        protected override void OnClose()
        {
            if(connection != null)
                connection.Disconnect();
        }       
    }
}
