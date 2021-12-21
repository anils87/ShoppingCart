using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingCart.Web.Models;
using ShoppingCart.Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            this._cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {            
            return View(await LoadCartItemByUserId());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");            
            var response = await _cartService.RemoveCartAsync<ResponseDto>(cartDetailsId, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<object> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return response;
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<object> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");           
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, accessToken);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return response;
        }

        private async Task<CartDto> LoadCartItemByUserId()
        {
            CartDto cartDtos = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);
            if (response != null && response.IsSuccess)
            {
                cartDtos = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            if(cartDtos.CartHeader!=null)
            {
                foreach(var detail in cartDtos.CartDetails)
                {
                    cartDtos.CartHeader.OrderTotal += (detail.Count * detail.Product.Price);
                }                
            }

            return cartDtos;
        }
       
    }
}
