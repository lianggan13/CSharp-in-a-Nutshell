using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _10__The_Barrier_Class
{
	/// <summary>
	///  线程执行屏障，
	///  控制多个线程在同一时刻汇合，让线程步调一致地执行
	/// </summary>
   public class _01__Barrier
    {
		static Barrier _barrier = new Barrier(3);

		public	static  void Show()
		{
			new Thread(Speak).Start();
			new Thread(Speak).Start();
			new Thread(Speak).Start();
			new Thread(Speak).Start();
		}

		static void Speak()
		{
			for (int i = 0; i < 10; i++)
			{
				Console.Write(i + " ");
				// 调用3次 SignalAndWait(3 个线程每个线程调用一次) ，
				// 阻塞解除，继续执行，直到再次调用 SignalAndWait,重新进入阻塞状态
				_barrier.SignalAndWait();
			}
		}
	}
}
