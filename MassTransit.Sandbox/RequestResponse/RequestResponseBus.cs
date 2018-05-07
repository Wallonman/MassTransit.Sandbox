using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration;
using MassTransit.Sandbox.ProducerConsumer;
using MassTransit.Sandbox.ProducerConsumer.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.RequestResponse
{
    public static class RequestResponseBus
    {
        private static ILog _logger;

        public static void Start()
        {
            // load the Log4Net config from app.config
            XmlConfigurator.Configure();

            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> ");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
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
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // request the log to Log4Net
                cfg.UseLog4Net();

                cfg.ReceiveEndpoint(host, "submit_order_queue", e =>
                {
                    /*
                     * Creates :
                     * Exchange : submit_order_queue => queue submit_order_queue
                     * Exchange : MassTransit.Sandbox.Step1.Contracts:ISubmitOrder => exchange submit_order_queue
                     * Queue : submit_order_queue
                     */
                    e.Consumer<SubmitOrderConsumer>();
                });
            });

            return bus;
        }
    }
}