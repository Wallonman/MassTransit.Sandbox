using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Log4NetIntegration;
using MassTransit.QuartzIntegration;
using MassTransit.Scheduling;
using MassTransit.Util;
using Quartz;
using Quartz.Impl;

namespace MassTransit.Sandbox.Scheduling
{
    public static class SchedulingBus
    {
        private static IScheduler _scheduler;
        private static readonly Uri SchedulerAddress = new Uri("rabbitmq://localhost/quartz");
        private static ScheduledRecurringMessage<ScheduleNotificationConsumer.SendNotificationCommand> _recurringScheduledMessage;

        public static async void Start()
        {
            _scheduler = CreateScheduler();

            var busControl = ConfigureBus();

            _scheduler.JobFactory = new MassTransitJobFactory(busControl);
            await _scheduler.Start();

            busControl.Start();
            Console.WriteLine("'q' to exit");
            Console.WriteLine("'1' -> Scheduling a message from a consumer");
            Console.WriteLine("'2' -> Scheduling a message from the bus");
            Console.WriteLine("'3' -> Scheduling a recurring message");
            Console.WriteLine("'4' -> Cancel Scheduling a recurring message");
            do
            {
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                        /*
                         * Scheduling a message from a consumer
                         * Push a IScheduleNotification message to the schedule_notification_queue
                         * The consumer will trigger a scheduled send
                         */
                        Console.Write("Sending IScheduleNotification");
                        await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/schedule_notification_queue"))
                            .Result.Send<IScheduleNotification>(new
                            {
                                DeliveryTime = DateTime.Now.AddSeconds(5),
                                EmailAddress = "test@yopmail.com",
                                Body = "Hello World!"
                            });
                        break;
                    case "2":
                        /*
                         * Scheduling a message from the bus
                         * Sends a SendNotificationCommand message to the notification_queue
                         * scheduled for 5 seconds later
                         */
                        Console.WriteLine("Sending SendNotificationCommand in 5 seconds");
                        await busControl.GetSendEndpoint(SchedulerAddress)
                            .Result.ScheduleSend(new Uri("rabbitmq://localhost/notification_queue"),
                                TimeSpan.FromSeconds(5),
                                new ScheduleNotificationConsumer.SendNotificationCommand
                                {
                                    EmailAddress = "test@yopmail.com",
                                    Body = "Hello World!",
                                });
                        break;
                    case "3":
                        /*
                         * Scheduling a recurring message
                         * Sends a SendNotificationCommand message to the notification_queue
                         * scheduled recurring evry 5 seconds
                         */
                        Console.WriteLine("Sending SendNotificationCommand every 5 seconds");
                        _recurringScheduledMessage = await busControl.GetSendEndpoint(SchedulerAddress)
                            .Result.ScheduleRecurringSend(new Uri("rabbitmq://localhost/notification_queue"), 
                            new PollExternalSystemSchedule(), 
                                new ScheduleNotificationConsumer.SendNotificationCommand
                                {
                                    EmailAddress = "test@yopmail.com",
                                    Body = "Hello World!",
                                });
                        break;

                    case "4":
                        if (_recurringScheduledMessage != null)
                        {
                            Console.WriteLine("Cancel sending SendNotificationCommand every 5 seconds");

                            await busControl.CancelScheduledRecurringSend(_recurringScheduledMessage);
                            _recurringScheduledMessage = null;
                        }
                        else
                            Console.WriteLine("No schedule to cancel, please press 3 before");

                        break;
                }
            } while (true);

            await _scheduler.Shutdown();
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

                cfg.UseLog4Net();

                /*
                 * Creates :
                 *    Exchange : quartz => no binding
                 *    Note : the MassTransit.QuartzService must be installed to consume those messages
                 */
                cfg.UseMessageScheduler(SchedulerAddress);

                /*
                 * Creates:
                 *      Queue : quartz with a binding from the exchange "quartz"
                 *      Exchange : 
                 *          - MassTransit.Scheduling:ScheduleMessage
                 *          - MassTransit.Scheduling:ScheduleRecurringMessage
                 */
                cfg.ReceiveEndpoint(host, "quartz",
                    e => { e.Consumer(() => new ScheduleMessageConsumer(_scheduler)); });

                /*
                 * Creates :
                 * Exchange : schedule_notification_queue => queue schedule_notification_queue
                 * Exchange : MassTransit.Sandbox.Scheduling:IScheduleNotification => exchange schedule_notification_queue
                 * Queue : schedule_notification_queue
                 */
                cfg.ReceiveEndpoint(host, "schedule_notification_queue",
                    e => { e.Consumer<ScheduleNotificationConsumer>(); });

                /*
                 * Creates :
                 * Exchange : notification_queue => queue notification_queue
                 * Exchange : MassTransit.Sandbox.Scheduling:ISendNotification => exchange notification_queue
                 * Queue : notification_queue
                 */
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