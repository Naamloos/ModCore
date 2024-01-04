namespace ModCore.Common.Discord.Gateway.Events
{
    public interface ISubscriber<T> : ISubscriber where T : IPublishable
    {
        public Task HandleEvent(T data);
    }

    public interface ISubscriber 
    {
        Gateway Gateway { get; set; }
    }
}
