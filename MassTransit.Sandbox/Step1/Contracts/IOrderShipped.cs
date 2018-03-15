using System;

namespace MassTransit.Sandbox.Step1.Contracts
{
    public interface IOrderShipped
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        DateTime ShippingDate { get; }
    }
}