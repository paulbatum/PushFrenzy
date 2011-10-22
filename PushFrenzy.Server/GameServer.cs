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
using SignalR.Hubs;

namespace PushFrenzy.Server
{
    public class GameServer
    {
        private ConcurrentDictionary<GameConfiguration, HostedGame> games = new ConcurrentDictionary<GameConfiguration, HostedGame>();        

        public GameConnection JoinGame(string clientId, string nickname, int gameSize, IHub hub)
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
                    connection = game.TryAddPlayer(clientId, nickname, hub);
                }
            }

            return connection;
        }
    }

    public class HostedGame
    {
        private GameConfiguration config;
        private List<string> clients;
        private Game game;
        private object gameLock = new object();
        private object commandLock = new object();
        private Timer gameTimer;
        private IHub hub;

        public bool Started { get; private set; }
        public Guid GameId { get; private set; }

        public HostedGame(GameConfiguration config)
        {
            GameId = new Guid();
            this.config = config;
            clients = new List<string>();
            game = new Game(config.BoardWidth, config.BoardHeight);
        }
       
        public GameConnection TryAddPlayer(string clientId, string name, IHub hub)
        {
            lock (gameLock)
            {
                if (Started)
                    return null;

                clients.Add(clientId);
                var player = new Player(name);
                game.AddPlayer(player);
                
                hub.GroupManager.AddToGroup(clientId, this.GameId.ToString()).Wait();

                if (clients.Count == config.NumberOfPlayers)
                {
                    this.hub = hub;
                    StartGame();                    
                }

                return new GameConnection(clientId, player, this);
            }
        }

        public void ProcessCommand(GameCommand command)
        {
            lock (commandLock)
            {
                var log = new DeferredCallLog();
                command.Execute(game, log);
                log.ExecuteCalls(hub.Agent, this.GameId.ToString());
            }
        }

        public void Disconnect(GameConnection connection)
        {
            clients.Remove(connection.ClientId);

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
            //gameTimer = new Timer(o => ProcessCommand(new TimerTickCommand()), null, 0, Game.ExpectedTickIntervalMilliseconds);
        }        

    }

    public class GameConnection
    {
        public string ClientId { get; private set; }
        public Player Player { get; private set; }
        private HostedGame hostedGame;

        internal GameConnection(string clientId, Player player, HostedGame hostedGame)
        {
            this.ClientId = clientId;
            this.Player = player;
            this.hostedGame = hostedGame;            
        }     

        public void Disconnect()
        {
            hostedGame.Disconnect(this);
        }

        public void Move(string direction)
        {
            var command = new PlayerMoveCommand
                    {
                        Direction = (Direction)Enum.Parse(typeof(Direction), direction),
                        Player = this.Player
                    };
            hostedGame.ProcessCommand(command);
        }

        public string GameId
        {
            get { return this.hostedGame.GameId.ToString(); }
        }
    }

}
