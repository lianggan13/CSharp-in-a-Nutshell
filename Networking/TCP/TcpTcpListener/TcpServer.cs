using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Utilities;

public class TcpServer
{
    public event Action<string, TcpClient>? Connected;

    public event Action<string, TcpClient>? Disconnected;

    public event Action<string, TcpClient, byte[]>? Received;

    //public event Action<TcpClient, Exception>? Errored;


    private int localPort;
    //private readonly ConcurrentDictionary<string, TcpClient> clients = new ConcurrentDictionary<string, TcpClient>();
    private readonly ConcurrentBag<TcpClient> clients = new ConcurrentBag<TcpClient>();


    public TcpServer(int localPort)
    {
        this.localPort = localPort;
    }

    public async Task RunAsync()
    {
        TcpListener? listener = new TcpListener(IPAddress.Any, localPort);
        //listener.Server.Listen(backlog: 999);
        try
        {
            LogHelper.Info($"TCP listener started at port {localPort}");
            listener.Start();

            while (true)
            {
                LogHelper.Info("Waiting for client...");
                try
                {
                    //await Accept(await listener.AcceptTcpClientAsync());
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Task t = Accept(client);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.Message, ex);
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.Error($"Exception of type {ex.GetType().Name}, Message: {ex.Message}", ex);
        }
        finally { listener.Stop(); }
    }

    const int BUFFER_SIZE = 2048;
    TimeSpan readTimeout = TimeSpan.FromSeconds(180);
    TimeSpan writeTimeout = TimeSpan.FromSeconds(5);

    private async Task Accept(TcpClient client)
    {
        await Task.Yield(); // 借助 await 实现线程的切换，让 await 之后的操作重新排队，并从线程池中申请线程继续执行

        string address = $"{client.Client.RemoteEndPoint}";
        client.NoDelay = true;
        client.ReceiveTimeout = (int)readTimeout.TotalMilliseconds;

        clients.Add(client);
        Connected?.Invoke(address, client);

        try
        {
            //Socket socket = client.Client;
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                stream.ReadTimeout = (int)readTimeout.TotalMilliseconds;

                do
                {
                    byte[] readBuffer = new byte[BUFFER_SIZE];
                    int bytesRead = 0; int chunkSize = 0;
                    do
                    {
                        using (var timeoutToken = new CancellationTokenSource())
                        {
                            timeoutToken.CancelAfter(readTimeout);
                            chunkSize = await stream.ReadAsync(readBuffer, bytesRead, BUFFER_SIZE - chunkSize, timeoutToken.Token);
                            bytesRead += chunkSize;
                        }
                    } while (stream.DataAvailable);


                    if (bytesRead == 0)
                    {
                        clients.TryTake(out _);
                        Disconnected?.Invoke(address, client);
                        break;
                    }

                    //byte[] recBuf = new byte[bytesRead];
                    //Array.Copy(readBuffer, recBuf, bytesRead);
                    //Received?.Invoke(address, client, recBuf);
                    Received?.Invoke(address, client, readBuffer.Take(bytesRead).ToArray());
                } while (true);
            }

        }
        catch (Exception ex)
        {
            LogHelper.Error($"Exception in client request handling of type {ex.GetType().Name}, Message: {ex.Message}", ex);

            clients.TryTake(out client);
            Disconnected?.Invoke(address, client);
        }
    }


    public void Send(TcpClient client, byte[] writeBuffer)
    {
        client.Client.Send(writeBuffer, writeBuffer.Length, SocketFlags.None);
    }

    public async void SendAsync(TcpClient client, byte[] writeBuffer)
    {
        using (var timeoutToken = new CancellationTokenSource())
        {
            timeoutToken.CancelAfter(writeTimeout);
            //using NetworkStream stream = client.GetStream();
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length, timeoutToken.Token);
            await stream.FlushAsync();
        }
    }
}
