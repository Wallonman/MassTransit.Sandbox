using System;

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
                Console.WriteLine("'1' -> Send Notification");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/schedule_notification_queue")) 
                                  .Result.Send<IScheduleNotification>(new { DeliveryTime = DateTime.Now.AddSeconds(5), EmailAddress = "test@yopmail.com", Body = "Hello World!" });

/*
                         schedulerEndpoint.ScheduleSend(_notificationService,
                            context.Message.DeliveryTime,
                            new SendNotification
                            {
                                EmailAddress = context.Message.EmailAddress,
                                Body = context.Message.Body
                            });
*/
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

                /*
                 * Creates :
                 *    Exchange : quartz => no binding
                 *    Note : the MassTransit.QuartzService ust be installed to consume those messages
                 */
                cfg.UseMessageScheduler(new Uri("rabbitmq://localhost/quartz"));

                cfg.ReceiveEndpoint(host, "schedule_notification_queue", e =>
                {
                    e.Consumer<ScheduleNotificationConsumer>();
                });

                cfg.ReceiveEndpoint(host, "notification_queue", e =>
                {
                    e.Consumer<NotificationConsumer>();
                });

            });

            return bus;
        }
    }
}