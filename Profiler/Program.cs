using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BG.Tests;

namespace Profiler
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {

                Console.WriteLine("Test initializing.");

                var test = new ServiceTest();

                Console.WriteLine("Test initialized.");

                test.RefreshCacheTest();

            } catch(Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex);
                Console.ReadLine();
            }

            Console.WriteLine("Profiling complete!");

        }
    }
}
