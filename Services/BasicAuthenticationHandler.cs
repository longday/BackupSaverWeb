using System;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace WebUI.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.Run(() => {

                string authHeader = Request.Headers["Authorization"];
                if(authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                
                    var credentials = Encoding.UTF8
                                        .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                        .Split('&', 2);

                    if (credentials.Length == 2 && IsValidUser(credentials[0], credentials[1]))
                    {
                        var principial = new ClaimsPrincipal();
                        var ticket = new AuthenticationTicket(principial, Scheme.Name);
                        return AuthenticateResult.Success(ticket);
                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid Login or Password");
                    }
                    
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }
            });
        }

        private bool IsValidUser(string login, string password)
        {
            var correctLogin = Environment.GetEnvironmentVariable("LOGIN") as string;
            var correctPassword = Environment.GetEnvironmentVariable("PASSWORD") as string;

            return login == correctLogin && password == correctPassword;
        }
    }
}