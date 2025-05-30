﻿using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace RandomNumberCore
{
    public interface ITestServiceChannel : IService, IClientChannel
    {
        
    }
    
    public interface IStreamingServiceChannel : IStreamingService, IClientChannel
    {
        
    }
    
    
    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract]
        Stream GetRandomStream();
    }

    
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
}