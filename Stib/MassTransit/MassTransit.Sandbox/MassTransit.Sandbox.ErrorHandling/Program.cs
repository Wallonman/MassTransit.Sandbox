using System;
using System.Threading.Tasks;

namespace MassTransit.Sandbox.ErrorHandling
{

    class Program
    {

        static void Main(string[] args)
        {
            HandlingExceptions.HandlingExceptions.Start();
        }


    }
}
