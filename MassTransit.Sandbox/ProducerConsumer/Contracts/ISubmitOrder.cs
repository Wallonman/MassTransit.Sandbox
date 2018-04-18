using System;

namespace MassTransit.Sandbox.ProducerConsumer.Contracts
{
    public interface ISubmitOrder
    {
        string OrderId { get; }
        DateTime OrderDate { get; }
        decimal OrderAmount { get; }
    }

    public class SubmitOrder : ISubmitOrder
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; }
        public decimal OrderAmount { get; }
    }
}