using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Utilities;

public class TcpServer
{
    /// <summary>
    /// 建立连接触发事件
    /// </summary>
    public event EventHandler<SocketEventArgs>? Connected;

    /// <summary>
    /// 断开连接触发事件
    /// </summary>
    public event EventHandler<SocketEventArgs>? Disconnected;

    /// <summary>
    /// 收到数据触发事件
    /// </summary>
    public event EventHandler<SocketReceivedDataEventArgs>? ReceivedData;

    /// <summary>
    /// 抛出异常触发事件
    public event EventHandler<SocketExceptionEventArgs>? ThrownException;

    private int localPort;
    private readonly ConcurrentDictionary<string, TcpClient> clients = new ConcurrentDictionary<string, TcpClient>();

    public TcpServer(int localPort)
    {
        this.localPort = localPort;
    }

    public async Task RunAsync()
    {
        TcpListener? listener = new TcpListener(IPAddress.Any, localPort);

        try
        {
            LogHelper.Info($"listener started at port {localPort}");
            listener.Start();

            while (true)
            {
                LogHelper.Info("waiting for client...");
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Task t = RunClientRequestAsync(client);
                }
                catch (Exception ex)
                {
                    LogHelper.Info(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.Info($"Exception of type {ex.GetType().Name}, Message: {ex.Message}");
        }
        finally { listener.Stop(); }
    }


    private async Task RunClientRequestAsync(TcpClient client)
    {
        await Task.Yield(); // 借助 await 实现线程的切换，让 await 之后的操作重新排队从线程池中申请线程继续执行

        var ipEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        GetIpAndPort(ipEndPoint, out string remoteIp, out int remotePort);
        //SocketReceivedDataEventArgs e = new SocketReceivedDataEventArgs(remoteIp, remotePort, null);

        string key = $"{remoteIp}:{remotePort}";
        LogHelper.Info($"client {key} connected");

        clients.AddOrUpdate(key, client, (s, c) => client);
        Connected?.Invoke(this, new SocketEventArgs(remoteIp, remotePort));

        try
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                //client.ReceiveTimeout = 3000;
                //client.SendTimeout = 3000;
                //stream.ReadTimeout = 3000;
                //stream.WriteTimeout = 3000;
                do
                {
                    byte[] readBuffer = new byte[10240];

                    int read = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);

                    if (read == 0)
                    {
                        // remove
                        clients.TryRemove(key, out TcpClient delclient);
                        Disconnected?.Invoke(client, new SocketEventArgs(remoteIp, remotePort));
                        break;
                    }

                    ReceivedData?.Invoke(this, new SocketReceivedDataEventArgs(remoteIp, remotePort,
                                                       readBuffer.Take(read).ToArray()));
                } while (true);
            }
        }
        catch (Exception ex)
        {
            LogHelper.Info($"Exception in client request handling of type {ex.GetType().Name}, Message: {ex.Message}");
            // remove
            clients.TryRemove(key, out TcpClient? delClient);
            Disconnected?.Invoke(client, new SocketEventArgs(remoteIp, remotePort));
            LogHelper.Info("client disconnected");
        }
    }


    public void GetIpAndPort(IPEndPoint iPEndPoint, out string ip, out int port)
    {
        ip = iPEndPoint.Address.MapToIPv4().ToString();
        port = iPEndPoint.Port;
    }

    public void Send(string address, byte[] writeBuffer)
    {
        if (clients.TryGetValue(address, out TcpClient? client))
        {
            client.Client.Send(writeBuffer, writeBuffer.Length, SocketFlags.None);
        }

    }
}
