using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RandomNumberFrameworkConsole
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        CompositeType NextInt();
    }
    
    [ServiceContract]
    public interface IStreamingService
    {
        [OperationContract]
        Stream GetRandomStream();
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
    
    public class RandomNumberStreamingService : IStreamingService
    {
        private readonly Random _random;

        public static void Configure(ServiceConfiguration config)
        {
            var contract = ContractDescription.GetContract(typeof(IStreamingService));
            ServiceEndpoint se = new ServiceEndpoint(contract, new NetTcpBinding(){TransferMode = TransferMode.Streamed, Security = new NetTcpSecurity(){Mode = SecurityMode.None}}, new EndpointAddress("net.tcp://localhost:808/Service/netTcp/streaming"));
            config.AddServiceEndpoint(se);

            config.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
        }
        
        public RandomNumberStreamingService()
        {
            this._random = new Random();
        }
        public Stream GetRandomStream()
        {
            return new RandomStream(this._random);
        }
    }

    

    public class RandomNumberService : IService
    {
        private readonly Random _random = new Random();
        private int _sequence;
        
        public static void Configure(ServiceConfiguration config)
        {
            var contract = ContractDescription.GetContract(typeof(IService));
            ServiceEndpoint se = new ServiceEndpoint(contract, new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:808/Service/netTcp"));
            config.AddServiceEndpoint(se);

            config.Description.Behaviors.Add(new ServiceMetadataBehavior { });
            config.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
        }

        public CompositeType NextInt()
        {
            return new CompositeType
            {
                SequenceNumber = _sequence++,
                Value = _random.Next()
            };
        }
    }
}