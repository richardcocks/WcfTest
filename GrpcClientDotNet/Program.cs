using Grpc.Net.Client;
using System.Diagnostics;


namespace GrpcClientDotNet;

class Program
{
    static async Task Main(string[] args)
    {

// The port number must match the port of the gRPC server.

        await TestStreamingGrpcService("https://localhost:7176");
        await TestGrpcService("https://localhost:7176");
       
    }

    private static async Task TestStreamingGrpcService(string address)
    {
        Console.WriteLine($"Testing GRPC streaming at {address}");
        
        using var channel = GrpcChannel.ForAddress(address);
        var client =new  RandomProvider.RandomProviderClient(channel);
        
        var cts = new CancellationTokenSource(5_000);
        var sw = Stopwatch.StartNew();

        Dictionary<int, int> bag = new Dictionary<int, int>();

        var stream = client.Stream(new NextIntStreamRequest());
            
        int counter = 0;
        try
        {
            var valueStream = stream.ResponseStream;
            while (!cts.Token.IsCancellationRequested && await valueStream.MoveNext(cts.Token))
            {
                var counterNo = Interlocked.Increment(ref counter);
                bag.Add(counterNo, valueStream.Current.Value);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            sw.Stop();
            await channel.ShutdownAsync();
        }
        Console.WriteLine($"First:{bag[1]:N0}. Last:{bag[counter]:N0}");
        Console.WriteLine($"Counter:{counter:N0}. Successfully filled:{bag.Count:N0} in {sw.ElapsedMilliseconds}ms");
    }

    private static async Task TestGrpcService(string address)
    {
        Console.WriteLine($"Testing GRPC at {address}");
        
        using var channel = GrpcChannel.ForAddress(address);
        var client =new  RandomProvider.RandomProviderClient(channel);
        
        
        for (int i = 0; i < 1_000; i++)
        {
            client.NextInt(new NextIntRequest());
        }
           
        var cts = new CancellationTokenSource(5_000);
        var sw = Stopwatch.StartNew();

        Dictionary<int, int> bag = new Dictionary<int, int>();

        int counter = 0;
        try
        {
            var req = new NextIntRequest();
            while (!cts.Token.IsCancellationRequested)
            {
                var nextInt = client.NextInt(req);
                var next = nextInt.Value;
                var counterNo = Interlocked.Increment(ref counter);
                bag.Add(counterNo, next);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            sw.Stop();
        }
        Console.WriteLine($"First:{bag[1]:N0}. Last:{bag[counter]:N0}");
        Console.WriteLine($"Counter:{counter:N0}. Successfully filled:{bag.Count:N0} in {sw.ElapsedMilliseconds}ms");



        await channel.ShutdownAsync();
    }
    
}