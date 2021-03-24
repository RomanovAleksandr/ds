using System;
using System.Linq;
using System.Text;
using NATS.Client;
using System.Text.Json;
using Library;

namespace RankCalculator
{
    class Program
    {
        private static NatsMessageBroker _natsMessageBroker;
        private static IStorage _storage;
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer with queue started");
            _storage = new RedisStorage();
            _natsMessageBroker = new NatsMessageBroker();

            ConnectionFactory cf = new ConnectionFactory();
            IConnection connection = cf.CreateConnection();

            var subscription = connection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string text = _storage.Load(Constants.TEXT + id);
                double rank = CalculateRank(text);
                _storage.Store(Constants.RANK + id, rank.ToString());
                PublishSimilarityCalculatedEvent(id, rank);
                Console.WriteLine("Consuming: {0} from subject {1}", id, args.Message.Subject);
            });

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            subscription.Unsubscribe();

            connection.Drain();
            connection.Close();
        }

        static double CalculateRank(string text)
        {
            return (float)text.Count(ch => !char.IsLetter(ch)) / (float)text.Length;
        }

        static void PublishSimilarityCalculatedEvent(string id, double rank)
        {
                Rank textSmilarity = new Rank(id, rank);
                string rankJson = JsonSerializer.Serialize(textSmilarity);
                _natsMessageBroker.Send("valuator.rank_calculated", rankJson);
        }
    }
}
