using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Scheduling
{
    internal class NotificationConsumer : IConsumer<ISendNotification>
    {

        public async Task Consume(ConsumeContext<ISendNotification> context)
        {
            await Console.Out.WriteLineAsync($"Received at {DateTime.Now} a SendNotification: {context.Message.EmailAddress}");

        }
    }
}