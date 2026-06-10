namespace ModCore.Common.PubSub
{
    public interface IRedisPubSub
    {
        Task PublishAsync<T>(
            string channel,
            T message,
            CancellationToken cancellationToken = default);

        Task SubscribeAsync<T>(
            string channel,
            Func<T, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default);

        Task UnsubscribeAsync(
            string channel,
            CancellationToken cancellationToken = default);
    }
}
