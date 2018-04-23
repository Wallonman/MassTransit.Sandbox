using System;
using MassTransit.Sandbox.Middleware.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Middleware
{
    public static class MiddlewareBus
    {
        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Send");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                    case "2":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"))
                            .Result
                            .Send<ISubmitOrder>(new { OrderId = value }); // the value "2" will throw an exception in the consumer
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

                /*
                 * Register the message consumer
                 */
                cfg.ReceiveEndpoint(host, "submit_order_queue", e => { e.Consumer<SubmitOrderConsumer>(); });

                /*
                 * Declare the use of the middleware custom filter
                 */
                 cfg.UseExceptionLogger();

            });

           

            return bus;
        }
    }
}