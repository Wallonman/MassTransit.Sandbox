namespace MassTransit.Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProducerConsumer.ProducerConsumerBus.Start();
            //test branch
        }
    }
}