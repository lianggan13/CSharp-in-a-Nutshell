/*----------------------------------------------------------------
* Copyright (c) 2022 ZhangLiang All Rights Reserved
* Domain:   DESKTOP-GE6HNEQ
* Author：  ZhangLiang
* TEL:      15694047739
* WX:       GanNo-13
*----------------------------------------------------------------*/

using System.Net;

namespace TcpTcpListener.Extensions
{
    public static class NetworkHelper
    {
        public static void ParseIpAndPort(this EndPoint endPoint, out string ip, out int port)
        {
            IPEndPoint iPEndPoint = (IPEndPoint)endPoint;
            ip = $"{iPEndPoint.Address.MapToIPv4()}";
            port = iPEndPoint.Port;
        }

        //public static string GetAddress(this EndPoint endPoint)
        //{
        //    IPEndPoint iPEndPoint = (IPEndPoint)endPoint;
        //    string ip = $"{iPEndPoint.Address.MapToIPv4()}";
        //    int port = iPEndPoint.Port;
        //    return ip + ":" + port;
        //}
    }
}
