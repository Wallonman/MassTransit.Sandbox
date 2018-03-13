using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Consumer
{
    public class ShipOrderConsumer :
        IConsumer<IOrderSubmitted>
    {
        public async Task Consume(ConsumeContext<IOrderSubmitted> context)
        {
            await Console.Out.WriteLineAsync($"ShipOrderConsumer Received IOrderSubmitted: {context.Message.OrderId}");

            await context.GetSendEndpoint(new Uri("rabbitmq://localhost/ship_order")).Result.Send<IOrderShipped>(
                new
                {
                    OrderId = context.Message.OrderId,
                    OrderDate = context.Message.OrderDate,
                    ShippingDate = DateTime.Now,
                });

        }
    }
}