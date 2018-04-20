using System.Linq;
using MassTransit.Audit;

namespace MassTransit.Sandbox.Audit
{
    public class MyConsumeMetadataFactory : IConsumeMetadataFactory
    {
        private readonly string _contextType;

        public MyConsumeMetadataFactory(string contextType)
        {
            _contextType = contextType;
        }

        public MessageAuditMetadata CreateAuditMetadata<T>(ConsumeContext<T> context) where T : class
        {
            return new MessageAuditMetadata
            {
                ContextType = _contextType,
                ConversationId = context.ConversationId,
                CorrelationId = context.CorrelationId,
                InitiatorId = context.InitiatorId,
                MessageId = context.MessageId,
                RequestId = context.RequestId,
                DestinationAddress = context.DestinationAddress?.AbsoluteUri,
                SourceAddress = context.SourceAddress?.AbsoluteUri,
                FaultAddress = context.FaultAddress?.AbsoluteUri,
                ResponseAddress = context.ResponseAddress?.AbsoluteUri,
                Headers = context.Headers?.GetAll()?.ToDictionary(k => k.Key, v => v.Value.ToString())
            };
        }
    }
}