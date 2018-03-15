using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Contracts;

namespace MassTransit.Sandbox.Step2.Consumers
{
    public class GenerateExceptionConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"Received SubmitOrder: {context.Message.OrderId}");

            /*
             * Creates :
             * Exchange : submit_order_queue_error => binding to queue submit_order_queue_error
             *            created at the moment this consumer is activated by an incoming message
             *            and the exeption is raised
             * Queue : submit_order_queue_error
             */
            throw new Exception("Very bad things happened");
        }
    }
}