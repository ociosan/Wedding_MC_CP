using Azure.Messaging.ServiceBus;

namespace Azure.Interfaces.Repository
{
    public interface IServiceBusRepository
    {
        Task SendMessageToQueueAsync(string queueName, string message);
        Task<string> ReadMessageFromQueueAsync(string queueName);
    }
}
