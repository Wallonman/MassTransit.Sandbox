using System;
using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages.Consumers
{
    public class ShipOrderCorrelatedConsumer :
        IConsumer<IOrderSubmittedWithoutCorrelatedBy>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ShipOrderCorrelatedConsumer));

        public Task Consume(ConsumeContext<IOrderSubmittedWithoutCorrelatedBy> context)
        {

            /*
             * NO CorrelationId in the message, yet well in the context
             */
            _logger.Info($"ShipOrderConsumer Received IOrderSubmitted CorrelationId: {context.CorrelationId}");
            /*
             * The conversationId is lways present
             */
            _logger.Info($"IOrderSubmitted ConversationId: {context.ConversationId}");

            return Task.CompletedTask;

        }
    }
}