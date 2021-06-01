using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace Mutex.MVC.Multiplayer
{
    public class WebSocketServer
    {
        public async void Start(string listenerPrefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();

                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }
  
        private async void ProcessRequest(HttpListenerContext listenerContext)
        {
            WebSocketContext webSocketContext;

            try
            {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
                string ipAddress = listenerContext.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine($"Connected: IP {ipAddress}");
            }
            catch (Exception e)
            {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine($"Exception: {e}");
                return;
            }

            WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                byte[] receiveBuffer = new byte[1024];

                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer, 0, receiveResult.Count), 
                                                  WebSocketMessageType.Binary, 
                                                  receiveResult.EndOfMessage, 
                                                  CancellationToken.None);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                if (webSocket != null)
                {
                    webSocket.Dispose();
                    Console.WriteLine("Server closed!");
                }
            }
        }
    }
}
