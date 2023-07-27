// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
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

await Task.Run(async () =>
{
    while (true)
    {
        Socket remote = await server.AcceptAsync();
        Console.WriteLine($"accepted the remote client: {remote.RemoteEndPoint}");

        if (remote.Connected)
        {
            await Task.Run(async () =>
            {
                ArraySegment<byte> recvBuffer = new(new byte[1024]);
                int dataLen;
                do
                {
                    dataLen = await remote.ReceiveAsync(recvBuffer, SocketFlags.None);
                    Console.WriteLine($"received the client data: {dataLen}...");
                    await Task.Delay(100);
                } while (dataLen > 0);
            });
        }
    }
});

do
{
    await Task.Delay(3000);
} while (true);