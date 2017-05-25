namespace MQ
{
    public enum ModuleEnum
    {
        User
    }

    public enum OperationEnum
    {
        Register
    }

    public class MsgModel<T>
    {
        public ModuleEnum Module { get; set; }
        public OperationEnum Operation { get; set; }

        public T Msg { get; set; }
    }

}
