using System;
using StackExchange.Redis;
using NATS.Client;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // ConnectionFactory cf = new ConnectionFactory();
            // IConnection c = cf.CreateConnection();
            // EventHandler<MsgHandlerEventArgs> h = (sender, args) =>
            // {
            //     // print the message
            //     Console.WriteLine(args.Message);

            //     // Unsubscribing from within the delegate function is supported.
            //     args.Message.ArrivalSubcription.Unsubscribe();
            // };
            // IAsyncSubscription s = c.SubscribeAsync("foo", h);
            // //ISyncSubscription sSync = c.SubscribeSync("foo");
            
            // // Draining and closing a connection
            // c.Drain();

            // // Closing a connection
            // c.Close();

            void printMessage(object sender, MsgHandlerEventArgs e)
            {
                System.Console.WriteLine(e.Message);
            }

            using (IConnection c = new ConnectionFactory().CreateConnection())
            {
                using (IAsyncSubscription s = c.SubscribeAsync("foo", printMessage))
                {
                    // Process for 5 seconds.
                    //Thread.Sleep(5000);
                }
            }
        }
    }
}
