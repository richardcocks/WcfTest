using System;
using System.IO;
using System.ServiceModel;

namespace RandomNumberFramework
{
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
}