// See https://aka.ms/new-console-template for more information
//Rx.SubEventStream();
// TPL.SimpleLink();

//TPL_DataFlow.CreateCustomBlock();
Concurrency.TPL_DataFlow.LimitCapacity();


Console.ReadKey();


int x = 6;

int y = x + 5;


Task.Run(async () =>
{
    while (true)
    {

        await Task.Delay(4000);
    }

});

// await Asynchronous.ProcessTasksAsync();

//// ch05
//{
//    Ch06();
//}

//ch12r02A.Show();



//TAP.Show().Wait();
//Rx.Show();

//Scheduler.Show();

Console.WriteLine("Press any key to quit.");
Console.ReadKey();




