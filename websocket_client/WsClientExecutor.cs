using System;

namespace Mutex.MVC.Multiplayer
{
    public class WsClientExecutor
    {
        private static readonly string uri = "ws://localhost/wsListener";

        public static void Main(string[] args)
        {
            WebSocketClient client = new WebSocketClient();
            client.Connect(uri).Wait();
            Console.WriteLine("Connected!");
            Console.ReadKey();
        }
    }
}
