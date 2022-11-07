// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Text;
using Utilities;

TcpServer tcpServer = new TcpServer(100);

tcpServer.Connected += TcpServer_Connected;
tcpServer.Disconnected += TcpServer_Disconnected;
tcpServer.Received += TcpServer_Received;

void TcpServer_Connected(string address, TcpClient client)
{
    LogHelper.Info($"{address} connected...");
}

void TcpServer_Disconnected(string address, TcpClient client)
{
    LogHelper.Warn($"{address} disconnected...");
}

void TcpServer_Received(string address, TcpClient client, byte[] data)
{
    // read: bytes --> string --> object
    // parse...
    // send: object --> string --> bytes

    string json = Encoding.UTF8.GetString(data);
    LogHelper.Info($">> [{address}] {json}");

    byte[] bytes = Encoding.UTF8.GetBytes(json);
    tcpServer.SendAsync(client, bytes);
    LogHelper.Info($"<< [{address}] {json}");
}

await tcpServer.RunAsync();

Console.ReadLine();