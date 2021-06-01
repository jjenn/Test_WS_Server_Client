using System;

namespace Mutex.MVC.Multiplayer
{
    public class WsServerExecutor
    {
        private static readonly string uri = "http://+:80/wsListener/";

        public static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer();
            server.Start(uri);
            Console.WriteLine("Server Started!");
            Console.ReadKey();
        }
    }
}