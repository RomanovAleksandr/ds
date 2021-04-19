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
            Task.Factory.StartNew(() =>  ProduceAsync(key, message));
        }

        private async Task ProduceAsync(string key, string message)
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