
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MPM.ServiceRegistering
{
    public static class Authentication
    {
        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Authentication:SecurityKey"))),
            ValidateAudience = false,
            // Ensure the token was issued by a trusted authorization server (default true):
            ValidateIssuer = false,
            ValidIssuer = configuration.GetValue<string>("Authentication:AuthenticationValidIssuer"),
            ValidAudience = configuration.GetValue<string>("Authentication:AuthenticationValidAudience"),

        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => configuration.Bind("Authentication:CookieSettings", options));


            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        }
    }


}
