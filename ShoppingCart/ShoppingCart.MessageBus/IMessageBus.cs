using System;
using System.Threading.Tasks;

namespace ShoppingCart.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(BaseMessage message, string topicName);
    }
}
