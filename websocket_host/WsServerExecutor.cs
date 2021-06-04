using System;

namespace Mutex.MVC.Multiplayer
{
    public class WsServerExecutor
    {
        private static readonly string uri = "http://192.168.1.113:8080/wsListener/";

        public static void Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer();
            server.Start(uri);
            Console.ReadKey();
        }
    }
}