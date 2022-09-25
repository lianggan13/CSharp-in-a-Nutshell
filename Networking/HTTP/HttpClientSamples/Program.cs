// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

Command[] s_Commands;

var samples = new HttpClientSamples();
s_Commands = SetupCommands(samples);

args = new string[] { "-m" };

//if (args.Length == 0 || args.Length > 1 || !s_Commands.Select(c => c.Option).Contains(args[0]))
//{
//    ShowUsage();
//    return;
//}

var c = s_Commands.SingleOrDefault(c => c.Option == args[0]);

await s_Commands.Single(c => c.Option == args[0]).ActionAsync();
Console.ReadLine();

static Command[] SetupCommands(HttpClientSamples samples) =>
   new Command[]
   {
                new Command("-s", nameof(HttpClientSamples.GetDataSimpleAsync), samples.GetDataSimpleAsync),
                new Command("-a", nameof(HttpClientSamples.GetDataAdvancedAsync), samples.GetDataAdvancedAsync),
                new Command("-e", nameof(HttpClientSamples.GetDataWithExceptionsAsync), samples.GetDataWithExceptionsAsync),
                new Command("-h", nameof(HttpClientSamples.GetDataWithHeadersAsync), samples.GetDataWithHeadersAsync),
                new Command("-m", nameof(HttpClientSamples.GetDataWithMessageHandlerAsync), samples.GetDataWithMessageHandlerAsync),
   };


void ShowUsage()
{
    Console.WriteLine("Usage: HttpClientSample [options]");
    Console.WriteLine();
    foreach (var command in s_Commands)
    {
        Console.WriteLine($"{command.Option} {command.Text}");
    }
}