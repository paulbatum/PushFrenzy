using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushFrenzy.Rules;

namespace PushFrenzy.Server
{
    public class JsonMessageLog : IMessageLog
    {
        public List<JsonObject> Messages { get; private set; }

        public JsonMessageLog()
        {
            Messages = new List<JsonObject>();
        }

        private JsonObject BuildGameMessageJson(GameMessageType messageType, Action<dynamic> applyValues)
        {
            dynamic jsonObject = new JsonObject();
            jsonObject.type = messageType.ToString();
            jsonObject.body = new JsonObject();
            applyValues(jsonObject.body);
            return jsonObject;
        }

        public void MovePlayer(Player player, Slot slot)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PlayerMoved, msg =>
            {
                msg.player = player.Name;
                msg.x = slot.X;
                msg.y = slot.Y;
            });
            
            Messages.Add(obj);
        }

        public void StartGame(Game game)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.NewGame, msg =>
            {
                msg.dimensions = new JsonObject();
                msg.dimensions.width = game.Board.Width;
                msg.dimensions.height = game.Board.Height;
            });

            Messages.Add(obj);            
        }

        public void AddPlayer(Player player)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PlayerAdded, msg =>
            {                
                msg.name = player.Name;
                msg.color = string.Format("rgb({0},{1},{2})", (int)player.Color.R, (int)player.Color.G, (int)player.Color.B);
                msg.x = player.Position.X;
                msg.y = player.Position.Y;
            });

            Messages.Add(obj);              
        }


        public void AddPiece(Slot slot)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PieceAdded, msg =>
            {
                msg.owner = slot.Piece.Owner.Name;
                msg.x = slot.X;
                msg.y = slot.Y;
            });

            Messages.Add(obj); 
        }




        public void MovePiece(Slot origin, Slot destination)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PieceMoved, msg =>
            {
                msg.owner = destination.Piece.Owner.Name;
                msg.origin = new JsonObject();
                msg.origin.x = origin.X;
                msg.origin.y = origin.Y;
                msg.destination = new JsonObject();
                msg.destination.x = destination.X;
                msg.destination.y = destination.Y;
            });

            Messages.Add(obj);
        }


        public void UpdateSweep(Sweep sweep)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.SweepUpdated, msg =>
            {
                var bounds = sweep.Bounds;
                msg.start = new JsonObject();
                msg.start.x = bounds.Item1.X;
                msg.start.y = bounds.Item1.Y;
                msg.end = new JsonObject();
                msg.end.x = bounds.Item2.X;
                msg.end.y = bounds.Item2.Y;
            });

            Messages.Add(obj);
        }


        public void RemovePiece(Slot slot)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PieceRemoved, msg =>
            {
                msg.owner = slot.Piece.Owner.Name;
                msg.x = slot.X;
                msg.y = slot.Y;
            });

            Messages.Add(obj); 
        }


        public void UpdateScores(Game game)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.ScoresUpdated, msg =>
            {
                msg.scores = new JsonArray(
                    game.Players.Select(p =>
                    {
                        dynamic scoreEntry = new JsonObject();
                        scoreEntry.name = p.Name;
                        scoreEntry.value = p.Score;
                        return (JsonValue)scoreEntry;
                    })
                );               
            });

            Messages.Add(obj); 
        }


        public void RemovePlayer(Player player)
        {
            JsonObject obj = BuildGameMessageJson(GameMessageType.PlayerRemoved, msg =>
            {
                msg.name = player.Name;
            });

            Messages.Add(obj);   
        }
    }

    public enum GameMessageType
    {
        NewGame,
        PlayerAdded,
        PlayerRemoved,
        PlayerMoved,        
        PieceAdded,
        PieceMoved,
        PieceRemoved,
        SweepUpdated,
        ScoresUpdated
    }
}
