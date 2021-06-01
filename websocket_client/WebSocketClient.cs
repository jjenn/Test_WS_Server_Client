using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mutex.MVC.Multiplayer
{
    public class WebSocketClient
    {
        private UTF8Encoding m_Encoding = new UTF8Encoding();

        public async Task Connect(string uri)
        {
            Thread.Sleep(1000);

            ClientWebSocket client = null;

            try
            {
                client = new ClientWebSocket();
                await client.ConnectAsync(new Uri(uri), CancellationToken.None);
                await Task.WhenAll(Receive(client), Send(client));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }

                Console.WriteLine("Connection closed!");
            }
        }

        public async Task Receive(ClientWebSocket client)
        {
            byte[] buffer = new byte[1024];

            while (client.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    string msg = m_Encoding.GetString(buffer).TrimEnd('\0');
                    Console.WriteLine($"Receive: {msg}");
                }
            }
        }

        public async Task Send(ClientWebSocket client)
        {
            while (client.State == WebSocketState.Open)
            {
                Console.WriteLine("Your msg:");
                string msg = Console.ReadLine();
                byte[] buffer = m_Encoding.GetBytes(msg);

                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                Console.WriteLine($"Sent!");

                await Task.Delay(1000);
            }
        }
    }
}
