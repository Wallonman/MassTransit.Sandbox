using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.RequestResponse.Contracts;

namespace MassTransit.Sandbox.RequestResponse.Consumers
{
    public class CheckOrderStatusConsumer : IConsumer<CheckOrderStatus>
    {
        public async Task Consume(ConsumeContext<CheckOrderStatus> context)
        {
            if (context.Message.OrderId == "666")
                throw new InvalidOperationException("Order not found");

            await context.RespondAsync<OrderStatusResult>(
                new
                {
                    OrderId = context.Message.OrderId,
                    Timestamp = DateTime.Now,
                    StatusCode = 1,
                    StatusText = "Sent",
                }
            );
        }
    }
}