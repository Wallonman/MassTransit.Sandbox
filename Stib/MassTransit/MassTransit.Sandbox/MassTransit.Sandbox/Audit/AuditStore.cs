using System;
using System.Threading.Tasks;
using MassTransit.Audit;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Audit
{
    public class AuditStore : IMessageAuditStore
    {
        private readonly string _code;

        public AuditStore(string code)
        {
            _code = code;
        }

        public async Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
        {
                await Console.Out.WriteLineAsync($"{_code} Payload: {JsonConvert.SerializeObject(message)}");
                await Console.Out.WriteLineAsync($"{_code} Metadata: {JsonConvert.SerializeObject(metadata)}");
        }
    }
}