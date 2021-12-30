using Microsoft.EntityFrameworkCore;
using ShoppingCart.EmailAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.EmailAPI.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<EmailLog> EmailLogs { get; set; }

    }
}
