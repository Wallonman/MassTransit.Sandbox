using System;

namespace MassTransit.Sandbox.CorrelatingMessages.Contracts
{
    public interface ISubmitOrderCorrelated : CorrelatedBy<Guid>
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }

    public class SubmitOrderCorrelated : ISubmitOrderCorrelated
    {
        public SubmitOrderCorrelated()
        {
            CorrelationId = Guid.NewGuid();
        }
        public Guid CorrelationId { get; }
        public string OrderId { get; }
        public DateTime OrderDate { get; }
        public decimal OrderAmount { get; }
    }
}