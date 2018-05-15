using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace WebApplication
{
    public partial class Startup
    {
        /// <summary>
        /// Gets the Local Logger.
        /// </summary>
        private global::Serilog.ILogger Log
        {
            get { return _cachedLog ?? (_cachedLog = global::Serilog.Log.ForContext(typeof(Startup))); }
        }
        private global::Serilog.ILogger _cachedLog;


        private static string clientId = ConfigurationManager.AppSettings["ida:OpenIdClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string authority = aadInstance + tenantId;

        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            #region Web application authentication
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri
                });
            #endregion

            #region  Start Web API "default" authentication
            // TODO: Kanske får ändra något  
            string crmIssuer = ConfigurationManager.AppSettings["ida:CRMIssuer"];
            List<string> validIssuers = ConfigurationManager.AppSettings["ida:ValidIssuers"]
               .Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
               .ToList();
            validIssuers.Add(crmIssuer);

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                    new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                    {
                        Tenant = ConfigurationManager.AppSettings["ida:Tenant"],
                        TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudience = ConfigurationManager.AppSettings["ida:Audience"],

                            // Special issuer validation
                            // The issuer field contains a unique guid of the Azure Ad instance which
                            // granted access.
                            IssuerValidator = (issuer, token, tokenValidationParameters) =>
                            {
                                if (validIssuers.Contains(issuer))
                                    return issuer;

                                // Log access attempt
                                Log.Warning("Invalid Azure AD Issuer '{InvalidIssuer}', denying access.", issuer);
                                throw new SecurityTokenInvalidIssuerException("Invalid issuer");
                            }
                        },
                        MetadataAddress = ConfigurationManager.AppSettings["ida:MetadataAddress"],
                    });
            #endregion
        }
    }
}
