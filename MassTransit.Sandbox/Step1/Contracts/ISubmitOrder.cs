using System;

namespace MassTransit.Sandbox.Step1.Contracts
{
    public interface ISubmitOrder
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }
}