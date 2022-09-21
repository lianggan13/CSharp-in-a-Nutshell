using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _40__EXTRA___Wait_and_Pulse
{
   public class _25__Two_way_signaling_and_races___solution
    {
		static readonly object _locker = new object();
		static bool _ready, _go;

		static  void Show()
		{
			new Thread(SaySomething).Start();

			for (int i = 0; i < 5; i++)
				lock (_locker)
				{
					while (!_ready) Monitor.Wait(_locker);
					_ready = false;
					_go = true;
					Monitor.PulseAll(_locker);
				}
		}

		static void SaySomething()
		{
			for (int i = 0; i < 5; i++)
				lock (_locker)
				{
					_ready = true;
					Monitor.PulseAll(_locker);           // Remember that calling
					while (!_go) Monitor.Wait(_locker);  // Monitor.Wait releases
					_go = false;                          // and reacquires the lock.
					Console.WriteLine("Wassup?");
				}
		}
	}
}
