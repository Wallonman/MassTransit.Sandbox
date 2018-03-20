using System;

namespace MassTransit.Sandbox.ProducerConsumer.Contracts
{
    public interface IOrderSubmitted
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
    }
}