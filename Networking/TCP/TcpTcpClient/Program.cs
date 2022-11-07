// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Text;
using Utilities;


const string Host = "localhost";
const int Port = 9910;

await SendAndReceiveAsync();
Console.ReadLine();

static async Task SendAndReceiveAsync()
{
    using (var client = new TcpClient())
    {
        await client.ConnectAsync(Host, Port);
        using (NetworkStream stream = client.GetStream())
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
        using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen: true))
        {
            writer.AutoFlush = true;
            string line = string.Empty;
            do
            {
                LogHelper.Info("enter a string, bye to exit");
                line = Console.ReadLine();
                LogHelper.Info($"<< {line}");

                await writer.WriteLineAsync(line);

                string result = await reader.ReadLineAsync();
                LogHelper.Info($">> {result}");
            } while (line != "bye");

            LogHelper.Info("so long, and thanks for all the fish");
        }
    }
}