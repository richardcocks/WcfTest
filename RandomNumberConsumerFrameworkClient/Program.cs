using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using RandomNumberCore;

namespace RandomNumberConsumerFrameworkClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            TestHttpBinding("localServiceEndpointHttps");
            TestHttpBinding("localServiceEndpointTcp");
            TestHttpBinding("localServiceEndpointNamedPipe");
        }

        private static void TestHttpBinding(string endpointConfigurationName)
        {
            var channelFactory = new ChannelFactory<RandomNumberCore.ITestServiceChannel>(endpointConfigurationName);
            
            
            TestEndpoint(channelFactory);
            
            channelFactory.Close();
        }

        private static void TestEndpoint(ChannelFactory<ITestServiceChannel> channelFactory)
        {
            var service = channelFactory.CreateChannel();
            service.Open();

            // Warm the service up
            for (int i = 0; i < 1_000; i++)
            {
                service.NextInt();
            }
                
            
            var cts = new CancellationTokenSource(5_000);

            Dictionary<int, int> bag = new Dictionary<int, int>();

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
        }
    }
}