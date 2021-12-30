using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShoppingCart.Web.Models;
using ShoppingCart.Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> productDtos = new();
            
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(productDtos);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto productDto = new();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId,accessToken);
            if (response != null && response.IsSuccess)
            {
                productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(productDto);
        }

        
        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            // ProductDto productDto = new();
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(m => m.Type == "sub")?.FirstOrDefault()?.Value
                }
            };
            CartDetailsDto cartDetailsDto = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, accessToken);
            if(resp != null && resp.IsSuccess)
            {
                cartDetailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }
            List<CartDetailsDto> cartDetailsDtos = new();
            cartDetailsDtos.Add(cartDetailsDto);
            cartDto.CartDetails = cartDetailsDtos;

            
            var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto,accessToken);
             if(addToCartResp !=null && addToCartResp.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {

            return SignOut("Cookies","oidc");
        }
    }
}
