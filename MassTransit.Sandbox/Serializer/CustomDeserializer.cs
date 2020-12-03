using System.Net.Mime;
using GreenPipes;

namespace MassTransit.Sandbox.Serializer
{
    public class CustomDeserializer : IMessageDeserializer
    {
        public void Probe(ProbeContext context)
        {
            // throw new System.NotImplementedException();
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            throw new System.NotImplementedException();
        }

        public ContentType ContentType => CustomSerializer.CustomContentType;
    }
}