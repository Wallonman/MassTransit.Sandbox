using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step1.Contracts;
using MassTransit.Sandbox.Step2.Consumers;

namespace MassTransit.Sandbox.Step2
{
    public static class HandlingExceptions
    {
        private static Task<ISendEndpoint> _sendEndpointTask;

        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Exception ");
                Console.WriteLine("'2' -> Exception to custom exchange");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        _sendEndpointTask.Result.Send<ISubmitOrder>(new { });
                        break;

                    case "2":
                        /*
                         * Creates :
                         * Exchange : submit_order_custom_error => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         * Queue : none
                         */
                        _sendEndpointTask.Result.Send<ISubmitOrder>(new { }
                        , context => context.FaultAddress = new Uri("rabbitmq://localhost/submit_order_custom_error"));
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
                    e.Consumer<GenerateExceptionConsumer>();
                });


                cfg.ReceiveEndpoint(host, "submit_order_fault", e =>
                {
                    e.Consumer<FaultConsumer>();
                });

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