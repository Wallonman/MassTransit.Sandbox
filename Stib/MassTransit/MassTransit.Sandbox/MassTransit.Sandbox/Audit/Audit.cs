using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Audit;
using MassTransit.Sandbox.ProducerConsumer.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.Audit
{
    public static class Audit
    {
        private static IMessageAuditStore _auditStore;

        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Send ");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue")) 
                                  .Result.Send<ISubmitOrder>(new { });
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


                cfg.ReceiveEndpoint(host, "submit_order_queue", e =>
                {
                    e.Consumer<SubmitOrderConsumer>();
                });

            });

            _auditStore = new AuditStore();
            bus.ConnectSendAuditObservers(_auditStore);

            return bus;
        }
    }
}