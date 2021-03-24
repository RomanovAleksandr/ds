using System;
using NATS.Client;
using System.Text.Json;
using Library;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer started");

            ConnectionFactory cf = new ConnectionFactory();
            IConnection connection = cf.CreateConnection();

            var subscription = connection.SubscribeAsync("valuator.similarity_calculated", (sender, args) =>
            {
                Similarity similarity = JsonSerializer.Deserialize<Similarity>(args.Message.Data);  
                Console.WriteLine("Consuming: from subject {2} similarity.Id {0} similarity.Value {1}", similarity.Id, similarity.Value, args.Message.Subject);
            });

            subscription = connection.SubscribeAsync("valuator.rank_calculated", (sender, args) =>
            {
                Rank similarity = JsonSerializer.Deserialize<Rank>(args.Message.Data);  
                Console.WriteLine("Consuming: from subject {2} rank.Id {0} rank.Value {1}", similarity.Id, similarity.Value, args.Message.Subject);
            });

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            subscription.Unsubscribe();

            connection.Drain();
            connection.Close();
        }
    }
}
