namespace RandomNumberCore;


[ServiceContract]
public interface IStreamingService
{
    [OperationContract]
    Stream GetRandomStream();
}
    

public class StreamingService : IStreamingService
{
    public Stream GetRandomStream()
    {
        return new RandomStream(Random.Shared);
    }
}