using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.MessageBus;
using ShoppingCart.ShoppingCartAPI.Message;
using ShoppingCart.ShoppingCartAPI.Models.DTOs;
using ShoppingCart.ShoppingCartAPI.RabbitMQSender;
using ShoppingCart.ShoppingCartAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private ResponseDto _responseDto;

        private readonly IMessageBus _messageBus;

        private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;
        public CartAPIController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository, IRabbitMQCartMessageSender rabbitMQCartMessageSender)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _couponRepository = couponRepository;
            this._responseDto = new ResponseDto();
            _rabbitMQCartMessageSender = rabbitMQCartMessageSender;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _responseDto.IsSuccess = true;
                _responseDto.Result = cartDto;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };                
            }

            return _responseDto;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody]CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                _responseDto.IsSuccess = true;
                _responseDto.Result = cartDt;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart([FromBody]CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                _responseDto.IsSuccess = true;
                _responseDto.Result = cartDt;                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }
        
        [HttpPost("RemoveCart")]
        public async Task<object> GetCart([FromBody]int cartId)
        {
            try
            {
                bool IsSuccess = await _cartRepository.RemoveFromCart(cartId);
                _responseDto.IsSuccess = IsSuccess;
                _responseDto.Result = IsSuccess;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("ClearCart/{userId}")]
        public async Task<object> ClearCart([FromBody]string userId)
        {
            try
            {
                bool IsSuccess = await _cartRepository.ClearCart(userId);
                _responseDto.Result = IsSuccess;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId,cartDto.CartHeader.CouponCode);
                _responseDto.IsSuccess = true;
                _responseDto.Result = isSuccess;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                _responseDto.IsSuccess = true;
                _responseDto.Result = isSuccess;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout([FromBody] CheckoutHeaderDto checkoutHeader)
        {
            try
            {
                //bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if(cartDto == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                {
                    CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                    if(checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                    {
                        _responseDto.IsSuccess = false;
                        _responseDto.ErrorMessage = new List<string>() { "Coupon Price has changed, please confirm" };
                        _responseDto.DisplayMessage = "Coupon Price has changed, please confirm";
                        return _responseDto;
                    }
                }
                checkoutHeader.CartDetails = cartDto.CartDetails;
                // logic to add message to process order.
                //passing queue topic name 
                //await _messageBus.PublishMessage(checkoutHeader, "checkoutmessagetopic");

                // passing queue name
                // await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                //rabbitMQ 
                _rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");

                await _cartRepository.ClearCart(checkoutHeader.UserId);

                _responseDto.IsSuccess = true;
                _responseDto.Result = checkoutHeader;

            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }
    }
}
