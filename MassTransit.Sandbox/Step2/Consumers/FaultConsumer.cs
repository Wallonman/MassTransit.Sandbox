using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Contracts;

namespace MassTransit.Sandbox.Step2.Consumers
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