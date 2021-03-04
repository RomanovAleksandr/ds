using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;

namespace Valuator
{
    class Nats{
        private static IConnection _connection;

        public Nats()
        {
            _connection = ConnectToNats();
        }
        private static IConnection ConnectToNats()
        {
            ConnectionFactory factory = new ConnectionFactory();

            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = "nats://localhost:4222";
            
            return factory.CreateConnection(options);
        }

        public void PubSub(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _connection.Publish("nats.demo.queuegroups", data);
        }
    }
}