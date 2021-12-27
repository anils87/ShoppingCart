using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Models
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public String CouponCode { get; set; }
        public double DiscountAmount { get; set; }
    }
}
