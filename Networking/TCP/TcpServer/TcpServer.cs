using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

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
        try
        {
            var listener = new TcpListener(IPAddress.Any, localPort);
            Console.WriteLine($"listener started at port {localPort}");
            listener.Start();

            while (true)
            {
                Console.WriteLine("waiting for client...");
                try
                {
                    System.Net.Sockets.TcpClient client = await listener.AcceptTcpClientAsync();
                    Task t = RunClientRequestAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception of type {ex.GetType().Name}, Message: {ex.Message}");
        }
    }


    private Task RunClientRequestAsync(TcpClient client)
    {
        return Task.Run(async () =>
        {
            //client.ReceiveTimeout = 3000;
            //client.SendTimeout = 3000;

            var ipEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            GetIpAndPort(ipEndPoint, out string remoteIp, out int remotePort);
            //SocketReceivedDataEventArgs e = new SocketReceivedDataEventArgs(remoteIp, remotePort, null);

            string key = $"{remoteIp}:{remotePort}";
            Console.WriteLine($"client {key} connected");

            clients.AddOrUpdate(key, client, (s, c) => client);
            Connected?.Invoke(this, new SocketEventArgs(remoteIp, remotePort));

            try
            {
                using (client)
                {
                    using (NetworkStream stream = client.GetStream())
                    {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in client request handling of type {ex.GetType().Name}, Message: {ex.Message}");
                // remove
                clients.TryRemove(key, out TcpClient? delClient);
                Disconnected?.Invoke(client, new SocketEventArgs(remoteIp, remotePort));
                Console.WriteLine("client disconnected");
            }
        });
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
