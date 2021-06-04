using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mutex.MVC.Multiplayer
{
    public class WebSocketServer
    {
        private HashSet<WebSocket> webSockets = default;
        private UTF8Encoding m_Encoding = new UTF8Encoding();

        public async void Start(string listenerPrefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            Console.WriteLine("Listening...");

            await Task.WhenAll(Receive(listener), CreateMessage());
        }

        public Task CreateMessage()
        {
            while (true)
            {
                string msg = Console.ReadLine();
                BroadCast(m_Encoding.GetBytes(msg));
            }
        }

        public async Task Receive(HttpListener listener)
        {
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

            if (webSockets == null)
            {
                webSockets = new HashSet<WebSocket>();
            }

            WebSocket webSocket = webSocketContext.WebSocket;
            webSockets.Add(webSocketContext.WebSocket);

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
                        BroadCast(receiveBuffer);
                    }
                }
            }
            catch (WebSocketException)
            {
                string ipAddress = listenerContext.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine($"Disconnected: {ipAddress}");
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
                    Console.WriteLine("Client Disconnected!");
                }
            }
        }

        private async void BroadCast(byte[] buffer)
        {
            Console.WriteLine($"Broadcast: {m_Encoding.GetString(buffer).TrimEnd('\0')}");

            foreach (WebSocket ws in webSockets)
            {
                if (ws.State == WebSocketState.Open)
                {
                     await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                }
            }
        }
    }
}
