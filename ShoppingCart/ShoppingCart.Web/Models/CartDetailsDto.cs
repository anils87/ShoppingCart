using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Models
{
    public class CartDetailsDto
    {
        public int CartDetailId { get; set; }

        public int CartHeaderId { get; set; }

        public virtual CartHeaderDto CartHeader { get; set; }

        public int ProductId { get; set; }

        public virtual ProductDto Product { get; set; }


        public int Count { get; set; }
    }
}
