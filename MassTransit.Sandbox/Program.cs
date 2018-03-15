using MassTransit.Sandbox.Step1;

namespace MassTransit.Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProducerConsumer.Start();
        }
    }
}