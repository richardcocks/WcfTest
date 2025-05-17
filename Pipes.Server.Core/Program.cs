using System.IO.Pipes;
using RandomSource;

namespace Pipes.Server.Core;

class Program
{
    static async Task Main(string[] args)
    {
        await using var pipeServer = new NamedPipeServerStream("net.pipe://randomtest", PipeDirection.InOut, -1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        
        await pipeServer.WaitForConnectionAsync();

        var random = new RandomProvider(new Random(0), 32);
        using var cts = new CancellationTokenSource(30_000);

        
        while (!cts.IsCancellationRequested && pipeServer.IsConnected)
        {
            var buffer = random.Take(1_000);

            var byteBuffer = MessagePack.MessagePackSerializer.Serialize(buffer, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            var lengthBytes = BitConverter.GetBytes(byteBuffer.Length);
            
            await pipeServer.WriteAsync(lengthBytes, cts.Token);
            await pipeServer.WriteAsync(byteBuffer, cts.Token);
        }
    }
}