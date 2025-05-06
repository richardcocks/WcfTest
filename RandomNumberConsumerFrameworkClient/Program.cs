using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace RandomNumberConsumerFrameworkClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Starting tests");
            TestEndpointConfiguration("localServiceEndpointTcp");
        //     TestEndpointConfiguration("localServiceEndpointHttps");
        //     
        //     TestEndpointConfiguration("localServiceEndpointNamedPipe");
        //     TestEndpointConfiguration("localServiceEndpointHttps");
        //     TestEndpointConfiguration("localServiceEndpointTcp");
        //     TestEndpointConfiguration("localServiceEndpointNamedPipe");
        //
        //
        }

        private static void TestEndpointConfiguration(string endpointConfigurationName)
        {
            Console.WriteLine($"Testing {endpointConfigurationName}");
            var channelFactory = new ChannelFactory<RandomNumberCore.ITestServiceChannel>(endpointConfigurationName);

            var service = channelFactory.CreateChannel();
            service.Open();

            // Warm the service up
            for (int i = 0; i < 1_000; i++)
            {
                service.NextInt();
            }
                
            
            var cts = new CancellationTokenSource(5_000);
            var sw = Stopwatch.StartNew();
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
            finally
            {
                sw.Stop();
            }

            Console.WriteLine($"Counter:{counter:N0}. Successfully filled:{bag.Count:N0} in {sw.ElapsedMilliseconds}ms");

            
            service.Close();
            service.Dispose();

            channelFactory.Close();
        }
    }
}