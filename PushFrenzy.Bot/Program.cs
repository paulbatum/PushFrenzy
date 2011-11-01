using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SignalR.Client.Hubs;

namespace PushFrenzy.Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Connect();
            using (ManualResetEvent mre = new ManualResetEvent(false))
                mre.WaitOne();

        }

        static async Task Connect()
        {
            var random = new Random();
            var directions = new[] { "Up", "Down", "Left", "Right" };            

            var connection = new HubConnection("http://localhost/PushFrenzy.Web/");
            await connection.Start();
            Console.WriteLine("Connected");

            var gameHub = connection.CreateProxy("PushFrenzy.Server.GameHub");
            dynamic dynamicHub = gameHub;
            
            var startGame = gameHub.Subscribe("startGame");
            
            startGame.Subscribe((object[] args) =>
            {
                Console.WriteLine("Game started");
                while (connection.IsActive)
                {
                    string next = directions[random.Next(0, 4)];
                    Console.WriteLine("Moving " + next);
                    dynamicHub.Move(next);
                    Thread.Sleep(500);
                }
            });

            await (Task) dynamicHub.JoinGame("Bot_" + random.Next().ToString(), 2);
            Console.WriteLine("Joined game");
        }
    }
}
