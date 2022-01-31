using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using ShoppingCart.ProductAPI.Models.DTOs;
using ShoppingCart.ProductAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.ProductAPI.Controllers
{    
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        protected ResponseDto responseDto;
        private IProductRepository _productRepository;
        //private readonly IMemoryCache _imemoryCache;
        private readonly IDistributedCache _distCache;
        public ProductController(IProductRepository productRepository, IDistributedCache distCache)
        {
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
            //_imemoryCache = imemoryCache;
            _distCache = distCache;
        }
        
        [HttpGet]          
        public async Task<object> GetProductList()
        {
            string cacheKey = "productList";
            try
            {
                // In Memory Cache
                //var memProductList = _imemoryCache.Get("productList");

                //Distributed Cache
                List<ProductDto> productDtos = new List<ProductDto>();
                string serializedProductList;
                var redisProductList = await _distCache.GetAsync(cacheKey);
                if (redisProductList != null)
                {
                    serializedProductList = Encoding.UTF8.GetString(redisProductList);
                    productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(serializedProductList);
                    responseDto.Result = productDtos;
                }
                else
                {
                    var productListDB = await _productRepository.GetProducts();
                    serializedProductList = JsonConvert.SerializeObject(productListDB);
                    redisProductList = Encoding.UTF8.GetBytes(serializedProductList);
                    var distCacheOption = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                        SlidingExpiration = TimeSpan.FromSeconds(20)
                    };
                    await _distCache.SetAsync(cacheKey, redisProductList, distCacheOption);                  
                    responseDto.Result = productListDB;
                    /*
                    // Im Memory Caching implememtation
                    var cacheExpirationOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                        Priority = CacheItemPriority.High,
                        SlidingExpiration = TimeSpan.FromSeconds(20)

                    };
                    _imemoryCache.Set("productList", productDtos,cacheExpirationOption);
                    */

                    //Distributed Caching Implementation

                }
                
                responseDto.IsSuccess = true;
            }
            catch(Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return responseDto;
        }
        [HttpGet]        
        [Route("{id}")]
        public async Task<object> GetProductById(int id)
        {
            try
            {
                ProductDto productDtos = await _productRepository.GetProductById(id);
                responseDto.Result = productDtos;
                responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return responseDto;
        }
        [HttpPost]
        [Authorize]
        public async Task<object> CreateProduct([FromBody]ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.CreateUpdateProduct(productDto);
                responseDto.Result = model;
                responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return responseDto;
        }

        [HttpPut]
        [Authorize]
        public async Task<object> UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _productRepository.CreateUpdateProduct(productDto);
                responseDto.Result = model;
                responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return responseDto;
        }

        [HttpDelete]
        [Authorize(Roles ="Admin")]
        [Route("{id}")]
        public async Task<object> DeletProductById(int id)
        {
            try
            {
                bool isSuccess = await _productRepository.DeleteProduct(id);
                responseDto.Result = isSuccess;
                responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                responseDto.IsSuccess = false;
                responseDto.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return responseDto;
        }
    }
}
