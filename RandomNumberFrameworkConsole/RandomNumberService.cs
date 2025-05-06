using System;
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

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        [DataMember]
        public int Value {get; set; }
        [DataMember]
        public int SequenceNumber { get; set; }
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