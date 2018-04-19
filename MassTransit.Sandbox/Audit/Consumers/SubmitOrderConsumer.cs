using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Audit.Consumers
{
    public class SubmitOrderConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"SubmitOrderConsumer received SubmitOrder: {JsonConvert.SerializeObject(context.Message)}");

            if (context.Message.OrderId == "2")
                throw new Exception("Ooops!");
        }
    }
}