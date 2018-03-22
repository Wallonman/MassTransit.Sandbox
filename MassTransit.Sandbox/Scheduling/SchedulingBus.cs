using System;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.Scheduling
{
    public static class SchedulingBus
    {

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

                        /*var schedulerEndpoint = bus.GetSendEndpoint(new Uri("rabbitmq://localhost/submit_order_queue"));
                         schedulerEndpoint.ScheduleSend(_notificationService,
                            context.Message.DeliveryTime,
                            new SendNotification
                            {
                                EmailAddress = context.Message.EmailAddress,
                                Body = context.Message.Body
                            });*/
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

                cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));

                cfg.ReceiveEndpoint(host, "submit_order_queue", e =>
                {
//                    e.Consumer<SubmitOrderConsumer>();
                });

            });

            return bus;
        }
    }
}