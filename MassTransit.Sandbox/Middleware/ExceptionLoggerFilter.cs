using System;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.Sandbox.Middleware
{
    public class ExceptionLoggerFilter<T> :
        IFilter<T>
        where T : class, PipeContext
    {
        long _exceptionCount;
        long _successCount;
        long _attemptCount;
        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("exceptionLogger");
            scope.Add("attempted", _attemptCount);
            scope.Add("succeeded", _successCount);
            scope.Add("faulted", _exceptionCount);
        }
        public async Task Send(T context, IPipe<T> next)
        {
            try
            {
                var test = 0;
                context.Interlocked.Increment(ref _attemptCount);
                await next.Send(context);
                Interlocked.Increment(ref _successCount);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _exceptionCount);
                await Console.Out.WriteLineAsync($"An exception occurred: {ex.Message}");
                // propagate the exception up the call stack
                throw;
            }
        }
    }
    
}