namespace MassTransit.Sandbox.Scheduling
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduling.SchedulingBus.Start();
        }
    }
}
