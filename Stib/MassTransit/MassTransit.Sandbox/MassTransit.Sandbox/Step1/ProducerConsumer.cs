using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Consumers;
using MassTransit.Sandbox.Step1.Contracts;

namespace MassTransit.Sandbox.Step1
{
    public static class ProducerConsumer
    {
        private static Task<ISendEndpoint> _sendEndpointTask;

        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("Enter message (or 'q' to exit)");
                Console.Write("> ");
                var value = Console.ReadLine();
                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                _sendEndpointTask.Result.Send<ISubmitOrder>(new
                {
                    OrderId = value,
                    OrderDate = DateTime.Today,
                    OrderAmount = 1
                });
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
                /*
                 * Creates :
                 * Exchange : ship_order_queue => queue ship_order_queue
                 * Exchange : MassTransit.Sandbox.Consumer:IOrderSubmitted => exchange ship_order_queue
                 * Queue : ship_order_queue
                 */
                cfg.ReceiveEndpoint(host, "ship_order_queue", e => { e.Consumer<ShipOrderConsumer>(); });
                /*
                 * Creates :
                 * Exchange : bill_order_queue => queue bill_order_queue
                 * Exchange : MassTransit.Sandbox.Consumer:IOrderSubmitted => exchange bill_order_queue
                 * Queue : bill_order_queue
                 */
                cfg.ReceiveEndpoint(host, "bill_order_queue", e => { e.Consumer<BillOrderConsumer>(); });
            });


            /*
             * Creates :
             * Exchange : submit_order_queue => no binding
             * Queue : none
             */
            _sendEndpointTask = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"));


            return bus;
        }
    }
}