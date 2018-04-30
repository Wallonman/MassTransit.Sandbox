using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware.Consumers
{
    public class RateLimitConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {

            await Console.Out.WriteLineAsync($"RateLimitConsumer received SubmitOrder: {JsonConvert.SerializeObject(context.Message)}");

//            Thread.Sleep(200);
        }
    }
}