using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages.Consumers
{
    public class BillOrderCorrelatedConsumer :
        IConsumer<IOrderCorrelatedSubmitted>
    {
        private ILog _logger;

        public async Task Consume(ConsumeContext<IOrderCorrelatedSubmitted> context)
        {
            _logger = log4net.LogManager.GetLogger(typeof(BillOrderCorrelatedConsumer));

            _logger.Info($"BillOrderConsumer Received IOrderSubmitted CorrelationId: {context.Message.CorrelationId}");

            
        }
    }
}