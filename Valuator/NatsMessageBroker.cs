using System.Threading.Tasks;
using NATS.Client;
using System.Text;

namespace Valuator
{
    class NatsMessageBroker{


        public void Send(string key, string message)
        {
            Task.Factory.StartNew(() =>
            {
                ConnectionFactory cf = new ConnectionFactory();
                IConnection c = cf.CreateConnection();
                byte[] data = Encoding.UTF8.GetBytes(message);
                c.Publish(key, data);
                c.Drain();
                c.Close();
            });
        }
    }
}