namespace MassTransit.Sandbox.Audit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AuditBus.Start();
        }
    }
}