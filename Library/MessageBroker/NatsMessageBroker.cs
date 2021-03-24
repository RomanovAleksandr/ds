using System.Threading.Tasks;
using NATS.Client;
using System.Text;

using System.Threading;

namespace Library
{
    public class NatsMessageBroker
    {
        public void Send(string key, string message)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task.Factory.StartNew(() =>  ProduceAsync(cts.Token, key, message), cts.Token);
        }

        private async Task ProduceAsync(CancellationToken ct, string key, string message)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                c.Publish(key, data);
                await Task.Delay(1000);
                c.Drain();
                c.Close();
            }
        }
    }
}