
using Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Utils
{
    public class FileUploadFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParameters = context.ApiDescription.ParameterDescriptions
                .Where(paramDesc => paramDesc.IsFromForm());

            if (formParameters.Any())
            {
                // already taken care by swashbuckle. no need to add explicitly.
                return;
            }
            if (operation.RequestBody != null)
            {
                // NOT required for form type
                return;
            }
            if (context.ApiDescription.HttpMethod == HttpMethod.Post.Method)
            {
                var uploadFileMediaType = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Type = "object",
                        Properties =
                    {
                        ["files"] = new OpenApiSchema()
                        {
                            Type = "array",
                            Items = new OpenApiSchema()
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        }
                    },
                        Required = new HashSet<string>() { "files" }
                    }
                };

                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = { ["multipart/form-data"] = uploadFileMediaType }
                };
            }
        }
    }
}
