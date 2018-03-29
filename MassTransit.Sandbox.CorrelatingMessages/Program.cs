using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.CorrelatingMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            CorrelatingMessagesBus.Start();
        }
    }
}
