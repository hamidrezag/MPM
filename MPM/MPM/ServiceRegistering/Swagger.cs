using Infrastructure.Utils;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Creadit.WebApi.ServiceRegistering
{
    public static class Swagger
    {
        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
             {
                 options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                 options.CustomSchemaIds(type => type.ToString());
                 options.AddSecurityDefinition("Bearer",
                   new OpenApiSecurityScheme
                   {
                       In = ParameterLocation.Header,
                       Description = "Please enter into field the word 'Bearer' following by space and JWT",
                       Name = "Authorization",
                       BearerFormat = "Bearer ",
                       Type = SecuritySchemeType.ApiKey
                   });
                 options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            }
                        },
                new string[] {}
                    }
                    });
                 options.OperationFilter<FileUploadFilter>();
                 //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 //options.IncludeXmlComments(xmlPath);
             });
        }
    }
}
