using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.ShoppingCartAPI.Models.DTOs;
using ShoppingCart.ShoppingCartAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private ResponseDto _responseDto;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._responseDto = new ResponseDto();
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
        [HttpPost("RemoveCart/{cartDetailId}")]
        public async Task<object> GetCart([FromBody]int cartId)
        {
            try
            {
                bool IsSuccess = await _cartRepository.RemoveFromCart(cartId);
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

    }
}
