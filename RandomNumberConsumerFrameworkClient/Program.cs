using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace RandomNumberConsumerFrameworkClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var channelFactory = new ChannelFactory<RandomNumberCore.ITestServiceChannel>("localServiceEndpoint");
            var service = channelFactory.CreateChannel();
            service.Open();

            var cts = new CancellationTokenSource(5_000);

            Dictionary<int, int> bag = new Dictionary<int, int>();

            var r = new Random();
            int counter = 0;
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var nextInt = service.NextInt();
                    var next = nextInt.Value;
                    var counterNo = Interlocked.Increment(ref counter);
                    bag.Add(counterNo, next);
                }
            }
            catch
            {
                // ignored
            }

            Console.WriteLine($"{counter:N0}\n{bag.Count:N0}");

            
            service.Close();
            service.Dispose();
            channelFactory.Close();
            
        }
    }
}