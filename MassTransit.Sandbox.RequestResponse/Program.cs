using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.RequestResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestResponseBus.Start();
        }
    }
}
