using System;
using System.Threading.Tasks;
using log4net;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.ProducerConsumer.Consumers
{
    public class SubmitOrderConsumer :
        IConsumer<ISubmitOrder>
    {
        private ILog _logger;

        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {

            _logger = log4net.LogManager.GetLogger(typeof(SubmitOrderConsumer));

            _logger.Info($"Received SubmitOrder: {context.Message.OrderId}");

//            await Console.Out.WriteLineAsync($"Received SubmitOrder: {context.Message.OrderId}");

            /*
             * Creates :
             * Exchange : MassTransit.Sandbox.Step1.Contracts:IOrderSubmitted => no binding
             *            created at the moment this consumer is activated by an incoming message
             * Queue : none
             */
            await context.Publish<IOrderSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                OrderDate = context.Message.OrderDate,
            });
        }
    }
}