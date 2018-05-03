using System;
using System.Diagnostics;
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
                Console.WriteLine("'2' -> Rate limit");
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
                                .Send<ISubmitOrder>(new { OrderAmount = i});
                        break;
                    case "2":
                        Console.Out.WriteLineAsync($"{DateTime.Now:O}> Start processing at ");
                        for (int i = 0; i <= 10; i++)
                            busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/middleware_rate_limit_queue"))
                                .Result
                                .Send<ISubmitOrder>(new { OrderAmount = i});
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
                 * Register the message consumer and the middleware Circuit Breaker
                 */
                cfg.ReceiveEndpoint(host, "middleware_circuit_breaker_queue", e =>
                {
                    e.Consumer<CircuitBreakerConsumer>();
                    e.UseCircuitBreaker(cb =>
                    {
                        // 100 message are going to be pushed, during the consume process
                        // some message processes will raise exceptions
                        // the consume process contains sleep() instruction to allow the
                        // circuit breaker to do its job ! thisis simulation after all, no ?
                        cb.TrackingPeriod = TimeSpan.FromSeconds(10);
                        cb.TripThreshold = 10;
                        cb.ActiveThreshold = 1;
                        cb.ResetInterval = TimeSpan.FromSeconds(2);
                    });
                });

                /*
                 * Register the message consumer and the middleware rate limit
                 */
                cfg.ReceiveEndpoint(host, "middleware_rate_limit_queue", e =>
                {
                    e.Consumer<RateLimitConsumer>();
                    e.UseRateLimit(1, TimeSpan.FromSeconds(1));
                });

                /*
                 * Register the message consumer and the middleware custom filter
                 */
                cfg.ReceiveEndpoint(host, "middleware_custom_queue", e => { e.Consumer<CustomMiddlewareConsumer>(); });

//                cfg.UseExceptionLogger();

            });

           

            return bus;
        }
    }
}