using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit.Sandbox.Consumer;

namespace MassTransit.Sandbox
{

    class Program
    {
        private static Task<ISendEndpoint> _sendEndpointTask;

        static void Main(string[] args)
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("Enter message (or 'q' to exit)");
                Console.Write("> ");
                string value = Console.ReadLine();
                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                _sendEndpointTask.Result.Send<ISubmitOrder>(new
                {
                    OrderId = value,
                    OrderDate = DateTime.Today,
                    OrderAmount = 1
                });
            }
            while (true);
            busControl.Stop();
        }

        static IBusControl ConfigureBus()
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
                cfg.ReceiveEndpoint(host, "ship_order_queue", e =>
                {
                    e.Consumer<ShipOrderConsumer>();
                });
                cfg.ReceiveEndpoint(host, "bill_order_queue", e =>
                {
                    e.Consumer<BillOrderConsumer>();
                });
            });


            /*
             * Un exchange est créé : submit_order_queue
             * La queue n'est pas créée !
             */
            _sendEndpointTask = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order"));
            

            return bus;
        }

    }
}
