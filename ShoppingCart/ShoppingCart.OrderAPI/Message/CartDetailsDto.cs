using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.OrderAPI.Message
{
    public class CartDetailsDto
    {
        
        public int CartDetailId { get; set; }

        public int CartHeaderId { get; set; }
        
        public int ProductId { get; set; }

        public virtual ProductDto Product { get; set; }


        public int Count { get; set; }

    }
}
