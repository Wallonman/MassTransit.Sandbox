using MassTransit.Sandbox.Middleware;

namespace MassTransit.Sandbox.Audit
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MiddlewareBus.Start();
        }
    }
}