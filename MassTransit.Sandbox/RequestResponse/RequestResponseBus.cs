using System;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using MassTransit.Log4NetIntegration;
using MassTransit.Sandbox.ProducerConsumer;
using MassTransit.Sandbox.ProducerConsumer.Consumers;
using MassTransit.Sandbox.ProducerConsumer.Contracts;
using MassTransit.Sandbox.RequestResponse.Consumers;
using MassTransit.Sandbox.RequestResponse.Contracts;

namespace MassTransit.Sandbox.RequestResponse
{
    public static class RequestResponseBus
    {
        private static ILog _logger;

        public static void Start()
        {
            // load the Log4Net config from app.config
            XmlConfigurator.Configure();

            var busControl = ConfigureBus();

            busControl.Start();
            do
            {
                Console.WriteLine("'q' to exit");
                Console.WriteLine("'1' -> Request / response");
                Console.WriteLine("'666' -> Request / exception");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("q".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;

                switch (value)
                {
                    case "1":
                    case "666":
                        Console.Out.WriteLine(
                            $"{DateTime.Now:O} Requesting status of OrderId {value}");

                        /* 
                         * create the request client
                         * Creates the response queue bus-WINBOOK-MassTransit.Sandbox.RequestResponse-yyyyyyb8yyfyyx1zbdk5jec9r5
                         * with autodelete = true and expiration = 60000 ms
                        */
                        var client = busControl.CreateClientFactory().CreateRequestClient<CheckOrderStatus>(new Uri("rabbitmq://localhost/check_order_queue"), TimeSpan.FromSeconds(30));
                        try
                        {
                            // send the message expecting the response in the Task Result
                            var result = client.GetResponse<OrderStatusResult>(new CheckOrderStatus { OrderId = value }).Result;
                            Console.Out.WriteLine(
                                $"{DateTime.Now:O} Received status {result.Message.StatusText} ({result.Message.StatusCode}) for OrderId {result.Message.OrderId}");
                            /*
                             * The payload received in the response queue is
                             * 
                             {

                                  "messageId": "00000000-2700-0a00-8b04-08d5b4a2ef65",

                                  "requestId": "00000000-2700-0a00-e326-08d5b4a2ef63",

                                  "conversationId": "00000000-2700-0a00-3635-08d5b4a2ef64",

                                  "sourceAddress": "rabbitmq://localhost/check_order_queue",

                                  "destinationAddress": "rabbitmq://localhost/bus-WINBOOK-MassTransit.Sandbox.RequestResponse-yyyyyyb8yyfyy7r9bdk5jewgdd?durable=false&autodelete=true",

                                  "messageType": [

                                    "urn:message:MassTransit.Sandbox.RequestResponse.Contracts:OrderStatusResult"

                                  ],

                                  "message": {

                                    "orderId": "1",

                                    "timestamp": "2018-05-08T07:17:03.6952914+02:00",

                                    "statusCode": 1,

                                    "statusText": "Sent"

                                  },

                                  "sentTime": "2018-05-08T05:17:03.6952914Z",

                                  "headers": {},

                                  "host": {

                                    "machineName": "WINBOOK",

                                    "processName": "MassTransit.Sandbox.RequestResponse",

                                    "processId": 13764,

                                    "assembly": "MassTransit.Sandbox.RequestResponse",

                                    "assemblyVersion": "1.0.0.0",

                                    "frameworkVersion": "4.0.30319.42000",

                                    "massTransitVersion": "5.0.0.1475",

                                    "operatingSystemVersion": "Microsoft Windows NT 6.2.9200.0"

                                  }

                                }
                             */
                        }
                        catch (System.AggregateException e)
                        {
                            /*
                             * The payload received in the response queue is
                                {

                                  "messageId": "00000000-2700-0a00-476f-08d5b4a2f145",

                                  "requestId": "00000000-2700-0a00-836c-08d5b4a2f141",

                                  "conversationId": "00000000-2700-0a00-c2b4-08d5b4a2f141",

                                  "sourceAddress": "rabbitmq://localhost/check_order_queue",

                                  "destinationAddress": "rabbitmq://localhost/bus-WINBOOK-MassTransit.Sandbox.RequestResponse-yyyyyyb8yyfyy7r9bdk5jewgdd?durable=false&autodelete=true",

                                  "messageType": [

                                    "urn:message:MassTransit:Fault[[MassTransit.Sandbox.RequestResponse.Contracts:CheckOrderStatus]]",

                                    "urn:message:MassTransit:Fault"

                                  ],

                                  "message": {

                                    "faultId": "00000000-2700-0a00-3d20-08d5b4a2f145",

                                    "faultedMessageId": "00000000-2700-0a00-8b89-08d5b4a2f141",

                                    "timestamp": "2018-05-08T05:17:06.8458671Z",

                                    "exceptions": [

                                      {

                                        "exceptionType": "System.InvalidOperationException",

                                        "stackTrace": "   à MassTransit.Sandbox.RequestResponse.Consumers.CheckOrderStatusConsumer.<Consume>d__0.MoveNext() dans C:\\Users\\Eric\\Documents\\Work\\Github\\MassTransit.Sandbox\\MassTransit.Sandbox\\RequestResponse\\Consumers\\CheckOrderStatusConsumer.cs:ligne 15\r\n--- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---\r\n   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   à MassTransit.Pipeline.ConsumerFactories.DefaultConstructorConsumerFactory`1.<Send>d__0`1.MoveNext()\r\n--- Fin de la trace de la pile à partir de l'emplacement précédent au niveau duquel l'exception a été levée ---\r\n   à System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   à System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n   à MassTransit.Pipeline.Filters.ConsumerMessageFilter`2.<GreenPipes-IFilter<MassTransit-ConsumeContext<TMessage>>-Send>d__4.MoveNext()",

                                        "message": "Order not found",

                                        "source": "MassTransit.Sandbox"

                                      }

                                    ],

                                    "host": {

                                      "machineName": "WINBOOK",

                                      "processName": "MassTransit.Sandbox.RequestResponse",

                                      "processId": 13764,

                                      "assembly": "MassTransit.Sandbox.RequestResponse",

                                      "assemblyVersion": "1.0.0.0",

                                      "frameworkVersion": "4.0.30319.42000",

                                      "massTransitVersion": "5.0.0.1475",

                                      "operatingSystemVersion": "Microsoft Windows NT 6.2.9200.0"

                                    },

                                    "message": {

                                      "orderId": "666"

                                    }

                                  },

                                  "sentTime": "2018-05-08T05:17:06.8458671Z",

                                  "headers": {},

                                  "host": {

                                    "machineName": "WINBOOK",

                                    "processName": "MassTransit.Sandbox.RequestResponse",

                                    "processId": 13764,

                                    "assembly": "MassTransit.Sandbox.RequestResponse",

                                    "assemblyVersion": "1.0.0.0",

                                    "frameworkVersion": "4.0.30319.42000",

                                    "massTransitVersion": "5.0.0.1475",

                                    "operatingSystemVersion": "Microsoft Windows NT 6.2.9200.0"

                                  }

                                }
                             */
                            var exception = e.InnerException as RequestFaultException;
                            if (exception != null)
                                // we've got an exception rasied by the consumer !
                                Console.WriteLine($"{DateTime.Now:O} {exception.Fault.Exceptions[0].Message}");
                            else
                                // re-throw any unexpected exceptions
                                throw;
                        }

                        break;
                }

            } while (true);
            busControl.Stop();
        }

        private static IBusControl ConfigureBus()
        {
            var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // request the log to Log4Net
                cfg.UseLog4Net();

                cfg.ReceiveEndpoint("check_order_queue", e =>
                {
                    e.Consumer<CheckOrderStatusConsumer>();
                });
            });

            return bus;
        }
    }
}