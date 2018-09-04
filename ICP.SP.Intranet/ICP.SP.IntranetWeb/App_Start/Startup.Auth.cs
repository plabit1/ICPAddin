using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using ICP.SP.IntranetWeb.Utils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IdentityModel.Claims;
using ICP.SP.IntranetWeb.Models;
using System.Threading.Tasks;

namespace ICP.SP.IntranetWeb
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientID"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:AppKey"];
        private static string graphResourceID = ConfigurationManager.AppSettings["ida:GraphResourceID"];

        private static string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions() { CookieManager = new SystemWebCookieManager() });
            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // when an auth code is received…
                        AuthorizationCodeReceived = (context) =>
                        {
                            // get the OpenID Connect code passed from Azure AD on successful auth
                            string code = context.Code;

                            // create the app credentials & get reference to the user
                            ClientCredential creds = new ClientCredential(clientId, clientSecret);
                            string signInUserId = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                            // use the OpenID Connect code to obtain access token & refresh token…
                            //  save those in a persistent store…
                            AuthenticationContext authContext = new AuthenticationContext(authority, new ADALTokenCache(signInUserId));

                            // obtain access token for the AzureAD graph
                            Uri redirectUri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                            AuthenticationResult authResult = authContext.AcquireTokenByAuthorizationCode(code, redirectUri, creds, graphResourceID);

                            // successful auth                            
                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            context.HandleResponse();
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}