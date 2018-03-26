using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.QuartzIntegration;
using MassTransit.Util;
using Quartz;
using Quartz.Impl;

namespace MassTransit.Sandbox.Scheduling
{
    public static class SchedulingBus
    {
        private static IScheduler _scheduler;

        public static void Start()
        {
            _scheduler = CreateScheduler();

            var busControl = ConfigureBus();

            _scheduler.JobFactory = new MassTransitJobFactory(busControl);
            _scheduler.Start();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Schedule Notification");
                Console.WriteLine("'2' -> Send Notification");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/schedule_notification_queue"))
                            .Result.Send<IScheduleNotification>(new
                            {
                                DeliveryTime = DateTime.Now.AddSeconds(5),
                                EmailAddress = "test@yopmail.com",
                                Body = "Hello World!"
                            });
                        break;
                    case "2":
                        busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/quartz"))
                            .Result.ScheduleSend(new Uri("rabbitmq://localhost/notification_queue"),
                                TimeSpan.FromSeconds(5),
                                 new ScheduleNotificationConsumer.SendNotificationCommand
                                 {
                                    EmailAddress = "test@yopmail.com",
                                    Body = "Hello World!",
                                });
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

                cfg.ReceiveEndpoint(host, "quartz", e =>
                {
                    var partitioner = e.CreatePartitioner(1);

                    e.Consumer(() => new ScheduleMessageConsumer(_scheduler));
//                    , x =>
//                                x.Message<ScheduleMessage>(m => m.UsePartitioner(partitioner, p => p.Message.CorrelationId)));
                });


                cfg.ReceiveEndpoint(host, "schedule_notification_queue",
                    e => { e.Consumer<ScheduleNotificationConsumer>(); });

                cfg.ReceiveEndpoint(host, "notification_queue", e => { e.Consumer<NotificationConsumer>(); });
            });

            return bus;
        }

        static IScheduler CreateScheduler()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            var scheduler = TaskUtil.Await(() => schedulerFactory.GetScheduler());

            return scheduler;
        }
    }
}