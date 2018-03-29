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

            _logger.Info($"Received SubmitOrder CoorelationId: {context.Message.CorrelationId}");

            /*
             * Creates :
             * Exchange : MassTransit.Sandbox.Step1.Contracts:IOrderSubmitted => no binding
             *            created at the moment this consumer is activated by an incoming message
             * Queue : none
             */
            await context.Publish<IOrderCorrelatedSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                OrderDate = context.Message.OrderDate,
            });
        }
    }
}