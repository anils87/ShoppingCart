using Microsoft.EntityFrameworkCore;
using ShoppingCart.EmailAPI.DBContext;
using ShoppingCart.EmailAPI.Message;
using ShoppingCart.EmailAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.EmailAPI.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDBContext> _dbContext;
        //private IMapper _mapper;
        public EmailRepository(DbContextOptions<ApplicationDBContext> dBContext)
        {
            _dbContext = dBContext;
         
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            // Implement and email sender or call some other class library
            EmailLog emailLog = new EmailLog()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has beed created successfully!"
            };
            await using var _db = new ApplicationDBContext(_dbContext);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
            

        }
    }
}
