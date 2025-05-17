using System.Buffers;
using System.Diagnostics;
using System.IO.Pipes;
using RandomSource;

namespace Pipes.Client.Core;

public static class Program
{
	static async Task Main()
    {
       
        var pipeClient = new NamedPipeClientStream(".", "net.pipe://randomtest", PipeDirection.InOut, PipeOptions.Asynchronous);
	
        using var cts = new CancellationTokenSource(10_000);
	
        await pipeClient.ConnectAsync(30, cts.Token);
	
        var buffer = new byte[4];
        var arrayPool = ArrayPool<byte>.Create();
        var bag = new HashSet<long>();
		
        var sw = Stopwatch.StartNew();
        while(!cts.IsCancellationRequested){
            await pipeClient.ReadExactlyAsync(buffer, cts.Token);
            var length = BitConverter.ToInt32(buffer);
            var streamBuffer = ArrayPool<byte>.Shared.Rent(length);
		
            // We can't use ReadExactlyAsync because streamBuffer might be the wrong length
            _ = await pipeClient.ReadAsync(streamBuffer, cts.Token);
            var sequenceVals = MessagePack.MessagePackSerializer.Deserialize<IEnumerable<SequenceValue>>(streamBuffer,MessagePack.Resolvers.ContractlessStandardResolver.Options);	
		
            foreach (var element in sequenceVals)
            {
	            //Console.WriteLine($"Adding {element.Sequence} - {element.Value}");
	            if (bag.Add(element.Value)) continue;
	            sw.Stop();
                Console.WriteLine($"Found duplicate {element.Value} at {element.Sequence} in {sw.ElapsedMilliseconds}ms");
                await cts.CancelAsync();
                return;
            }
		}
	}
}