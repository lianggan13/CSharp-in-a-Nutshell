// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;


var domainEntry = Dns.GetHostEntry("www.baidu.com");
Console.WriteLine(domainEntry.HostName);
foreach (var ip in domainEntry.AddressList)
{
    Console.WriteLine(ip);
}

var domainEntryByAddress = Dns.GetHostEntry("127.0.0.1");
Console.WriteLine(domainEntryByAddress.HostName);
foreach (var ip in domainEntryByAddress.AddressList)
{
    Console.WriteLine(ip);
}
Thread.Sleep(10000);
Console.ReadKey();


if (args.Length == 0)
    //args = new string[] { "-p", "9400" };
    args = new string[] { "-p", "9400", "-g", "230.0.0.1" };

if (!ParseCommandLine(args, out int port, out string groupAddress))
{
    ShowUsage();
    return;
}
await ReaderAsync(port, groupAddress);
Console.ReadLine();

static void ShowUsage() =>
   Console.WriteLine("Usage: UdpReceiver -p port  [-g groupaddress]");

static bool ParseCommandLine(string[] args, out int port, out string groupAddress)
{
    port = 0;
    groupAddress = string.Empty;
    if (args.Length < 2 || args.Length > 5)
    {
        return false;
    }
    if (args.SingleOrDefault(a => a == "-p") == null)
    {
        Console.WriteLine("-p required");
        return false;
    }

    // get port number
    string port1 = GetValueForKey(args, "-p");
    if (port1 == null || !int.TryParse(port1, out port))
    {
        return false;
    }

    // get optional group address
    groupAddress = GetValueForKey(args, "-g");
    return true;
}

static string GetValueForKey(string[] args, string key)
{
    int? nextIndex = args.Select((a, i) => new { Arg = a, Index = i }).SingleOrDefault(a => a.Arg == key)?.Index + 1;
    if (!nextIndex.HasValue)
    {
        return null;
    }
    return args[nextIndex.Value];
}

static async Task ReaderAsync(int port, string groupAddress)
{
    using (var client = new UdpClient(port))
    //using (var client = new UdpClient(new IPEndPoint(IPAddress.Any, port)))
    {
        //IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        if (groupAddress != null)
        {
            client.JoinMulticastGroup(IPAddress.Parse(groupAddress));
            Console.WriteLine($"joining the multicast group {IPAddress.Parse(groupAddress)}");
        }

        bool completed = false;
        do
        {
            Console.WriteLine("starting the receiver");
            UdpReceiveResult result = await client.ReceiveAsync();
            byte[] datagram = result.Buffer;
            string received = Encoding.UTF8.GetString(datagram);
            Console.WriteLine($"received {received}");
            if (received == "bye")
            {
                completed = true;
            }
        } while (!completed);
        Console.WriteLine("receiver closing");

        if (groupAddress != null)
        {
            client.DropMulticastGroup(IPAddress.Parse(groupAddress));
        }
    }
}
