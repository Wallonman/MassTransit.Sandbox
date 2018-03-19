using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Sandbox.Step1.Contracts;
using MassTransit.Sandbox.Step2.Consumers;

namespace MassTransit.Sandbox.Step2
{
    public static class HandlingExceptions
    {
        private static Task<ISendEndpoint> _sendEndpointTask;
        private static Task<ISendEndpoint> _sendEndpointTaskWithRetry;
        private static Task<ISendEndpoint> _sendEndpointTaskIgnoreRetry;

        public static void Start()
        {
            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Exception ");
                Console.WriteLine("'2' -> Exception to custom exchange");
                Console.WriteLine("'3' -> Exception with retries");
                Console.WriteLine("'4' -> Exception ignoring retry policy");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        /*
                         * Creates :
                         * Exchange : MassTransit:Fault => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         *            The exchange MassTransit:Fault--MassTransit.Sandbox.Step1.Contracts:ISubmitOrder is bind to MassTransit:Fault
                         * Queue : submit_order_queue_error
                         *         no binding, the message is moved by the middleware to this queue when the exception is raised
                         */
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

                    case "3":
                        /*
                         * Creates :
                         * Exchange : submit_order_custom_error => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         * Queue : none
                         */
                        _sendEndpointTaskWithRetry.Result.Send<ISubmitOrder>(new { });
                        break;

                    case "4":
                        /*
                         * Creates :
                         * Exchange : submit_order_custom_error_ignored => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         * Queue : none
                         */
                        _sendEndpointTaskIgnoreRetry.Result.Send<ISubmitOrder>(new { });
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

                // general bus configration for retry Policy
                cfg.UseRetry(configurator => configurator.Immediate(1));

                cfg.ReceiveEndpoint(host, "submit_order_queue", e =>
                {
                    e.Consumer<GenerateExceptionConsumer>();
                });

                /*
                 * Creates :
                 * Exchange : submit_order_fault => binds to queue submit_order_fault 
                 *            MassTransit:Fault--MassTransit.Sandbox.Step1.Contracts:ISubmitOrder => submit_order_fault
                 * Queue : submit_order_fault
                 */
                cfg.ReceiveEndpoint(host, "submit_order_fault", e =>
                {
                    e.Consumer<FaultConsumer>();
                });

                cfg.ReceiveEndpoint(host, "submit_order_queue_retry", e =>
                {
                    // override the general retry policy
                    e.UseRetry(configurator => configurator.Incremental(3, new TimeSpan(0,0,1), new TimeSpan(0, 0, 1)));
                    e.Consumer<GenerateExceptionConsumer>();
                });

                cfg.ReceiveEndpoint(host, "submit_order_queue_retry_ignored", e =>
                {
                    // override the general retry policy
                    e.UseRetry(configurator =>
                    {
                        configurator.Immediate(5);
                        configurator.Ignore<Exception>(
                            exception => exception.Message.Equals("Very bad things happened"));
                    });
                    e.Consumer<GenerateExceptionConsumer>();
                });


            });


            /*
             * Creates :
             * Exchange : submit_order_queue => no binding
             * Exchange : submit_order_queue_retry => no binding
             * Queue : none
             */
            _sendEndpointTask = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"));
            _sendEndpointTaskWithRetry = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue_retry"));
            _sendEndpointTaskIgnoreRetry = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue_retry_ignored"));


            return bus;
        }
    }
}