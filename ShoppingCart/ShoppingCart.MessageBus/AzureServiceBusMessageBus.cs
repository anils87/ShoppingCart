//using Microsoft.Azure.ServiceBus;
//using Microsoft.Azure.ServiceBus.Core;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        // need to maintain in appsettings.json file
        private string connectionString = "Endpoint=sb://shoppingcart.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=X+IHMlsGDZU2bquTpOb5HO4IYY8wGZGDTcMHtUGye7A=";
        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(topicName);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };
            await sender.SendMessageAsync(finalMessage);
            await sender.DisposeAsync();

            // Code for Microsoft.Azure.ServiceBus packages
            /*
            ISenderClient senderClient = new TopicClient(connectionString,topicName);
            var jsonMessage = JsonConvert.SerializeObject(message);
            var finalMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await senderClient.SendAsync(finalMessage);
            await senderClient.CloseAsync();            
            */
        }
    }
}
