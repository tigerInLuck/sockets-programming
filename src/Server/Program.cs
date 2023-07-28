// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

Console.WriteLine("The Socket Server is starting...");

IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 8001;
Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint endPoint = new(ip, port);
server.Bind(endPoint);
server.Listen(1000);
Console.WriteLine($"the server is listening to port: {endPoint}...");
Console.WriteLine("Press the 'Ctrl+C' to exit server...");

while (true)
{
    Socket remote = await server.AcceptAsync();
    Console.WriteLine($"accepted the remote client: {remote.RemoteEndPoint}");
    await Task.Factory.StartNew(async () =>
    {
        ArraySegment<byte> recvBuffer = new(new byte[1024]);
        while (true)
        {
            try
            {
                int dataLen = await remote.ReceiveAsync(recvBuffer, SocketFlags.None);
                if (dataLen > 0)
                {
                    Console.WriteLine($"received the client {remote.RemoteEndPoint} data: {Encoding.UTF8.GetString(new ReadOnlySpan<byte>(recvBuffer.Array))}...");
                    await remote.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("It's " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff"))), SocketFlags.None);
                }
                await Task.Delay(10);
            }
            catch (Exception)
            {
                if (remote.Connected) continue;
                Console.WriteLine($"the client {remote.RemoteEndPoint} has been disconnected...");
                remote.Dispose();
                return;
            }
        }
    });
}