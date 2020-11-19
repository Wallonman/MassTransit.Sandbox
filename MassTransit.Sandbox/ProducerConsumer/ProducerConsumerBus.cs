using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration;
using MassTransit.Sandbox.ProducerConsumer.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.ProducerConsumer
{
    public static class ProducerConsumerBus
    {
        private static Task<ISendEndpoint> _sendEndpointTask;
        private static ILog _logger;

        public static void Start()
        {
            // load the Log4Net config from app.config
            XmlConfigurator.Configure();

            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("Enter message (or 'q' to exit)");
                Console.Write("> ");
                var value = Console.ReadLine();
                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                _logger = log4net.LogManager.GetLogger(typeof(ProducerConsumerBus));

                _logger.Info("Sending ISubmitOrder message");
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
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // request the log to Log4Net
                cfg.UseLog4Net();

                cfg.ReceiveEndpoint("submit_order_queue", e =>
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
                cfg.ReceiveEndpoint("ship_order_queue", e => { e.Consumer<ShipOrderConsumer>(); });
                /*
                 * Creates :
                 * Exchange : bill_order_queue => queue bill_order_queue
                 * Exchange : MassTransit.Sandbox.Consumer:IOrderSubmitted => exchange bill_order_queue
                 * Queue : bill_order_queue
                 */
                cfg.ReceiveEndpoint("bill_order_queue", e => { e.Consumer<BillOrderConsumer>(); });
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