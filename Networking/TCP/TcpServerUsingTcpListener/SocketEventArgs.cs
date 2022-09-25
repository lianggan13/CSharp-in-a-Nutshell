public class SocketEventArgs : EventArgs
{
    public SocketEventArgs(string ip, int port)
    {
        Ip = ip;
        Port = port;
        Address = $"{ip}:{port}";
    }

    /// <summary>
    /// 获取IP地址
    /// </summary>
    public string Ip { get; private set; }

    /// <summary>
    /// 获取通信端口
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// 获取通信地址
    /// </summary>
    public string Address { get; private set; }
}
