using System;
using System.ServiceModel;

namespace RandomNumberFrameworkConsole
{
    public static class Program
    {
        public static void Main()
        {
            
            using (ServiceHost serviceHost = new ServiceHost(typeof(RandomNumberStreamingService), new Uri("net.tcp://localhost:808")))
            
            {
                try
                {
                    // Open the ServiceHost to start listening for messages.
                    serviceHost.Open();
            

                    // The service can now be accessed.
                    Console.WriteLine("The service is ready.");
                    Console.WriteLine("Press <ENTER> to terminate service.");
                    Console.ReadLine();

                    // Close the ServiceHost.
                    serviceHost.Close();
            
                }
                catch (TimeoutException timeProblem)
                {
                    Console.WriteLine(timeProblem.Message);
                    Console.ReadLine();
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine(commProblem.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}