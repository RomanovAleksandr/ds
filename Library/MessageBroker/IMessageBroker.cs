namespace Library
{
    public interface IMessageBroker
    {
        void Send(string key, string message);
    }
}