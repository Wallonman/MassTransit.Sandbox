using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Audit
{
    public class ReceiveObserver : IReceiveObserver
    {
        public async Task PreReceive(ReceiveContext context)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ReceiveObserver.PreReceive context: {context}");
        }

        public async Task PostReceive(ReceiveContext context)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ReceiveObserver.PostReceive ");
        }

        public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ReceiveObserver.PostConsume");
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ReceiveObserver.ConsumeFault context");
        }

        public async Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ReceiveObserver.ReceiveFault context");
        }
    }
}