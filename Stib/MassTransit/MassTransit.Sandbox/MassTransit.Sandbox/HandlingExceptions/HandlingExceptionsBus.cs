using System;
using GreenPipes;
using MassTransit.Sandbox.HandlingExceptions.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.HandlingExceptions
{
    public static class HandlingExceptionsBus
    {

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
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue")) // Exchange : submit_order_queue => no binding
                                  .Result.Send<ISubmitOrder>(new { });
                        break;

                    case "2":
                        /*
                         * Creates :
                         * Exchange : submit_order_custom_error => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         * Queue : none
                         */
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"))
                                  .Result.Send<ISubmitOrder>(new { }
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
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue_retry")) // Exchange : submit_order_queue_retry => no binding
                                  .Result.Send<ISubmitOrder>(new { });
                        break;

                    case "4":
                        /*
                         * Creates :
                         * Exchange : submit_order_custom_error_ignored => no binding 
                         *            created at the moment this consumer is activated by an incoming message
                         *            and the exeption is raised
                         * Queue : none
                         */
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue_retry_ignored"))
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

                // general bus configration for retry Policy
                cfg.UseRetry(configurator => configurator.None());

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
                    e.Consumer<GenerateExceptionConsumer>(consumerConfig =>
                    {
                        consumerConfig.UseRetry(configurator =>
                        {
                            configurator.Interval(10, TimeSpan.FromMilliseconds(200));
                            configurator.Ignore<ArgumentException>();
                            // exception => exception.Message.Equals("Very bad things happened"));
                        });
                    });
                });


            });

            return bus;
        }
    }
}