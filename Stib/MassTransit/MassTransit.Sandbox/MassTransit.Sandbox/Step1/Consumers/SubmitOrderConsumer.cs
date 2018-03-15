using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Contracts;

namespace MassTransit.Sandbox.Step1.Consumers
{
    public class SubmitOrderConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"Received SubmitOrder: {context.Message.OrderId}");

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