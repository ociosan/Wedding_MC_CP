using Azure.Interfaces.Repository;
using Azure.Messaging.ServiceBus;

namespace Azure.Repository
{
    public class ServiceBusRepository : IServiceBusRepository
    {
        private readonly ServiceBusClient _serviceBusClient;

        public ServiceBusRepository()
        {
            _serviceBusClient = new ServiceBusClient(Environment.GetEnvironmentVariable("Service_Bus_Namespace"), new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            });
        }

        public async Task SendMessageToQueueAsync(string queueName, string message)
        {
            ServiceBusSender sender = _serviceBusClient.CreateSender(queueName);
            await sender.SendMessageAsync(new ServiceBusMessage(message));
        }
    }
}
