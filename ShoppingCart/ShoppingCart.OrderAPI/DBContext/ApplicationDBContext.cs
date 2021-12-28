using Microsoft.EntityFrameworkCore;
using ShoppingCart.OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.OrderAPI.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }


    }
}
