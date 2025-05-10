using Grpc.Core;
using RandomSource;


namespace RandomNumberGrpc.Services;

public sealed class RandomProviderService(ILogger<RandomProviderService> logger)
    : RandomProvider.RandomProviderBase, IDisposable
{
    private readonly IEnumerator<SequenceValue> _enumerator = new RandomSource.RandomProvider(32).GetEnumerator();


    public override async Task Stream(NextIntStreamRequest request, IServerStreamWriter<ValueWithSequence> responseStream, ServerCallContext context)
    {
        
        logger.LogDebug("Streaming starting at {enumerator}", _enumerator.Current);
        while (!context.CancellationToken.IsCancellationRequested && _enumerator.MoveNext())
        {
            await responseStream.WriteAsync(new ValueWithSequence
            {
                SequenceNumber = _enumerator.Current.Sequence,
                Value = _enumerator.Current.Value
            });
        }
    }

    public override Task<ValueWithSequence> NextInt(NextIntRequest request, ServerCallContext context)
    {
        _enumerator.MoveNext();
        return Task.FromResult(new ValueWithSequence
        {
            SequenceNumber = _enumerator.Current.Sequence,
            Value = _enumerator.Current.Value
        });
    }
    public void Dispose()
    {
        _enumerator.Dispose();
    }
}