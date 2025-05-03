using CoreWCF;
using System;
using System.Runtime.Serialization;

namespace RandomNumberCore
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
        
        public CompositeType NextInt()
        {
            var sequenceNo = Interlocked.Increment(ref _sequenceNumber);
            return new CompositeType{SequenceNumber = sequenceNo, Value = Random.Shared.Next()};
        }
    }
}
