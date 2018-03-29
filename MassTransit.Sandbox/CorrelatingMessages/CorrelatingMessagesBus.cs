using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration;
using MassTransit.Sandbox.CorrelatingMessages.Consumers;
using MassTransit.Sandbox.CorrelatingMessages.Contracts;

namespace MassTransit.Sandbox.CorrelatingMessages
{
    public static class CorrelatingMessagesBus
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

                _logger = log4net.LogManager.GetLogger(typeof(CorrelatingMessagesBus));

                _logger.Info("Sending ISubmitOrderCorrelated message");
                _sendEndpointTask.Result.Send<SubmitOrderCorrelated>(new
                {
                    OrderId = value,
                    OrderDate = DateTime.Today,
                    OrderAmount = 1,
                    
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

                // request the log to Log4Net
                cfg.UseLog4Net();

                cfg.ReceiveEndpoint(host, "submit_order_correlated_queue", e =>
                {
                    e.Consumer<SubmitOrderCorrelatedConsumer>();
                });
 
                cfg.ReceiveEndpoint(host, "ship_order_correlated_queue", e =>
                {
                    e.Consumer<ShipOrderCorrelatedConsumer>();
                });
 
                cfg.ReceiveEndpoint(host, "bill_order_correlated_queue", e =>
                {
                    e.Consumer<BillOrderCorrelatedConsumer>();
                });
 
            });

            _sendEndpointTask = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_correlated_queue"));

            return bus;
        }
    }
}