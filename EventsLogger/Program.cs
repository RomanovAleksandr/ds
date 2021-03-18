using System;
using Valuator;
using RankCalculator;
using NATS.Client;
using System.Text;
using System.Text.Json;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer started");

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.similarity_calculated", (sender, args) =>
            {
                string m = Encoding.UTF8.GetString(args.Message.Data);
                Similarity similarity = JsonSerializer.Deserialize<Similarity>(args.Message.Data);  
                Console.WriteLine("Consuming: from subject {2} similarity.Id {0} similarity.Value {1}", similarity.Id, similarity.Value, args.Message.Subject);
            });
            s = c.SubscribeAsync("valuator.rank_calculated", (sender, args) =>
            {
                string m = Encoding.UTF8.GetString(args.Message.Data);
                Rank similarity = JsonSerializer.Deserialize<Rank>(args.Message.Data);  
                Console.WriteLine("Consuming: from subject {2} rank.Id {0} rank.Value {1}", similarity.Id, similarity.Value, args.Message.Subject);
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
