using Microsoft.AspNetCore.Identity;
using ShoppingCart.Identity.Models;
using ShoppingCart.IdentityAPI.DBContext;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;

namespace ShoppingCart.Identity.Intializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDBContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DBInitializer(ApplicationDBContext dBContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _db = dBContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if(_roleManager.FindByNameAsync(SD.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            ApplicationUser adminUser = new ApplicationUser()
            {                
                UserName = "adminsc@gmail.com",
                Email = "adminsc@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Anil",
                LastName = "Yadav"
            };

            _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

            var tempUser1 = _userManager.AddClaimsAsync(adminUser, new Claim[] {
                 new Claim(JwtClaimTypes.Name,adminUser.FirstName + " "+ adminUser.LastName),
                 new Claim(JwtClaimTypes.GivenName,adminUser.FirstName),
                 new Claim(JwtClaimTypes.FamilyName,adminUser.LastName),
                 new Claim(JwtClaimTypes.Role,SD.Admin),
             }).Result;


            ApplicationUser customerUser = new ApplicationUser()
            {
                UserName = "customersc@gmail.com",
                Email = "customersc@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Customer",
                LastName = "User"
            };

            _userManager.CreateAsync(customerUser, "Customer123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

            var tempUser2 = _userManager.AddClaimsAsync(customerUser, new Claim[] {
                 new Claim(JwtClaimTypes.Name,customerUser.FirstName + " "+ customerUser.LastName),
                 new Claim(JwtClaimTypes.GivenName,customerUser.FirstName),
                 new Claim(JwtClaimTypes.FamilyName,customerUser.LastName),
                 new Claim(JwtClaimTypes.Role,SD.Customer),
             }).Result;



        }
    }
}
