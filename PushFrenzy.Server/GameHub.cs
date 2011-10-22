using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalR.Hubs;

namespace PushFrenzy.Server
{
    [HubName("game")]
    public class GameHub : Hub, IDisconnect
    {
        private static GameServer server = new GameServer();
        private static ConcurrentDictionary<string, GameConnection> connections = new ConcurrentDictionary<string, GameConnection>();

        public void JoinGame(string nickname, int gameSize)
        {            
            var id = this.Context.ClientId;            
            GameConnection connection = server.JoinGame(id, nickname, gameSize, this);
            connections[id] = connection;            
        }

        public void Move(string direction)
        {
            var connection = connections[Context.ClientId];
            connection.Move(direction);
        }

        public void Disconnect()
        {
            GameConnection connection;
            connections.TryRemove(Context.ClientId, out connection);
            if (connection != null)
            {
                RemoveFromGroup(connection.GameId);
                connection.Disconnect();
            }
        }
    }
}
