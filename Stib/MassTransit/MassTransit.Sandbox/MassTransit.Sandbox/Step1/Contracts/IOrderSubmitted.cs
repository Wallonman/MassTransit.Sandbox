using System;

namespace MassTransit.Sandbox.Step1.Contracts
{
    public interface IOrderSubmitted
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
    }
}