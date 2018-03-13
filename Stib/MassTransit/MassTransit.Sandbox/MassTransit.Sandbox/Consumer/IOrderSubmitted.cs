using System;

namespace MassTransit.Sandbox.Consumer
{
    public interface IOrderSubmitted
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
    }
}