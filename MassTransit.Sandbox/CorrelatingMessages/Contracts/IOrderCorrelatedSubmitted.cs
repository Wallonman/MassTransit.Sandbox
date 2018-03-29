using System;

namespace MassTransit.Sandbox.CorrelatingMessages.Contracts
{
    public interface IOrderCorrelatedSubmitted : CorrelatedBy<Guid>
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
    }
}