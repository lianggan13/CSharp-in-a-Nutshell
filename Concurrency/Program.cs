﻿// See https://aka.ms/new-console-template for more information
//Rx.SubEventStream();
// TPL.SimpleLink();
using Concurrency;

//TPL_DataFlow.CreateCustomBlock();

int x = 6;

int y = x + 5;


Task.Run(async () =>
{
	while (true)
	{

		await Task.Delay(4000);
	}

});

await Asynchronous.ProcessTasksAsync();

//// ch05
//{
//    Ch06();
//}

//ch12r02A.Show();

Console.WriteLine("Press any key to quit.");
Console.ReadKey();
