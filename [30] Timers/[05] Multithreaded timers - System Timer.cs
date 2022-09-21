using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _30__Timers
{
  public  class _05__Multithreaded_timers___System_Timer
    {
		public static  void Show()
		{
			var tmr = new System.Timers.Timer();       // Doesn't require any args
			tmr.Interval = 500;
			tmr.Elapsed += tmr_Elapsed;    // Uses an event instead of a delegate
			tmr.Start();                   // Start the timer
			Console.ReadLine();
			tmr.Stop();                    // Stop the timer
			Console.ReadLine();
			tmr.Start();                   // Restart the timer
			Console.ReadLine();
			tmr.Dispose();                 // Permanently stop the timer
		}

		static void tmr_Elapsed(object sender, EventArgs e)
		{
			Console.WriteLine("Tick");
		}
	}
}
