using MassTransit.Scheduling;

namespace MassTransit.Sandbox.Scheduling
{
    public class PollExternalSystemSchedule : DefaultRecurringSchedule
    {
        public PollExternalSystemSchedule()
        {
            // CronExpression = "0 0/1 * 1/1 * ? *"; // this means every minute
            CronExpression = "0/5 * * ? * *"; // this means every 5 seconds
        }
    }
    
}