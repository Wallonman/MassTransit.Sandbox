using MassTransit.Sandbox.HandlingExceptions;

namespace MassTransit.Sandbox.ErrorHandling
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HandlingExceptionsBus.Start();
        }
    }
}