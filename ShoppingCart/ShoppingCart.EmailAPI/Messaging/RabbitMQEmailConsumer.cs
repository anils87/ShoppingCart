using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShoppingCart.EmailAPI.Message;
using ShoppingCart.EmailAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.EmailAPI.Messaging
{
    public class RabbitMQEmailConsumer : BackgroundService
    {
        
        private IConnection _connection;
        private IModel _channel;
        // Fanout Exchange Type 
        //private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

        // Direct Exchange Type
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

        private readonly EmailRepository _emailRepository;
        string queueName = "";
        public RabbitMQEmailConsumer(EmailRepository emailRepository)
        {

            _emailRepository = emailRepository;
            
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            // Fanot Exchange Type implementation
            //_channel.ExchangeDeclare(ExchangeName,ExchangeType.Fanout);
            //queueName = _channel.QueueDeclare().QueueName;
            //_channel.QueueBind(queueName, ExchangeName, "");

            // Direct Exchange Type Implementation
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content);
                HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            // Fanout Exchange Implementation
            //_channel.BasicConsume(queueName, false, consumer);

            // Direct Exchange Implementation
            _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
        {           
          try
            {
                await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);               
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
