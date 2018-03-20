using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.HandlingExceptions.Consumers
{
    public class FaultConsumer :
        IConsumer<Fault<ISubmitOrder>>
    {
        public async Task Consume(ConsumeContext<Fault<ISubmitOrder>> context)
        {
            await Console.Out.WriteLineAsync($"Received Fault: {context.Message.Exceptions}");

        }
    }
}