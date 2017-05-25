namespace MQ.Interfaces
{
    public interface IMsgSerializer
    {
        string Serializer<T>(MsgModel<T> msgModel);

    }
}
