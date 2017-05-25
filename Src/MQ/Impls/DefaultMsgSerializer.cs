using MQ.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MQ.Impls
{
    public class DefaultMsgSerializer : IMsgSerializer
    {
        public string Serializer<T>(MsgModel<T> msgModel)
        {
            var jsonStr = JsonConvert.SerializeObject(msgModel);
            
            return jsonStr;
        }
    }
}
