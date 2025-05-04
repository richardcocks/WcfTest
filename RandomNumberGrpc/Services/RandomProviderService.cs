using Grpc.Core;

namespace RandomNumberGrpc.Services;

public class RandomProviderService : RandomProvider.RandomProviderBase
{
    private readonly ILogger<RandomProviderService> _logger;

    private int _sequence;
    
    public RandomProviderService(ILogger<RandomProviderService> logger)
    {
        _logger = logger;
    }

    public override async Task Stream(NextIntStreamRequest request, IServerStreamWriter<ValueWithSequence> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var sequence = Interlocked.Increment(ref _sequence);
            
            await responseStream.WriteAsync(new ValueWithSequence
            {
                SequenceNumber = sequence,
                Value = Random.Shared.Next()
            });    
        }
    }

    public override Task<ValueWithSequence> NextInt(NextIntRequest request, ServerCallContext context)
    {
        var sequence = Interlocked.Increment(ref _sequence);
        return Task.FromResult(new ValueWithSequence
        {
            SequenceNumber = sequence,
            Value = Random.Shared.Next()
        });
    }
}