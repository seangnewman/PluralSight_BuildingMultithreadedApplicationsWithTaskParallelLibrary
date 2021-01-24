using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalyzer.AdvancedTopics
{
    class Program
    {

        static object syncRoot = new object();
        static object lock1 = new object();
        static object lock2 = new object();

        static ThreadLocal<decimal?> threadLocal = new ThreadLocal<decimal?>();
        static AsyncLocal<decimal?> threadAsync = new AsyncLocal<decimal?>();
        //static void Main(string[] args)
        static async Task Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            int total = 0;
            #region Working with Shared Variables

            ////for (int i = 0; i < 100; i++)
            ////{
            ////    total += Compute(i);
            ////}

            //Parallel.For(0, 100, (i) => {
            //    var result = Compute(i);
            //    // lock (syncroot) insures only one thread can access the variable 
            //    lock (syncRoot)
            //    {
            //        // Only lock for short, atomic operations
            //        total += result;
            //    }

            //});

            #endregion

            #region Performing Atomic Operations

            //Parallel.For(0, 100, (i) => {
            //    var result = Compute(i);


            //    Interlocked.Add(ref total, (int)result);  // Interlocked only works with integers, we will lose some data in the conversion

            //});

            #endregion

            #region Deadlocks and Nested Locks
            // Do not share lock objects for multiple resource
            // Use on lock object for each shared resource
            // Give lock object a meaningfull name
            // Do not user these types!  stirng, type instance (typeof())  or this
            //     

            // Most common situation is nested locks
            //var t1 = Task.Run( () => {
            //    lock (lock1)
            //    {
            //        Thread.Sleep(1);
            //    }

            //    lock (lock2)         
            //    {
            //        Console.WriteLine("Hello!");
            //    }
            //});
            //var t2 = Task.Run(() => {
            //    // This will result in deadlock,  t1 is watiing for t2... t2 is waiting for t1
            //    lock (lock2)
            //    {
            //        Thread.Sleep(1);
            //    }

            //    lock (lock1)
            //    {
            //        Console.WriteLine("World!");
            //    }
            //});

            //await Task.WhenAll(t1, t2);
            #endregion

            #region Cancel Parallel Operations

            //var cancellationTokenSource = new CancellationTokenSource();
            //cancellationTokenSource.CancelAfter(2000);

            //var parallelOptions = new ParallelOptions
            //{
            //    CancellationToken = cancellationTokenSource.Token,
            //    MaxDegreeOfParallelism = 1
            //};
            //try
            //{
            //   Parallel.For(0, 100, parallelOptions, (i) => {
            //        Interlocked.Add(ref total, (int)Compute(i));
            //    });
            //}
            //catch (Exception ex )
            //{

            //    Console.WriteLine("Cancellation Requested!" ); ;
            //}

            #endregion

            #region ThreadLocak and Async Local Variables

            //var cancellationTokenSource = new CancellationTokenSource();
            //cancellationTokenSource.CancelAfter(2000);

            //var parallelOptions = new ParallelOptions
            //{
            //    CancellationToken = cancellationTokenSource.Token,
            //    MaxDegreeOfParallelism = 1
            //};
            //try
            //{
            //    Parallel.For(0, 100, parallelOptions, (i) => {
            //        Interlocked.Add(ref total, (int)Compute(i));
            //    });
            //}
            //catch (Exception ex)
            //{

            //    Console.WriteLine("Cancellation Requested!"); ;
            //}

            #endregion

            #region Thread Local and Async Local Variables

            var options = new ParallelOptions { MaxDegreeOfParallelism = 2};
            //creating thread local variables
            // Data is reusing threads,  so cannot trust value

            Parallel.For(0, 100, options, (i) => {
                var currentValue = threadLocal.Value;
                threadLocal.Value = Compute(i);
            });
           
            Parallel.For(0, 100, options, (i) => {
                var currentValue = threadAsync.Value;
                threadAsync.Value = Compute(i);
            });
            #endregion
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
