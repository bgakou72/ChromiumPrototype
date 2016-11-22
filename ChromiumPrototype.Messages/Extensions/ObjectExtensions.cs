using Microsoft.ServiceBus.Messaging;

namespace ChromiumPrototype.Messages.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetMessageType(this object obj)
        {
            return obj.GetType().GetMessageType();
        }

        public static BrokeredMessage ToBrokeredMessage<T>(this T obj)
        {
            var message = new BrokeredMessage(obj)
            {
                ContentType = obj.GetMessageType()
            };
            return message;
        }
    }
}