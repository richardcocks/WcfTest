using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace RandomNumberConsumerFrameworkClient
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting tests");
            // TestEndpointConfiguration("localServiceEndpointTcp");
            await TestStreamingEndpoint("localServiceEndpointTcpStreaming");
        //     TestEndpointConfiguration("localServiceEndpointHttps");
        //     
        //     TestEndpointConfiguration("localServiceEndpointNamedPipe");
        //     TestEndpointConfiguration("localServiceEndpointHttps");
        //     TestEndpointConfiguration("localServiceEndpointTcp");
        //     TestEndpointConfiguration("localServiceEndpointNamedPipe");
        //
        //
        }

        private static async Task TestStreamingEndpoint(string endpointConfigurationName)
        {
            
            Console.WriteLine($"Testing streaming {endpointConfigurationName}");
            
            using(var channelFactory = new ChannelFactory<RandomNumberCore.IStreamingServiceChannel>(endpointConfigurationName))
            using (var service = channelFactory.CreateChannel())
            {
                service.Open();

                var cts = new CancellationTokenSource(5_00);
                var sw = Stopwatch.StartNew();

                Dictionary<int, int> bag = new Dictionary<int, int>(3_000_000);

                var counter = 0;
                using (var randomStream = service.GetRandomStream())
                {

                    // We'll test other lengths later.

                    byte[] buffer = new byte[4];

                    while (!cts.Token.IsCancellationRequested)
                    {
                        var readBytes = await randomStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);

                        if (readBytes != 4)
                        {
                            throw new Exception("Stream did not read 4 bytes");
                        }

                        var next = BitConverter.ToInt32(buffer, 0);
                        var counterNo = Interlocked.Increment(ref counter);
                        bag.Add(counterNo, next);
                    }
                }

                sw.Stop();


                Console.WriteLine($"First:{bag[1]:N0}. Last:{bag[counter]:N0}");
                Console.WriteLine(
                    $"Counter:{counter:N0}. Successfully filled:{bag.Count:N0} in {sw.ElapsedMilliseconds}ms");


                service.Close();

            channelFactory.Close();
            }

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

            Console.WriteLine($"First:{bag[1]:N0}. Last:{bag[counter]:N0}");
            Console.WriteLine($"Counter:{counter:N0}. Successfully filled:{bag.Count:N0} in {sw.ElapsedMilliseconds}ms");

            
            service.Close();
            service.Dispose();

            channelFactory.Close();
        }
    }
}