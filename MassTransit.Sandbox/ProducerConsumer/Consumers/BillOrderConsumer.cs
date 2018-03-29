using System;
using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.ProducerConsumer.Consumers
{
    public class BillOrderConsumer :
        IConsumer<IOrderSubmitted>
    {
        private ILog _logger;

        public async Task Consume(ConsumeContext<IOrderSubmitted> context)
        {
            _logger = log4net.LogManager.GetLogger(typeof(BillOrderConsumer));

            _logger.Info($"BillOrderConsumer Received IOrderSubmitted: {context.Message.OrderId}");
//            await Console.Out.WriteLineAsync($"BillOrderConsumer Received IOrderSubmitted: {context.Message.OrderId}");

            
        }
    }
}