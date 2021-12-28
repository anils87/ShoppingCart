using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShoppingCart.OrderAPI.Extension;
using ShoppingCart.OrderAPI.Message;
using ShoppingCart.OrderAPI.Models;
using ShoppingCart.OrderAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckout;
        private readonly string checkoutMessageTopic;
        private readonly OrderRepository _orderRepository;

        private readonly IConfiguration _configuration;

        private ServiceBusProcessor _checkoutProcessor;
        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionCheckout = _configuration.GetValue<string>("SubscriptionCheckout");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _checkoutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckout);

        }

        public async Task Start()
        {
            _checkoutProcessor.ProcessMessageAsync += OnCheckoutMessageReceived;
            _checkoutProcessor.ProcessErrorAsync += ErrorHandler;
            await _checkoutProcessor.StartProcessingAsync();
        }


        public async Task Stop()
        {
        
            await _checkoutProcessor.StopProcessingAsync();
            await _checkoutProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnCheckoutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);
            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                OrderTime = DateTime.Now,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime

            };
            foreach(var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price = detailList.Product.Price,
                    Count = detailList.Count
                };
                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);
        }
    }
}
