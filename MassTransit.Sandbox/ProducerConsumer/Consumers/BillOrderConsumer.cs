using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.ProducerConsumer.Consumers
{
    public class BillOrderConsumer :
        IConsumer<IOrderSubmitted>
    {
        public async Task Consume(ConsumeContext<IOrderSubmitted> context)
        {
            await Console.Out.WriteLineAsync($"BillOrderConsumer Received IOrderSubmitted: {context.Message.OrderId}");

            
        }
    }
}