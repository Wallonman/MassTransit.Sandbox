using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware.Consumers
{
    public class CustomMiddlewareConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {

            await Console.Out.WriteLineAsync($"CustomMiddlewareConsumer received SubmitOrder: {JsonConvert.SerializeObject(context.Message)}");

            if (context.Message.OrderId == "44")
                throw new Exception("Ooops!");

        }
    }
}