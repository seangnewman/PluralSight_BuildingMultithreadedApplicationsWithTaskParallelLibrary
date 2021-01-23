using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {
        private static string API_URL = "https://ps-async.fekberg.com/api/stocks";
        private Stopwatch stopwatch = new Stopwatch();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }


        private  async void  Search_Click(object sender, RoutedEventArgs e)
        {
            BeforeLoadingStockData();
            #region A Problem to Solve in Parallel
            //var stocks = new Dictionary<string, IEnumerable<StockPrice>>
            //{
            //    { "MSFT", Generate("MSFT") },
            //    { "GOOGL", Generate("GOOGL") },
            //    { "PS", Generate("PS") },
            //    { "AMAZ", Generate("AMAZ") }
            //};

            //var msft = Calculate(stocks["MSFT"]);
            //var googl = Calculate(stocks["GOOGL"]);
            //var ps = Calculate(stocks["PS"]);
            //var amaz = Calculate(stocks["AMAZ"]);

            //Stocks.ItemsSource = new[] { msft, googl, ps, amaz };


            #endregion

            #region First Parallel Operation
            var stocks = new Dictionary<string, IEnumerable<StockPrice>>
            {
                { "MSFT", Generate("MSFT") },
                { "GOOGL", Generate("GOOGL") },
                { "PS", Generate("PS") },
                { "AMAZ", Generate("AMAZ") }
            };

            //Thread Safe that allows data to be added
            var bag = new ConcurrentBag<StockCalculation>();
            var complete = await Task.Run(() =>
            {

            Parallel.Invoke(
                // MaxDegreeOfParallelism used to control number of cores
                 //   new ParallelOptions { MaxDegreeOfParallelism = 4},
                    () =>
                    {
                        var msft = Calculate(stocks["MSFT"]);
                        bag.Add(msft);
                    },
                    () =>
                    {
                        var googl = Calculate(stocks["GOOGL"]);
                        bag.Add(googl);
                    },
                    () =>
                    {
                        var ps = Calculate(stocks["PS"]);
                        bag.Add(ps);
                    },
                    () =>
                    {
                        var amaz = Calculate(stocks["AMAZ"]);
                        bag.Add(amaz);
                    });

                return bag;
            });
            Stocks.ItemsSource = bag;


            #endregion
            AfterLoadingStockData();
        }

        private IEnumerable<StockPrice> Generate(string stockIdentifier)
        {
            return Enumerable.Range(1, random.Next(10, 250))
                .Select(x => new StockPrice { 
                    Identifier = stockIdentifier, 
                    Open = random.Next(10, 1024) 
                });
        }

        private StockCalculation Calculate(IEnumerable<StockPrice> prices)
        {
            #region Start stopwatch
            var calculation = new StockCalculation();
            var watch = new Stopwatch();
            watch.Start();
            #endregion

            var end = DateTime.UtcNow.AddSeconds(4);

            // Spin a loop for a few seconds to simulate load
            while (DateTime.UtcNow < end)
            { }

            #region Return a result
            calculation.Identifier = prices.First().Identifier;
            calculation.Result = prices.Average(s => s.Open);

            watch.Stop();

            calculation.TotalSeconds = watch.Elapsed.Seconds;

            return calculation;
            #endregion
        }








        private void BeforeLoadingStockData()
        {
            stopwatch.Restart();
            StockProgress.Visibility = Visibility.Visible;
            StockProgress.IsIndeterminate = true;
        }

        private void AfterLoadingStockData()
        {
            StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
