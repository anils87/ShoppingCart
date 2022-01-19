using Newtonsoft.Json;
using RabbitMQ.Client;
using ShoppingCart.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.PaymentAPI.RabbitMQSender
{
    public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
    {
        private readonly string _hostname;
        private readonly string _username; 
        private readonly string _password;
        private IConnection _connection;
        // Fanout Exchange Type
        //private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";

        // Direct Exchange Type
        private const string ExchangeName = "DirectPaymentUpdate_Exchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        public RabbitMQPaymentMessageSender()
        {
            _hostname = "localhost";
            _username = "guest";
            _password = "guest";
        }
        public void SendMessage(BaseMessage baseMessage)
        {
            if (ConnectionExists())
            {
                using var channel = _connection.CreateModel();
                //channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

                // implement Fanout
                //channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout, durable: false);

                //implement Direct Exchange
                channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: false);
                channel.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);
                channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
                channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName,"PaymentEmail");
                channel.QueueBind(PaymentOrderUpdateQueueName, ExchangeName, "PaymentOrder");

                var json = JsonConvert.SerializeObject(baseMessage);
                var body = Encoding.UTF8.GetBytes(json);
                //channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                // implement Fanout
                //channel.BasicPublish(exchange: ExchangeName, "", basicProperties: null, body: body);


                //implement Direct Pushlish message
                channel.BasicPublish(exchange: ExchangeName, "PaymentEmail", basicProperties: null, body: body);
                channel.BasicPublish(exchange: ExchangeName, "PaymentOrder", basicProperties: null, body: body);
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = factory.CreateConnection();

            }
            catch (Exception ex)
            {
                // write logs
            }
        }

        private bool ConnectionExists()
        {
            if(_connection != null)
            {
                return true;
            }
            CreateConnection();
            return _connection != null ;
        }
    }
}
