using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MassTransit.Sandbox.ProducerConsumer.Contracts;

namespace MassTransit.Sandbox.Audit
{
    /// <summary>
    /// The Observer tries to watch the elapsed time spend by each consumation
    /// 
    /// </summary>
    /// <seealso cref="MassTransit.IConsumeObserver" />
    public class ConsumeObserver : IConsumeObserver
    {
        /// <summary>
        /// The dictionary that holds the MessageCallState per Message
        /// </summary>
        readonly ConcurrentDictionary<Guid, MessageCallState> _dictionary =
            new ConcurrentDictionary<Guid, MessageCallState>();

        public async Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            var contextHeaders = context.Headers; // is empty, how to add items in the header ?

            if (context.MessageId.HasValue)
                lock (_dictionary)
                {
                    _dictionary.TryAdd(context.MessageId.Value, new MessageCallState {Watch = Stopwatch.StartNew()});
                }

            var order = context.Message as ISubmitOrder;
            if (order != null)
            {
                // if you want to get access to the message properties
                // not sure that's a good idea ... 
                order.OrderAmount = 1;
            }

            await Console.Out.WriteLineAsync(
                $"{DateTime.Now:O} ConsumeObserver.PreConsume message {context.MessageId}");
        }

        public async Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            long elapsedMilliseconds = 0;
            lock (_dictionary)
            {
                if (context.MessageId != null && _dictionary.ContainsKey(context.MessageId.Value))
                {
                    if (_dictionary.TryRemove(context.MessageId.Value, out MessageCallState messageCallState))
                    {
                        messageCallState.Watch.Stop();
                        elapsedMilliseconds = messageCallState.Watch.ElapsedMilliseconds;
                    }
                }
            }
            await Console.Out.WriteLineAsync(
                $"{DateTime.Now:O} ConsumeObserver.PostConsume message {context.MessageId} ElapsedMilliseconds : {elapsedMilliseconds}");
        }

        public async Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            await Console.Out.WriteLineAsync($"{DateTime.Now:O} ConsumeObserver.ConsumeFault");
        }
    }

    /// <summary>
    /// A call state object that watches the time
    /// </summary>
    class MessageCallState
    {
        public Stopwatch Watch { get; set; }
    }
}