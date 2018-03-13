using System;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.Sandbox.SendEnpoint
{
    public class Class1 : ISendEndpointProvider
    {
        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }
    }

}