using ShoppingCart.OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader order);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);

    }
}
