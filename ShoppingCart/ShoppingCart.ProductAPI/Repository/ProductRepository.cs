using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.ProductAPI.DBContext;
using ShoppingCart.ProductAPI.Models;
using ShoppingCart.ProductAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _db;
        private IMapper _mapper;

        public ProductRepository(ApplicationDBContext dBContext, IMapper mapper)
        {
            _db = dBContext;
            _mapper = mapper;
        }
        public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
        {
            Product product = _mapper.Map<ProductDto, Product>(productDto);
            if(product.ProductId > 0)
            {
                _db.Products.Update(product);
            }
            else
            {
                _db.Products.Add(product);
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<Product, ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            try
            {
                Product product = await _db.Products.FirstOrDefaultAsync(m => m.ProductId == productId);
                if(product == null)
                {
                    return false;
                }
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();                
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            

        }

        public async Task<ProductDto> GetProductById(int productId)
        {
            Product product = await _db.Products.Where(m=>m.ProductId == productId).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            IEnumerable<Product> productList = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(productList);
        }
    }
}
