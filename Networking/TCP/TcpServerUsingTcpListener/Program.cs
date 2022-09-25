// See https://aka.ms/new-console-template for more information

using System.Text;
using Utilities;

TcpServer tcpServer = new TcpServer(9910);

tcpServer.Connected += (s, e) =>
{
    // connected...
    LogHelper.Info($"{e.Address} connected...");
};
tcpServer.Disconnected += (s, e) =>
{
    // disconnected...
    LogHelper.Warn($"{e.Address} disconnected!");
};
tcpServer.ReceivedData += (s, e) =>
{
    // read: bytes --> string --> object
    // parse...
    // send: object --> string --> bytes

    string json = Encoding.UTF8.GetString(e.Data);
    LogHelper.Info($">> [{e.Address}] {json}");

    byte[] bytes = Encoding.UTF8.GetBytes(json);
    tcpServer.Send(e.Address, bytes);
    LogHelper.Info($"<< [{e.Address}] {json}");
};

await tcpServer.RunAsync();

Console.ReadLine();