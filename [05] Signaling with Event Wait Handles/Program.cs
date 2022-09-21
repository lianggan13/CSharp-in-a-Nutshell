using System;

namespace _05__Signaling_with_Event_Wait_Handles
{
    /// <summary>
    /// 锁和安全性
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /*       
            事件等待句柄(event wait handlers)发送信号
	        AutoResetEvent
		        进站闸门：插入一张票只允许一个人通过
		        Set 向闸机中插入一张票据 
		        WaitOne 在闸机门口等待、阻塞，直至闸机门开启
	        ManualResetEvent
		        大门
		        Set 开启大门，并允许任意数量的调用WaitOne方法的线程通过大门
		        Reset 关闭大门，调用 WaitOne 方法会发生阻塞
             
             */

            //_01__AutoResetEvent.Show();

            //_05__Two_way_signaling.Show();

            //_10__CountdownEvent.Show();

            _15__Wait_Handles_and_continuations.Show();

            Console.ReadKey();
        }
    }
}
