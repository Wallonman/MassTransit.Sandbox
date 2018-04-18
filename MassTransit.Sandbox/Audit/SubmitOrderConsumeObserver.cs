using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using Newtonsoft.Json;

namespace MassTransit.Sandbox.Audit
{
    public class SubmitOrderConsumeObserver : IConsumeMessageObserver<SubmitOrder>
    {

        public async Task PreConsume(ConsumeContext<SubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} SubmitOrderConsumeObserver.PreConsume Payload: {JsonConvert.SerializeObject(context.Message)}");
        }

        public async Task PostConsume(ConsumeContext<SubmitOrder> context)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} SubmitOrderConsumeObserver.PostConsume Payload: {JsonConvert.SerializeObject(context.Message)}");
        }

        public async Task ConsumeFault(ConsumeContext<SubmitOrder> context, Exception exception)
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} SubmitOrderConsumeObserver.ConsumeFault Payload: {JsonConvert.SerializeObject(context.Message)}");
        }
    }
}