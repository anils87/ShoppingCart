using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Identity
{
    public static class SD
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";


        public static IEnumerable<IdentityResource> identityResources => new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };
        

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope> {
            new ApiScope("shoppingcart","Shopping Cart Server"),
            new ApiScope(name: "read",displayName :"read your data"),
            new ApiScope(name: "write",displayName :"write your data"),
            new ApiScope(name: "delete",displayName :"delete your data"),
        };

        public static IEnumerable<Client> Clients => new List<Client> {
            new Client {
                ClientId="client",
                ClientSecrets= { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {"read","write","profile"}
            } ,
            new Client {
                ClientId="shoppingcart",
                ClientSecrets= { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris={ "https://localhost:44333/signin-oidc" },
                PostLogoutRedirectUris= { "https://localhost:44333/signout-callback-oidc" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "shoppingcart"
                }
            },
        };

    }
}
