using ShoppingCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseDto { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
