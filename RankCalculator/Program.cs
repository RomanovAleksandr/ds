using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;
using NATS.Client;
using Valuator;

namespace RankCalculator
{
    class Program
    {
        private static bool _exit = false;
        private static IConnection _connection;
        private static readonly IStorage _storage = new RedisStorage();
        static void Main(string[] args)
        {
            using (_connection = ConnectToNats())
            {
                SubscribeQueueGroups();

                Console.Clear();
                Console.WriteLine($"Connected to {_connection.ConnectedUrl}.");
                Console.WriteLine("Consumers started");
                Console.ReadKey(true);
                _exit = true;

                _connection.Drain(5000);
            }
        }

        private static IConnection ConnectToNats()
        {
            ConnectionFactory factory = new ConnectionFactory();

            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = "nats://localhost:4222";
            
            return factory.CreateConnection(options);
        }

        private static void SubscribeQueueGroups()
        {
            EventHandler<MsgHandlerEventArgs> handler = (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                LogMessage(id);
                string text = _storage.Load("TEXT-"+id);
                string rank = ((float)text.Count(ch => !char.IsLetter(ch)) / (float)text.Length).ToString();
                _storage.Store("RANK-" + id, rank);
            };

            IAsyncSubscription s = _connection.SubscribeAsync(
                "nats.demo.queuegroups", "load-balancing-queue", handler);
        }

         private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffffff")} - {message}");
        }
    }
}
