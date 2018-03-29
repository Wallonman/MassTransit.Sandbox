using System;
using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages.Consumers
{
    public class ShipOrderCorrelatedConsumer :
        IConsumer<IOrderCorrelatedSubmitted>
    {
        private ILog _logger;

        public async Task Consume(ConsumeContext<IOrderCorrelatedSubmitted> context)
        {
            _logger = log4net.LogManager.GetLogger(typeof(ShipOrderCorrelatedConsumer));

            _logger.Info($"ShipOrderConsumer Received IOrderSubmitted CorrelationId: {context.Message.CorrelationId}");


        }
    }
}