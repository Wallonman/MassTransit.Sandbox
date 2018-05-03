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

            await Console.Out.WriteLineAsync($"{DateTime.Now:O}> CircuitBreakerConsumer received SubmitOrder: {context.Message.OrderAmount}");


            if (context.Message.OrderAmount > 20 && context.Message.OrderAmount < 40)
            {
                Thread.Sleep(1000);
                throw new TimeoutException("Chao monkey is there!");
            }

            Thread.Sleep(200);
            context.Message.OrderAmount *= 10;
            await context.GetSendEndpoint(new Uri("rabbitmq://localhost/middleware_circuit_breaker_queue_success")).Result.Send(context.Message);
        }
    }
}