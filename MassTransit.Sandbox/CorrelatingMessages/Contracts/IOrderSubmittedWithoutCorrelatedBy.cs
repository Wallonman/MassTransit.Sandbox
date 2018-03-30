using System;

namespace MassTransit.Sandbox.CorrelatingMessages.Contracts
{
    /// <summary>
    /// A standard message, doesn't implement the CorrelatedBy&lt;Guid&gt;
    /// But have its own correlation property
    /// </summary>
    public interface IOrderSubmittedWithoutCorrelatedBy
    {
        string OrderId { get; }
        DateTime OrderDate { get; }

        Guid SomeGuidValue { get; }
    }
}