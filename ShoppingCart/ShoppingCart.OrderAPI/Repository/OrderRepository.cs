using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.OrderAPI.DBContext;
using ShoppingCart.OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDBContext> _dbContext;
        //private IMapper _mapper;
        public OrderRepository(DbContextOptions<ApplicationDBContext> dBContext)
        {
            _dbContext = dBContext;
         
        }

        public async Task<bool> AddOrder(OrderHeader order)
        {
            await using var _db = new ApplicationDBContext(_dbContext);
            _db.OrderHeaders.Add(order);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDBContext(_dbContext);
            var orderHeaderFromDB = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);
            if(orderHeaderFromDB != null)
            {
                orderHeaderFromDB.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }

        }
    }
}
