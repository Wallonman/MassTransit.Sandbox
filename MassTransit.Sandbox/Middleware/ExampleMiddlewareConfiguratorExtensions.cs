using GreenPipes;

namespace MassTransit.Sandbox.Middleware
{
    public static class ExampleMiddlewareConfiguratorExtensions
    {
        public static void UseExceptionLogger<T>(this IPipeConfigurator<T> configurator)
            where T : class, PipeContext
        {
            configurator.AddPipeSpecification(new ExceptionLoggerSpecification<T>());
        }
    }
}