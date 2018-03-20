using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.ProducerConsumer.Consumers
{
    public class ShipOrderConsumer :
        IConsumer<IOrderSubmitted>
    {
        public async Task Consume(ConsumeContext<IOrderSubmitted> context)
        {
            await Console.Out.WriteLineAsync($"ShipOrderConsumer Received IOrderSubmitted: {context.Message.OrderId}");

            /*
             * Creates :
             * Exchange : shipped_order_queue => no binding
             * Queue : none
             */
            await context.GetSendEndpoint(new Uri("rabbitmq://localhost/shipped_order_queue")).Result.Send<IOrderShipped>(
                new
                {
                    OrderId = context.Message.OrderId,
                    OrderDate = context.Message.OrderDate,
                    ShippingDate = DateTime.Now,
                });

        }
    }
}