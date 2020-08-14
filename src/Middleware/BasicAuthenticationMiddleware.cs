using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Claims;

namespace WebUI.Middleware
{
    public class BasicAuthenticationMiddleware 
    {
        private readonly RequestDelegate _next;

        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if(authHeader != null && authHeader.StartsWith("Basic "))
            {
                string authHeaderData = authHeader.Substring("Basic".Length).Trim();
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderData))
                                                .Split(':', 2);
                string login = credentials[0];
                string password = credentials[1];

                if(IsValidUser(login, password))
                {
                    var claims = new[] { new Claim("name", login)};
                    var identity = new ClaimsIdentity(claims, "Basic");
                    context.User = new ClaimsPrincipal(identity);
                    await _next.Invoke(context);
                }
                else
                {
                    SetUnauthorizedStatusCode(context, "Ivnalid data.Please, try again");
                }
            }
            else
            {
                SetUnauthorizedStatusCode(context, "Enter user data");
            }
        }

        private bool IsValidUser(string login, string password)
        {
            var correctLogin = Environment.GetEnvironmentVariable("LOGIN") as string;
            var correctPassword = Environment.GetEnvironmentVariable("PASSWORD") as string;

            return login == correctLogin && password == correctPassword;
        }

        private void SetUnauthorizedStatusCode(HttpContext context, string message)
        {
            if(string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", $"Basic realm=\"{message}\"");
        }
    }
}