using ShoppingCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Services.IServices
{
    public interface IProductService : IBaseService
    {
        public Task<T> GetAllProductsAsync<T>(string token);
        public Task<T> GetProductByIdAsync<T>(int id, string token);
        public Task<T> CreateProductAsync<T>(ProductDto productDto, string token);
        public Task<T> UpdateProductAsync<T>(ProductDto productDto, string token);
        public Task<T> DeleteProductAsync<T>(int id, string token);


    }
}
