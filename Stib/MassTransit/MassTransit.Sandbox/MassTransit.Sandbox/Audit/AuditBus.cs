using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Audit;
using MassTransit.Sandbox.Audit.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.Audit
{
    public static class AuditBus
    {

        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue")) 
                            .Result.Send<ISubmitOrder>(new { });

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

            bus.ConnectSendAuditObservers(new AuditStore("SendAuditObserver"));
            bus.ConnectConsumeAuditObserver(new AuditStore("ConsumeAuditObserver"));
//            bus.ConnectConsumeAuditObserver(new AuditStore("ConsumeAuditObserver"), 
//                conf => conf.Include ... // todo : how to pass arg func<T, bool> ?

            return bus;
        }
    }
}