using System;
using System.Threading.Tasks;
using MassTransit.Sandbox.Step2;

namespace MassTransit.Sandbox.ErrorHandling
{

    class Program
    {

        static void Main(string[] args)
        {
            HandlingExceptions.Start();
        }


    }
}
