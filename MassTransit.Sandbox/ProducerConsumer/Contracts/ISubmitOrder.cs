using System;

namespace MassTransit.Sandbox.ProducerConsumer.Contracts
{
    public interface ISubmitOrder
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }
}