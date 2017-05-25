namespace MQ.Interfaces
{
    public interface IMsgSender
    {
        bool SendMsg<T>(MsgModel<T> msgModel) where T : new();
    }
}
