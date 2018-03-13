using System;

namespace MassTransit.Sandbox.Consumer
{
    public interface ISubmitOrder
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }
}