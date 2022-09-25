public class SocketExceptionEventArgs : SocketEventArgs
{
    public SocketExceptionEventArgs(Exception exception, string ip, int port)
        : base(ip, port)
    {
        Exception = exception;
    }

    /// <summary>
    /// 获取异常信息
    /// </summary>
    public Exception Exception { get; private set; }
}
