using System;

namespace Mutex.MVC.Multiplayer
{
    public class WsClientExecutor
    {
        private static readonly string uri = "ws://192.168.1.113:8080/wsListener";

        public static void Main(string[] args)
        {
            WebSocketClient client = new WebSocketClient();
            client.Connect(uri).Wait();
            Console.WriteLine("Connected!");
            Console.ReadKey();
        }
    }
}
