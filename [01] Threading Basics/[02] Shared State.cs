using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Threading_Basics
{
    public  class _02__Shared_State
    {
        public static void Show()
        { 
            // LocalState
            {
                new Thread(Go).Start();     // Call Go() on a new thread
                Go();                       // Call Go() on the main thread
            }
        
            // Shared state - unsafe
            {
                new ThreadSharedStateUnsafe().Show();
            }

            // Shared state with closure - unsafe
            {
                bool done = false;
                ThreadStart action = () => { if (!done) { Console.WriteLine("Done"); done = true;  } };
                new Thread(action).Start();     // Call action on a new thread  
                action();                       // Call action on the main thread     
            }

            // Shared state - safe
            {
                new ThreadSharedStateSafe().Show();
            }

            // Lambdas and Captured variables - unsafe
            {
                for (int i = 0; i < 10; i++)
                    new Thread(() => Console.Write(i)).Start();
            }

            // Lambdas and Captured variables - safe
            {
                for (int i = 0; i < 10; i++)
                {
                    int temp = i;
                    new Thread(() => Console.Write(temp)).Start();
                }
                  
            }
        }

        private static void Go()
        {
            // Declare and use a local variable - 'cycles'
            for (int cycles = 0; cycles < 5; cycles++) Console.Write('?');
        }

    }

    public class ThreadSharedStateUnsafe
    {
        bool _done;
        public  void Show()
        {
            new Thread(Go).Start();     // Call Go() on a new thread
            Go();                       // Call Go() on the main thread
        }
        void Go()
        {
            if (!_done) { Console.WriteLine("Done"); _done = true; }
        }
    }

    public class ThreadSharedStateSafe
    {
        static bool _done;
        static readonly object _locker = new object();

        public void Show()
        {
            new Thread(Go).Start();
            Go();
        }

        private void Go()
        {
            lock (_locker) { if (!_done) { Console.WriteLine("Done"); _done = true; } }
        }
    }
}
