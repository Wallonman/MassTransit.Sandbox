using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware.Consumers
{
    public class CircuitBreakerConsumer :
        IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {

            await Console.Out.WriteLineAsync($"CircuitBreakerConsumer received SubmitOrder: {JsonConvert.SerializeObject(context.Message)}");

            Thread.Sleep(100);

            if (context.Message.OrderAmount > 20 && context.Message.OrderAmount < 30)
                Thread.Sleep(2000);

        }
    }
}