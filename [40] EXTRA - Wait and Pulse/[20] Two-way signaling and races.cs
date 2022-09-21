using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _40__EXTRA___Wait_and_Pulse
{
    public class _20__Two_way_signaling_and_races
    {
		static readonly object _locker = new object();
		static bool _go;

		static  void Show()
		{
			new Thread(SaySomething).Start();

			for (int i = 0; i < 5; i++)
				lock (_locker)
				{
					_go = true;
					Monitor.PulseAll(_locker);
				}
		}

		static void SaySomething()
		{
			for (int i = 0; i < 5; i++)
				lock (_locker)
				{
					while (!_go) Monitor.Wait(_locker);
					_go = false;
					Console.WriteLine("Wassup?");
				}
		}
	}
}
