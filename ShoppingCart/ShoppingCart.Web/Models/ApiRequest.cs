using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using static ShoppingCart.Web.SD;

namespace ShoppingCart.Web.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }

        public object Data { get; set; }

        public string AccessToken { get; set; }
    }
}
