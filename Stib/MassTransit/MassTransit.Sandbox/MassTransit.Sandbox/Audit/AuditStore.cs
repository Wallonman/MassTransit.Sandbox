using System;
using System.Threading.Tasks;
using MassTransit.Audit;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Audit
{
    public class AuditStore : IMessageAuditStore
    {
        public async Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
        {
                await Console.Out.WriteLineAsync($"Audit Payload: {JsonConvert.SerializeObject(message)}");
                await Console.Out.WriteLineAsync($"Audit Metadata: {JsonConvert.SerializeObject(metadata)}");
        }
    }
}