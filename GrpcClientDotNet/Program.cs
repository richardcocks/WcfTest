using Grpc.Net.Client;
using System.Diagnostics;
using RandomNumberGrpc;

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
        var client =new  RandomNumberGrpc.RandomProvider.RandomProviderClient(channel);
        
        var cts = new CancellationTokenSource(5_000);
        var sw = Stopwatch.StartNew();

        HashSet<long> bag = [];

        using var stream = client.Stream(new NextIntStreamRequest());

        try
        {
            var valueStream = stream.ResponseStream;
            while (!cts.Token.IsCancellationRequested && await valueStream.MoveNext(cts.Token))
            {
                if (bag.Add(valueStream.Current.Value)) continue;
                sw.Stop();
                Console.WriteLine($"Found duplicate {valueStream.Current.SequenceNumber} - {valueStream.Current.Value} in {sw.ElapsedMilliseconds}ms!");
                break;
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
    }

    private static async Task TestGrpcService(string address)
    {
        Console.WriteLine($"Testing GRPC at {address}");
        
        using var channel = GrpcChannel.ForAddress(address);
        var client =new  RandomNumberGrpc.RandomProvider.RandomProviderClient(channel);
        
        var cts = new CancellationTokenSource(30_000);
        var sw = Stopwatch.StartNew();

        HashSet<long> bag = [];

        try
        {
            var req = new NextIntRequest();
            while (!cts.Token.IsCancellationRequested)
            {
                var next = await client.NextIntAsync(req);
                if (bag.Add(next.Value)) continue;
                
                sw.Stop();
                
                Console.WriteLine($"Found duplicate {next.SequenceNumber} - {next.Value} in {sw.ElapsedMilliseconds}ms!");
                Console.WriteLine($"Collected {bag.Count} at {(double)bag.Count / sw.ElapsedMilliseconds}/ms!");
                break;
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
    }
    
}