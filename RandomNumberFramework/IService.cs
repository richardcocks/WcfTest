using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;

namespace RandomNumberFramework
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        CompositeType NextInt();
    }
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        [DataMember]
        public int Value {get; set; }
        [DataMember]
        public int SequenceNumber { get; set; }
    }
    public class Service : IService
    {
        private static int _sequenceNumber = 0;
    private readonly Random _random;
        public Service()
        {
            this._random = new Random();
        }
        
        public CompositeType NextInt()
        {
            var sequenceNo = Interlocked.Increment(ref _sequenceNumber);
            return new CompositeType{SequenceNumber = sequenceNo, Value = _random.Next()};
        }
    }
}
