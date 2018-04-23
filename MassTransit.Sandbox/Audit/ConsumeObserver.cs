using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.Audit
{
    public class ConsumeObserver : IConsumeObserver
    {
        public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            var contextHeaders = context.Headers; // is empty, how to add items in the header ?
            var contextSupportedMessageTypes = context.SupportedMessageTypes;

            var order = context.Message as ISubmitOrder;
            if (order != null)
            {
                // if you want to get access to the message properties
                // not sure that's a good idea ... 
                order.OrderAmount = 1;
            }

            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ConsumeObserver.PreConsume message {context.MessageId}");
        }

        public async Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ConsumeObserver.PostConsume");
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ConsumeObserver.ConsumeFault");
        }
    }
}