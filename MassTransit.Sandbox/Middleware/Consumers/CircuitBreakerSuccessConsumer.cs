using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware.Consumers
{
    public class CircuitBreakerSuccessConsumer :
        IConsumer<ISubmitOrder>
    {
        public long SuccessCount;

        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {

            Interlocked.Increment(ref SuccessCount);
            await Console.Out.WriteLineAsync($"{DateTime.Now:O}> CircuitBreakerSuccessConsumer received SubmitOrder / total received: {context.Message.OrderAmount} / {SuccessCount}");

        }
    }
}