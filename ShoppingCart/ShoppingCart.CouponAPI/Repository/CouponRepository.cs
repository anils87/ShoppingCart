using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.CouponAPI.DBContext;
using ShoppingCart.CouponAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDBContext _dBContext;
        private IMapper _mapper;
        public CouponRepository(ApplicationDBContext dBContext, IMapper mapper)
        {
            _dBContext = dBContext;
            _mapper = mapper;
        }
        public async Task<CouponDto> GetCouponByCode(string couponCode)
        {
            var coupon = await _dBContext.Coupons.FirstOrDefaultAsync(u => u.CouponCode == couponCode);
            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
