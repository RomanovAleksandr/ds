using System;
using System.Linq;
using System.Text;
using NATS.Client;
using Valuator;
using System.Text.Json;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer started");
            var storage = new RedisStorage();
            var nats = new NatsMessageBroker();

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string text = storage.Load("TEXT-"+id);
                double rank = (float)text.Count(ch => !char.IsLetter(ch)) / (float)text.Length;
                storage.Store("RANK-" + id, rank.ToString());
                Rank textSmilarity = new Rank(id, rank);
                string rankJson = JsonSerializer.Serialize(textSmilarity);
                nats.Send("valuator.rank_calculated", rankJson);
                Console.WriteLine("Consuming: {0} from subject {1}", id, args.Message.Subject);
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}
