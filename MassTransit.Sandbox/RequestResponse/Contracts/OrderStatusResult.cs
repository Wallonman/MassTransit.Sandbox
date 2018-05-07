using System;

namespace MassTransit.Sandbox.RequestResponse.Contracts
{
    public class OrderStatusResult
    {
        public string OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public short StatusCode { get; set; }
        public string StatusText { get; set; }
    }
}