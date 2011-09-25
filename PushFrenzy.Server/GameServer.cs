using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PushFrenzy.Rules;
using PushFrenzy.Rules.GameCommands;
using Microsoft.ServiceModel.WebSockets;
using Timer = System.Threading.Timer;

namespace PushFrenzy.Server
{
    public class GameServer
    {
        private ConcurrentDictionary<GameConfiguration, HostedGame> games = new ConcurrentDictionary<GameConfiguration, HostedGame>();        

        public GameConnection JoinGame(GameService gameService, string nickname, int gameSize)
        {
            GameConfiguration config = GameConfiguration.FromNumberOfPlayers(gameSize);
            GameConnection connection = null;

            while (connection == null)
            {
                HostedGame game = games.GetOrAdd(config, c => new HostedGame(c));
                
                if (game.Started)
                {
                    games.TryUpdate(config, new HostedGame(config), game);
                }
                else
                {
                    connection = game.TryAddPlayer(gameService, nickname);
                }
            }

            return connection;
        }
    }

    public class HostedGame
    {
        private GameConfiguration config;
        private WebSocketCollection<GameService> clients;
        private Game game;
        private object gameLock = new object();
        private object commandLock = new object();
        private object sendLock = new object();
        private Timer gameTimer; 

        public bool Started { get; private set; }

        public HostedGame(GameConfiguration config)
        {
            this.config = config;
            clients = new WebSocketCollection<GameService>();
            game = new Game(config.BoardWidth, config.BoardHeight);
        }
       
        public GameConnection TryAddPlayer(GameService client, string name)
        {
            lock (gameLock)
            {
                if (Started)
                    return null;

                clients.Add(client);
                var player = new Player(name);
                game.AddPlayer(player);

                if (clients.Count == config.NumberOfPlayers)
                    StartGame();

                return new GameConnection(client, player, this);
            }
        }

        public void ProcessCommand(GameCommand command)
        {
            lock (commandLock)
            {
                var log = new JsonMessageLog();
                command.Execute(game, log);
                DispatchMessages(log);
            }
        }

        public void Disconnect(GameConnection connection)
        {
            clients.Remove(connection.Client);

            if (clients.Count == 0 && gameTimer != null)
                gameTimer.Dispose();

            if (Started)
                ProcessCommand(new RemovePlayerCommand(connection.Player));
            else
                game.Players.Remove(connection.Player);
        }

        private void StartGame()
        {
            ProcessCommand(new StartGameCommand());
            Started = true;
            gameTimer = new Timer(o => ProcessCommand(new TimerTickCommand()), null, 0, Game.ExpectedTickIntervalMilliseconds);
        }        

        private void DispatchMessages(JsonMessageLog log)
        {
            if (log.Messages.Count > 0)
            {
                var array = new JsonArray(log.Messages);
                clients.Broadcast(array.ToString());
            }
        }
    }

    public class GameConnection
    {
        public GameService Client { get; private set; }
        public Player Player { get; private set; }
        private HostedGame hostedGame;

        internal GameConnection(GameService client, Player player, HostedGame hostedGame)
        {
            this.Client = client;
            this.Player = player;
            this.hostedGame = hostedGame;            
        }

        public void ProcessCommand(string message)
        {
            var command = ParseCommand(Player, message);
            hostedGame.ProcessCommand(command);
        }

        public void Disconnect()
        {
            hostedGame.Disconnect(this);
        }

        private GameCommand ParseCommand(Player origin, string value)
        {
            dynamic jsonValue = JsonValue.Parse(value);
            string commandType = (string)jsonValue.Type;
            switch (commandType)
            {
                case "PlayerMoveCommand":
                    return new PlayerMoveCommand
                    {
                        Direction = (Direction)Enum.Parse(typeof(Direction), (string)jsonValue.Direction),
                        Player = origin
                    };
                default:
                    throw new ArgumentException("Unknown command '{0}'", commandType);
            }
        }
    }

}
