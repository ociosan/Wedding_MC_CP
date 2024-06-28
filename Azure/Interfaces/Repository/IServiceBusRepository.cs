namespace Azure.Interfaces.Repository
{
    public interface IServiceBusRepository
    {
        Task SendMessageToQueueAsync(string queueName, string message);
    }
}
