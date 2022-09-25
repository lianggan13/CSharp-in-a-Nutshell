// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

TcpServer tcpServer = new TcpServer(9910);

tcpServer.Connected += (s, e) =>
{
    // connected...
};
tcpServer.Disconnected += (s, e) =>
{
    // disconnected...
};
tcpServer.ReceivedData += (s, e) =>
{
    // read: bytes --> string --> object
    // parse...
    // send: object --> string --> bytes
};

await tcpServer.RunAsync();

Console.ReadLine();