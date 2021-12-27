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

        private readonly ICouponService _couponService;
        public CartController(ICartService cartService, ICouponService couponService)
        {
            this._cartService = cartService;
            this._couponService = couponService;
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


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartItemByUserId());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<object> Checkout(CartDto cartDto)
        {
            try
            {   
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, accessToken);
                return RedirectToAction(nameof(Confirmation));
               
            }
            catch(Exception ex)
            {
                return View(cartDto);
                     
            }           
        }
        
        public async Task<IActionResult> Confirmation()
        {
            return View();
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
                if (!string.IsNullOrEmpty(cartDtos.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon<ResponseDto>(cartDtos.CartHeader.CouponCode, accessToken);
                    if(coupon!=null && coupon.IsSuccess)
                    {
                        var couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result));
                        cartDtos.CartHeader.DiscountTotal = couponDto.DiscountAmount;
                    }

                }
                foreach(var detail in cartDtos.CartDetails)
                {
                    cartDtos.CartHeader.OrderTotal += (detail.Count * detail.Product.Price);
                }
                cartDtos.CartHeader.OrderTotal -= cartDtos.CartHeader.DiscountTotal;
            }

            return cartDtos;
        }
       
    }
}
