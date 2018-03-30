using System;

namespace MassTransit.Sandbox.CorrelatingMessages.Contracts
{
    /// <summary>
    /// A message that implements the CorrelatedBy&lt;Guid&gt;
    /// A CorrelationId will be automatically managed by MT
    /// (if a correlationId is passed by the producer)
    /// </summary>
    /// <seealso cref="Guid" />
    public interface ISubmitOrderCorrelated : CorrelatedBy<Guid>
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }

}