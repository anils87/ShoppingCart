using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.CouponAPI.Models.DTOs;
using ShoppingCart.CouponAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private ResponseDto _reposonse;
        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            _reposonse = new ResponseDto();
        }
        [HttpGet("{code}")]
        public async Task<ResponseDto> GetCouponByCode(string code)
        {
            try
            {
                var result = await _couponRepository.GetCouponByCode(code);
                _reposonse.IsSuccess = true;
                _reposonse.Result = result;
            }
            catch(Exception ex)
            {
                _reposonse.IsSuccess = false;
                _reposonse.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return _reposonse;
        }
    }
}
