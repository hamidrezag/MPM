using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Creadit.WebApi.ServiceRegistering
{
    public static class CorsSettings
    {
        public static void RegisterCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy",builder=>builder.AllowAnyOrigin()
                .AllowAnyMethod().AllowAnyHeader());
            });
        }
    }
}
