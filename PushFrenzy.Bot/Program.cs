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
            for (int i = 0; i < 500; i++)
            {
                Thread.Sleep(2000);
                Connect();
            }
            using (ManualResetEvent mre = new ManualResetEvent(false))
                mre.WaitOne();

        }

        static async Task Connect()
        {
            var directions = new[] { "Up", "Down", "Left", "Right" };            

            var connection = new HubConnection("http://localhost/PushFrenzy.Web/");

            var gameHub = connection.CreateProxy("PushFrenzy.Server.GameHub");
            dynamic dynamicHub = gameHub;

            ManualResetEvent mre = new ManualResetEvent(false);
            
            gameHub.On("startGame", () => mre.Set());

            await connection.Start();
            Console.WriteLine("Connected");

            await (Task) dynamicHub.JoinGame("Bot_" + random.Next().ToString(), 4);
            Console.WriteLine("Joined game");

            mre.WaitOne();
            Console.WriteLine("Game started");

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
