using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Scheduling
{
    /// <summary>
    /// Consumes a message from the notification_queue
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{MassTransit.Sandbox.Scheduling.ISendNotification}" />
    internal class NotificationConsumer : IConsumer<ISendNotification>
    {

        public async Task Consume(ConsumeContext<ISendNotification> context)
        {
            await Console.Out.WriteLineAsync($"NotificationConsumer> Received at {DateTime.Now} a SendNotification: {context.Message.EmailAddress}");

        }
    }
}