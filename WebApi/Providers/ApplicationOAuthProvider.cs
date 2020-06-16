using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApi.Models;
using Core;
using DAL;
using Application.Web.Models;

namespace WebApi.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var userManager = context.OwinContext.GetUserManager<UserManager>();

            User user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            //if (user.IsApproved == false)
            //{
            //    context.SetError("invalid_grant", "The User is not approved. Please contact Admin.");
            //    context.Request.Context.Authentication.SignOut(_publicClientId);
            //}
            //else 
            if (user.ApprovalStatus.Trim().ToUpper() == "ACTIVE")
            {
                ClaimsIdentity oAuthIdentity = await this.GenerateUserIdentityAsync(user, userManager, OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookiesIdentity = await this.GenerateUserIdentityAsync(user, userManager, CookieAuthenticationDefaults.AuthenticationType);

                List<Claim> roles = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                //  AuthenticationProperties properties = CreateProperties(user.UserName, Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value)));

                //AuthenticationProperties properties = CreateProperties(user.UserName, user.FirstName, user.LastName,
                //    Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value)));

                //Getting the role to be displayed depending upon rolerank
                var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new UnitOfWork()));
                var allroles = roleManager.Roles.Select(a => new EnumerationModel
                {
                    PositionNameID = a.Id.ToString(),
                    PositionName = a.Name,
                    PositionRank = a.RoleRank
                }).ToList();

                AuthenticationProperties properties = CreateProperties(user.UserName, user.FirstName, user.LastName,
                    Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value)));
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);

                context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }
            else
            {
                string message = user.ApprovalStatus.Trim().ToUpper() == "PENDING"
                               ? "The User is not approved. Please contact Admin."
                               : "User access has been suspended. Please contact Admin.";
                context.SetError("invalid_grant", message);
                context.Request.Context.Authentication.SignOut(_publicClientId);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(User user, UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, authenticationType);

            // Add custom user claims here
            return userIdentity;
        }

        public static AuthenticationProperties CreateProperties(string userName, string FirstName, string LastName, string Roles)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "FirstName",FirstName },
                {"LastName",LastName },
                {"Roles", Roles }
            };
            return new AuthenticationProperties(data);
        }
    }
}