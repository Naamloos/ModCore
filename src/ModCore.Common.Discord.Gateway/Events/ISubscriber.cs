namespace ModCore.Common.Discord.Gateway.Events
{
    public interface ISubscriber<T> : ISubscriber where T : IPublishable
    {
        public ValueTask HandleEvent(T data);
    }

    public interface ISubscriber 
    {
        Gateway Gateway { get; set; }
    }
}
