using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StockAnalyzer.AdvancedTopics
{
    class Program
    {

        static object syncRoot = new object();
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            decimal total = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    total += Compute(i);
            //}

            Parallel.For(0, 100, (i) => {
                var result = Compute(i);
                // lock (syncroot) insures only one thread can access the variable 
                lock (syncRoot)
                {
                    // Only lock for short, atomic operations
                    total += result;
                }
               
            });

            Console.WriteLine(total);
            Console.WriteLine($"It took : {stopWatch.ElapsedMilliseconds}ms to run");
        }

        static Random random = new Random();
         static decimal Compute(int value)
        {
            var randomMilliseconds = random.Next(10, 50);
            var end = DateTime.Now + TimeSpan.FromMilliseconds(randomMilliseconds);

            // Make the function spin for a while
            while (DateTime.Now < end)
            {

            }

            return value + 0.5m;

        }


    }
}
