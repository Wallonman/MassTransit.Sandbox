using System;
using GreenPipes;
using log4net.Config;
using MassTransit.Log4NetIntegration;
using MassTransit.Sandbox.Middleware.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware
{
    public static class MiddlewareBus
    {
        public static void Start()
        {
            // load the Log4Net config from app.config
            XmlConfigurator.Configure();

            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Circuit breaker");
                Console.WriteLine("'4' -> Custom");
                Console.WriteLine("'44' -> Custom with exception");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        for (int i = 0; i <= 100; i++)
                            busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/middleware_circuit_breaker_queue"))
                                .Result
                                .Send<ISubmitOrder>(new { OrderAmount = i}); //new Random().Next(0, 10) });
                        break;
                    case "4":
                    case "44":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/middleware_custom_queue"))
                            .Result
                            .Send<ISubmitOrder>(new { OrderId = value }); // the value "44" will throw an exception in the consumer
                        break;

                }

            } while (true);
            busControl.Stop();
        }

        private static IBusControl ConfigureBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseLog4Net();

                /*
                 * Register the message consumer and the middleware custom filter
                 */
                cfg.ReceiveEndpoint(host, "middleware_circuit_breaker_queue", e =>
                {
                    e.Consumer<CircuitBreakerConsumer>();
                    e.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMilliseconds(100);
                        cb.TripThreshold = 90;
                        cb.ActiveThreshold = 1;
                        cb.ResetInterval = TimeSpan.FromSeconds(10);
                    });
                });

                /*
                 * Register the message consumer and the middleware custom filter
                 */
                cfg.ReceiveEndpoint(host, "middleware_custom_queue", e => { e.Consumer<CustomMiddlewareConsumer>(); });
                cfg.UseExceptionLogger();

            });

           

            return bus;
        }
    }
}