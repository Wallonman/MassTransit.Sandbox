using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Contracts;

namespace MassTransit.Sandbox.Step1.Consumers
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