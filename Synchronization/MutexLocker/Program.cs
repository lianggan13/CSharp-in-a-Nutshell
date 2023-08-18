// See https://aka.ms/new-console-template for more information

bool mutexCreated;
var mutex = new Mutex(false, "SingletonAppMutex", out mutexCreated);
if (!mutexCreated)
{
    Console.WriteLine("You can only start one instance of the application.");
    Console.WriteLine("Exiting.");
    return;
}
Console.WriteLine("Application running");
Console.WriteLine("Press return to exit");
Console.ReadLine();