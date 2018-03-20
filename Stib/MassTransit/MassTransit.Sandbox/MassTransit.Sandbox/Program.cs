namespace MassTransit.Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProducerConsumer.ProducerConsumer.Start();
        }
    }
}