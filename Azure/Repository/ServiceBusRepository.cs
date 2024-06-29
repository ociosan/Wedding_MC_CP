using Azure.Interfaces.Repository;
using Azure.Messaging.ServiceBus;
using System.Text;

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

        public async Task<string> ReadMessageFromQueueAsync(string queueName)
        {
            string body = string.Empty;
            ServiceBusReceiver serviceBusReceiver = _serviceBusClient.CreateReceiver(queueName);

            var messageInQueue = await serviceBusReceiver.ReceiveMessageAsync();
            if (messageInQueue is null)
                return body;
            else
            {
                body = Encoding.ASCII.GetString(messageInQueue.Body);
                await serviceBusReceiver.CompleteMessageAsync(messageInQueue);
            }

            return body;
        }
    }
}
