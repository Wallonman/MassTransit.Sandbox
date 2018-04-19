using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Audit;
using MassTransit.Sandbox.Audit.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

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
                Console.WriteLine("'1' -> Observing received and consumed messages ");
                Console.WriteLine("'2' -> Observing a thrown Exception");
                Console.WriteLine("'3' -> Observing specific consumed messages");
                Console.WriteLine("'4' -> Displaying configuration");
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
                    case "3":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"))
                            .Result
                            .Send(new SubmitOrder { OrderId = Guid.NewGuid().ToString() });
                        break;
                    case "4":
                        var probeResult = busControl.GetProbeResult();
                        Console.Write(JsonConvert.SerializeObject(probeResult));
                        break;
                }

                /*if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                if ("3".Equals(value, StringComparison.OrdinalIgnoreCase))
                    busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"))
                        .Result
                        .Send(new SubmitOrder {OrderId = Guid.NewGuid().ToString()}); 
                else
                    if ("3".Equals(value, StringComparison.OrdinalIgnoreCase))
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"))
                            .Result
                            .Send<ISubmitOrder>(new {OrderId = value}); // the value "2" will throw an exception in the consumer
                else
                    if ("4".Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        var probeResult = busControl.GetProbeResult();
                        Console.Write(JsonConvert.SerializeObject(probeResult));
                    }*/
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
            });

            /*
             * Connect the Audit observer
             */
            bus.ConnectSendAuditObservers(new AuditStore("SendAuditObserver"));
            bus.ConnectConsumeAuditObserver(new AuditStore("ConsumeAuditObserver"));

            /*
             * Connect the Receive observer
             */
            bus.ConnectReceiveObserver(new ReceiveObserver());

            /*
             * Connect the Consume observer
             */
            bus.ConnectConsumeObserver(new ConsumeObserver());

            bus.ConnectConsumeMessageObserver(new SubmitOrderConsumeObserver());

            return bus;
        }
    }
}