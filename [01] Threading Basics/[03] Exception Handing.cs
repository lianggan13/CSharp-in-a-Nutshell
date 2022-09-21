using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01__Threading_Basics
{
   public  class _03__Exception_Handing
    {
        public static void Show()
        {
            // Exception Handing - Wrong place
            {
                try
                {
                    new Thread(GoNoCatch).Start();
                }
                catch (Exception ex)
                {
                    // We'll never get here!
                    Console.WriteLine("Exception!");
                }
            }

            // Exception Handing - Right place
            {
                new Thread(GoCatch).Start();       
            }


        }

        private static void GoCatch()
        {
            throw new NotImplementedException();
        }

        private static void GoNoCatch()
        {
            try
            {
                throw null;    // The NullReferenceException will get caught below
            }
            catch (Exception ex)
            {
                //Typically log the exception, and/or signal another thread
                // that we've come unstuck
                ex.Dump("Caught!");
            }
        }
    }
}
