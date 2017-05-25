using System;
using MQ.Interfaces;
using RabbitMQ.Client;

namespace MQ.Impls.RabbitMQ
{
    public class MsgSender : IMsgSender
    {
        IModel channel;
        private IMsgSerializer msgSerializer;

        public MsgSender(IModel channel, IMsgSerializer msgSerializer)
        {
            this.channel = channel;
            this.msgSerializer = msgSerializer;
        }

        public bool SendMsg<T>(MsgModel<T> msgModel) where T : new()
        {
            msgSerializer.Serializer<string>(new MsgModel<string> { Module = ModuleEnum.User, Operation = OperationEnum.Register, Msg = "msg" });

            channel.BasicPublish("","",null,)

            return true;
        }
    }
}
