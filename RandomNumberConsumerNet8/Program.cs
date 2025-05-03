using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace RandomNumberConsumerNet8
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            Console.WriteLine("Starting tests");
            
            TestEndpointConfiguration(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "https://localhost:7151/Service.svc");
            TestEndpointConfiguration(new NetTcpBinding(SecurityMode.Transport), "net.tcp://localhost:808/Service/netTcp");
            TestEndpointConfiguration(new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport), "net.pipe://localhost/Service");
            
            TestEndpointConfiguration(new BasicHttpBinding(BasicHttpSecurityMode.Transport), "https://localhost:7151/Service.svc");
            TestEndpointConfiguration(new NetTcpBinding(SecurityMode.Transport), "net.tcp://localhost:808/Service/netTcp");
            TestEndpointConfiguration(new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport), "net.pipe://localhost/Service");


        }

        private static void TestEndpointConfiguration(Binding endpointBinding, string address)
        {
            Console.WriteLine($"Testing {endpointBinding}");
            ArgumentNullException.ThrowIfNull(endpointBinding);
            using var channelFactory = new ChannelFactory<RandomNumberCore.ITestServiceChannel>(endpointBinding, new EndpointAddress(address)); ;
            using var service = channelFactory.CreateChannel();
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
            channelFactory.Close();
        }
    }
}