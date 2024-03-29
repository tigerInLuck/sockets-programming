﻿// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

Console.WriteLine("The Socket Client is starting...");

IPAddress ip = IPAddress.Parse("127.0.0.1");
int port = 8001;
Socket client = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPEndPoint serverEndPoint = new(ip, port);
await client.ConnectAsync(serverEndPoint);

Console.WriteLine("the clinet has been connected to Server...");
await Task.Factory.StartNew(async () =>
{
    const string message = "What time is it?";
    ArraySegment<byte> sendBuffer = new(Encoding.UTF8.GetBytes(message));
    while (client.Connected)
    {
        await client.SendAsync(sendBuffer, SocketFlags.None);
        await Task.Delay(1000 * 10);
    }
});

await Task.Factory.StartNew(async () =>
{
    while (client.Connected)
    {
        ArraySegment<byte> recvBuffer = new(new byte[1024]);
        try
        {
            int dataLen = await client.ReceiveAsync(recvBuffer, SocketFlags.None);
            if (dataLen > 0)
            {
                Console.WriteLine($"received the server echo: {Encoding.UTF8.GetString(new ReadOnlySpan<byte>(recvBuffer.Array))}...");
            }
            await Task.Delay(15);
        }
        catch (Exception)
        {
            if (client.Connected) continue;
            Console.WriteLine("the server has been disconnected...");
            client.Dispose();
            return;
        }
    }
});

while (true)
{
    await Task.Delay(5000);
}