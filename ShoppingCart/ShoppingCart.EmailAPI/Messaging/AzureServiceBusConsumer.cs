using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShoppingCart.EmailAPI.Extension;
using ShoppingCart.EmailAPI.Message;
using ShoppingCart.EmailAPI.Models;
using ShoppingCart.EmailAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionName;       
        private readonly string orderUpdatePaymentResultTopic;
        private readonly EmailRepository _emailRepository;

        private readonly IConfiguration _configuration;
        
        private ServiceBusProcessor _orderUpdatePaymentStatusProcess;

        public AzureServiceBusConsumer(EmailRepository emailRepository, IConfiguration configuration)
        {
            _emailRepository = emailRepository;
            _configuration = configuration;           

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionName = _configuration.GetValue<string>("SubscriptionName");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            
            _orderUpdatePaymentStatusProcess = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionName);

        }

        public async Task Start()
        {
          
            _orderUpdatePaymentStatusProcess.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _orderUpdatePaymentStatusProcess.ProcessErrorAsync += ErrorHandler;
            await _orderUpdatePaymentStatusProcess.StartProcessingAsync();
        }


        public async Task Stop()
        {
            await _orderUpdatePaymentStatusProcess.StopProcessingAsync();
            await _orderUpdatePaymentStatusProcess.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }       

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            try
            {
                await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
