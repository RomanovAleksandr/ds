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
        private static IConnection _connection;
        private static readonly IStorage _storage = new RedisStorage();
        static void Main(string[] args)
        {
            using (_connection = ConnectToNats())
            {
                SubscribeQueueGroups();

                Console.WriteLine("Consumers started");
                Console.ReadKey(true);

                _connection.Drain();
                _connection.Close();
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
                "valuator.processing.rank", "load-balancing-queue", handler);
        }

         private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffffff")} - {message}");
        }
    }
}
