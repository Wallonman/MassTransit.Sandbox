using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Consumer
{
    public class SubmitOrderConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"Received SubmitOrder: {context.Message.OrderId}");

            await context.Publish<IOrderSubmitted>(new
            {
                OrderId = context.Message.OrderId,
                OrderDate = context.Message.OrderDate,
            });
        }
    }
}