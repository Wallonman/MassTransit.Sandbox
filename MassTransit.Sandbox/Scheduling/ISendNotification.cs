namespace MassTransit.Sandbox.Scheduling
{
    public interface ISendNotification
    {
        string EmailAddress { get; }
        string Body { get; }
    }
}