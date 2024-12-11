using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MVM.ServiceRegistering
{
    public static class Versionning
    {
        public static void RegisterApiVersionning(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.ReportApiVersions = true;
            });
            services.AddEndpointsApiExplorer();
            services.AddMvcCore().AddApiExplorer();
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });


        }
    }
}
