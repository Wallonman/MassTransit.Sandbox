using System;

namespace MassTransit.Sandbox.Consumer
{
    public interface IOrderShipped
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        DateTime ShippingDate { get; }
    }
}