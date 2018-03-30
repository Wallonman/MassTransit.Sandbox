using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages.Consumers
{
    public class SubmitOrderCorrelatedConsumer :
        IConsumer<ISubmitOrderCorrelated>
    {
        private ILog _logger;

        public async Task Consume(ConsumeContext<ISubmitOrderCorrelated> context)
        {

            _logger = log4net.LogManager.GetLogger(typeof(SubmitOrderCorrelatedConsumer));

            _logger.Info($"Received SubmitOrder CorrelationId: " +
                        /*
                         * The CorrelationIs is set on the message
                         * and/or on the context (depending on the producer behavior)
                         */
                         $"{context.Message.CorrelationId} / " +
                         $"{context.CorrelationId}");

            _logger.Info($"SubmitOrder ConversationId: {context.ConversationId}");

            /*
             * Publish the next message 
             * Don't forget to relay the CorrelationId
             */
            await context.Publish<IOrderSubmittedWithoutCorrelatedBy>(new
            {
                OrderId = context.Message.OrderId,
                OrderDate = context.Message.OrderDate,
                /*
                  set the correlation id to the field by the 
                  MessageCorrelation.UseCorrelationId (defined in the definition of the bus)
                */
                SomeGuidValue = context.CorrelationId, 
            });
            // or you can also pass the correlationId by the context
            //            }, publishContext => publishContext.CorrelationId = context.CorrelationId);
        }
    }
}