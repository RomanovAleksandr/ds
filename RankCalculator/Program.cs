using System;
using StackExchange.Redis;
using NATS.Client;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory cf = new ConnectionFactory();
            IConnection c = cf.CreateConnection();
            EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            {
                // print the message
                Console.WriteLine(args.Message);

                // Unsubscribing from within the delegate function is supported.
                args.Message.ArrivalSubcription.Unsubscribe();
            };
            IAsyncSubscription s = c.SubscribeAsync("foo", h);

            // Draining and closing a connection
            c.Drain();

            // Closing a connection
            c.Close();
        }
    }
}
