using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Consumer
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