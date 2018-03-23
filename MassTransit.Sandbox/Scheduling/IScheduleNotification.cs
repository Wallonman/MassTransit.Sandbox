using System;

namespace MassTransit.Sandbox.Scheduling
{
    public interface IScheduleNotification
    {
        DateTime DeliveryTime { get; }
        string EmailAddress { get; }
        string Body { get; }
    }
}