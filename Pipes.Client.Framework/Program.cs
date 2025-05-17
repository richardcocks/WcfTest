using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using RandomSource;

namespace Pipes.Client.Framework
{
    internal class Program
    {
        static async Task Main()
        {
       
            var pipeClient = new NamedPipeClientStream(".", "net.pipe://randomtest", PipeDirection.InOut, PipeOptions.Asynchronous);

            using (var cts = new CancellationTokenSource(10_000))
            {
                await pipeClient.ConnectAsync(30, cts.Token);

                var buffer = new byte[4];
                var arrayPool = ArrayPool<byte>.Create();
                var bag = new HashSet<long>();

                var sw = Stopwatch.StartNew();
                while (!cts.IsCancellationRequested)
                {
                    _ = await pipeClient.ReadAsync(buffer, 0, 4, cts.Token);
                    var length = BitConverter.ToInt32(buffer, 0);
                    var streamBuffer = ArrayPool<byte>.Shared.Rent(length);
                    
                    _ = await pipeClient.ReadAsync(streamBuffer,0,length, cts.Token);
                    var sequenceVals =
                        MessagePack.MessagePackSerializer.Deserialize<IEnumerable<SequenceValue>>(streamBuffer,
                            MessagePack.Resolvers.ContractlessStandardResolver.Options);

                    foreach (var element in sequenceVals)
                    {
                        //Console.WriteLine($"Adding {element.Sequence} - {element.Value}");
                        if (bag.Add(element.Value)) continue;
                        sw.Stop();
                        Console.WriteLine(
                            $"Found duplicate {element.Value} at {element.Sequence} in {sw.ElapsedMilliseconds}ms");
                        cts.Cancel();
                        return;
                    }
                }
            }
        }
    }
}