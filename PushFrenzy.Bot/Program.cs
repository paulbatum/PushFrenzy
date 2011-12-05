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
        private static Random random;

        static void Main(string[] args)
        {
            random = new Random();
            Connect();
            using (ManualResetEvent mre = new ManualResetEvent(false))
                mre.WaitOne();

        }

        static async void Connect()
        {
            for (int i = 0; i < 63; i++)
            {                
                await ConnectClient();
            }
        }

        static async Task ConnectClient()
        {

            var connection = new HubConnection("http://localhost/PushFrenzy.Web/");

            var gameHub = connection.CreateProxy("PushFrenzy.Server.GameHub");
            dynamic dynamicHub = gameHub;

            gameHub.On("startGame", () => MoveLoop(connection, dynamicHub));

            await connection.Start();
            Console.WriteLine("Connected");

            await (Task) dynamicHub.JoinGame("Bot_" + random.Next().ToString(), 8);
            Console.WriteLine("Joined game");
        }

        private static async void MoveLoop(HubConnection connection, dynamic dynamicHub)
        {
            Console.WriteLine("Game started");

            var directions = new[] { "Up", "Down", "Left", "Right" };            

            while (connection.IsActive)
            {
                string next = directions[random.Next(0, 4)];
                //Console.WriteLine("Moving " + next);
                dynamicHub.Move(next);
                await Task.Delay(250);
            }
        }
    }
}
