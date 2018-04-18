using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.Audit
{
    public class ConsumeObserver : IConsumeObserver
    {
        public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ConsumeObserver.PreConsume");
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