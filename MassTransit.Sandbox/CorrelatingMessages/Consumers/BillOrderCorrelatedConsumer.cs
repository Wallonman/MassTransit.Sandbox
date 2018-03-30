using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages.Consumers
{
    public class BillOrderCorrelatedConsumer :
        IConsumer<IOrderSubmittedWithoutCorrelatedBy>
    {
        private readonly ILog _logger = log4net.LogManager.GetLogger(typeof(BillOrderCorrelatedConsumer));

        public Task Consume(ConsumeContext<IOrderSubmittedWithoutCorrelatedBy> context)
        {
            _logger.Info($"BillOrderConsumer Received IOrderSubmitted CorrelationId: " +
                        /*
                          It isn't possible to get the CorrelationId from the message : 
                                $"{context.Message.CorrelationId} "
                          does not compile

                         But it is always possible to get it from the context 
                         (if it has be set by the message producer)
                        */
                         $"{context.CorrelationId}");

            _logger.Info($"IOrderSubmitted ConversationId: {context.ConversationId}");

            return Task.CompletedTask;
        }
    }
}