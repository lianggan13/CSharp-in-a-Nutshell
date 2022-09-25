public class SocketReceivedDataEventArgs : SocketEventArgs
{
    public SocketReceivedDataEventArgs(string ip, int port, byte[] data)
        : base(ip, port)
    {
        Data = data;
    }

    /// <summary>
    /// 获取接收的数据
    /// </summary>
    public byte[] Data { get; private set; }
}
