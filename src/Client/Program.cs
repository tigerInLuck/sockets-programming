// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

Console.WriteLine("The Socket Client is starting...");

IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 8001;
Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint serverEndPoint = new(ip, port);
await client.ConnectAsync(serverEndPoint);

if (client.Connected)
{
    Console.WriteLine("the clinet has been connected to Server...");
    await Task.Run(async () =>
    {
        const string message = "What time is it?";
        ArraySegment<byte> sendBuffer = new(new byte[1024]);
        while (true)
        {
            await client.SendAsync(sendBuffer, SocketFlags.None);
            await Task.Delay(1500);
        }

    });

    await Task.Run(async () =>
    {
        ArraySegment<byte> recvBuffer = new(new byte[1024]);
        int dataLen;
        do
        {
            dataLen = await client.ReceiveAsync(recvBuffer, SocketFlags.Peek);
            Console.WriteLine($"received the server data: {dataLen}...");
        } while (dataLen > 0);
    });
}