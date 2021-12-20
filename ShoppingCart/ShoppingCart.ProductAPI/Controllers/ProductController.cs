using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.ProductAPI.Models.DTOs;
using ShoppingCart.ProductAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ProductAPI.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        protected ResponseDto responseDto;
        private IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            this.responseDto = new ResponseDto();
        }
        
        [HttpGet]  
        [Authorize]
        public async Task<object> GetProductList()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
                responseDto.Result = productDtos;
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
        [Authorize]
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
